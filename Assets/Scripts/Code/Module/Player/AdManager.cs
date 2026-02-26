using System;
using System.Collections.Generic;
using UnityEngine;

namespace TaoTie
{
    public class AdManager: IManager
    { 
	    /// <summary>
	    /// 无广链接
	    /// </summary>
	    public static bool NONE_AD_LINK = true;
	    public const string TTVideoAdId = "43if0f154m6110fmej";
	    public const string WXVideoAdId = "adunit-e4d84cbe60b6e84f";
	    public const string KsVideoAdId = "todo:";
	    public const string TapTapVideoAdId = "todo:";
	    public const string AliPayAdId = "202509282202083890";
	    public const string FBAdId = "todo:";
	    public const string TJVideoAdId = "todo:";
	    public const string BLVideoAdId = "todo:";
	    public static AdManager Instance;

	    enum AdState
	    {
		    Loading,
		    Loaded,
		    Playing,
		    Success,
		    Fail,
	    }
	    
	    private AdState State;
	    bool isLoaded = false;
#if UNITY_WEBGL_TT
	    TTSDK.CreateRewardedVideoAdParam param { get; } = new () { AdUnitId = TTVideoAdId };
	    private TTSDK.TTRewardedVideoAd ad;
#elif UNITY_WEBGL_TAPTAP
	    TapTapMiniGame.TapRewardedVideoAd ad;
	    TapTapMiniGame.TapCreateRewardedVideoAdParam param { get; } = new() {adUnitId = TapTapVideoAdId};
#elif UNITY_WEBGL_KS
		KSWASM.RewardVideoAd ad = null;
#elif UNITY_WEBGL_WeChat || UNITY_WEBGL_BILIGAME
	    WeChatWASM.WXRewardedVideoAd ad;
	    WeChatWASM.WXCreateRewardedVideoAdParam param { get; } = new (){adUnitId = 
#if UNITY_WEBGL_WeChat
		    WXVideoAdId
#else
			BLVideoAdId
#endif
	    };
#elif UNITY_WEBGL_QG
	    private QGMiniGame.QGRewardedVideoAd ad;
#elif UNITY_WEBGL_ALIPAY
	    private AlipaySdk.Ad.AlipayAdManager adManager;
	    private AlipaySdk.Ad.AlipayRewardAdEntity ad;
#elif  UNITY_WEBGL_FACEBOOK
	    private Meta.InstantGames.AdInstance ad;
#elif UNITY_WEBGL_MINIHOST
	    minihost.TJRewardedVideoAd ad;
	    minihost.TJCreateRewardedVideoAdParam param { get; } = new (){adUnitId = TJVideoAdId};
#endif
	    public void Init()
        {
	        Instance = this;
#if UNITY_WEBGL_ALIPAY
	        adManager = AlipaySdk.AlipaySDK.API.GetAdManager();
#endif
	        GetNewAd().Coroutine();
        }

        public void Destroy()
        {
	        Instance = null;
#if UNITY_WEBGL_TT || UNITY_WEBGL_KS || UNITY_WEBGL_WeChat || UNITY_WEBGL_TAPTAP || UNITY_WEBGL_QG || UNITY_WEBGL_MINIHOST || UNITY_WEBGL_BILIGAME
	        if (ad != null)
	        {
		        ad.Destroy();
		        ad = null;
	        }
#endif
        }

