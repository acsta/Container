using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace TaoTie
{
	public class MenuPara
	{
		public int Id;
		public string Name;
		public string ImgPath;
		public string RedDot;
		public Color ActiveColor = Color.white;
		public Color UnActiveColor = Color.white;
	}
	public class UIMenu : UIBaseContainer, IOnCreate
	{
		public UICopyGameObject Space;

		public List<MenuPara> Paras;
		public UIMenuItem[] UIMenuItems;
		public int ActiveIndex;
		private Action<MenuPara> onActiveIndexChanged;
		private Func<MenuPara,bool> onClickItem;
		private bool changeScale;
		#region override
		public void OnCreate()
		{
			Space = AddComponent<UICopyGameObject>();
			Space.InitListView(0,OnGetItemByIndex);
		}

		#endregion

		#region 事件绑定

		public void OnGetItemByIndex(int index, GameObject go)
		{
			var para = Paras[index];
			if (Space.GetUIItemView<UIMenuItem>(go) == null)
			{
				Space.AddItemViewComponent<UIMenuItem>(go);
			}
			var item = Space.GetUIItemView<UIMenuItem>(go);
			UIMenuItems[index] = item;
			item.SetData(para, index, (type, inx) =>
			{
				if (this.onClickItem == null || onClickItem(Paras[index]))
				{
					SetActiveIndex(inx);
				}
			}, index == ActiveIndex, changeScale);
		}

		#endregion

		public void SetData(List<MenuPara> paras, Action<MenuPara> onActiveIndexChanged, int activeIndex = -1, 
			Func<MenuPara, bool> onClickItem = null, bool changeScale = true)
		{
			this.changeScale = changeScale;
			this.onClickItem = onClickItem;
			this.onActiveIndexChanged = onActiveIndexChanged;
			Paras = paras;
			UIMenuItems = new UIMenuItem[Paras.Count];
			Space.SetListItemCount(Paras.Count);
			Space.RefreshAllShownItem();
			SetActiveIndex(activeIndex);
		}
		
		public void SetActiveIndex(int index, bool force = false)
		{
			if (!force && (index < 0 || ActiveIndex == index)) return;
			if (ActiveIndex >= 0)
				UIMenuItems[ActiveIndex].SetIsActive(false,changeScale);
			ActiveIndex = index;
			UIMenuItems[ActiveIndex].SetIsActive(true,changeScale);
			onActiveIndexChanged(Paras[index]);
			LayoutRebuilder.ForceRebuildLayoutImmediate(GetRectTransform());
		}
	}
}
