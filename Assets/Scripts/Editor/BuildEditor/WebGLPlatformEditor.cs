using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace TaoTie
{
    public static class WebGLPlatformEditor
    {
	    static Dictionary<WebGLPlatform, string> defineSetting = new Dictionary<WebGLPlatform, string>()
	    {
		    {WebGLPlatform.TikTok, "UNITY_WEBGL_TT"},
		    {WebGLPlatform.WeChat, "UNITY_WEBGL_WeChat"},
		    {WebGLPlatform.KuaiShou, "UNITY_WEBGL_KS"},
		    {WebGLPlatform.Bilibili, "UNITY_WEBGL_BILIGAME"},
		    {WebGLPlatform.TapTap,"UNITY_WEBGL_TAPTAP"},
		    {WebGLPlatform.AliPay,"UNITY_WEBGL_ALIPAY"},
		    {WebGLPlatform.QuickGame,"UNITY_WEBGL_QG"},
		    {WebGLPlatform.MeiTuan,"UNITY_WEBGL_MEITUAN"},
		    {WebGLPlatform.FaceBook,"UNITY_WEBGL_FACEBOOK"},
		    {WebGLPlatform._4399,"UNITY_WEBGL_4399"},
		    {WebGLPlatform.MiniHost,"UNITY_WEBGL_MINIHOST"},
	    };
	    static Dictionary<WebGLPlatform, string> packageSetting = new Dictionary<WebGLPlatform, string>()
	    {
		    {WebGLPlatform.TikTok, "com.bytedance.starksdk"},
		    {WebGLPlatform.WeChat, "com.qq.weixin.minigame"},
		    {WebGLPlatform.KuaiShou, "com.kuaishou.minigame"},
		    {WebGLPlatform.Bilibili, "com.bilibili.minigame"},
		    {WebGLPlatform.TapTap, "com.taptap.minigame"},
		    {WebGLPlatform.AliPay, "com.alipay.alipaysdk"},
		    {WebGLPlatform.QuickGame, "com.quickapp.qg"},
		    {WebGLPlatform.FaceBook,"com.unity.meta-instant-games-sdk"},
		    {WebGLPlatform._4399,"com.4399.h5game"},
		    {WebGLPlatform.MiniHost,"cn.tuanjie.minihost"},
	    };

	    static Dictionary<WebGLPlatform, int> webglVersionSetting = new Dictionary<WebGLPlatform, int>()
	    {
		    {WebGLPlatform.WebGL, 2},
		    {WebGLPlatform.TikTok, 2},
		    {WebGLPlatform.WeChat, 2},
		    {WebGLPlatform.KuaiShou, 1},//没写但不支持2
		    {WebGLPlatform.Bilibili, 2},//需要联系商务申请加白
		    {WebGLPlatform.TapTap, 2},
		    {WebGLPlatform.AliPay, 1},//暂不支持2
		    {WebGLPlatform.QuickGame, 2},
		    {WebGLPlatform.MeiTuan, 1},//暂不支持2
		    {WebGLPlatform.FaceBook, 2},
		    {WebGLPlatform._4399, 2},
		    {WebGLPlatform.MiniHost, 2},
	    };

	    public static WebGLPlatform GetCurrentWebGLPlatform()
	    {
		    var definesString = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.WebGL);
		    foreach (var item in defineSetting)
		    {
			    if (definesString.Contains(item.Value))
			    {
				    return item.Key;
			    }
		    }

		    return WebGLPlatform.WebGL;
	    }
	    public static WebGLPlatform Renderer(WebGLPlatform webGLPlatform)
	    {
		    var definesString = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.WebGL).Trim();
		    var defines = definesString.Split(';').ToList();
		    webGLPlatform = WebGLPlatform.WebGL;
		    for (int i = 0; i < defines.Count; i++)
		    {
			    foreach (var item in defineSetting)
			    {
				    if (defines[i] == item.Value)
				    {
					    webGLPlatform = item.Key;
					    break;
				    }
			    }
		    }

		    var newWebGLPlatform = (WebGLPlatform) EditorGUILayout.EnumPopup(new GUIContent(""), webGLPlatform, (a) =>
		    {
			    if (PlayerSettings.colorSpace == ColorSpace.Linear || PlayerSettings.GetUseDefaultGraphicsAPIs(BuildTarget.WebGL))
			    {
				    return webglVersionSetting[(WebGLPlatform) a] == 2;
			    }
			    GraphicsDeviceType[] type = PlayerSettings.GetGraphicsAPIs(BuildTarget.WebGL);
			    for (int i = 0; i < type.Length; i++)
			    {
				    if (type[i] == GraphicsDeviceType.OpenGLES3)
				    {
					    return webglVersionSetting[(WebGLPlatform) a] == 2;
				    }
			    }
			    return true;
		    }, false);
		    if (newWebGLPlatform != webGLPlatform)
		    {
			    string packages = "Packages/manifest.json";
			    if (EditorUtility.DisplayDialogComplex("警告!",
				        $"当前目标平台为{webGLPlatform}, 是否切换到{newWebGLPlatform}(切完需要重新聚焦Unity)",
				        "切换", "取消", "不切换") == 0)
			    {
				    Packages info = Newtonsoft.Json.JsonConvert.DeserializeObject<Packages>(File.ReadAllText(packages));
				    foreach (var item in packageSetting)
				    {
					    info.dependencies.Remove(item.Value);
				    }

				    foreach (var item in defineSetting)
				    {
					    definesString = definesString.Replace(item.Value, "");
				    }
				    definesString = definesString.Replace(";;", ";");

				    if (packageSetting.TryGetValue(newWebGLPlatform, out var package))
				    {
					    info.dependencies[package] = $"file:../Modules/{package}";
				    }
				    if (defineSetting.TryGetValue(newWebGLPlatform, out var define))
				    {
					    if (!string.IsNullOrEmpty(definesString))
					    {
						    definesString += definesString.EndsWith(";") ? define+";" : $";{define};";
					    }
					    else
					    {
						    definesString = define+";";
					    }
				    }
				    
				    PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.WebGL, definesString);
				    File.WriteAllText(packages, Newtonsoft.Json.JsonConvert.SerializeObject(info,
					    new Newtonsoft.Json.JsonSerializerSettings()
					    {
						    Formatting = Newtonsoft.Json.Formatting.Indented,
					    }));
				    
				    AssetDatabase.SaveAssets();
				    webGLPlatform = newWebGLPlatform;
				    AssetDatabase.Refresh();
			    }
		    }

		    return webGLPlatform;
	    }
    }
}