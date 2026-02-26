using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using YooAsset.Editor;
using YooAsset;
using System.IO.Compression;
using System.Linq;

namespace TaoTie
{
    public static class BuildHelper
    {
        const string programName = "jzxmh";
        /// <summary>
        /// 需要打全量首包的
        /// </summary>
        private static HashSet<string> buildAllChannel = new HashSet<string>() {};

        const string relativeDirPrefix = "Release";

        private static string[] ignoreFile = new[] {"BuildReport_", ".report", "link.xml", ".json"};

        public static readonly Dictionary<PlatformType, BuildTarget> buildmap =
            new Dictionary<PlatformType, BuildTarget>(PlatformTypeComparer.Instance)
            {
                {PlatformType.Android, BuildTarget.Android},
                {PlatformType.Windows, BuildTarget.StandaloneWindows64},
                {PlatformType.IOS, BuildTarget.iOS},
                {PlatformType.MacOS, BuildTarget.StandaloneOSX},
                {PlatformType.Linux, BuildTarget.StandaloneLinux64},
                {PlatformType.WebGL, BuildTarget.WebGL},
            };

        public static readonly Dictionary<PlatformType, BuildTargetGroup> buildGroupmap =
            new Dictionary<PlatformType, BuildTargetGroup>(PlatformTypeComparer.Instance)
            {
                {PlatformType.Android, BuildTargetGroup.Android},
                {PlatformType.Windows, BuildTargetGroup.Standalone},
                {PlatformType.IOS, BuildTargetGroup.iOS},
                {PlatformType.MacOS, BuildTargetGroup.Standalone},
                {PlatformType.Linux, BuildTargetGroup.Standalone},
                {PlatformType.WebGL, BuildTargetGroup.WebGL},
            };

        public static void KeystoreSetting()
        {
            PlayerSettings.Android.keystoreName = "p4.keystore";
            PlayerSettings.Android.keyaliasName = "jzxmh";
            PlayerSettings.keyaliasPass = "123jzxmh456.";
            PlayerSettings.keystorePass = "#123456jzxmh";
        }

        private static string[] cdnList =
        {
            "https://cdn.hxwgame.cn/FantasyHouse/p4/cdn",
            "https://cdn.hxwgame.cn/FantasyHouse/p4/cdn",
            "https://cdn.hxwgame.cn/FantasyHouse/p4/cdn", 
            "https://cdn.hxwgame.cn/FantasyHouse/p4/cdn"
        };
        private static string[] cdnList2 =
        {
            "https://cdn.hxwgame.cn/FantasyHouse/p4/cdn",
            "https://cdn.hxwgame.cn/FantasyHouse/p4/cdn",
            "https://cdn.hxwgame.cn/FantasyHouse/p4/cdn", 
            "https://cdn.hxwgame.cn/FantasyHouse/p4/cdn"
        };
        private static string[] cdnTestList =
        {
            "https://108-minigames.oss-cn-hangzhou.aliyuncs.com/FantasyHouse/p4/cdnTest",
            "https://108-minigames.oss-cn-hangzhou.aliyuncs.com/FantasyHouse/p4/cdnTest",
            "https://108-minigames.oss-cn-hangzhou.aliyuncs.com/FantasyHouse/p4/cdnTest", 
            "https://108-minigames.oss-cn-hangzhou.aliyuncs.com/FantasyHouse/p4/cdnTest"
        };
        /// <summary>
        /// 设置打包模式
        /// </summary>
        public static void SetCdnConfig(string channel,bool buildHotfixAssembliesAOT, int mode = 1, string cdnPath = "")
        {
            var cdn = Resources.Load<CDNConfig>("CDNConfig");
            cdn.Channel = channel;
            cdn.BuildHotfixAssembliesAOT = buildHotfixAssembliesAOT;
            
            if (mode == (int) Mode.自定义服务器)
            {
                cdn.DefaultHostServer = cdnPath;
                cdn.FallbackHostServer = cdnPath;
                cdn.UpdateListUrl = cdnPath;
                cdn.TestUpdateListUrl = cdnPath;
            }
            else
            {
                cdn.DefaultHostServer = cdnList[mode];
                cdn.FallbackHostServer = cdnList2[mode];
                cdn.UpdateListUrl = cdnList[mode];
                cdn.TestUpdateListUrl = cdnTestList[mode];
            }
            EditorUtility.SetDirty(cdn);
            AssetDatabase.SaveAssetIfDirty(cdn);
        }

