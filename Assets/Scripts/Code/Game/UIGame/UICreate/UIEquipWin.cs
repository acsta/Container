using System.Collections;
using System.Collections.Generic;
using System;
using SuperScrollView;
using UnityEngine;
using UnityEngine.UI;

namespace TaoTie
{
	public class UIEquipWin : UIBaseView, IOnCreate, IOnEnable<int, Action<int,int>, Player>
	{
		public static string PrefabPath => "UIGame/UICreate/Prefabs/UIEquipWin.prefab";
		public UIButton Close;
		public UIImage Rare;
		public UIImage Icon;
		public UITextmesh Name;
		public UITextmesh HeadTitle;
		public UITextmesh Title;
		public UITextmesh Details;
		public UIButton Try;
		public UITextmesh TryText;
		public UIButton Control;
		public UITextmesh ControlText;
		public UICopyGameObject EffectGroup;
		private int equipId;
		public ClothConfig Config => ClothConfigCategory.Instance.Get(equipId);
		private EquipGroupConfig groupConfig;
		private Action<int,int> onClickItem;
		private Player player;
		private int groupCount;
		public UIAnimator UICommonWin;


		#region override
		public void OnCreate()
		{
			UICommonWin = AddComponent<UIAnimator>("UICommonWin");
			HeadTitle =  this.AddComponent<UITextmesh>("UICommonWin/Win/Title");
			this.Close = this.AddComponent<UIButton>("UICommonWin/Win/Close");
			this.Rare = this.AddComponent<UIImage>("UICommonWin/Win/Content/ClothItem/Rare");
			this.Icon = this.AddComponent<UIImage>("UICommonWin/Win/Content/ClothItem/Icon");
			this.Name = this.AddComponent<UITextmesh>("UICommonWin/Win/Content/ClothItem/Title/Name");
			this.Title = this.AddComponent<UITextmesh>("UICommonWin/Win/Content/EquipEffect/Title");
			this.Details = this.AddComponent<UITextmesh>("UICommonWin/Win/Content/EquipEffect/Details");
			this.Try = this.AddComponent<UIButton>("UICommonWin/Win/Content/Buttons/Try");
			this.TryText = this.AddComponent<UITextmesh>("UICommonWin/Win/Content/Buttons/Try/Text");
			this.Control = this.AddComponent<UIButton>("UICommonWin/Win/Content/Buttons/Control");
			ControlText = AddComponent<UITextmesh>("UICommonWin/Win/Content/Buttons/Control/Text");
			EffectGroup = AddComponent<UICopyGameObject>("UICommonWin/Win/Content/EffectList/Effects");
			EffectGroup.InitListView(0, OnGetItemByIndex);
		}
		public void OnEnable(int id, Action<int,int> onClickItem,Player player)
		{
			SoundManager.Instance.PlaySound("Audio/Sound/Win_Open.mp3");
			this.player = player;
			this.onClickItem = onClickItem;
			equipId = id;
			var config = Config;
			Icon.SetSpritePath(config.Icon).Coroutine();
			var name = I18NManager.Instance.I18NGetText(config);
			HeadTitle.SetText(name);
			Name.SetText(name);
			var icon = RareConfigCategory.Instance.GetRare(config.Rare).Icon;
			Rare.SetSpritePath($"UIGame/UICreate/Atlas/bg_{icon}2.png").Coroutine();
			var module = CharacterConfigCategory.Instance.Get(config.Module);
			Title.SetText(I18NManager.Instance.I18NGetText(module));
			if (config.EffectType != 0)
			{
				Details.SetI18NKey(Enum.Parse<I18NKey>("Text_Equip_Effect_" + config.EffectType), config.Param);
			}
			else
			{
				Details.SetI18NKey(I18NKey.Text_Equip_Effect_0);
			}
			this.Close.SetOnClick(OnClickClose);
			this.Try.SetOnClick(OnClickEquip);
			if (config.GroupId == 0 || !EquipGroupConfigCategory.Instance.TryGet(config.GroupId, out groupConfig))
			{
				groupConfig = null;
			}
			EffectGroup.SetListItemCount(groupConfig?.Count.Length??0);
			RefreshState();
		}
		public override async ETTask CloseSelf()
		{
			SoundManager.Instance.PlaySound("Audio/Sound/Win_Close.mp3");
			await UICommonWin.Play("UIWin_Close");
			await base.CloseSelf();
		}
		#endregion

