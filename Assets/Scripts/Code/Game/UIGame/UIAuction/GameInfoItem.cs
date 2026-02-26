using System.Collections;
using System.Collections.Generic;
using System;
using System.Text;
using SuperScrollView;
using UnityEngine;
using UnityEngine.UI;

namespace TaoTie
{
	public class GameInfoItem : UIBaseContainer, IOnCreate
	{
		public UIPointerClick PointerClick;
		public UITextmesh Desc;
		public UICopyGameObject Content;
		// public UITextmesh Name;
		// public UIImage Icon;
		public UITextmesh Eff;
		public UIImage Light;
		public UIImage Mask;
		
		// public UIImage Bg;
		// public UIImage DiffBg;
		public UITextmesh Chinese;
		public UITextmesh English;

		private GameInfoConfig config;
		private Action<GameInfoConfig> onClickThis;
		private List<ItemConfig> items = new List<ItemConfig>();
		#region override
		public void OnCreate()
		{
			Mask = AddComponent<UIImage>("Mask");
			Light = AddComponent<UIImage>("Light");
			PointerClick = AddComponent<UIPointerClick>();
			// Bg = this.AddComponent<UIImage>("Bg");
			// DiffBg = this.AddComponent<UIImage>("Diff");
			this.Desc = this.AddComponent<UITextmesh>("DescBg/Desc");
			this.Content = this.AddComponent<UICopyGameObject>("DescBg/ScrollView/Viewport/Content");
			this.Content.InitListView(0,GetContentItemByIndex);
			// this.Name = this.AddComponent<UITextmesh>("Name/Text");
			// this.Icon = this.AddComponent<UIImage>("Icon");
			this.Eff = this.AddComponent<UITextmesh>("Desc/Text");
			Chinese = AddComponent<UITextmesh>("Diff/Chinese");
			English = AddComponent<UITextmesh>("Diff/English");
			PointerClick.SetOnClick(OnClickSelf);
		}
	
		#endregion

		#region 事件绑定
		public void GetContentItemByIndex(int index, GameObject obj)
		{
			var cfg = items[index];
			if (Content.GetUIItemView<UIAuctionItem>(obj) == null)
			{
				Content.AddItemViewComponent<UIAuctionItem>(obj);
			}
			var uiitem = Content.GetUIItemView<UIAuctionItem>(obj);
			uiitem.SetData(cfg);
		}

		#endregion

		public void SetData(GameInfoConfig config,Action<GameInfoConfig> onClickThis)
		{
			Chinese.SetActive(I18NManager.Instance.CurLangType == LangType.Chinese);
			English.SetActive(I18NManager.Instance.CurLangType != LangType.Chinese);
			Light.SetActive(onClickThis == null);
			Mask.SetActive(onClickThis != null);
			this.config = config;
			this.onClickThis = onClickThis;
			items.Clear();
			if (config.Type == (int)GameInfoTargetType.Container)
			{
				for (int i = 0; i < config.Ids.Length; i++)
				{
					var container = ContainerConfigCategory.Instance.Get(config.Ids[i]);
					items.Add(container.ItemConfig);
				}
			}
			else if (config.Type == (int)GameInfoTargetType.Items)
			{
				for (int i = 0; i < config.Ids.Length; i++)
				{
					items.Add(ItemConfigCategory.Instance.Get(config.Ids[i]));
				}
			}
			else if (config.Type == (int)GameInfoTargetType.RandItems && config.TempItems != null)
			{
				for (int i = 0; i < config.TempItems.Count; i++)
				{
					items.Add(ItemConfigCategory.Instance.Get(config.TempItems[i]));
				}
			}
			else if (config.Type == (int) GameInfoTargetType.PlayType)
			{
				for (int i = 0; i < config.Ids.Length; i++)
				{
					var playType = PlayTypeConfigCategory.Instance.Get(config.Ids[i]);
					items.Add(playType.ItemConfig);
				}
			}
			else if (config.Type == (int) GameInfoTargetType.Raise)
			{
				for (int i = 0; i < config.Ids.Length; i++)
				{
					items.Add(ItemConfigCategory.Instance.Get(config.Ids[i]));
				}
			}

			// Icon.SetSpritePath(config.Icon).Coroutine();
			var color = RareConfigCategory.Instance.GetRare(config.Rare).Color;
			// Bg.SetColor(color);
			// DiffBg.SetColor(color);
			Desc.SetText(I18NManager.Instance.I18NGetText(config));
			// Name.SetText(I18NManager.Instance.I18NGetText(config));
			string eff = "";
			if (config.AwardType == 0)
			{
				eff = "+";
			}
			else if (config.AwardType == 1)
			{
				eff = "x";
			}
			else
			{
				Log.Error("未处理奖励类型");
			}
			
			eff = $"<color={color}>{eff+ I18NManager.Instance.TranslateMoneyToStr(config.RewardCount)}</color>";
			if (config.Type == (int) GameInfoTargetType.Raise)
			{
				Eff.SetI18NKey(I18NKey.Text_Info_Eff2, eff);
			}
			else if (config.Type == (int) GameInfoTargetType.PlayType)
			{
				StringBuilder sb = new StringBuilder();
				sb.Append(I18NManager.Instance.I18NGetText(items[0]));
				for (int i = 1; i < items.Count; i++)
				{
					sb.Append("," + I18NManager.Instance.I18NGetText(items[i]));
				}

				Eff.SetI18NKey(I18NKey.Text_Info_Eff3, sb.ToString(), eff);
			}
			else
			{
				Eff.SetI18NKey(I18NKey.Text_Info_Eff);
				if (items.Count == 1 && items[0].Type == (int)ItemType.Container)
				{
					var container = ContainerConfigCategory.Instance.Get(items[0].ContainerId);
					if (!string.IsNullOrEmpty(container.Color))
					{
						Eff.SetI18NText($"<color=#{container.Color}>{I18NManager.Instance.I18NGetText(items[0])}</color>", eff);
					}
					else
					{
						Eff.SetI18NText(I18NManager.Instance.I18NGetText(items[0]), eff);
					}
				}
				else
				{
					int containerId = items[0].ContainerId;
					for (int i = 1; i < items.Count; i++)
					{
						if (items[i].ContainerId != containerId)
						{
							containerId = 0;
							break;
						}
					}

					ContainerConfig container = null;
					if (containerId != 0)
					{
						container = ContainerConfigCategory.Instance.Get(containerId);
					}
					if (!string.IsNullOrEmpty(container?.Color))
					{
						Eff.SetI18NText(I18NManager.Instance.I18NGetText(I18NKey.Text_Info_Eff_All)+
						                $"<color=#{container.Color}>{I18NManager.Instance.I18NGetText(container)}</color>", eff);
					}
					else
					{
						Eff.SetI18NText(I18NManager.Instance.I18NGetText(I18NKey.Text_Info_Eff_All), eff);
					}
					
				}
			}

			Content.SetListItemCount(items.Count);
			Content.RefreshAllShownItem();
		}

		public void OnClickSelf()
		{
			Light.SetActive(true);
			Mask.SetActive(false);
			onClickThis?.Invoke(config);
		}
	}
}