        public static void Build(PlatformType type, BuildOptions buildOptions, bool isBuildExe, bool clearReleaseFolder,
            bool clearABFolder, bool buildHotfixAssembliesAOT, bool isBuildAll, bool packAtlas, bool isContainsAb, 
            string channel, bool buildDll = true)
        {
            if (buildmap[type] == EditorUserBuildSettings.activeBuildTarget)
            {
                //pack
                BuildHandle(type, buildOptions, isBuildExe, clearReleaseFolder,clearABFolder, buildHotfixAssembliesAOT, 
                    isBuildAll, packAtlas, isContainsAb, channel, buildDll);
            }
            else
            {
                EditorUserBuildSettings.activeBuildTargetChanged = delegate()
                {
                    if (EditorUserBuildSettings.activeBuildTarget == buildmap[type])
                    {
                        //pack
                        BuildHandle(type, buildOptions, isBuildExe, clearReleaseFolder,clearABFolder, buildHotfixAssembliesAOT, 
                            isBuildAll, packAtlas, isContainsAb, channel, buildDll);
                    }
                };
                if (buildGroupmap.TryGetValue(type, out var group))
                {
                    EditorUserBuildSettings.SwitchActiveBuildTarget(group, buildmap[type]);
                }
                else
                {
                    EditorUserBuildSettings.SwitchActiveBuildTarget(buildmap[type]);
                }

            }
        }
        public static void BuildPackage(PlatformType type, string packageName)
        {
            string platform = "";
            BuildTarget buildTarget = BuildTarget.StandaloneWindows;
            switch (type)
            {
                case PlatformType.Windows:
                    buildTarget = BuildTarget.StandaloneWindows64;
 
                    platform = "pc";
                    break;
                case PlatformType.Android:
                    KeystoreSetting();
                    buildTarget = BuildTarget.Android;
                    platform = "android";
                    break;
                case PlatformType.IOS:
                    buildTarget = BuildTarget.iOS;
                    platform = "ios";
                    break;
                case PlatformType.MacOS:
                    buildTarget = BuildTarget.StandaloneOSX;
                    platform = "pc";
                    break;
                case PlatformType.Linux:
                    buildTarget = BuildTarget.StandaloneLinux64;
                    platform = "pc";
                    break;
                case PlatformType.WebGL:
                    buildTarget = BuildTarget.WebGL;
                    platform = "webgl";
                    break;
            }

            string jstr = File.ReadAllText("Assets/AssetsPackage/config.bytes");
            var obj = JsonHelper.FromJson<PackageConfig>(jstr);
            int version = obj.GetPackageMaxVersion(packageName);
            if (version<0)
            {
                Debug.LogError("指定分包版本号不存在");
                return;
            }
            if (buildmap[type] == EditorUserBuildSettings.activeBuildTarget)
            {
                //pack
                BuildPackage(buildTarget, false, version, packageName,null);
            }
            else
            {
                EditorUserBuildSettings.activeBuildTargetChanged = delegate()
                {
                    if (EditorUserBuildSettings.activeBuildTarget == buildmap[type])
                    {
                        //pack
                        BuildPackage(buildTarget, false, version, packageName,null);
                    }
                };
                if (buildGroupmap.TryGetValue(type, out var group))
                {
                    EditorUserBuildSettings.SwitchActiveBuildTarget(group, buildmap[type]);
                }
                else
                {
                    EditorUserBuildSettings.SwitchActiveBuildTarget(buildmap[type]);
                }

            }

            string fold = $"{AssetBundleBuilderHelper.GetDefaultBuildOutputRoot()}/{buildTarget}";
            var config = Resources.Load<CDNConfig>("CDNConfig");
            var rename = config.GetChannel();
            string targetPath = Path.Combine(relativeDirPrefix, $"{rename}_{platform}");
            if (!Directory.Exists(targetPath)) Directory.CreateDirectory(targetPath);
            FileHelper.CleanDirectory(targetPath);
            var dir = $"{fold}/{packageName}/{version}";
            FileHelper.CopyFiles(dir, targetPath, ignoreFile);
            UnityEngine.Debug.Log("完成cdn资源打包");
#if UNITY_EDITOR
            Application.OpenURL($"file:///{targetPath}");
#endif
        }
        private static bool BuildInternal(BuildTarget buildTarget,bool isBuildAll, bool isContainsAb, string channel)
        {
            string jstr = File.ReadAllText("Assets/AssetsPackage/config.bytes");
            var obj = JsonHelper.FromJson<PackageConfig>(jstr);
            int buildVersion = obj.GetPackageMaxVersion(Define.DefaultName);
            Debug.Log($"开始构建 : {buildTarget}");
            bool res = BuildPackage(buildTarget, isBuildAll, buildVersion, Define.DefaultName, channel);
            if (!res) return res;
            if (isContainsAb)
            {
                if (obj.OtherPackageMaxVer != null)
                {
                    foreach (var item in obj.OtherPackageMaxVer)
                    {
                        for (int i = 0; i < item.Value.Length; i++)
                        {
                            if(item.Value[i] == Define.DefaultName) continue;
                            res &= BuildPackage(buildTarget, isBuildAll, item.Key, item.Value[i], channel);
                            if (!res) return res;
                        }
                    }
                }
            }
            return res;
        }

        public static bool BuildPackage(BuildTarget buildTarget, bool isBuildAll, int buildVersion,
            string packageName, string channel)
        {
            var buildoutputRoot = AssetBundleBuilderHelper.GetDefaultBuildOutputRoot();
            var streamingAssetsRoot = AssetBundleBuilderHelper.GetStreamingAssetsRoot();

            var buildParameters = new ScriptableBuildParameters();
            buildParameters.BuildOutputRoot = buildoutputRoot;
            buildParameters.BuildinFileRoot = streamingAssetsRoot;
            buildParameters.BuildTarget = buildTarget;
            buildParameters.PackageName = packageName;
            buildParameters.BuildPipeline = EBuildPipeline.ScriptableBuildPipeline.ToString();
            buildParameters.BuildBundleType = (int)EBuildBundleType.AssetBundle; //必须指定资源包类型
            buildParameters.PackageVersion = buildVersion.ToString();
            buildParameters.BuildinFileCopyParams = buildAllChannel.Contains(channel)?"buildin;buildinplus":"buildin;";
            buildParameters.VerifyBuildingResult = true;
            if (packageName == Define.DefaultName)
            {
                buildParameters.BuildinFileCopyOption = isBuildAll
                    ? EBuildinFileCopyOption.ClearAndCopyAll
                    : EBuildinFileCopyOption.ClearAndCopyByTags;
            }
            else
            {
                buildParameters.BuildinFileCopyOption =
                    isBuildAll ? EBuildinFileCopyOption.OnlyCopyAll : EBuildinFileCopyOption.OnlyCopyByTags;
            }

            buildParameters.EncryptionServices = new FileStreamEncryption();
            buildParameters.CompressOption = ECompressOption.LZ4;
            buildParameters.FileNameStyle = EFileNameStyle.HashName;
            buildParameters.DisableWriteTypeTree = true; //禁止写入类型树结构（可以降低包体和内存并提高加载效率）
            buildParameters.IgnoreTypeTreeChanges = false;
            buildParameters.EnableSharePackRule = true;
            buildParameters.SingleReferencedPackAlone = true;
            buildParameters.WriteLinkXML = true;
            buildParameters.BuiltinShadersBundleName = GetBuiltinShaderBundleName(packageName);
            buildParameters.ClearBuildCacheFiles = false; //不清理构建缓存，启用增量构建，可以提高打包速度！
            buildParameters.UseAssetDependencyDB = true; //使用资源依赖关系数据库，可以提高打包速度！
            // 执行构建
            ScriptableBuildPipeline builder = new ScriptableBuildPipeline();
            var buildResult = builder.Run(buildParameters,true);
            if (buildResult.Success)
            {
                Debug.Log($"构建成功! "+buildResult.OutputPackageDirectory);
                if (packageName == Define.DefaultName)
                {
                    string link = buildResult.OutputPackageDirectory + "/link.xml";
                    if (!Directory.Exists("Assets/Editor"))
                    {
                        Directory.CreateDirectory("Assets/Editor");
                    }
                    if (File.Exists(link))
                    {
                        File.Copy(link,"Assets/Editor/link.xml",true);
                    }
                }
            }
            else
                Debug.LogError(buildResult.ErrorInfo);
            return buildResult.Success;
        }
        /// <summary>
        /// 内置着色器资源包名称
        /// 注意：和自动收集的着色器资源包名保持一致！
        /// </summary>
        private static string GetBuiltinShaderBundleName(string packageName)
        {
            var uniqueBundleName = AssetBundleCollectorSettingData.Setting.UniqueBundleName;
            var packRuleResult = DefaultPackRule.CreateShadersPackRuleResult();
            return packRuleResult.GetBundleName(packageName, uniqueBundleName);
        }
        public static void HandleAtlas()
        {
            //清除图集
            AtlasHelper.ClearAllAtlas();
            //设置图片
            AtlasHelper.SettingPNG();
            //生成图集
            AtlasHelper.GeneratingAtlas();
        }

