using System;
using System.Collections.Generic;

namespace TaoTie
{
    public class TableItem: UIBaseContainer,IOnCreate
    {
        public ClothItem[] Cloths;
        public UIEmptyView Empty;
        public UIImage EmptyImage;
        public void OnCreate()
        {
            Cloths = new ClothItem[4];
            for (int i = 0; i < Cloths.Length; i++)
            {
                Cloths[i] = AddComponent<ClothItem>("ClothItem"+i);
            }

            Empty = AddComponent<UIEmptyView>("ClothItem0/Empty");
            EmptyImage = AddComponent<UIImage>("ClothItem0/Empty/Icon");
        }

        public void SetData(int moduleId, List<ClothConfig> data, int tab = 0, Action<int,int> onClickItem = null)
        {
            Empty.SetActive(tab == 0);
            for (int i = 0; i < 4; i++)
            {
                var index = tab * 4 + i - 1;
                Cloths[i].SetActive(index < data.Count);
                if (index < data.Count)
                {
                    Cloths[i].SetData(index >= 0 ? data[index] : null, index >= data.Count ? null : onClickItem,
                        moduleId);
                }
            }

            if (tab == 0)
            {
                var config = CharacterConfigCategory.Instance.Get(moduleId);
                EmptyImage.SetSpritePath(config.Icon).Coroutine();
            }
        }

    }
}