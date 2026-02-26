#if ODIN_INSPECTOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using UnityEditor;
using UnityEditor.Animations;
using UnityEditor.SceneManagement;
using UnityEngine;
using Object = UnityEngine.Object;

namespace TaoTie
{

    public class PrefabProcessEditor: OdinEditorWindow
    {
        
        [ShowIf(nameof(ProcessAssetType),AssetType.SceneObject)][ValueDropdown(nameof(GetTypeId),AppendNextDrawer = true)]
        public string TypeName;
        [ShowIf(nameof(ProcessAssetType),AssetType.SceneObject)][ShowIf("@"+nameof(TypeName)+"!=null && "+nameof(TypeName)+"!=\"\"")]
        [ValueDropdown(nameof(GetLabelId),AppendNextDrawer = true)]
        public string Label;
        
    
        public IEnumerable GetTypeId()
        {
            var config = AssetDatabase.LoadAssetAtPath<AssetsManagerConfig>(AssetsManagerConfig.ConfigPath);
            ValueDropdownList<string> list = new ValueDropdownList<string>();
            if (config == null||config.Labels == null) return list;
            if (config.Labels.Count > 0)
            {
                for (int i = 0; i < config.Labels.Count; i++)
                {
                    list.Add(config.Labels[i].Label);
                }
               
            }
            return list;
        }

        public IEnumerable GetLabelId()
        {
            var config = AssetDatabase.LoadAssetAtPath<AssetsManagerConfig>(AssetsManagerConfig.ConfigPath);
            ValueDropdownList<string> list = new ValueDropdownList<string>();
            HashSet<string> temp = new HashSet<string>();
            if (config == null||config.Labels == null) return list;
            if (config.Labels.Count > 0)
            {
                for (int i = 0; i < config.Labels.Count; i++)
                {
                    if (TypeName == config.Labels[i].Label)
                    {
                        if (config.Labels[i].Collects != null)
                        {
                            for (int j = 0; j < config.Labels[i].Collects.Count; j++)
                            {
                                temp.Add(config.Labels[i].Collects[j].Label);
                            }
                            
                        }
                    }
                }

                foreach (var i in temp)
                {
                    list.Add(i);
                }
            }
            return list;
        }

        [MenuItem("Tools/工具/TA/资源入库", false, 160)]
        public static void GeneratingAtlas()
        {
            GetWindow(typeof(PrefabProcessEditor));
        }

        public string SubPath;
        public GameObject[] Prefabs = new GameObject[0];

        public AssetType ProcessAssetType;
        
        [Button("入库")]
        public void Process()
        {
            if (Prefabs == null) return;
            
            ProcessHelper.ProcessPrefabs(Prefabs, ProcessAssetType, SubPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
}
#endif