        static void BuildHandle(PlatformType type, BuildOptions buildOptions, bool isBuildExe, bool clearReleaseFolder,
            bool clearABFolder, bool buildHotfixAssembliesAOT, bool isBuildAll, bool packAtlas, bool isContainsAb, 
            string channel, bool buildDll = true)
        {
            var render = GraphicsSettings.renderPipelineAsset as UniversalRenderPipelineAsset;
            if (render!= null)
            {
                render.renderScale = 0.75f;
                //默认0.75
            }
            var scene = EditorSceneManager.OpenScene("Assets/AssetsPackage/Scenes/InitScene/Init.unity");
            var init = GameObject.FindObjectOfType<Init>();
            if (init != null && init.CodeMode != CodeMode.LoadFromUrl)
            {
                if (HybridCLR.Editor.SettingsUtil.Enable)
                {
                    init.CodeMode = CodeMode.Wolong;
                    EditorSceneManager.SaveScene(scene);
                }
                else
                {
                    init.CodeMode = CodeMode.BuildIn;
                    buildHotfixAssembliesAOT = true;
                    EditorSceneManager.SaveScene(scene);
                }
            }
            string jstr = File.ReadAllText("Assets/AssetsPackage/config.bytes");
            var obj = JsonHelper.FromJson<PackageConfig>(jstr);
            
            var vs = Application.version.Split(".");
            var bundleVersionCode = int.Parse(vs[vs.Length-1]);
            string exeName = programName + "_" + channel;
            string platform = "";
            BuildTarget buildTarget = BuildTarget.StandaloneWindows;
            int buildVersion = obj.GetPackageMaxVersion(Define.DefaultName);
            switch (type)
            {
                case PlatformType.Windows:
                    buildTarget = BuildTarget.StandaloneWindows64;
                    exeName += ".exe";
                    platform = "pc";
                    break;
                case PlatformType.Android:
                    KeystoreSetting();
                    PlayerSettings.Android.bundleVersionCode = bundleVersionCode + 1;
                    EditorUserBuildSettings.exportAsGoogleAndroidProject = false;
                    buildTarget = BuildTarget.Android;
                    exeName += Application.version + ".apk";
                    platform = "android";
                    break;
                case PlatformType.IOS:
                    buildTarget = BuildTarget.iOS;
                    platform = "ios";
                    break;
                case PlatformType.MacOS:
                    buildTarget = BuildTarget.StandaloneOSX;
                    platform = "pc";
                    break;
                case PlatformType.Linux:
                    buildTarget = BuildTarget.StandaloneLinux64;
                    platform = "pc";
                    break;
                case PlatformType.WebGL:
                    buildTarget = BuildTarget.WebGL;
                    platform = "webgl";
                    exeName += "_" + buildVersion;
                    break;
            }

            PackagesManagerEditor.Clear("com.thridparty-moudule.hotreload"); //HotReload存在时打包会报错
            if ((buildOptions & BuildOptions.Development) == 0)
            {
                PackagesManagerEditor.Clear("com.thridparty-moudule.srdebugger"); //正式包去掉srdebugger
            }
            AssetDatabase.RefreshSettings();
            if (buildDll)
            {
                //打程序集
                FileHelper.CleanDirectory(Define.HotfixDir);
                if ((buildOptions & BuildOptions.Development) == 0)
                    BuildAssemblyEditor.BuildCodeRelease();
                else
                    BuildAssemblyEditor.BuildCodeDebug();
            }
            
            AssetDatabase.SaveAssets();
            //处理图集资源
            if (packAtlas) HandleAtlas();
            
            if (isBuildExe)
            {
                var root = AssetBundleBuilderHelper.GetStreamingAssetsRoot();
                if (Directory.Exists(root))
                {
                    FileHelper.CleanDirectory(root);
                }
                AssetDatabase.Refresh();
            }

            if (clearABFolder)
            {
                string abPath = AssetBundleBuilderHelper.GetDefaultBuildOutputRoot();
                if (Directory.Exists(abPath))
                {
                    FileHelper.CleanDirectory(abPath);
                }
            }
                              
            //打ab
            if (!BuildInternal(buildTarget, isBuildAll, isContainsAb, channel))
            {
                return;
            }

            if (clearReleaseFolder && Directory.Exists(relativeDirPrefix))
            {
                FileHelper.CleanDirectory(relativeDirPrefix);
            }
            else
            {
                Directory.CreateDirectory(relativeDirPrefix);
            }

            if (isBuildExe || buildTarget == BuildTarget.WebGL)
            {
                if (HybridCLR.Editor.SettingsUtil.Enable)
                {
                    HybridCLR.Editor.SettingsUtil.buildHotfixAssembliesAOT = buildHotfixAssembliesAOT;
                    HybridCLR.Editor.Commands.PrebuildCommand.GenerateAll();
                }
            }
            
            var config = Resources.Load<CDNConfig>("CDNConfig");
            var rename = config.GetChannel();
            string targetPath = Path.Combine(relativeDirPrefix, $"{rename}_{platform}");

            if (!Directory.Exists(targetPath)) Directory.CreateDirectory(targetPath);
            FileHelper.CleanDirectory(targetPath);
            
            if(isBuildExe)
            {
                UnityEngine.Debug.Log("开始EXE打包");
                AssetDatabase.Refresh();
#if UNITY_WEBGL
                bool webgl1 = true;
                if (PlayerSettings.colorSpace == ColorSpace.Linear || PlayerSettings.GetUseDefaultGraphicsAPIs(BuildTarget.WebGL))
                {
                    webgl1 = false;
                }
                else
                {
                    GraphicsDeviceType[] graphicsAPIs = PlayerSettings.GetGraphicsAPIs(BuildTarget.WebGL);
                    for (int i = 0; i < graphicsAPIs.Length; i++)
                    {
                        if (graphicsAPIs[i] == GraphicsDeviceType.OpenGLES3)
                        {
                            webgl1 = false;
                            break;
                        }
                    }
                }

                if (webgl1)
                {
                    var define = "UNITY_WEBGL_1";
                    var definesString = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.WebGL).Trim();
                    if (!string.IsNullOrEmpty(definesString))
                    {
                        definesString += definesString.EndsWith(";") ? define+";" : $";{define};";
                    }
                    else
                    {
                        definesString = define+";";
                    }
                    PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.WebGL, definesString);
                }

