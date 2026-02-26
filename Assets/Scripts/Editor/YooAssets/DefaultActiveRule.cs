namespace YooAsset.Editor
{
    [DisplayName("AOT分组")]
    public class AOTGroup : IActiveRule
    {
        public bool IsActiveGroup(GroupData data)
        {
            return HybridCLR.Editor.SettingsUtil.Enable;
        }
    }
}