		#region 事件绑定
		public void OnClickClose()
		{
			CloseSelf().Coroutine();
		}
		public void OnClickEquip()
		{
			var module = Config.Module;
			var isEquip = player.SubModule[module - 1] == equipId;
			onClickItem?.Invoke(isEquip ? -1 : equipId, module);
			RefreshState();
			CloseSelf().Coroutine();
		}
		public void OnClickBuy()
		{	
			var config = Config;
			if (PlayerDataManager.Instance.GetItemCount(config.Id) > 0)
			{
				UIManager.Instance.OpenBox<UIToast, I18NKey>(UIToast.PrefabPath,I18NKey.Text_Buy_Already).Coroutine();
				return;
			}
			if (config.GetWay == 0)
			{
				OnClickAdAsync().Coroutine();
			}
			else if (config.GetWay == 1)
			{
				if (PlayerDataManager.Instance.TotalMoney >= config.Price)
				{
					PlayerDataManager.Instance.ChangeMoney(-config.Price);
					PlayerDataManager.Instance.ChangeItem(config.Id, 1);
					CloseSelf().Coroutine();
				}
				else
				{
					UIManager.Instance.OpenBox<UIToast,I18NKey>(UIToast.PrefabPath,I18NKey.Notice_Common_LackOfMoney).Coroutine();
				}
			}
		}
		
		private async ETTask OnClickAdAsync()
		{
			try
			{
				var res = await AdManager.Instance.PlayAd();
				if (res)
				{
					PlayerDataManager.Instance.ChangeItem(Config.Id, 1);
					CloseSelf().Coroutine();
				}
			}
			catch (Exception ex)
			{
				Log.Error(ex);
			}
		}

		private void OnGetItemByIndex(int index, GameObject obj)
		{
			EffectItem item = EffectGroup.GetUIItemView<EffectItem>(obj);
			if (item == null)
			{
				item = EffectGroup.AddItemViewComponent<EffectItem>(obj);
			}

			item.SetData(groupConfig.EffectType[index], groupConfig.Param[index], groupConfig.Count[index],
				groupCount >= groupConfig.Count[index]);
		}
		#endregion

		private void RefreshState()
		{
			var config = Config;
			var has = PlayerDataManager.Instance.GetItemCount(equipId) > 0;
			var isEquip = player.SubModule[config.Module - 1] == equipId;
			Try.SetActive(!has);
			if (!has)
			{
				ControlText.SetI18NKey(I18NKey.Text_Equip_Buy);
				this.Control.SetOnClick(OnClickBuy);
				TryText.SetI18NKey(isEquip?I18NKey.Text_UnEquip:I18NKey.Text_Equip_Try);
			}
			else
			{
				ControlText.SetI18NKey(isEquip?I18NKey.Text_UnEquip:I18NKey.Text_Equip);
				this.Control.SetOnClick(OnClickEquip);
			}

			groupCount = GetGroupCount();
			EffectGroup.RefreshAllShownItem();
		}
		
		private int GetGroupCount()
		{
			int res = 0;
			var conf = Config;
			for (int i = 1; i < player.SubModule.Length; i++)
			{
				var module = CharacterConfigCategory.Instance.Get(i + 1);
				if (player.SubModule[i] != 0 && module.DefaultCloth != player.SubModule[i])
				{
					var cloth = ClothConfigCategory.Instance.Get(player.SubModule[i]);
					if (cloth.GroupId == conf.GroupId)
					{
						res++;
					}
				}
			}

			return res;
		}
	}
}
