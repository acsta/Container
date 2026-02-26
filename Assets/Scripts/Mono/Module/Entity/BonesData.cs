using UnityEngine;

namespace TaoTie
{
    public class BonesData: MonoBehaviour
    {
        public string[] bones;
#if UNITY_EDITOR
        [Sirenix.OdinInspector.Button("搜集骨骼信息")]
        public void Collect()
        {
            var smr = GetComponent<SkinnedMeshRenderer>();
            if (smr == null || smr.bones == null || smr.bones.Length == 0) return;
            // 获取新骨骼层级
            bones = new string[smr.bones.Length];
                   
            // 重新映射骨骼
            for (int j = 0; j < smr.bones.Length; j++)
            {
                string boneName = smr.bones[j].name;
                bones[j] = boneName;
            }
        }
#endif
    }
}