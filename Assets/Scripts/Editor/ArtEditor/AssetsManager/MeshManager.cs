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
    public class MeshManager : EditorWindow
    {
        enum FilterType : uint
        {
            None = 1 << 0,
            Normal = 1 << 1,
            Tangent   = 1 << 2,
            Color  = 1 << 3,
        }
        FilterType filterType = FilterType.None;
        List<Mesh> m_toFilterMeshs = new List<Mesh>();
        
        [MenuItem("Tools/工具/TA/模型处理")]
        private static void ShowWindow()
        {
            var window = GetWindow<MeshManager>();
            window.titleContent = new GUIContent("Mesh Filter");
            window.Show();
        }

        private void OnGUI()
        {
            m_toFilterMeshs.Clear();
            ChooseFilterType();
            DoMeshFilter();
            
            //EditorSceneManager.SaveScene(scene);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorSceneManager.SaveOpenScenes();
        }

        private void ChooseFilterType()
        {
            EditorGUILayout.BeginHorizontal();
            
            EditorGUILayout.LabelField("处理类型", GUILayout.Width(70));
            filterType = (FilterType)EditorGUILayout.EnumPopup(filterType, GUILayout.Width(100));
            
            EditorGUILayout.EndHorizontal();
        }

        private void DoMeshFilter()
        {
            EditorGUILayout.BeginHorizontal();
            
            if (GUILayout.Button("模型处理"))
            {
                var ids = AssetDatabase.FindAssets("t:Mesh", new[] { "Assets/AssetsPackage" });

                List<string> paths = new List<string>();
                for (int i = 0; i < ids.Length; ++i)
                {
                    var path = AssetDatabase.GUIDToAssetPath(ids[i]);
                    paths.Add(path);
                }

                for (int i = 0; i < paths.Count; ++i)
                {
                    var mesh = AssetDatabase.LoadAssetAtPath<Mesh>(paths[i]);
                    m_toFilterMeshs.Add(mesh);
                }
                
                switch (filterType)
                {
                    case FilterType.Normal:
                    {
                        break;
                    }
                    case FilterType.Tangent:
                    {
                        DoProcessTangent();
                        break;
                    }
                    case FilterType.Color:
                    {
                        DoProcessColor();
                        break;
                    }
                    default:
                    {
                        Log.Error("Choose invalid filter type");
                        break;
                    }
                }
            }
            EditorGUILayout.EndHorizontal();
        }

        private void DoProcessTangent()
        {
            foreach (var mesh in m_toFilterMeshs)
            {
                if (mesh.tangents.Length > 0)
                {
                    mesh.tangents = Array.Empty<Vector4>();
                }
            }
        }
        
        private void DoProcessColor()
        {
            foreach (var mesh in m_toFilterMeshs)
            {
                if (mesh.colors.Length > 0)
                {
                    mesh.colors = Array.Empty<Color>();
                }
            }
        }
        
    }
}