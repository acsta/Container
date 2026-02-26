using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using UnityEditor;
using UnityEditor.Animations;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using Object = UnityEngine.Object;

namespace TaoTie
{
    public class EnableGPUInstance : EditorWindow
    {
        List<Material> m_materials = new List<Material>();
        
        [MenuItem("Tools/工具/TA/Enable GPU Instance")]
        private static void ShowWindow()
        {
            var window = GetWindow<EnableGPUInstance>();
            window.titleContent = new GUIContent("Enable GPU Instance");
            window.Show();
        }
        
        private void OnGUI()
        {
            var beginTime = System.DateTime.UtcNow.Ticks;
            m_materials.Clear();
            
            EditorGUILayout.BeginHorizontal();
            
            if (GUILayout.Button("确定"))
            {
                var ids = AssetDatabase.FindAssets("t:GameObject", new[]
                {
                    "Assets/AssetsPackage",
                });

                List<string> paths = new List<string>();
                for (int i = 0; i < ids.Length; ++i)
                {
                    var path = AssetDatabase.GUIDToAssetPath(ids[i]);
                    paths.Add(path);
                }

                for (int i = 0; i < paths.Count; ++i)
                {
                    var GO = AssetDatabase.LoadAssetAtPath<GameObject>(paths[i]);
                    var renders = GO.GetComponentsInChildren<Renderer>();
                    if (renders.Length == 0) continue;

                    for (int renderIndex = 0; renderIndex < renders.Length; ++renderIndex)
                    {
                        for (int materialIndex = 0; materialIndex < renders[renderIndex].sharedMaterials.Length; ++materialIndex)
                        {
                            m_materials.Add(renders[renderIndex].sharedMaterials[materialIndex]);
                        }   
                    }
                    
                }

                ProcessMaterials();
                var endTime = System.DateTime.UtcNow.Ticks;
                var deltaTime = endTime - beginTime;
                Debug.Log($"found and enable {m_materials.Count} material's GPU Instance. Took {deltaTime / 10000.0} ms.");

            }
            EditorGUILayout.EndHorizontal();
        }

        void ProcessMaterials()
        {
            foreach (var material in m_materials)
            {
                material.enableInstancing = true;
            }
        }
    }
}