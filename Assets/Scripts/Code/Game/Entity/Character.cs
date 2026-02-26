using System.Collections.Generic;
using UnityEngine;

namespace TaoTie
{
    public abstract class Character: Unit, IEntity
    {
        public string RootName;
        protected int[] subModule;
        private Dictionary<int, GameObject> temp;
        private DictionaryComponent<string, Transform> boneDict;
        public string Name;
        public virtual void Init()
        {
            InitAsync().Coroutine();
        }
        protected virtual async ETTask InitAsync()
        {
            ConfigId = GameConst.CharacterUnitId;
            temp = new Dictionary<int, GameObject>();
            var ghc = AddComponent<GameObjectHolderComponent>();
            await ghc.WaitLoadGameObjectOver();
            boneDict = DictionaryComponent<string, Transform>.Create();
            CacheBones(boneDict, ghc.EntityView);
            for (int i = 0; i < subModule.Length; i++)
            {
                if (subModule[i] != 0)
                {
                    await SetModule(i + 1, subModule[i]);
                }
            }
        }
        
        public virtual void Destroy()
        {
            foreach (var item in temp)
            {
                var osmr = item.Value.GetComponent<SkinnedMeshRenderer>();
                osmr.rootBone = null;
                GameObjectPoolManager.GetInstance().RecycleGameObject(item.Value);
            }
            temp = null;
            boneDict?.Dispose();
            boneDict = null;
            RootName = null;
        }
        
        public async ETTask SetModule(int moduleId, int id)
        {
            if (id < 0)
            {
                if (temp.TryGetValue(moduleId, out var oldM))
                {
                    var osmr = oldM.GetComponent<SkinnedMeshRenderer>();
                    osmr.rootBone = null;
                    GameObjectPoolManager.GetInstance().RecycleGameObject(oldM);
                    temp.Remove(moduleId);
                    subModule[moduleId - 1] = 0;
                }
                return;
            }
            var config = ClothConfigCategory.Instance.Get(id);
            if (config.Module != moduleId)
            {
                Log.Error("设置衣服模块不对应 moduleId = " + moduleId + " clothId=" + id);
                return;
            }
            var ghc = GetComponent<GameObjectHolderComponent>();
            await ghc.WaitLoadGameObjectOver();
            if (IsDispose)
            {
                return;
            }
            var root = ghc.GetCollectorObj<Transform>(config.RootBone);
            if (root == null)
            {
                Log.Error("Not found "+config.RootBone);
                return;
            }
            var obj = await GameObjectPoolManager.GetInstance().GetGameObjectAsync(config.Path);
            if (IsDispose)
            {
                GameObjectPoolManager.GetInstance().RecycleGameObject(obj);
                return;
            }
            SkinnedMeshRenderer smr = obj.GetComponent<SkinnedMeshRenderer>();
            if (smr == null)
            {
                Log.Error("未找到SkinnedMeshRenderer");
                GameObjectPoolManager.GetInstance().RecycleGameObject(obj);
                return;
            }
            BonesData data = obj.GetComponent<BonesData>();
            if (data == null)
            {
                Log.Error("未找到BonesData");
                GameObjectPoolManager.GetInstance().RecycleGameObject(obj);
                return;
            }
            obj.transform.SetParent(ghc.EntityView);
            // 获取新骨骼层级
            Transform[] newBones = new Transform[smr.bones.Length];
            // 重新映射骨骼
            for (int j = 0; j < data.bones.Length; j++)
            {
                string boneName = data.bones[j];
                if (boneDict.TryGetValue(boneName, out Transform newBone))
                {
                    newBones[j] = newBone;
                }
                else
                {
                    Log.Error($"骨骼缺失: {boneName}");
                }
            }
            smr.bones = newBones;
            smr.rootBone = root;
            if (temp.TryGetValue(config.Module, out var old))
            {
                smr = old.GetComponent<SkinnedMeshRenderer>();
                smr.rootBone = null;
                GameObjectPoolManager.GetInstance().RecycleGameObject(old);
            }
            temp[config.Module] = obj;
            subModule[moduleId - 1] = id;
        }
        
        // 构建骨骼字典 (递归收集所有骨骼)
        private void CacheBones(DictionaryComponent<string, Transform> boneDict, Transform current)
        {
            boneDict[current.name] = current;
            foreach (Transform child in current)
            {
                CacheBones(boneDict, child);
            }
        }
    }
}