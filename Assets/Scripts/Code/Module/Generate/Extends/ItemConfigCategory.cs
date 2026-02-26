using System.Collections.Generic;

namespace TaoTie
{
    public partial class ItemConfigCategory
    {

        private UnOrderDoubleKeyMap<int, int, ItemConfig> containerTypeMapId =
            new UnOrderDoubleKeyMap<int, int, ItemConfig>();

        private UnOrderMultiMap<int,ItemConfig> globalPlayTypeItems = new UnOrderMultiMap<int,ItemConfig>();
        private static readonly List<ItemConfig> Empty = new List<ItemConfig>();
        
        public override void AfterEndInit()
        {
            base.AfterEndInit();
            globalPlayTypeItems.Clear();
            containerTypeMapId.Clear();
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].Type == (int) ItemType.Story)
                {
                    list[i].Type = (int) ItemType.None;
                }

                if (list[i].Type > (int) ItemType.MAX)
                {
                    globalPlayTypeItems.Add(list[i].Type, list[i]);
                }
                else
                {
                    containerTypeMapId.Add(list[i].ContainerId,list[i].Type,list[i]);
                    if ((list[i].StoryIds?.Length ?? 0) > 0)
                    {
                        containerTypeMapId.Add(list[i].ContainerId, (int) ItemType.Story, list[i]);
                    }
                }
            }
        }

        /// <summary>
        /// 获取指定集装箱、玩法对应物品
        /// </summary>
        /// <param name="containerId"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public List<ItemConfig> GetItemIdsByContainerAndType(int containerId, ItemType type)
        {
            if (containerTypeMapId.TryGetList(containerId, (int)type, out var res))
            {
                return res;
            }

            return Empty;
        }

        /// <summary>
        /// 获取指定集装箱、玩法对应物品
        /// </summary>
        /// <returns></returns>
        public List<ItemConfig> GetGlobalItems(ItemType type)
        {
            if (globalPlayTypeItems.TryGetValue((int) type, out var res))
            {
                return res;
            }
            return Empty;
        }
    }
}