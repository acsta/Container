using UnityEngine;

namespace TaoTie
{
    public static class SystemInfoHelper
    {
#if UNITY_WEBGL_ALIPAY
        private static AlipaySdk.SystemInfo systemInfo;
        private static AlipaySdk.SystemInfo SystemInfo
#elif UNITY_WEBGL_KS
        private static KSWASM.SystemInfo systemInfo;
        private static KSWASM.SystemInfo SystemInfo
#elif UNITY_WEBGL_TT
        private static TTSDK.TTSystemInfo systemInfo;
        private static TTSDK.TTSystemInfo SystemInfo
#elif UNITY_WEBGL_WeChat || UNITY_WEBGL_BILIGAME
        private static WeChatWASM.SystemInfo systemInfo;
        private static WeChatWASM.SystemInfo SystemInfo
#elif UNITY_WEBGL_TAPTAP
        private static TapTapMiniGame.SystemInfo systemInfo;
        private static TapTapMiniGame.SystemInfo SystemInfo
#elif UNITY_WEBGL_QG
        private static QGMiniGame.QGSystemInfo systemInfo;
        private static QGMiniGame.QGSystemInfo SystemInfo
#elif UNITY_WEBGL_MINIHOST
        private static minihost.SystemInfo systemInfo;
        private static minihost.SystemInfo SystemInfo
#else
        //占位
        private static object systemInfo;
        private static object SystemInfo
#endif
        {
            get
            {
                if (systemInfo == null)
                {
#if UNITY_WEBGL_ALIPAY
                    systemInfo = JsonHelper.FromJson<AlipaySdk.SystemInfo>(AlipaySdk.AlipaySDK.API.GetSystemInfoSync());
#elif UNITY_WEBGL_KS
                    systemInfo = KSWASM.KS.GetSystemInfoSync();
#elif UNITY_WEBGL_TT
                    systemInfo = TTSDK.TT.GetSystemInfo();
#elif UNITY_WEBGL_TAPTAP
                    systemInfo = TapTapMiniGame.Tap.GetSystemInfoSync();
#elif UNITY_WEBGL_WeChat || UNITY_WEBGL_BILIGAME
                    systemInfo = WeChatWASM.WX.GetSystemInfoSync();
#elif UNITY_WEBGL_QG
                    systemInfo = QGMiniGame.QG.GetSystemInfoSync();
#elif UNITY_WEBGL_MINIHOST
                    systemInfo = minihost.TJ.GetSystemInfoSync();
#else
                    systemInfo = 0;
#endif
                }
                return systemInfo;
            }
        }
        
        public static float screenHeight
        {
            get
            {
#if !UNITY_EDITOR && (UNITY_WEBGL_KS || UNITY_WEBGL_TT || UNITY_WEBGL_WeChat || UNITY_WEBGL_BILIGAME || UNITY_WEBGL_TAPTAP || UNITY_WEBGL_ALIPAY || UNITY_WEBGL_QG || UNITY_WEBGL_MINIHOST)
                return (float)SystemInfo.screenHeight;
#else
                return Screen.height;
#endif
            }
        }
        
        public static float screenWidth
        {
            get
            {
#if !UNITY_EDITOR && (UNITY_WEBGL_KS || UNITY_WEBGL_TT || UNITY_WEBGL_WeChat || UNITY_WEBGL_BILIGAME || UNITY_WEBGL_TAPTAP || UNITY_WEBGL_ALIPAY || UNITY_WEBGL_QG || UNITY_WEBGL_MINIHOST)
                return (float)SystemInfo.screenWidth;
#else
                return Screen.width;
#endif
            }
        }

        public static Rect safeArea
        {
            get
            {
#if !UNITY_EDITOR && (UNITY_WEBGL_KS || UNITY_WEBGL_TT || UNITY_WEBGL_WeChat || UNITY_WEBGL_BILIGAME || UNITY_WEBGL_TAPTAP || UNITY_WEBGL_ALIPAY || UNITY_WEBGL_QG || UNITY_WEBGL_MINIHOST)
                var safeArea = SystemInfo.safeArea;
                return Rect.MinMaxRect((float)safeArea.left,(float)safeArea.top,(float)safeArea.right,(float)safeArea.bottom);
#else
                var screenSafeArea = Screen.safeArea;
                return Rect.MinMaxRect(screenSafeArea.xMin, Screen.height - screenSafeArea.yMax, screenSafeArea.xMax, Screen.height - screenSafeArea.yMin);
#endif
            }
        }
    }
}