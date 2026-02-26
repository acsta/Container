using UnityEngine;

namespace TaoTie
{
    public class Box: Unit,IEntity<int>
    {
        private static int FresnelPow = Shader.PropertyToID("_FresnelPow");
        private static int FresnelIntensity = Shader.PropertyToID("_FresnelIntensity");
        private static int FresnelTint = Shader.PropertyToID("_FresnelTint");
        private MeshRenderer[] renderers;
        private ListComponent<Material> baseMats;
        public int ItemId { get; private set; }
        public ItemConfig ItemConfig => ItemConfigCategory.Instance.Get(ItemId);
        public override EntityType Type => EntityType.Box;
        
        /// <summary>
        /// 生成后的物品类型(只有Normal物品算钱)
        /// </summary>
        public BoxType BoxType;
        public int ItemResultId { get; private set; }
        public ItemConfig ItemResult => ItemConfigCategory.Instance.Get(ItemResultId);
        /// <summary>
        /// 玩法修改后的价格(不需要和情报计算)
        /// </summary>
        public BigNumber Price{ get; private set; }
        public void Init(int p1)
        {
            BoxType = BoxType.Normal;
            ItemId = p1;
            ConfigId = ItemConfig.UnitId;
            ItemResultId = 0;
            Price = null;
            InitAsync().Coroutine();
        }

        private async ETTask InitAsync()
        {
            var ghc = AddComponent<GameObjectHolderComponent>();
            if (!PlayTypeConfigCategory.Instance.Contain(ItemConfig.Type)) return;
            await ghc.WaitLoadGameObjectOver();
            if(IsDispose) return;
            renderers = ghc.EntityView.GetComponentsInChildren<MeshRenderer>();
            baseMats = ListComponent<Material>.Create();
            var playType = PlayTypeConfigCategory.Instance.Get(ItemConfig.Type);
            if (!ColorUtility.TryParseHtmlString(playType.FresnelTint, out var color))
            {
                color = Color.white;
            }
            for (int i = 0; i < renderers.Length; i++)
            {
                using ListComponent<Material> temp = ListComponent<Material>.Create();
                for (int j = 0; j < renderers[i].sharedMaterials.Length; j++)
                {
                    baseMats.Add(renderers[i].sharedMaterials[j]);
                    var mat = Material.Instantiate(renderers[i].sharedMaterials[j]);
                    temp.Add(mat);
                    mat.SetColor(FresnelTint, color);
                    mat.SetFloat(FresnelIntensity, playType.FresnelIntensity);
                    mat.SetFloat(FresnelPow, playType.FresnelPow);
                }
                renderers[i].SetMaterials(temp);
            }
        }
        public void Destroy()
        {
            if (renderers != null)
            {
                int index = 0;
                for (int i = 0; i < renderers.Length; i++)
                {
                    using ListComponent<Material> temp = ListComponent<Material>.Create();
                    for (int j = 0; j < renderers[i].materials.Length; j++)
                    {
                        temp.Add(baseMats[index]);
                        var mat = renderers[i].materials[j];
                        index++;
                        GameObject.Destroy(mat);
                    }
                    
                    renderers[i].SetMaterials(temp);
                }
                renderers = null;
                baseMats?.Dispose();
                baseMats = null;
            }
            ItemResultId = 0;
            Price = null;
        }
        public void SetAppraisalResult(int p1)
        {
            ItemResultId = p1;
        }
        public void SetMiniGameResult(BigNumber p1)
        {
            Price = p1;
        }
        /// <summary>
        /// 获取最终价格
        /// </summary>
        /// <returns></returns>
        public BigNumber GetFinalPrice(GameInfoConfig config)
        {
            if(BoxType != BoxType.Normal) return BigNumber.Zero;
            if (Price != null)
            {
                return Price;
            }
            //没用过的检疫单不算价格
            if(ItemConfig.Type == (int)ItemType.Quarantine) return BigNumber.Zero;
            if (ItemResultId != 0)
            {
                if (config != null)
                {
                    return config.GetItemPrice(ItemResultId);
                }
                return ItemResult.Price;
            }
            if (config != null)
            {
                return config.GetItemPrice(ItemId, Price);
            }
            return ItemConfig.Price;
        }
        
        public BigNumber GetGamInfoAddon(GameInfoConfig config)
        {
            if(BoxType != BoxType.Normal || config==null) return BigNumber.Zero;
            if (ItemResultId != 0)
            {
                return config.GetItemAddOn(ItemResultId);
            }
            return config.GetItemAddOn(ItemId);
        }
    }
}