        /// <summary>
        /// 当前平台是否支持广告
        /// </summary>
        /// <returns></returns>
        public bool PlatformHasAD()
        {
#if UNITY_WEBGL_TT || UNITY_WEBGL_KS || UNITY_WEBGL_TAPTAP || UNITY_WEBGL_QG || UNITY_EDITOR || UNITY_WEBGL_MINIHOST || UNITY_WEBGL_BILIGAME || UNITY_WEBGL_4399 || UNITY_WEBGL_FACEBOOK
	        return true;
#elif UNITY_WEBGL_WeChat
	        return IsEligibility();
#else
	        return NONE_AD_LINK;
#endif
        }
        private async ETTask GetNewAd()
        {
	        if(!PlatformHasAD()) return;
	        await ETTask.CompletedTask;
#if UNITY_WEBGL_TT
	        if (ad != null)
	        {
		        if (isLoaded)
		        {
			        State = AdState.Loaded;
			        isLoaded = false;
		        }
		        else
		        {
			        State = AdState.Loading;
		        }
		        return;
	        }
	        State = AdState.Loading;
	        ad = TTSDK.TT.CreateRewardedVideoAd(param);
	        ad.OnClose += OnPlayVideoOver;
	        ad.OnError += OnPlayVideoError;
	        ad.OnLoad += OnPlayVideoLoad;
	        ad.Load();
#elif UNITY_WEBGL_KS && !UNITY_EDITOR
	        if (ad != null)
	        {
		        if (isLoaded)
		        {
			        State = AdState.Loaded;
			        isLoaded = false;
		        }
		        else
		        {
			        State = AdState.Loading;
		        }
		        return;
	        }
	        State = AdState.Loading;
	        ad = KSWASM.KS.CreateRewardedVideoAd(KsVideoAdId);
	        if (ad != null)
	        {
		        ad.OnClose((data) =>
		        {
			        var isEnd = data.isEnded;
			        Log.Info($"激励视频关闭 ended: {isEnd}");
			        State = isEnd ? AdState.Success : AdState.Fail;
		        });
		        ad.OnError((result) =>
		        {
			        State = AdState.Fail;
			        Log.Info("Reward AD Error");
		        });
				ad.OnLoad((result) =>
				{
					OnPlayVideoLoad();
				});
	        }
#elif (UNITY_WEBGL_WeChat || UNITY_WEBGL_BILIGAME) && !UNITY_EDITOR
	        if (ad != null)
	        {
		        if (isLoaded)
		        {
			        State = AdState.Loaded;
			        isLoaded = false;
		        }
		        else
		        {
			        State = AdState.Loading;
		        }
		        return;
	        }
	        State = AdState.Loading;
	        ad = WeChatWASM.WX.CreateRewardedVideoAd(param);
	        ad.OnClose((rewardedVideoAdOnCloseResponse) =>
	        {
		        var isEnd = rewardedVideoAdOnCloseResponse.isEnded;
		        Log.Info($"激励视频关闭 ended: {isEnd}");
		        State = isEnd ? AdState.Success : AdState.Fail;
	        });
	        ad.OnError((errorResponse) =>
	        {
		        Log.Error($"激励视频错误 errorCode: {errorResponse.errCode}");
		        
		        if (ad != null)
		        {
			        ad.Destroy();
			        ad = null;
		        }
		        
		        if (State == AdState.Loading)
		        {
			        GetNewAd().Coroutine();
		        }
		        else if (State == AdState.Playing)
		        {
			        State = AdState.Fail;
		        }
	        });
	        ad.OnLoad((loadResponse) =>
	        {
		        OnPlayVideoLoad();
	        });
#elif UNITY_WEBGL_TAPTAP && !UNITY_EDITOR
	        if (ad != null)
	        {
		        if (isLoaded)
		        {
			        State = AdState.Loaded;
			        isLoaded = false;
		        }
		        else
		        {
			        State = AdState.Loading;
		        }
		        return;
	        }
	        State = AdState.Loading;
	        ad = TapTapMiniGame.Tap.CreateRewardedVideoAd(param);
	        ad.OnClose((rewardedVideoAdOnCloseResponse) =>
	        {
		        var isEnd = rewardedVideoAdOnCloseResponse.isEnded;
		        Log.Info($"激励视频关闭 ended: {isEnd}");
		        State = isEnd ? AdState.Success : AdState.Fail;
	        });
	        ad.OnError((errorResponse) =>
	        {
		        Log.Error($"激励视频错误 errorCode: {errorResponse.errCode}");
		        
		        if (ad != null)
		        {
			        ad.Destroy();
			        ad = null;
		        }
		        
		        if (State == AdState.Loading)
		        {
			        GetNewAd().Coroutine();
		        }
		        else if (State == AdState.Playing)
		        {
			        State = AdState.Fail;
		        }
	        });
	        ad.OnLoad((loadResponse) =>
	        {
		        OnPlayVideoLoad();
	        });
#elif UNITY_WEBGL_QG && !UNITY_EDITOR
	        if (ad != null)
	        {
		        if (isLoaded)
		        {
			        State = AdState.Loaded;
			        isLoaded = false;
		        }
		        else
		        {
			        State = AdState.Loading;
		        }
		        return;
	        }
	        string bannerId = null;
	        var info = QGMiniGame.QG.GetAdInfo();
	        if (info.ContainsKey("rewardedVideoAd"))
	        {
		        var rvad = info["rewardedVideoAd"];
		        if (rvad.ContainsKey("id1"))
		        {
			        bannerId = (string)rvad["id1"];
		        }
	        }
	        State = AdState.Loading;
	        if (!string.IsNullOrEmpty(bannerId))
	        {
		        ad = QGMiniGame.QG.CreateRewardedVideoAd(new QGMiniGame.QGCommonAdParam()
		        {
			        adUnitId = bannerId
		        });
		        ad.OnClose((data) =>
		        {
			        var isEnd = data.isEnded;
			        Log.Info($"激励视频关闭 ended: {isEnd}");
			        State = isEnd ? AdState.Success : AdState.Fail;
		        });
		        ad.OnError((result) =>
		        {
			        State = AdState.Fail;
			        Log.Info("Reward AD Error");
		        });
		        ad.OnLoad(OnPlayVideoLoad);
	        }
#elif UNITY_WEBGL_ALIPAY && !UNITY_EDITOR
	        if (ad != null)
	        {
		        if (isLoaded)
		        {
			        State = AdState.Loaded;
			        isLoaded = false;
		        }
		        else
		        {
			        State = AdState.Loading;
		        }
		        return;
	        }
	        
	        ad = adManager.CreateRewardAd(AliPayAdId);
	        ad.OnClose = (isEnd) =>
	        {
		        Log.Info($"激励视频关闭 ended: {isEnd}");
		        State = isEnd ? AdState.Success : AdState.Fail;
	        };
	        ad.OnError = ((result) =>
	        {
		        State = AdState.Fail;
		        Log.Info("Reward AD Error:"+result);
	        });
	        ad.OnLoad = OnPlayVideoLoad;
#elif UNITY_WEBGL_FACEBOOK
	        if (ad != null)
	        {
		        if (isLoaded)
		        {
			        State = AdState.Loaded;
			        isLoaded = false;
		        }
		        else
		        {
			        State = AdState.Loading;
		        }
		        return;
	        }
			State = AdState.Loading;
	        var initTask = Meta.InstantGames.Sdk.FBInstant.GetRewardedVideoAsync(FBAdId);
	        while (!initTask.IsCompleted)
	        {
		        await TimerManager.Instance.WaitAsync(1);
	        }

	        if (initTask.IsFaulted)
	        {
		        Log.Error(initTask.Exception);
		        State = AdState.Fail;
		        return;
	        }
	        else
	        {
		        ad = await initTask;
	        }
	        var task = ad.LoadAsync();
	        while (!task.IsCompleted)
	        {
		        await TimerManager.Instance.WaitAsync(1);
	        }
	        if (task.IsFaulted)
	        {
		        Log.Error(task.Exception);
		        State = AdState.Fail;
		        return;
	        }
	        OnPlayVideoLoad();
#elif UNITY_WEBGL_MINIHOST && !UNITY_EDITOR
	        if (ad != null)
	        {
		        if (isLoaded)
		        {
			        State = AdState.Loaded;
			        isLoaded = false;
		        }
		        else
		        {
			        State = AdState.Loading;
		        }
		        return;
	        }
	        State = AdState.Loading;
	        ad = minihost.TJ.CreateRewardedVideoAd(param);
	        if (ad != null)
	        {
		        ad.OnClose((data) =>
		        {
			        var isEnd = data.isEnded;
			        Log.Info($"激励视频关闭 ended: {isEnd}");
			        State = isEnd ? AdState.Success : AdState.Fail;
		        });
		        ad.OnError((result) =>
		        {
			        State = AdState.Fail;
			        Log.Info("Reward AD Error");
		        });
				ad.OnLoad((result) =>
				{
					OnPlayVideoLoad();
				});
	        }
#endif
        }

