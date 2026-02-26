using System;
using System.Collections.Generic;
using SuperScrollView;
using UnityEngine;

namespace TaoTie
{
    public class UIBagWin: UIBaseContainer,IOnCreate
    {
        private int moduleId;
        public UILoopListView2 ScrollView;
        public UIButton BagClose;
        public UIMenu Menu;
        public UIAnimator Table;
        public GroupInfoTable GroupInfoTable;
        private List<ClothConfig> clothConfigs;

        private Action<int, int> onClickItem;
        public void OnCreate()
        {
            clothConfigs = new List<ClothConfig>();
            GroupInfoTable = AddComponent<GroupInfoTable>("Top/Tip");
            this.ScrollView = this.AddComponent<UILoopListView2>("ScrollView");
            this.ScrollView.InitListView(0,GetScrollViewItemByIndex);
            Menu = AddComponent<UIMenu>("Top/UIMenu");
            Table = AddComponent<UIAnimator>();
            BagClose = AddComponent<UIButton>("Top/CloseBag");
        }

        public void SetData(Action<int, int> onClickItem)
        {
            this.onClickItem = onClickItem;
            if (!ColorUtility.TryParseHtmlString("#FCC63A", out var activeColor))
            {
                activeColor = Color.white;
            }
            if (!ColorUtility.TryParseHtmlString("#AF5C09", out var unActiveColor))
            {
                unActiveColor = Color.white;
            }
            List<MenuPara> paras = new List<MenuPara>();
            var list = CharacterConfigCategory.Instance.GetAllList();
            for (int i = 1; i < list.Count; i++)
            {
                paras.Add(new MenuPara()
                {
                    Id = list[i].Id,
                    ImgPath = list[i].Icon,
                    ActiveColor = activeColor,
                    UnActiveColor = unActiveColor
                });
            }
            Menu.SetData(paras, OnMenuChange, changeScale: false);
            BagClose.SetOnClick(OnClickCloseBag);
        }
        public void Open(Action<int, int> onClickItem)
        {
            Table.Play("Bag_open").Coroutine();
            SetData(onClickItem);
            var id = moduleId;
            moduleId = -1;
            ShowList(id);
        }

        private void OnMenuChange(MenuPara para)
        {
            ShowList(para.Id);
        }
        
        public LoopListViewItem2 GetScrollViewItemByIndex(LoopListView2 listView, int index)
        {
            var table = (clothConfigs.Count +  4) / 4;
            if (index < 0 || index>= table) return null;
			
            var item = listView.NewListViewItem("TableItem",index);
            TableItem tableItem;
            if (!item.IsInitHandlerCalled)
            {
                item.IsInitHandlerCalled = true;
                tableItem = ScrollView.AddItemViewComponent<TableItem>(item);
            }
            else
            {
                tableItem = ScrollView.GetUIItemView<TableItem>(item);
            }
            var y = tableItem.GetRectTransform().sizeDelta.y;
            var x = ScrollView.GetRectTransform().rect.width;
            tableItem.GetRectTransform().sizeDelta = new Vector2(x, y);
            tableItem.SetData(moduleId, clothConfigs, index, OnClickItem);
            return item;
        }
        
        public void ShowList(int id)
        {
            if (moduleId == id) return;
            moduleId = id;
            var temp = ClothConfigCategory.Instance.GetModule(id);
            var module = CharacterConfigCategory.Instance.Get(id);
            clothConfigs.Clear();
            for (int i = 0; i < temp.Count; i++)
            {
                if (temp[i].Id != module.DefaultCloth && PlayerDataManager.Instance.GetItemCount(temp[i].Id) > 0)
                {
                    clothConfigs.Add(temp[i]);
                }
            }

            clothConfigs.Sort((a, b) =>
            {
                return b.Rare - a.Rare;
            });
            var table = (clothConfigs.Count + 4) / 4;
            ScrollView.SetListItemCount(table);
            ScrollView.RefreshAllShownItem();
            Menu.SetActiveIndex(moduleId - 2);
        }
        
        private void OnClickCloseBag()
        {
            Table.Play("Bag_close").Coroutine();
        }
        
        private void OnClickItem(int id, int moduleId)
        {
           onClickItem?.Invoke(id, moduleId);
        }
    }
}