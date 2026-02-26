using System;

namespace TaoTie
{
    public class ClothItem: UIBaseContainer,IOnCreate
    {
        public UIButton button;
        public UIImage Icon;
        public UITextmesh Name;
        public UIImage Rare;
        private Action<int,int> onClickItem;
        private int moduleId;
        private ClothConfig data;
        
        public void OnCreate()
        {
            Rare = AddComponent<UIImage>("Rare");
            Name = AddComponent<UITextmesh>("Title/Name");
            button = AddComponent<UIButton>();
            Icon = AddComponent<UIImage>("Icon");
            button.SetOnClick(OnClickSelf);
        }
        
        public void SetData(ClothConfig config, Action<int,int> onClickItem, int moduleId)
        {
            this.data = config;
            this.moduleId = moduleId;
            this.onClickItem = onClickItem;
            Icon.SetActive(data != null);
            Rare.SetActive(data != null);
            
            if (data != null)
            {
                Icon.SetSpritePath(config.Icon).Coroutine();
                Name.SetText(I18NManager.Instance.I18NGetText(config));
                var icon = RareConfigCategory.Instance.GetRare(config.Rare).Icon;
                Rare.SetSpritePath($"UIGame/UICreate/Atlas/bg_{icon}.png").Coroutine();
            }
            else
            {
                Name.SetI18NKey(I18NKey.Text_Empty);
            }
        }

        public void OnClickSelf()
        {
            if (data == null)
            {
                onClickItem?.Invoke(-1, moduleId);
            }
            else
            {
                onClickItem?.Invoke(data.Id, moduleId);
            }
            
        }
    }
}