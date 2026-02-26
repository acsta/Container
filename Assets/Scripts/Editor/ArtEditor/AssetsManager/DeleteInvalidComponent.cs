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
    public class DeleteInvalidComponent : EditorWindow
    {
        enum DeleteType : uint
        {
            None = 1 << 0,
            Script = 1 << 1
        }
        
        DeleteType filterType = DeleteType.None;
        List<GameObject> m_toFilterMeshs = new List<GameObject>();
        static int m_goCount;
        static int m_missingCount;
        
        [MenuItem("Tools/工具/TA/remove invalid component")]
        private static void ShowWindow()
        {
            var window = GetWindow<DeleteInvalidComponent>();
            window.titleContent = new GUIContent("Mesh Filter");
            window.Show();
        }
        
        private void OnGUI()
        {
            var beginTime = System.DateTime.UtcNow.Ticks;
            m_toFilterMeshs.Clear();
            
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
                    var mesh = AssetDatabase.LoadAssetAtPath<GameObject>(paths[i]);
                    m_toFilterMeshs.Add(mesh);
                }
                
                foreach (var go in m_toFilterMeshs)
                    search(go);
                var endTime = System.DateTime.UtcNow.Ticks;
                var deltaTime = endTime - beginTime;
                Debug.Log($"Searched in {m_goCount} GameObjects, found and removed {m_missingCount} missing scripts. Took {deltaTime / 10000.0} ms.");

            }
            EditorGUILayout.EndHorizontal();
        }

        [MenuItem("GameObject/Remove Missing Scripts")]
        static void Apply()
        {
            var beginTime = System.DateTime.UtcNow.Ticks;
            m_goCount = 0;
            m_missingCount = 0;
            
            foreach (var go in Selection.gameObjects)
                search(go);
            
            var endTime = System.DateTime.UtcNow.Ticks;
            var deltaTime = endTime - beginTime;

            Debug.Log($"Searched in {m_goCount} GameObjects, found and removed {m_missingCount} missing scripts. Took {deltaTime / 10000.0} ms.");

            
            //EditorSceneManager.SaveScene(scene);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorSceneManager.SaveOpenScenes();
        }
        
        static void search(GameObject go)
        {
            m_goCount++;
            m_missingCount += GameObjectUtility.RemoveMonoBehavioursWithMissingScript(go);
            foreach (Transform child in go.transform)
                search(child.gameObject);
        }
    }
}