                var rp = GraphicsSettings.currentRenderPipeline as UniversalRenderPipelineAsset;
                rp.GraphicsPlatformType = webgl1 ? GraphicsPlatformType.Webgl1 : GraphicsPlatformType.DX11;
                EditorUtility.SetDirty(rp);
                AssetDatabase.SaveAssetIfDirty(rp);
#endif
#if UNITY_WEBGL_TT
                TTSDK.Tool.StarkBuilderSettings setting = TTSDK.Tool.StarkBuilderSettings.Instance;
                if (setting != null)
                {
                    setting.assetBundleFSEnabled = true;
                    setting.isOldBuildFormat = false;
                    setting.webglPackagePath = Path.GetFullPath("Release");
                    if (PlayerSettings.colorSpace == ColorSpace.Linear || PlayerSettings.GetUseDefaultGraphicsAPIs(BuildTarget.WebGL))
                    {
                        setting.isWebGL2 = true;
                    }
                    GraphicsDeviceType[] apis = PlayerSettings.GetGraphicsAPIs(BuildTarget.WebGL);
                    for (int i = 0; i < apis.Length; i++)
                    {
                        if (apis[i] == GraphicsDeviceType.OpenGLES3)
                        {
                            setting.isWebGL2 = true;
                        }
                    }
                }

                TTSDK.Tool.API.BuildManager.Build(TTSDK.Tool.Framework.Wasm);
                UnityEngine.Debug.Log("完成打包");
                var newPath = relativeDirPrefix + "/tt-minigame/";
                //新打包格式
                if (Directory.Exists(newPath))
                {
                    if (File.Exists(newPath + "game.js"))
                    {
                        var txt = File.ReadAllText(newPath + "game.js");
                        txt = txt.Replace("['正在加载资源']", "['正在加载资源','正在加载配置','正在生成世界','正在生成小镇','正在生成码头','正在生成角色','正在生成集装箱']");
                        txt = txt.Replace("'编译中'", "'正在编译'");
                        txt = txt.Replace("'初始化中'", "'正在初始化'");
                        txt = txt.Replace("textDuration: 1500,", "textDuration: 6000,");
                        txt = txt.Replace("scaleMode: scaleMode.default,", "scaleMode: scaleMode.noBorder,");
                        txt = txt.Replace("width: 106,", "width: 64,");
                        txt = txt.Replace("height: 40,", "height: 64,");
                        var preload = GetGuidFiles(buildTarget, config, rename, platform);
                        if (!string.IsNullOrEmpty(preload))
                        {
                            txt = txt.Replace(
                                "// 'DATA_CDN/StreamingAssets/WebGL/textures_005b9e6b32e22099edc38cba5b3d11de',",
                                preload);
                        }
                        File.WriteAllText(newPath + "game.js", txt);
                    }
                    var icons = PlayerSettings.GetIconsForTargetGroup(BuildTargetGroup.Unknown);
                    if (icons.Length > 0 && icons[0] != null)
                    {
                        var path = AssetDatabase.GetAssetPath(icons[0]);
                        File.Copy(path,$"{newPath}/images/unity_logo.png", true);
                    }
                    File.Copy("Assets/LoadingBG.png",$"{newPath}/images/background.png", true);
                    if (File.Exists(newPath + "game.json"))//json格式报错
                    {
                        var gamejStr = File.ReadAllText(newPath + "game.json");
                        gamejStr = gamejStr.Replace("0.1.0","4.21.0");
                        var gameInfo = Newtonsoft.Json.JsonConvert.DeserializeObject(gamejStr);
                        gamejStr = Newtonsoft.Json.JsonConvert.SerializeObject(gameInfo);
                        File.WriteAllText(newPath + "game.json",gamejStr);
                    }
                }
#elif UNITY_WEBGL_WeChat
                WeChatWASM.WXConvertCore.config.ProjectConf.relativeDST = Path.GetFullPath("Release");
                WeChatWASM.WXConvertCore.config.ProjectConf.DST = Path.GetFullPath("Release");
                WeChatWASM.WXConvertCore.config.ProjectConf.CDN = $"{config.DefaultHostServer}/{rename}_{platform}/";
                var fields = typeof(CacheKeys).GetFields();
                foreach (var item in fields)
                {
                    if (item.IsStatic)
                    {
                        var val = item.GetValue(null) as string;
                        if (!string.IsNullOrEmpty(val))
                        {
                            if(!WeChatWASM.WXConvertCore.config.PlayerPrefsKeys.Contains(val)) 
                                WeChatWASM.WXConvertCore.config.PlayerPrefsKeys.Add(val);
                        }
                    }
                }
                if (PlayerSettings.colorSpace == ColorSpace.Linear || PlayerSettings.GetUseDefaultGraphicsAPIs(BuildTarget.WebGL))
                {
                    WeChatWASM.WXConvertCore.config.CompileOptions.Webgl2 = true;
                }
                GraphicsDeviceType[] apis = PlayerSettings.GetGraphicsAPIs(BuildTarget.WebGL);
                for (int i = 0; i < apis.Length; i++)
                {
                    if (apis[i] == GraphicsDeviceType.OpenGLES3)
                    {
                        WeChatWASM.WXConvertCore.config.CompileOptions.Webgl2 = true;
                    }
                }
                