        public async ETTask<bool> PlayAd()
        {
#if UNITY_WEBGL_WeChat || UNITY_WEBGL_TAPTAP || UNITY_WEBGL_ALIPAY || UNITY_WEBGL_MINIHOST || UNITY_WEBGL_BILIGAME || UNITY_WEBGL_TT || UNITY_WEBGL_KS || UNITY_WEBGL_QG
	        Log.Info("AD State" + State);
	        
	        OnPlayVideoLoad();
	        while (State != AdState.Loaded)
	        {
		        Log.Info("State != AdState.Loaded   AD State" + State);
		        await TimerManager.Instance.WaitAsync(1);
	        }
	        
	        if (State == AdState.Loaded)
	        {
		        Log.Info("State == AdState.Loaded AD State" + State);
		        State = AdState.Playing;
#if UNITY_WEBGL_BILIGAME
		        WeChatWASM.WX.ReportScene(new WeChatWASM.ReportSceneParams()
		        {
			        sceneId = 1007,
		        });
#endif
		        ad.Show();
	        }
	        while (State == AdState.Playing)
	        {
		        Log.Info("State == AdState.Playing        AD State" + State);
		        await TimerManager.Instance.WaitAsync(1);
	        }
	        
	        var res = State == AdState.Success;
	        Log.Info((res ? "Play Ad Successfully" : "Play Ad fail") + State);
	        GetNewAd().Coroutine();
	        return res;
#elif UNITY_WEBGL_FACEBOOK
	        OnPlayVideoLoad();
	        while (State != AdState.Loaded)
	        {
		        await TimerManager.Instance.WaitAsync(1);
	        }
	        if (State == AdState.Loaded)
	        {
		        Log.Info("State == AdState.Loaded AD State" + State.ToString());
		        State = AdState.Playing;
		        var task = ad.ShowAsync();
		        while (!task.IsCompleted)
		        {
			        await TimerManager.Instance.WaitAsync(1);
		        }
		        if (task.IsFaulted)
		        {
			        Log.Error(task.Exception);
			        State = AdState.Fail;
		        }
		        else
		        {
			        State = AdState.Success;
		        }
	        }
	        while (State == AdState.Playing)
	        {
		        Log.Info("State == AdState.Playing        AD State" + State.ToString());
		        await TimerManager.Instance.WaitAsync(1);
	        }
	        
	        var res = State == AdState.Success;
	        Log.Info((res ? "Play Ad Successfully" : "Play Ad fail") + State);
	        GetNewAd().Coroutine();
	        return true;
#elif UNITY_WEBGL_4399
	        ETTask<bool> task = ETTask<bool>.Create();
	        H5Game.API.PlayAd((res, str) =>
	        {
		        if(!res) Log.Error(str);
		        task.SetResult(res);
	        });
	        return await task;
#else
	        await ETTask.CompletedTask;
	        return true;
#endif
        }

