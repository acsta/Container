using System;

namespace TaoTie
{
    public class GroupInfo: UIBaseContainer,IOnCreate
    {
        public UITextmesh Count;
        public UIImage Icon;
        public UITextmesh Details;
        public void OnCreate()
        {
            Icon = AddComponent<UIImage>("Icon");
            Count = AddComponent<UITextmesh>("Icon/Count");
            Details = AddComponent<UITextmesh>("Details");
        }

        public void SetData(int index, int count, int effect, int param, bool active)
        {
            Icon.SetSpritePath($"UIGame/UICreate/Atlas/group{index}.png").Coroutine();
            Icon.SetImageGray(!active).Coroutine();
            Count.SetText(count.ToString());
            Details.SetI18NKey(Enum.Parse<I18NKey>("Text_Equip_Effect_" + effect+"_Short"), param);
            Details.SetTextColor(active?GameConst.WHITE_COLOR:GameConst.GRAY_COLOR);
        }
    }
}