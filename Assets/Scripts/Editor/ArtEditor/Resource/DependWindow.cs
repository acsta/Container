#if ODIN_INSPECTOR
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

namespace TaoTie
{
    public class DependWindow: OdinEditorWindow
    {
        public static void ShowWindow()
        {
            DependWindow window = GetWindowWithRect<DependWindow>(new Rect(0, 0, 600, 600));
            window.titleContent = new GUIContent("依赖查找");
        }

        public Object target;

        [HideReferenceObjectPicker]
        public List<Object> depends = new List<Object>();
        [Button("查找")]
        public void Dependent()
        {
            depends.Clear();
            var path = AssetDatabase.GetAssetPath(target);
            var paths = AssetDatabase.GetDependencies(path, true);
            for (int i = 0; i < paths.Length; i++)
            {
                depends.Add(AssetDatabase.LoadAssetAtPath<Object>(paths[i]));
            }
        }
    }
}
#endif