                WeChatWASM.WXConvertCore.config.ProjectConf.preloadFiles = GetPreloadFiles();
                if (WeChatWASM.WXConvertCore.DoExport() == WeChatWASM.WXConvertCore.WXExportError.SUCCEED)
                {
                    UnityEngine.Debug.Log("完成打包");
                    if (WeChatWASM.WXConvertCore.config.ProjectConf.assetLoadType == 0)
                    {
                        string[] fls = Directory.GetFiles(WeChatWASM.WXConvertCore.config.ProjectConf.DST +"/webgl");
                        for (int i = 0; i < fls.Length; i++)
                        {
                            if (fls[i].EndsWith(".data") || fls[i].EndsWith("data.zip") || fls[i].EndsWith("data.br")|| fls[i].EndsWith("bin.txt"))
                            {
                                var name = Path.GetFileName(fls[i]);
                                File.Copy(fls[i], targetPath + "/" + name);
                            }
                        }
                    }
                    var newPath = relativeDirPrefix + "/minigame/";
                    if (Directory.Exists(newPath))
                    {
                        if (File.Exists(newPath + "game.js"))
                        {
                            var txt = File.ReadAllText(newPath + "game.js");
                            txt = txt.Replace(
                                $"{YooAssetSettingsData.Setting.DefaultYooFolderName}/{Define.DefaultName}/",
                                WeChatWASM.WXConvertCore.config.ProjectConf.CDN);
                            File.WriteAllText(newPath + "game.js", txt);
                        }
                    }
                }
#elif UNITY_WEBGL_KS
                var selection = Selection.objects;
                KSWASM.editor.KSEditorScriptObject configks = KSWASM.editor.UnityUtil.GetEditorConf("kuaishou", "Packages/com.kuaishou.minigame/Editor/MiniGameConfig.asset");
                
                configks.buildOptions = KSWASM.editor.MiniHostBuildWindow.GetBuildOptions(configks);
                configks.ProjectConf.DST = Path.GetFullPath("Release"); 
                configks.ProjectConf.CDN = $"{config.DefaultHostServer}/{rename}_{platform}/";
                var fields = typeof(CacheKeys).GetFields();
                foreach (var item in fields)
                {
                    if (item.IsStatic)
                    {
                        var val = item.GetValue(null) as string;
                        if (!string.IsNullOrEmpty(val))
                        {
                            if(!configks.PlayerPrefsKeys.Contains(val)) 
                                configks.PlayerPrefsKeys.Add(val);
                        }
                    }
                }
                KSWASM.editor.MiniHostBuildWindowHelper.UpdateWebGL2();
                if (KSWASM.editor.KSConvertCore.DoExport(configks) == KSWASM.editor.KSConvertCore.KSExportError.SUCCEED)
                {
                    UnityEngine.Debug.Log("完成打包");
                    if (configks.ProjectConf.assetLoadType == 0)
                    {
                        string[] fls = Directory.GetFiles(configks.ProjectConf.DST +"/webgl");
                        for (int i = 0; i < fls.Length; i++)
                        {
                            if (fls[i].EndsWith(".data") || fls[i].EndsWith("data.zip") || fls[i].EndsWith("data.br")|| fls[i].EndsWith("bin.txt"))
                            {
                                var name = Path.GetFileName(fls[i]);
                                File.Copy(fls[i], targetPath + "/" + name);
                            }
                        }
                    }
                }
                Selection.objects = selection;
#elif UNITY_WEBGL_TAPTAP
                minihost.editor.TJEditorScriptObject ttCf = TapTapMiniGame.TapTapUtil.GetEditorConf();
                ttCf.buildOptions = minihost.editor.UnityUtil.GetBuildOptions(ttCf);
                ttCf.ProjectConf.DST =  Path.GetFullPath("Release"); 
                ttCf.ProjectConf.CDN = $"{config.DefaultHostServer}/{rename}_{platform}/";
                var fields = typeof(CacheKeys).GetFields();
                foreach (var item in fields)
                {
                    if (item.IsStatic)
                    {
                        var val = item.GetValue(null) as string;
                        if (!string.IsNullOrEmpty(val))
                        {
                            if(!ttCf.PlayerPrefsKeys.Contains(val)) 
                                ttCf.PlayerPrefsKeys.Add(val);
                        }
                    }
                }
                TapTapMiniGame.TapTapBuildWindowHelper.UpdateWebGL2();
                TapTapMiniGame.TapTapConvertCore.DoExport(ttCf, true);
                UnityEngine.Debug.Log("完成打包");
                if (ttCf.ProjectConf.assetLoadType == 0)
                {
                    string[] fls = Directory.GetFiles(ttCf.ProjectConf.DST +"/webgl");
                    for (int i = 0; i < fls.Length; i++)
                    {
                        if (fls[i].EndsWith(".data") || fls[i].EndsWith("data.zip") || fls[i].EndsWith("data.br")|| fls[i].EndsWith("bin.txt"))
                        {
                            var name = Path.GetFileName(fls[i]);
                            File.Copy(fls[i], targetPath + "/" + name);
                        }
                    }
                }
#elif UNITY_WEBGL_BILIGAME
                var BLConfig = WeChatWASM.UnityUtil.GetEditorConf();
                if (string.IsNullOrEmpty(BLConfig.ProjectConf.Appid))
                {
                    BLConfig.ProjectConf.Appid = "biligame2195b75ef4e07bd3";
                    //测试号
                }
                if (string.IsNullOrEmpty(BLConfig.ProjectConf.CDN))
                {
                    BLConfig.ProjectConf.CDN = "http://miniapp.bilibili.com/";
                }
                BLConfig.ProjectConf.projectName = Application.productName;
                BLConfig.ProjectConf.DST = Path.GetFullPath("Release");
                var fields = typeof(CacheKeys).GetFields();
                foreach (var item in fields)
                {
                    if (item.IsStatic)
                    {
                        var val = item.GetValue(null) as string;
                        if (!string.IsNullOrEmpty(val))
                        {
                            if(!BLConfig.PlayerPrefsKeys.Contains(val)) BLConfig.PlayerPrefsKeys.Add(val);
                        }
                    }
                }
                var win = EditorWindow.GetWindow(typeof(WeChatWASM.WXEditorWindow)) as WeChatWASM.WXEditorWindow;
                win?.DoExport(true);
                Debug.Log("完成打包");
                var workSpace = new DirectoryInfo(BLConfig.ProjectConf.DST).FullName;
                var res = BashUtil.RunCommand3(null, "cmd.exe", new string[] {"npm", "install", "bili-sgame-cli@latest", "-g"});
                Debug.Log(res.StdOut);
                if (res.ExitCode != 0)
                {
                    Debug.LogError(res.StdErr);
                }
                
