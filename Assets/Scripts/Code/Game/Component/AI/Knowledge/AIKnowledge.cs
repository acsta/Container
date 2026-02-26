using System;

namespace TaoTie
{
    public class AIKnowledge: IDisposable
    {
        public Entity Entity;
        public string DecisionArchetype;
        public AIConfig Config => Entity.GetComponent<BidderComponent>().Config;

        /// <summary> 当前剩余资金 </summary>
        public BigNumber Money;
        /// <summary> 有了判断误差范围后，生成对总价值的判断。当deviation变化后，judge会重新生成 </summary>
        public BigNumber Judge;
        /// <summary> 是否已经触发过消极 </summary>
        public bool IsNegative;
        /// <summary> 本关卡剩余喊价次数。被消极度影响。为0的时候本关卡就不再喊价 </summary>
        public long BidCount;
        /// <summary> 判断误差范围.中途会变动 </summary>
        public float DeviationMin;
        /// <summary> 判断误差范围.中途会变动 </summary>
        public float DeviationMax;
        /// <summary> 上次叫价时间 </summary>
        public long LastBidTime;
        /// <summary> 复仇对象Id </summary>
        public long RevengeTarget;
        /// <summary> 剩余复仇次数 </summary>
        public int RevengeCount;
        /// <summary> 是否已激活诱导抬价 </summary>
        public bool IsRaisePrice;
        /// <summary> 剩余诱导抬价次数 </summary>
        public int RaisePriceCount;
        /// <summary> 墙头草是否已经触发过跟风 </summary>
        public bool IsFollow;
        /// <summary> 是否志在必得(特殊盲盒关卡小几率出现) </summary>
        public bool DeterminedToHave;
        /// <summary> 随机加价权值 </summary>
        public readonly BigNumber[] Width = new BigNumber[4];
        /// <summary> 是否被高价震慑 </summary>
        public bool IsHighPriceDeterrence;
        /// <summary> 是否生气 </summary>
        public bool IsAnger;
        public void Init(Entity aiEntity, string config)
        {
            Entity = aiEntity;
            DecisionArchetype = config;
            LastBidTime = int.MinValue;
            RevengeTarget = -1;
            RevengeCount = 0;
            IsRaisePrice = false;
            DeterminedToHave = false;
            Money = Config.InitMoney;
            Width[0] = Config.SidelinesWeight;
            Width[1] = Config.LowWeight;
            Width[2] = Config.MediumWeight;
            Width[3] = Config.HighWeight;
            for (int i = 1; i < Width.Length; i++)
            {
                Width[i] += Width[i - 1];
            }

            IsHighPriceDeterrence = false;
            IsAnger = false;
        }

        public void Dispose()
        {
            Entity = null;
            DecisionArchetype = null;
            LastBidTime = int.MinValue;
            RevengeTarget = -1;
            RevengeCount = 0;
            IsRaisePrice = false;
            DeterminedToHave = false;
            IsHighPriceDeterrence = false;
            IsAnger = false;
        }

        public void Ready(float prefabDeviation)
        {
            if (prefabDeviation == BigNumber.Zero) 
            {
                DeviationMin = Config.Deviation[0];
                DeviationMax = Config.Deviation[1];
            }
            else
            {
                DeviationMin = prefabDeviation;
                DeviationMax = prefabDeviation;
            }

            BidCount = 9999;
            IsNegative = false;
            //该npc激活消极
            if (UnityEngine.Random.Range(0f, 1f) < Config.Negative)
            {
                int ran = UnityEngine.Random.Range(1, Config.TotalWight);
                for (int i = 0; i < Config.NegativeBehaviorWight.Length; i++)
                {
                    if (ran < Config.NegativeBehaviorWight[i])
                    {
                        IsNegative = true;
                        BidCount = Config.NegativeBehaviorArray[i];
                        Log.Info("<color=#88FFEF>[Auction]</color> AI" + Entity.Id + "触发消极。消极喊价次数是" + Config.NegativeBehaviorArray[i]);
                        break;
                    }
                }
            }
            LastBidTime = int.MinValue;
            IsRaisePrice = false;
            RaisePriceCount = 0;
            IsFollow = false;
            DeterminedToHave = false;
            IsHighPriceDeterrence = false;
            IsAnger = false;
        }
    }
}