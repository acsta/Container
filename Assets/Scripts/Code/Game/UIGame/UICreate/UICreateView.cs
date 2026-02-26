using System.Collections;
using System.Collections.Generic;
using System;
using SuperScrollView;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace TaoTie
{
	public class UICreateView : UIBaseView, IOnCreate,  IOnEnable<Player>, IOnTopWidthPaddingChange
	{
		public static string PrefabPath => "UIGame/UICreate/Prefabs/UICreateView.prefab";
		public UIButton btn_start;
		public CreateItem[] Item;
		
		public UIButton Close;
		private Player player;
		public UIEventTrigger Drager;
		public UICashGroup CashGroup;
		public UIButton BagOpen;
		public UIBagWin BagWin;
		public UIShopWin ShopWin;
		
		private Vector3 startRot;
		private Vector2 startDragPos;

		#region override
		public void OnCreate()
		{
			BagWin = AddComponent<UIBagWin>("UICommonView/Bg/Content/Table");
			ShopWin = AddComponent<UIShopWin, Action<int, int>>("UICommonView/Bg/Content/Shop", OnClickItem);
			BagOpen = AddComponent<UIButton>("UICommonView/Bg/Content/Shop/Top/OpenBag");
			CashGroup = AddComponent<UICashGroup>("CashGroup");
			this.btn_start = this.AddComponent<UIButton>("UICommonView/Bg/Content/Items/Enter");
			Item = new CreateItem[8];
			for (int i = 0; i < Item.Length; i++)
			{
				this.Item[i] = this.AddComponent<CreateItem>("UICommonView/Bg/Content/Items/Item"+i);
			}
			
			
			this.Close = this.AddComponent<UIButton>("UICommonView/Bg/Close");
			Drager = AddComponent<UIEventTrigger>("UICommonView/Bg/Content/Drager");
			Drager.AddOnBeginDrag(OnBeginDrag);
			Drager.AddOnDrag(OnDrag);
		}
		public void OnEnable(Player player)
		{
			BagWin.SetData(OnClickItem);
			BagOpen.SetOnClick(OnClickOpenBag);
			this.player = player;
			this.btn_start.SetOnClick(OnClickSave);
			this.Close.SetOnClick(OnClickClose);
			for (int i = 0; i < Item.Length; i++)
			{
				this.Item[i].SetData(i + 2, OnClickEquipItem, PlayerDataManager.Instance.Show[i + 1]);
			}
			ShowList(2);
			RefreshGroupInfo();
		}
		#endregion

		#region 事件绑定

		
		private void OnClickOpenBag()
		{
			BagWin.Open(OnClickItem);
		}

		public void OnClickSave()
		{
			bool hasNoPay = false;
			bool change = false;
			List<int> noPay = new List<int>();
			for (int i = 0; i < player.SubModule.Length; i++)
			{
				if (player.SubModule[i] != PlayerDataManager.Instance.Show[i])
				{
					change = true;
					if (PlayerDataManager.Instance.GetItemCount(player.SubModule[i]) <= 0)
					{
						hasNoPay = true;
						noPay.Add(player.SubModule[i]);
					}
				}
			}

			if (change)
			{
				if (hasNoPay)
				{
					UIManager.Instance.OpenWindow<UIBuyWin, List<int>>(UIBuyWin.PrefabPath, noPay).Coroutine();
					return;
				}
				PlayerDataManager.Instance.ChangeShow(player.SubModule);
				UIManager.Instance.OpenBox<UIToast,I18NKey>(UIToast.PrefabPath,I18NKey.Text_Save_Success).Coroutine();
			}
			
		}
		
		public void OnClickClose()
		{
			for (int i = 0; i < player.SubModule.Length; i++)
			{
				if (player.SubModule[i] != PlayerDataManager.Instance.Show[i])
				{
					UIManager.Instance.OpenBox<UIMsgBoxWin,MsgBoxPara>(UIMsgBoxWin.PrefabPath, new MsgBoxPara()
					{
						Content = I18NManager.Instance.I18NGetText(I18NKey.Text_Notice_Save),
						CancelText = I18NManager.Instance.I18NGetText(I18NKey.Global_Btn_Cancel),
						CancelCallback = (win) =>
						{
							UIManager.Instance.CloseBox(win).Coroutine();
						},
						ConfirmText = I18NManager.Instance.I18NGetText(I18NKey.Global_Btn_Confirm),
						ConfirmCallback = (win) =>
						{
							UIManager.Instance.CloseBox(win).Coroutine();
							SceneManager.Instance.SwitchScene<HomeScene>().Coroutine();
						},
					}).Coroutine();
					return;
				}
			}
			SceneManager.Instance.SwitchScene<HomeScene>().Coroutine();
		}
		private void OnBeginDrag(PointerEventData data)
		{
			startDragPos = data.position;
			startRot = player.Rotation.eulerAngles;
		}

		private void OnDrag(PointerEventData data)
		{
			var deltaX = (data.position - startDragPos).x;
			player.Rotation = Quaternion.Euler(startRot + Vector3.down * deltaX);
			
		}

		#endregion
		
		public void OnClickItem(int id, int moduleId)
		{
			var module = CharacterConfigCategory.Instance.Get(moduleId);
			if (id <= 0 || id == module.DefaultCloth)
			{
				OnEquipItem(id, moduleId);
				return;
			}

			UIManager.Instance
				.OpenWindow<UIEquipWin, int, Action<int,int>, Player>(UIEquipWin.PrefabPath, id, OnEquipItem, player)
				.Coroutine();
		}

		public void OnEquipItem(int id, int moduleId)
		{
			if (id <= 0)
			{
				var module = CharacterConfigCategory.Instance.Get(moduleId);
				if (module.DefaultCloth != 0)
				{
					id = module.DefaultCloth;
				}
			}
			if (moduleId > 1) this.Item[moduleId - 2].SetData(moduleId, OnClickEquipItem, id);
			player.SetModule(moduleId, id).Coroutine();
			RefreshGroupInfo();
		}

		private void RefreshGroupInfo()
		{
			using DictionaryComponent<int ,int> temp = DictionaryComponent<int, int>.Create();
			for (int i = 1; i < player.SubModule.Length; i++)
			{
				var module = CharacterConfigCategory.Instance.Get(i + 1);
				if (player.SubModule[i] != 0 && module.DefaultCloth != player.SubModule[i])
				{
					var cloth = ClothConfigCategory.Instance.Get(player.SubModule[i]);
					if (cloth.GroupId > 0)
					{
						if (temp.ContainsKey(cloth.GroupId))
						{
							temp[cloth.GroupId]++;
						}
						else
						{
							temp[cloth.GroupId] = 1;
						}
					}
				}
			}

			BagWin.GroupInfoTable.SetData(temp);
			ShopWin.GroupInfoTable.SetData(temp);
		}

		public void OnClickEquipItem(int id, int moduleId)
		{
			ShowList(moduleId);
			if (id > 0)
			{
				OnClickItem(id, moduleId);
			}
		}
	
		public void ShowList(int id)
		{
			BagWin.ShowList(id);
		}
	}
}