        private void OnPlayVideoLoad()
        {
	        isLoaded = State != AdState.Loading;
	        if (State == AdState.Loading)
	        {
		        Log.Info("Load AD");
		        State = AdState.Loaded;
	        }

        }
        
        private void OnPlayVideoOver(bool ended, int count)
        {
        	Log.Info($"激励视频关闭 ended: {ended}, count: {count}");
            State = ended ? AdState.Success : AdState.Fail;
        }
        
        private void OnPlayVideoError(int errorCode,string msg)
        {
        	Log.Error($"激励视频错误 errorCode: {errorCode} \r\n{msg}");
            if (State == AdState.Loading)
            {
	            GetNewAd().Coroutine();
            }
            else if (State == AdState.Playing)
            {
	            State = AdState.Fail;
            }
        }

#if UNITY_WEBGL_WeChat
        private bool IsEligibility()
        {
#if UNITY_EDITOR
	        return true;
#endif
	        var vs = WeChatWASM.WX.GetSystemInfoSync().SDKVersion.Split(".");
	        var baseVerion = new List<int>() {2, 0, 4};
	        for (int i = 0; i < vs.Length; i++) 
	        {
		        int.TryParse(vs[i], out var data);
		        if (data != baseVerion[i])
		        {
			        return data.CompareTo(baseVerion[i]) > 0;
		        }
	        }

	        return false;
        }
#endif
    }
}