                res = BashUtil.RunCommand3(workSpace, "cmd.exe", new string[] {"bili-sgame-cli", "unity", "1.0.0", BLConfig.ProjectConf.Appid});
                Debug.Log(res.StdOut);
                if (res.ExitCode != 0)
                {
                    Debug.LogError("转换失败");
                    Debug.LogError(res.StdErr);
                }
                else
                {
                    Debug.Log("转换成功");
                    var biligame = workSpace + "/biligame/";
                    if (Directory.Exists(biligame))
                    {
                        var dataPackageFiles = new List<string>();
                        FileHelper.GetAllFiles(dataPackageFiles, biligame + "data-package");
                        for (int i = 0; i < dataPackageFiles.Count; i++)
                        {
                            if (dataPackageFiles[i].EndsWith(".txt"))
                            {
                                string file = Path.GetFileName(dataPackageFiles[i]);
                                string gamejs = biligame + "/game.js";
                                var txt = File.ReadAllText(gamejs);
                                txt = txt.Replace($"dataUrl: \"data-package/{file}\",", $"dataUrl: \"{config.DefaultHostServer}/{rename}_{platform}/{file}\",");
                                txt = txt.Replace("DATA_CDN: \"\",",$"DATA_CDN: \"{BLConfig.ProjectConf.CDN}\",");
                                File.WriteAllText(gamejs,txt);
                                File.Move(dataPackageFiles[i], targetPath + "/" + file);
                                break;
                            }
                        }
                        Directory.Delete(workSpace+"/biligame/data-package",true);
                        string gamejson = biligame + "/game.json";
                        var jObj = Newtonsoft.Json.JsonConvert.DeserializeObject<Newtonsoft.Json.Linq.JObject>(File.ReadAllText(gamejson));
                        var subpackages = jObj["subpackages"].ToObject<Newtonsoft.Json.Linq.JArray>();
                        int index = 0;
                        for (var pack = subpackages.First; pack != null; pack = pack.Next)
                        {
                            if (pack["name"].ToString().Contains("data-package"))
                            {
                                subpackages.RemoveAt(index);
                                jObj["subpackages"] = subpackages;
                                File.WriteAllText(gamejson, Newtonsoft.Json.JsonConvert.SerializeObject(jObj));
                                break;
                            }
                            index++;
                        }

                        var loaderJs = biligame + "Build/webgl.loader.js";
                        var loaderTxt = File.ReadAllText(loaderJs);
                        loaderTxt = loaderTxt.Replace(
                            "var s=URL.createObjectURL(new Blob([e],{type:\"application/javascript\"}));",
                            "var s = GameGlobal.managerConfig.frameworkUrl;");
                        File.WriteAllText(loaderJs, loaderTxt);
                    }
                }
#elif UNITY_WEBGL_ALIPAY
                AlipayConvertCore.AlipayConfig.AlipayProjectCfg.DerivedPath = Path.GetFullPath("Release");
                AlipayConvertCore.AlipayConfig.CompileOptions.UseStreamingAssets = false;
                AlipayConvertCore.AlipayConfig.AlipayProjectCfg.CDN = $"{config.DefaultHostServer}/{rename}_{platform}/";
                if (AlipayConvertCore.WebglBuildAndConvert())
                {
                    UnityEngine.Debug.Log("完成打包");
                    if (AlipayConvertCore.AlipayConfig.AlipayProjectCfg.DataFileLoadType == "CDN")
                    {
                        string[] fls = Directory.GetFiles(AlipayConvertCore.AlipayConfig.AlipayProjectCfg.DerivedPath+"/webgl");
                        for (int i = 0; i < fls.Length; i++)
                        {
                            if (fls[i].EndsWith(".data") || fls[i].EndsWith("data.zip") || fls[i].EndsWith("data.br"))
                            {
                                var name = Path.GetFileName(fls[i]);
                                File.Copy(fls[i], targetPath + "/" + name);
                            }
                        }
                    }
                }
#elif UNITY_WEBGL_QG
                var qgGameConfig = QGMiniGameCore.QGWindowHepler.Instance.GetQGGameConfig();
                qgGameConfig.buildSrc = Path.GetFullPath("Release");
                qgGameConfig.envConfig.assetCache.cdn =  $"{config.DefaultHostServer}/{rename}_{platform}/";
                qgGameConfig.envConfig.wasmUrl = qgGameConfig.envConfig.assetCache.cdn + buildVersion + ".wasm.zip";
                qgGameConfig.envConfig.wasmDataUrl = qgGameConfig.envConfig.assetCache.cdn + buildVersion + ".wasm.zip";
                if (PlayerSettings.colorSpace == ColorSpace.Linear || PlayerSettings.GetUseDefaultGraphicsAPIs(BuildTarget.WebGL))
                {
                    qgGameConfig.useWebgl2 = true;
                }
                GraphicsDeviceType[] apis = PlayerSettings.GetGraphicsAPIs(BuildTarget.WebGL);
                for (int i = 0; i < apis.Length; i++)
                {
                    if (apis[i] == GraphicsDeviceType.OpenGLES3)
                    {
                        qgGameConfig.useWebgl2 = true;
                    }
                }
                QGMiniGameCore.QGWindowHepler.Instance.OnConfigSetting();
                QGMiniGameCore.QGGameBuild.Instance.SetPlayer(qgGameConfig.useWebgl2);
                QGMiniGameCore.QGGameBuild.Instance.DoBuild();
                UnityEngine.Debug.Log("完成打包");
                
