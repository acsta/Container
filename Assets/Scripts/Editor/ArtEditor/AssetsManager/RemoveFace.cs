// #if ODIN_INSPECTOR
// using System;
// using System.Collections;
// using System.Collections.Generic;
// using System.IO;
// using System.Linq;
// using System.Security.Cryptography;
// using System.Text;
// using Sirenix.OdinInspector;
// using Sirenix.OdinInspector.Editor;
// using Sirenix.Utilities;
// using UnityEditor;
// using UnityEditor.Animations;
// using UnityEditor.SceneManagement;
// using UnityEngine;
// using Object = UnityEngine.Object;
//
// namespace TaoTie
// {
//     public class RemoveFace : OdinEditorWindow
//     {
//         [Range(0f, 1f)]
//         public float Quality = 1;
//         public List<Mesh> FilterMeshs = new List<Mesh>();
//
//         [MenuItem("Tools/工具/TA/减面", false, 160)]
//         public static void OpenWindow()
//         {
//             GetWindow(typeof(RemoveFace));
//         }
//         
//         // [MenuItem("Tools/工具/TA/减面")]
//         // private static void ShowWindow()
//         // {
//         //     var window = GetWindow<RemoveFace>();
//         //     window.titleContent = new GUIContent("Remove Face");
//         //     window.Show();
//         // }
//
//         [Button("减面")]
//         private void Process()
//         {
//             //Quality = EditorGUILayout.Slider("Quality", Quality, 0f, 1f);
//             //FM = FilterMeshs.
//                 
//             DoRemoveFace();
//         }
//
//         private void DoRemoveFace()
//         {
//             foreach (var mesh in FilterMeshs)
//             {
//                 if (mesh == null)
//                 {
//                     Log.Error("Null mesh");   
//                 }
//                 
//                 var meshSimplifier = new UnityMeshSimplifier.MeshSimplifier();
//                 meshSimplifier.Initialize(mesh);
//                 
//                 meshSimplifier.SimplifyMesh(Quality);
//                 
//                 var newMesh = meshSimplifier.ToMesh();
//                 var meshPath = AssetDatabase.GetAssetPath(mesh);
//                 if (meshPath.EndsWith(".asset"))
//                 {
//                     meshPath = meshPath.Substring(0, meshPath.Length - ".asset".Length);
//                     meshPath += "_Low.asset";
//                 }
//                 AssetDatabase.CreateAsset(newMesh,
//                     meshPath);
//             }
//             
//             AssetDatabase.SaveAssets();
//             AssetDatabase.Refresh();
//             EditorSceneManager.SaveOpenScenes();
//         }
//     }   
// }
// #endif