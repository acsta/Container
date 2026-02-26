using System.Collections.Generic;

namespace TaoTie
{
    public partial class ShareConfigCategory
    {
        private MultiMap<int, ShareConfig> groups;
        public override void AfterEndInit()
        {
            base.AfterEndInit();
            groups = new MultiMap<int, ShareConfig>();
            for (int i = 0; i < list.Count; i++)
            {
                groups.Add(list[i].Scene, list[i]);
            }
        }

        public List<ShareConfig> GetShareConfig(int scene)
        {
            if (groups.TryGetValue(scene, out var res))
            {
                return res;
            }

            return null;
        }
    }
}