                if (qgGameConfig.useSelfLoading || qgGameConfig.useSubPkgLoading)
                {
                    string[] fls = Directory.GetFiles(qgGameConfig.buildSrc+"/webgl_qg/gzip");
                    for (int i = 0; i < fls.Length; i++)
                    {
                        if (fls[i].EndsWith(".zip"))
                        {
                            string zipFilePath = targetPath + "/" + buildVersion + ".wasm.zip";
                            File.Copy(fls[i], zipFilePath);
                        }
                    }
                }
#elif UNITY_WEBGL_MINIHOST
                minihost.editor.TJEditorScriptObject tjConfig = minihost.editor.UnityUtil.GetEditorConf("minihost", "Assets/TJ-WASM-SDK-V2/Editor/MiniGameConfig.asset");
                tjConfig.ProjectConf.CDN = $"{config.DefaultHostServer}/{rename}_{platform}/";
                tjConfig.ProjectConf.DST = Path.GetFullPath("Release");
                if (PlayerSettings.colorSpace == ColorSpace.Linear || PlayerSettings.GetUseDefaultGraphicsAPIs(BuildTarget.WebGL))
                {
                    tjConfig.CompileOptions.Webgl2 = true;
                }
                GraphicsDeviceType[] apis = PlayerSettings.GetGraphicsAPIs(BuildTarget.WebGL);
                for (int i = 0; i < apis.Length; i++)
                {
                    if (apis[i] == GraphicsDeviceType.OpenGLES3)
                    {
                        tjConfig.CompileOptions.Webgl2 = true;
                    }
                }
                if (minihost.editor.TJConvertCore.DoExport(tjConfig) == minihost.editor.TJConvertCore.TJExportError.SUCCEED)
                {
                    var zipped = Path.Combine(tjConfig.ProjectConf.DST, "game.zip");
                    minihost.editor.UnityUtil.ZipGame(zipped, Path.Combine(tjConfig.ProjectConf.DST, "minigame"));
                    UnityEngine.Debug.Log("完成打包");
                    if (tjConfig.ProjectConf.assetLoadType == 0)
                    {
                        string[] fls = Directory.GetFiles(tjConfig.ProjectConf.DST +"/webgl");
                        for (int i = 0; i < fls.Length; i++)
                        {
                            if (fls[i].EndsWith(".data") || fls[i].EndsWith("data.zip") || fls[i].EndsWith("data.br")|| fls[i].EndsWith("bin.txt"))
                            {
                                var name = Path.GetFileName(fls[i]);
                                File.Copy(fls[i], targetPath + "/" + name);
                            }
                        }
                    }
                    if (tjConfig.AutoUploadOnBuild)
                    {
                        minihost.editor.UploadWindow.ShowWindow(zipped);
                    }
                }
#else
#if UNITY_WEBGL
                PlayerSettings.WebGL.template = $"PROJECT:TaoTie";
#endif
                string[] levels = {
                    "Assets/AssetsPackage/Scenes/InitScene/Init.unity",
                };
                BuildPipeline.BuildPlayer(levels, $"{relativeDirPrefix}/{exeName}", buildTarget, buildOptions);
                UnityEngine.Debug.Log("完成打包");
                //清下缓存
                if (Directory.Exists(Application.persistentDataPath))
                {
                    Directory.Delete(Application.persistentDataPath, true);
                }

                if (buildTarget == BuildTarget.WebGL)
                {
                    var icons = PlayerSettings.GetIconsForTargetGroup(BuildTargetGroup.Unknown);
                    if (icons.Length > 0 && icons[0] != null)
                    {
                        var path = AssetDatabase.GetAssetPath(icons[0]);
                        File.Copy(path,$"{relativeDirPrefix}/{exeName}/icon.png", true);
                    }
                }
#endif
            }

            string fold = $"{AssetBundleBuilderHelper.GetDefaultBuildOutputRoot()}/{buildTarget}";
            
            var dirs = new DirectoryInfo(fold).GetDirectories();
            for (int i = 0; i < dirs.Length; i++)
            {
                var version = obj.GetPackageMaxVersion(dirs[i].Name);
                string dir = $"{fold}/{dirs[i].Name}/{version}";
                if (dir != null)
                {
                    FileHelper.CopyFiles(dir, targetPath, ignoreFile);
                }
            }

