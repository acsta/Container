using System.Collections.Generic;
using UnityEngine;

namespace TaoTie
{
    public class BlackBoyComponent: Component, IComponent
    {
        public int ConfigId { get; private set; } = 100001;
        public SaleEventConfig SaleEventConfig => SaleEventConfigCategory.Instance.Get(ConfigId);
            
        private GameObjectHolderComponent ghc => parent.GetComponent<GameObjectHolderComponent>();
        private SkinnedMeshRenderer[] skins;
        public void Init()
        {
            skins = ghc.EntityView.GetComponentsInChildren<SkinnedMeshRenderer>();
            for (int i = 0; i < skins.Length; i++)
            {
                var skin = skins[i];
                for (int j = 0; j < skin.materials.Length; j++)
                {
                    var newMat = skin.materials[j];
                    newMat.color = Color.black;
                }
            }

            var list = SaleEventConfigCategory.Instance.GetAllList();
            var index = Random.Range(0, list.Count);
            ConfigId = list[index].Id;
        }
        public void Destroy()
        {
            if (skins != null)
            {
                for (int i = 0; i < skins.Length; i++)
                {
                    var skin = skins[i];
                    for (int j = 0; j < skin.materials.Length; j++)
                    {
                        GameObject.Destroy(skin.materials[j]);
                        skin.materials[j] = skin.sharedMaterials[j];
                    }
                }

                skins = null;
            }
        }
    }
}