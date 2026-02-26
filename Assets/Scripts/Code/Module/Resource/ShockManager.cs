using UnityEngine;

namespace TaoTie
{
    public class ShockManager : IManager
    {
        public static ShockManager Instance;
#if UNITY_WEBGL_TT
        TTSDK.VibrateShortParam option = new TTSDK.VibrateShortParam();
        TTSDK.VibrateLongParam loption = new TTSDK.VibrateLongParam();
#elif UNITY_WEBGL_WeChat || UNITY_WEBGL_BILIGAME
        WeChatWASM.VibrateShortOption option = new WeChatWASM.VibrateShortOption();
        WeChatWASM.VibrateLongOption loption = new WeChatWASM.VibrateLongOption();
#elif UNITY_WEBGL_TAPTAP
        TapTapMiniGame.VibrateShortOption option = new TapTapMiniGame.VibrateShortOption();
        TapTapMiniGame.VibrateLongOption loption = new TapTapMiniGame.VibrateLongOption();
#elif UNITY_WEBGL_KS
        KSWASM.VibrateShortOption option = new KSWASM.VibrateShortOption();
        KSWASM.VibrateLongOption loption = new KSWASM.VibrateLongOption();
#elif UNITY_WEBGL_MINIHOST
        minihost.VibrateShortOption option = new minihost.VibrateShortOption();
        minihost.VibrateLongOption loption = new minihost.VibrateLongOption();
#endif
        
        public bool IsOpen { get; private set; }

        public void Init()
        {
            Instance = this;
            IsOpen = CacheManager.Instance.GetInt(CacheKeys.ShockCycle, 1) == 1;
        }

        public void Destroy()
        {
            Instance = null;
        }

        public void SetOpen(bool open)
        {
            IsOpen = open;
            CacheManager.Instance.SetInt(CacheKeys.ShockCycle, IsOpen ? 1 : 0);
            CacheManager.Instance.Save();
        }
        
        public void Vibrate()
        {
            if(!IsOpen) return;
#if UNITY_WEBGL_TT && !UNITY_EDITOR
            TTSDK.TT.VibrateShort(option);
#elif UNITY_WEBGL_TAPTAP && !UNITY_EDITOR
            TapTapMiniGame.Tap.VibrateShort(option);
#elif (UNITY_WEBGL_WeChat || UNITY_WEBGL_BILIGAME) && !UNITY_EDITOR
            WeChatWASM.WX.VibrateShort(option);
#elif UNITY_WEBGL_ALIPAY && !UNITY_EDITOR
            AlipaySdk.AlipaySDK.API.VibrateShort(null);
#elif UNITY_WEBGL_KS && !UNITY_EDITOR
            KSWASM.KS.VibrateShort(option);
#elif UNITY_WEBGL_QG && !UNITY_EDITOR
            QGMiniGame.QG.VibrateShort();
#elif UNITY_WEBGL_MINIHOST && !UNITY_EDITOR
            minihost.TJ.VibrateShort(option);
#else
            BridgeHelper.DoVibrate(30);
#endif
        }
        
        public void LongVibrate()
        {
            if(!IsOpen) return;
#if UNITY_WEBGL_TT && !UNITY_EDITOR
            TTSDK.TT.VibrateLong(loption);
#elif UNITY_WEBGL_TAPTAP && !UNITY_EDITOR
            TapTapMiniGame.Tap.VibrateLong(loption);
#elif (UNITY_WEBGL_WeChat || UNITY_WEBGL_BILIGAME) && !UNITY_EDITOR
            WeChatWASM.WX.VibrateLong(loption);
#elif UNITY_WEBGL_ALIPAY && !UNITY_EDITOR
            AlipaySdk.AlipaySDK.API.VibrateLong(null);
#elif UNITY_WEBGL_KS && !UNITY_EDITOR
            KSWASM.KS.VibrateLong(loption);
#elif UNITY_WEBGL_QG && !UNITY_EDITOR
            QGMiniGame.QG.VibrateLong();
#elif UNITY_WEBGL_MINIHOST && !UNITY_EDITOR
            minihost.TJ.VibrateLong(loption);
#else
            BridgeHelper.DoVibrate(400);
#endif
        }
    }
}