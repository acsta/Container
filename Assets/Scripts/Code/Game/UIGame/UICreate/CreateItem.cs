using System;

namespace TaoTie
{
    public class CreateItem : UIBaseContainer, IOnCreate
    {
        public UIButton Button;
        public UIImage Type;

        private Action<int, int> onClickSelf;
        private int id, moduleId;
        public void OnCreate()
        {
            Button = AddComponent<UIButton>();
            Type = AddComponent<UIImage>("Type");
            Button.SetOnClick(OnClickSelf);
        }

        public void SetData(int moduleId, Action<int, int> onClick, int clothId)
        {
            onClickSelf = onClick;
            this.moduleId = moduleId;
            var module = CharacterConfigCategory.Instance.Get(moduleId);
            this.id = clothId;
            if (clothId != module.DefaultCloth && clothId > 0)
            {
                ClothConfig config = ClothConfigCategory.Instance.Get(clothId);
                Type.SetSpritePath(config.Icon).Coroutine();
                var icon = RareConfigCategory.Instance.GetRare(config.Rare).Icon;
                Button.SetSpritePath($"UIGame/UICreate/Atlas/{icon}.png").Coroutine();
            }
            else
            {
                CharacterConfig config = CharacterConfigCategory.Instance.Get(moduleId);
                Type.SetSpritePath(config.Icon).Coroutine();
                Button.SetSpritePath("UIGame/UICreate/Atlas/gray.png").Coroutine();
            }
            
        }

        private void OnClickSelf()
        {
            onClickSelf?.Invoke(id,moduleId);
        }
    }
}