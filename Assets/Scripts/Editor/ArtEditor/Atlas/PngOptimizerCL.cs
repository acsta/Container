#if UNITY_EDITOR_WIN
using System.IO;
using UnityEditor;
using UnityEngine;

namespace TaoTie
{
    public static class PngOptimizerCL
    {
        private const string program = "Tools/PngOptimizerCL/PngOptimizerCL.exe";
        
        [MenuItem("Tools/工具/TA/批量压缩图片", false, 100)]
        public static void ProcessImage()
        {
            var workSpace = Directory.GetParent(Application.dataPath).ToString();
            var guids = AssetDatabase.FindAssets("t:Texture", new string[]{"Assets/AssetsPackage","Assets/Resources"});
            foreach (var guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                if (path.Contains("Tmp") || path.Contains("Fonts") || path.Contains("FmodBanks") ||
                    path.Contains("Shaders"))
                {
                    continue;
                }

                if (path.Contains("/Atlas/"))
                {
                    continue;
                }

                BashUtil.RunCommand(workSpace, program, $"-file:\"{path}\"");
            }
            AssetDatabase.Refresh();
        }
    }
}
#endif