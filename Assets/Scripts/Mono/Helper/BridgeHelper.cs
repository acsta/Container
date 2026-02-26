using UnityEngine;

namespace TaoTie
{
    public static partial class BridgeHelper
    {
        public static void Quit()
        {
#if UNITY_WEBGL
#if UNITY_WEBGL_QG
            QGMiniGame.QG.ExitApplication();
#else
            CloseWindow();
#endif
#else
            Application.Quit();
#endif
        }

        public static void DoVibrate(int during = 500)
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            Vibrate(during);
#elif UNITY_ANDROID ||UNITY_IOS
            Handheld.Vibrate();//0.5s
#else
            Log.Info("Vibrate Callback");
#endif
        }
        
        public static void CopyBuffer(string content)
        {
#if UNITY_WEBGL && !UNITY_EDITOR
#if UNITY_WEBGL_TT
            TTSDK.TT.SetClipboardData(content);
#elif UNITY_WEBGL_WeChat || UNITY_WEBGL_BILIGAME
            WeChatWASM.WX.SetClipboardData(new WeChatWASM.SetClipboardDataOption()
            {
                data = content
            });
#elif UNITY_WEBGL_TAPTAP
            TapTapMiniGame.Tap.SetClipboardData(new TapTapMiniGame.SetClipboardDataOption()
            {
                data = content
            });
#elif UNITY_WEBGL_MINIHOST
            minihost.TJ.SetClipboardData(new minihost.SetClipboardDataOption()
            {
                data = content
            });
#elif UNITY_WEBGL_KS
            KSWASM.KS.SetClipboardData(new KSWASM.SetClipboardDataOption()
            {
                data = content
            });
#elif UNITY_WEBGL_QG
            QGMiniGame.QG.SetClipboardData(content);
#elif UNITY_WEBGL_ALIPAY
            AlipaySdk.AlipaySDK.API.SetClipboard(content, null);
#else
            CopyTextToClipboard(content);
#endif
#else
            GUIUtility.systemCopyBuffer = content;
#endif
        }
    }
}