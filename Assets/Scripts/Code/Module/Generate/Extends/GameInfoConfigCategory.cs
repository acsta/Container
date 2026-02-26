using System.Collections.Generic;

namespace TaoTie
{
    public partial class GameInfoConfigCategory
    {
        private UnOrderMultiMap<int, GameInfoConfig> map;
        private List<GameInfoConfig> empty = new List<GameInfoConfig>();
        public override void AfterEndInit()
        {
            base.AfterEndInit();
            map = new UnOrderMultiMap<int, GameInfoConfig>();
            for (int i = 0; i < list.Count; i++)
            {
                map.Add(list[i].Level, list[i]);
            }
        }

        public List<GameInfoConfig> GetByLevel(int lv)
        {
            if (map.TryGetValue(lv, out var res))
            {
                return res;
            }

            return empty;
        }
    }
    
    public partial class GameInfoConfig: II18NSwitchConfig
    {
        /// <summary>
        /// 临时随机的物品
        /// </summary>
        public List<int> TempItems { get; private set; }
        /// <summary>
        /// 是否目标物品
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool IsTargetItem(ItemConfig item)
        {
            if (Type == (int)GameInfoTargetType.Container)
            {
                for (int i = 0; i < Ids.Length; i++)
                {
                    if (Ids[i] == item.ContainerId) return true;
                }
            }
            else if (Type == (int)GameInfoTargetType.Items)
            {
                for (int i = 0; i < Ids.Length; i++)
                {
                    if (Ids[i] == item.Id) return true;
                }
            }
            else if (Type == (int)GameInfoTargetType.RandItems)
            {
                return TempItems != null && TempItems.Contains(item.Id);
            }
            else if (Type == (int)GameInfoTargetType.PlayType)
            {
                var type = item.Type / 10 * 10;
                for (int i = 0; i < Ids.Length; i++)
                {
                    if (Ids[i] == type) return true;
                }
            }
            return false;
        }
        /// <summary>
        /// 获取物品价格
        /// </summary>
        /// <param name="itemId"></param>
        /// <param name="changePrice">玩法修改后的价格</param>
        /// <returns></returns>
        public BigNumber GetItemPrice(int itemId,BigNumber changePrice = null)
        {
            var item = ItemConfigCategory.Instance.Get(itemId);
            if (IsTargetItem(item))
            {
                if (AwardType == 0)
                {
                    return (changePrice == null?item.Price:changePrice) + RewardCount;
                }
                else if (AwardType == 1)
                {
                    return (changePrice == null?item.Price:changePrice) * RewardCount;
                }
                else
                {
                    Log.Error("未处理的类型AwardType = " + AwardType);
                }
                return changePrice == null?item.Price:changePrice;
            }

            return changePrice == null?item.Price:changePrice;
        }
        /// <summary>
        /// 获取附加价格
        /// </summary>
        /// <param name="itemId"></param>
        /// <returns></returns>
        public BigNumber GetItemAddOn(int itemId)
        {
            var item = ItemConfigCategory.Instance.Get(itemId);
            if (IsTargetItem(item))
            {
                if (AwardType == 0)
                {
                    return RewardCount;
                }
                else if (AwardType == 1)
                {
                    return item.Price * (RewardCount - 1);
                }
                else
                {
                    Log.Error("未处理的类型AwardType = " + AwardType);
                }
                return BigNumber.Zero;
            }

            return BigNumber.Zero;
        }
        /// <summary>
        /// 获取最终抬价奖励
        /// </summary>
        /// <param name="playerRaiseRewardsMoney"></param>
        /// <returns></returns>
        public BigNumber GetRaiseRewards(BigNumber playerRaiseRewardsMoney)
        {
            if (Type == (int) GameInfoTargetType.Raise)
            {
                if (AwardType == 0)
                {
                    playerRaiseRewardsMoney += RewardCount;
                }
                else  if (AwardType == 1)
                {
                    playerRaiseRewardsMoney *= RewardCount;
                }
            }
            return playerRaiseRewardsMoney;
        }
        public string GetI18NText(LangType lang)
        {
            switch (lang)
            {
                case LangType.Chinese:
                    return Chinese;
                default:
                case LangType.English:
                    return English;
            }
        }

        public string GetI18NText(LangType lang, int type = 0)
        {
            if (type == 1)
            {
                switch (lang)
                {
                    case LangType.Chinese:
                        return ChineseDesc;
                    default:
                    case LangType.English:
                        return EnglishDesc;
                }
            }
            return GetI18NText(lang);
        }

        /// <summary>
        /// 生成随机物品
        /// </summary>
        public void GenerateTempItems()
        {
            if (Type != (int)GameInfoTargetType.RandItems) return;
            if (TempItems == null)
            {
                TempItems = new List<int>();
            }
            else
            {
                TempItems.Clear();
            }

            if (Ids == null || Ids.Length != 2)
            {
                Log.Error("GameInfoConfig Ids配置错误 id="+Id);
                return;
            }
            var items = ItemConfigCategory.Instance.GetItemIdsByContainerAndType(Ids[0], ItemType.None);
            items.RandomSort();
            for (int i = 0; i < Ids[1]; i++)
            {
                TempItems.Add(items[i].Id);
            }
        }
    }
}