            DirectoryInfo info = new DirectoryInfo(targetPath);
            StringBuilder sb = new StringBuilder();
            var files = info.GetFiles();
            for (int i = 0; i < files.Length; i++)
            {
                sb.AppendLine(files[i].Name);
            }
            File.WriteAllText(relativeDirPrefix + "/reslist.txt",sb.ToString());
            UnityEngine.Debug.Log("完成cdn资源打包");
#if UNITY_EDITOR
            Application.OpenURL($"file:///{targetPath}");
#endif
        }

        public static void PrintFile()
        {
            var config = Resources.Load<CDNConfig>("CDNConfig");
            var rename = config.GetChannel();
            string platform = "pc";
#if UNITY_ANDROID
            platform = "android";
#elif UNITY_IOS
            platform = "ios";
#elif UNITY_WEBGL
            platform = "webgl";
#endif
            string targetPath = Path.Combine(relativeDirPrefix, $"{rename}_{platform}");
            DirectoryInfo info = new DirectoryInfo(targetPath);
            if(!info.Exists) return;
            StringBuilder sb = new StringBuilder();
            var files = info.GetFiles();
            for (int i = 0; i < files.Length; i++)
            {
                sb.AppendLine(files[i].Name);
            }
            File.WriteAllText(relativeDirPrefix + "/reslist.txt",sb.ToString());
        }
        public static void BuildApk(string channel,BuildOptions buildOptions)
        {
            var bundleVersionCode = int.Parse(Application.version.Split(".")[2]);
            string exeName = programName + "_" + channel;
            KeystoreSetting();
            PlayerSettings.Android.bundleVersionCode = bundleVersionCode;
            EditorUserBuildSettings.exportAsGoogleAndroidProject = false;
            BuildTarget buildTarget  = BuildTarget.Android;
            exeName += Application.version + ".apk";
            AssetDatabase.Refresh();
            string[] levels =
            {
                "Assets/AssetsPackage/Scenes/InitScene/Init.unity",
            };
            UnityEngine.Debug.Log("开始EXE打包");
            BuildPipeline.BuildPlayer(levels, $"{relativeDirPrefix}/{exeName}", buildTarget, buildOptions);
            UnityEngine.Debug.Log("完成打包");
        }

        public static void CollectSVC(Action<bool> callBack)
        {
            string savePath = "Assets/AssetsPackage/RenderAssets/ShaderVariants.shadervariants";
            ShaderVariantCollector.Run(savePath, Define.DefaultName, 1000, () =>
            {
                ShaderVariantCollection collection =
                    AssetDatabase.LoadAssetAtPath<ShaderVariantCollection>(savePath);
                if (collection != null)
                {
                    Debug.Log($"ShaderCount : {collection.shaderCount}");
                    Debug.Log($"VariantCount : {collection.variantCount}");
                }
                callBack?.Invoke(collection != null);
            });
        }

        public static string GetPreloadFiles()
        {
            StringBuilder preloadList = new StringBuilder();
            var files = new List<string>();
            FileHelper.GetAllFiles(files, Application.streamingAssetsPath);
            for (int i = 0; i < files.Count; i++)
            {
                if(files[i].EndsWith(".meta")) continue;
                preloadList.Append(Path.GetFileName(files[i]) + ";");
            }
            return preloadList.ToString();
        }

        public static string GetGuidFiles(BuildTarget buildTarget,CDNConfig config,string rename,string platform)
        {
            var bytes = AssetDatabase.LoadAssetAtPath<TextAsset>("Assets/AssetsPackage/Config/UnitConfigCategory.bytes")?.bytes;
            if (bytes == null) return null;
            UnitConfigCategory unitConfigCategory = ProtobufHelper.FromBytes<UnitConfigCategory>(bytes);
           
            var units = unitConfigCategory.GetAllList();
            HashSet<string> perfabs = new HashSet<string>();
            for (int i = 0; i < units.Count; i++)
            {
                perfabs.Add(units[i].Perfab);
            }
            
            var files = new List<string>();
            string fold = $"{AssetBundleBuilderHelper.GetDefaultBuildOutputRoot()}/{buildTarget}";
            FileHelper.GetAllFiles(files, fold);
            BuildReport buildReport = null;
            for (int i = 0; i < files.Count; i++)
            {
                if (files[i].EndsWith(".report"))
                {
                    string jsonData = FileUtility.ReadAllText(files[i]);
                    buildReport = BuildReport.Deserialize(jsonData);
                    break;
                }
            }

            if (buildReport != null)
            {
                HashSet<string> bundles = new HashSet<string>();
                for (int j = 0; j < buildReport.AssetInfos.Count; j++)
                {
                    if (perfabs.Contains(buildReport.AssetInfos[j].Address))
                    {
                        bundles.Add(buildReport.AssetInfos[j].MainBundleName);
                    }
                }

                MultiMap<string, string> maps = new MultiMap<string, string>();
                Dictionary<string, ReportBundleInfo> path = new Dictionary<string, ReportBundleInfo>();
                perfabs.Clear();
                for (int j = 0; j < buildReport.BundleInfos.Count; j++)
                {
                    maps.Add(buildReport.BundleInfos[j].BundleName, buildReport.BundleInfos[j].DependBundles);
                    path.Add(buildReport.BundleInfos[j].BundleName,buildReport.BundleInfos[j]);
                }
                List<string> allBundles = bundles.ToList();
                for (int i = 0; i < allBundles.Count; i++)
                {
                    if (maps.TryGetValue(allBundles[i], out var list))
                    {
                        for (int j = 0; j < list.Count; j++)
                        {
                            if (!allBundles.Contains(list[j]))
                            {
                                allBundles.Add(list[j]);
                            }
                        }
                    }
                }
                
                StringBuilder preloadList = new StringBuilder();
                foreach (var item in allBundles)
                {
                    if (path.TryGetValue(item, out var p))
                    {
                        if(!p.GetTagsString().Contains("buildin")) 
                            preloadList.AppendLine($"'{config.DefaultHostServer}/{rename}_{platform}/{p.FileName}',");
                    }
                }
            
                return preloadList.ToString();
            }

            return null;
        }
    }
}
