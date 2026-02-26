using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace TaoTie
{
    public class AltasEditor
    {

        [MenuItem("Tools/工具/UI/设置图片", false, 31)]
        public static void SettingPNG()
        {
            AtlasHelper.SettingPNG();
        }
        
        [MenuItem("Tools/工具/UI/生成图集", false, 32)]
        public static void ClearAllAtlasAndGenerate()
        {
            try
            {
                AssetDatabase.StartAssetEditing();
                AtlasHelper.GeneratingAtlas();
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
            }
            finally
            {
                AssetDatabase.StopAssetEditing();
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
        }
        
        [MenuItem("Tools/工具/UI/搜索或批量替换Sprite", false, 502)]
        public static void ReplaceImage()
        {
            Rect _rect = new Rect(0, 0, 900, 600);
            ReplaceImage window = EditorWindow.GetWindowWithRect<ReplaceImage>(_rect, true, "搜索或批量替换Sprite");
            window.Show();
        }
        [MenuItem("Tools/工具/UI/查找未使用的图片", false, 503)]
        public static void CheckUnUseImage()
        {
            Rect _rect = new Rect(0, 0, 900, 600);
            CheckUnuseImage window = EditorWindow.GetWindowWithRect<CheckUnuseImage>(_rect, true, "查找未使用的图片");
            window.Show();
        }

        [MenuItem("Tools/工具/UI/检查丢失image", false, 504)]
        public static void CheckLossImage()
        {
            Rect _rect = new Rect(0, 0, 900, 600);
            CheckEmptyImage window = EditorWindow.GetWindowWithRect<CheckEmptyImage>(_rect, true, "检查预设丢失image");
            window.Show();
        }
        [MenuItem("Tools/工具/UI/创建图片字体", false, 500)]
        [MenuItem("Assets/工具/UI/创建图片字体", false, 203)]
        public static void CreateArtFont()
        {
            ArtistFont.BatchCreateArtistFont();
        }
        [MenuItem("Tools/工具/UI/裁剪字体", false, 501)]
        public static void FontSubset()
        {
            Rect _rect = new Rect(0, 0, 900, 600);
            FontSubsetEditor window = EditorWindow.GetWindowWithRect<FontSubsetEditor>(_rect, true, "裁剪字体");
            window.Show();
        }
        
        [MenuItem("Tools/工具/TA/资源分析输出excel", false, 202)]
        public static void ResourceAnalysis()
        {
            ResourceCheckTool.ResourceAnalysis();
        }
        
        [MenuItem("Tools/工具/TA/资源可视化窗口", false, 208)]
        public static void OpenWindow()
        {
            ArtToolsWindow.OpenWindow();
        }

        [MenuItem("Tools/工具/TA/Fbx压缩工具", false, 54)]
        [MenuItem("Assets/工具/TA/Fbx压缩工具", false, 54)]
        public static void ShowFbxToolWindow()
        {
            FbxHelperWindow.ShowWindow();
        }
        [MenuItem("Tools/工具/TA/依赖查找", false, 208)]
        public static void OpenDependWindow()
        {
            DependWindow.ShowWindow();
        }
        [MenuItem("Tools/工具/TA/设置场景贴图格式", false, 208)]
        public static void SetLightMap()
        {
            AtlasHelper.SetSceneTextures();
        }
        
        [MenuItem("Assets/工具/UI/生成图集",false,400)]
        static void ClearSelectionAtlasAndGenerate()
        {
            string[] guids = Selection.assetGUIDs;
            for (int i = 0; i < guids.Length; i++)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[i]);
                if (Directory.Exists(path))
                {
                    var vs = path.Split("/");
                    if (vs[vs.Length - 1] == "UIItem" || vs[vs.Length - 1] == "UICloth" )
                    {
                        string uiPath = Path.Combine(Application.dataPath, "AssetsPackage", vs[vs.Length - 1]);
                        DirectoryInfo uiDirInfo = new DirectoryInfo(uiPath);
                        foreach (DirectoryInfo dirInfo in uiDirInfo.GetDirectories())
                        {
                            AtlasHelper.SetImagesFormat(dirInfo, AtlasHelper.ImageType.DiscreteImages);
                        }
                    }
                    else if (vs[vs.Length - 2] == "AssetsPackage")
                    {
                        for (int j = 0; j < AtlasHelper.uipaths.Length; j++)
                        {
                            if (vs[vs.Length - 1] == AtlasHelper.uipaths[j])
                            {
                                //将UI目录下的Atlas 打成 图集
                                string uiPath = Path.Combine(Application.dataPath, "AssetsPackage", AtlasHelper.uipaths[j]);
                                DirectoryInfo uiDirInfo = new DirectoryInfo(uiPath);
                                foreach (DirectoryInfo dirInfo in uiDirInfo.GetDirectories())
                                {
                                    AtlasHelper.GeneratingAtlasByDir(dirInfo);
                                }
                            }
                           
                        }
                    }
                    else if (vs[vs.Length - 3] == "AssetsPackage")
                    {
                        for (int j = 0; j < AtlasHelper.uipaths.Length; j++)
                        {
                            if (vs[vs.Length - 2] == AtlasHelper.uipaths[j])
                            {
                                //将UI目录下的Atlas 打成 图集
                                DirectoryInfo uiDirInfo = new DirectoryInfo(path);
                                AtlasHelper.GeneratingAtlasByDir(uiDirInfo);
                            }
                           
                        }
                    }
                }
            }
        }

        [MenuItem("Assets/复制相对路径", false, 500)]
        static void CopyPath()
        {
            string[] guids = Selection.assetGUIDs;
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < guids.Length; i++)
            {
                if (i > 0) sb.AppendLine();
                string path = AssetDatabase.GUIDToAssetPath(guids[i]);
                if(path.Contains("/AssetsPackage/"))
                    path = path.Split("/AssetsPackage/")[1];
                sb.Append(path);
            }

            GUIUtility.systemCopyBuffer = sb.ToString();
        }
        [MenuItem("Assets/复制名称", false, 500)]
        static void CopyName()
        {
            string[] guids = Selection.assetGUIDs;
            StringBuilder sb = new StringBuilder();
            List<string> paths = new List<string>();
            for (int i = 0; i < guids.Length; i++)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[i]);
                paths.Add(path);
            }
            paths.Sort();
            for (int i = 0; i < paths.Count; i++)
            {
                string path = paths[i];
                if (i > 0) sb.AppendLine();
                sb.Append(Path.GetFileNameWithoutExtension(path));
            }
            GUIUtility.systemCopyBuffer = sb.ToString();
        }
    }
}