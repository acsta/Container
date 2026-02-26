using UnityEngine;

namespace TaoTie
{
    public class ExportBones: MonoBehaviour
    {
#if UNITY_EDITOR
        [Sirenix.OdinInspector.Button("搜集骨骼信息")]
        public void Export()
        {
            var smrs = GetComponentsInChildren<SkinnedMeshRenderer>(true);
            for (int i = 0; i < smrs.Length; i++)
            {
                if (smrs[i].transform.parent.parent.name == "Parts")
                {
                    BonesData data = smrs[i].gameObject.GetComponent<BonesData>();
                    if (data == null)
                    {
                        data = smrs[i].gameObject.AddComponent<BonesData>();
                    }
                    data.Collect();
                    var prefab = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>(
                        "Assets/AssetsPackage/Unit/Charater/Prefabs/" + smrs[i].transform.parent.name + "/" +
                        smrs[i].transform.name + ".prefab");
                    var newData = prefab.GetComponent<BonesData>();
                    if (newData == null)
                    {
                        newData = prefab.AddComponent<BonesData>();
                    }
                    newData.bones = data.bones;
                    UnityEditor.EditorUtility.SetDirty(prefab);
                    UnityEditor.AssetDatabase.SaveAssetIfDirty(prefab);
                    GameObject.DestroyImmediate(data);
                }
            }
        }
#endif
    }
}