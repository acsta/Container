using System;
using System.Collections.Generic;
using UnityEditor;

namespace TaoTie
{
    public class PlayerManager: IManager
    {
        public static PlayerManager Instance;
        public int Uid { get; private set; }
        public bool OnLine { get; private set; }
       
        public void Init()
        {
            Instance = this;
        }

        public void Destroy()
        {
            Instance = null;
        }

        public async ETTask<bool> Login(bool jump = false)
        {
            string code, nick = null, avatar = null;
            LoginPlatform platform = LoginPlatform.Dev;
            
            
#if !UNITY_EDITOR && UNITY_WEBGL
#if UNITY_WEBGL_TT
            ETTask<string> res = ETTask<string>.Create(true);
            TTSDK.TT.Login((string code, string anonymousCode, bool isLogin) =>
            {
                Log.Info("LoginSuc: " + code);
            
                res.SetResult(code);
            }, (string errMsg) =>
            {
                Log.Error(errMsg);
                res.SetResult(null);
            });
            platform = LoginPlatform.TikTok;
            code = await res;
#elif UNITY_WEBGL_TAPTAP
            ETTask<string> res = ETTask<string>.Create(true);
            TapTapMiniGame.Tap.Login(new ()
            {
                success = (data) =>
                {
                    Log.Info("LoginSuc: " + data.code);
                    res.SetResult(data.code);
                },
                fail = (data) =>
                {
                    Log.Error(data.errMsg);
                    res.SetResult(null);
                }
            });

            platform = LoginPlatform.TapTap;
            code = await res;
#elif UNITY_WEBGL_WeChat || UNITY_WEBGL_BILIGAME
            ETTask<string> res = ETTask<string>.Create(true);
            WeChatWASM.WX.Login(new WeChatWASM.LoginOption()
            {
                success = (data) =>
                {
                    Log.Info("LoginSuc: " + data.code);
                    res.SetResult(data.code);
                },
                fail = (data) =>
                {
                    Log.Error(data.errMsg);
                    res.SetResult(null);
                }
            });

            platform = 
#if UNITY_WEBGL_WeChat
                LoginPlatform.WeChat;
#elif UNITY_WEBGL_BILIGAME
                LoginPlatform.Bilibili;
#endif
            code = await res;
#elif UNITY_WEBGL_ALIPAY
            ETTask<string> res = ETTask<string>.Create(true);
            AlipaySdk.AlipaySDK.API.GetAuthCode(null, (jStr) =>
            {
                if (JsonHelper.TryFromJson(jStr, out AlipaySdk.AuthCodeResult data))
                {
                    if (data.error != 0)
                    {
                        Log.Error(data.error+" "+data.errorMessage);
                        res.SetResult(null);
                    }
                    else
                    {
                        res.SetResult(data.authCode);
                    }
                }
                else
                {
                    Log.Error(jStr);
                    res.SetResult(null);
                }
                
            });
            platform = LoginPlatform.AliPay;
            code = await res;
#elif UNITY_WEBGL_KS
            ETTask<string> res = ETTask<string>.Create(true);
            KSWASM.KS.Login(new KSWASM.LoginOption()
            {
                success = (data) =>
                {
                    Log.Info("LoginSuc: " + data.code);
                    res.SetResult(data.code);
                },
                fail = (data) =>
                {
                    Log.Error(data.msg);
                    res.SetResult(null);
                }
            });
            platform = LoginPlatform.KuaiShou;
            code = await res;
#elif UNITY_WEBGL_QG
            ETTask<string> res = ETTask<string>.Create(true);
            QGMiniGame.QG.Login(
                (data) =>
                {
                    Log.Info("LoginSuc: " + data.data.openId);
                    nick = data.data.nickName;
                    avatar = data.data.avatar;
                    res.SetResult(data.data.openId);
                },
                (data) =>
                {
                    Log.Error(data.errMsg);
                    res.SetResult(null);
                }
            );
            platform = LoginPlatform.QuickGame;
            code = await res;
#elif UNITY_WEBGL_MINIHOST
            ETTask<string> res = ETTask<string>.Create(true);
            minihost.TJ.Login(new minihost.LoginOption()
            {
                success = (data) =>
                {
                    Log.Info("LoginSuc: " + data.code);
                    res.SetResult(data.code);
                },
                fail = (data) =>
                {
                    Log.Error(data.errMsg);
                    res.SetResult(null);
                }
            });

            platform = LoginPlatform.MiniHost;
            code = await res;
#elif UNITY_WEBGL_4399
            ETTask<string> res = ETTask<string>.Create(true);
            H5Game.API.Login((data) =>
            {
                if (data == null)
                {
                    res.SetResult(null);
                    Log.Error("Login fail");
                }
                else
                {
                    res.SetResult(data.uId);
                    nick = data.userName;
                    avatar = data.Avatar;
                }
            });
            platform = LoginPlatform._4399;
            code = await res;
#else
            if (AdManager.NONE_AD_LINK)
            {
                code = CacheManager.Instance.GetString(CacheKeys.LastToken, "hh"+Guid.NewGuid());
                CacheManager.Instance.SetString(CacheKeys.LastToken, code);
            }
            else
            {
                ETTask<string> res = ETTask<string>.Create(true);
                await UIManager.Instance.OpenWindow<UILoginWin, ETTask<string>>(UILoginWin.PrefabPath, res, UILayerNames.TipLayer);
                code = await res;
            }
            nick = code;
#endif
#else
            ETTask<string> res = ETTask<string>.Create(true);
            await UIManager.Instance.OpenWindow<UILoginWin, ETTask<string>>(UILoginWin.PrefabPath, res, UILayerNames.TipLayer);
            code = await res;
            nick = code;
#endif
            
            if (string.IsNullOrEmpty(code) && !jump)
            {
                return false;
            }
            else
            {
                if (!int.TryParse(CacheManager.Instance.GetString(CacheKeys.Account, "-1"), out var uid))
                {
                    uid = -1;
                }
                PlayerData localData = null;
                var win = await UIManager.Instance.OpenWindow<UINetView>(UINetView.PrefabPath, UILayerNames.TopLayer);
                var data = await APIManager.Instance.MiniGameLogin(Define.Debug?LoginPlatform.Dev:platform, code);
                Uid = uid;
                if (data == null)
                {
                    AdManager.NONE_AD_LINK = false;
                    OnLine = false;
                    localData = CacheManager.Instance.GetValue<PlayerData>(CacheKeys.PlayerData);
                    Log.Info("离线模式");
                }
                else
                {
                    if (!data.anchor && AdManager.NONE_AD_LINK)
                    {
                        var task = ETTask.Create();
                        await UIManager.Instance.OpenWindow<UICopyWin,string, ETTask>(UICopyWin.PrefabPath,code, task,UILayerNames.TopLayer);
                        await task;
                    }
                    if(data.uid != Uid)
                    {
                        Uid = data.uid;
                    }
                    localData = CacheManager.Instance.GetValue<PlayerData>(CacheKeys.PlayerData);
                    JsonHelper.TryFromJson(data.data, out PlayerData serverData);
                    if (localData != null)
                    {
                        if (serverData != null && serverData.Version > localData.Version)
                        {
                            localData = serverData;
                        }
                    }
                    else
                    {
                        localData = serverData;
                    }
                    OnLine = true;
                    Uid = data.uid;
                }
                
                CacheManager.Instance.SetString(CacheKeys.LastToken, code);
                CacheManager.Instance.SetString(CacheKeys.Account,Uid.ToString());
                CacheManager.Instance.Save();
                PlayerDataManager.Instance.AfterLogin(localData, nick, avatar, platform);
                UIManager.Instance.CloseWindow(win).Coroutine();
                return true;
            }
        }
       
    }
}