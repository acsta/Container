using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Scripting;

namespace TaoTie
{
    [Preserve]
    public class SkipUnityLogo
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
        private static void BeforeSplashScreen()
        {
#if UNITY_WEBGL
            if(Debug.isDebugBuild) return;
            //使用URP时，华为系列跳过Unity的logo会黑屏
            if (PlatformUtil.IsHuaWeiGroup() &&
                GraphicsSettings.currentRenderPipeline?.GetType()?.Name?.Contains("UniversalRenderPipelineAsset") ==
                true)
            {
                return;
            }
            Application.focusChanged += Application_focusChanged;
#else
            System.Threading.Tasks.Task.Run(AsyncSkip);
#endif
        }

#if UNITY_WEBGL
        private static void Application_focusChanged(bool obj)
        {
            Application.focusChanged -= Application_focusChanged;
            SplashScreen.Stop(SplashScreen.StopBehavior.StopImmediate);
        }
#else
        private static void AsyncSkip()
        {
            SplashScreen.Stop(SplashScreen.StopBehavior.StopImmediate);
        }
#endif
    }

}