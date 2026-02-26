using System.Collections.Generic;

namespace TaoTie
{
    public class SDKManager: IManager
    {
        public static SDKManager Instance;
        private bool? isAuth;
        private bool canSliderBar;
        public void Init()
        {
            canSliderBar = false;
#if UNITY_WEBGL_TAPTAP && !UNITY_EDITOR
            TapTapMiniGame.Tap.GetLeaderBoardManager();
#endif
#if UNITY_WEBGL_TT && !UNITY_EDITOR
            TTSDK.TT.CheckScene(TTSDK.TTSideBar.SceneEnum.SideBar, (res) =>
            {
                canSliderBar = res;
            }, null, null);
#elif UNITY_WEBGL_KS && !UNITY_EDITOR
            KSWASM.KS.CheckSliderBarIsAvailable(new KSWASM.CheckSliderBarIsAvailableOption()
            {
                success = (res) =>
                {
                    canSliderBar = res.available;
                }
            });
#elif UNITY_WEBGL_BILIGAME && !UNITY_EDITOR
            WeChatWASM.WX.CheckScene(new WeChatWASM.CheckSceneOption()
            {
                scene = "sidebar",
                success = (res) =>
                {
                    canSliderBar = res.isExist;
                }
            });
#endif
            Instance = this;
        }

        public void Destroy()
        {
            Instance = null;
        }

        public bool IsAuth()
        {
            return isAuth == true;
        }
        public async ETTask<bool> Auth()
        {
            if (isAuth != null) return (bool) isAuth;
            var res = await CheckAuth();
            isAuth = res;
            return res;
        }
        
        private async ETTask<bool> CheckAuth()
        {
            await ETTask.CompletedTask;
            string nick = null, avatar = null;
#if !UNITY_EDITOR
#if UNITY_WEBGL_TT
            ETTask<bool> auth = ETTask<bool>.Create(true);
            TTSDK.TT.GetUserInfoAuth((isAuth) =>
            {
                if (isAuth)
                {
                    auth.SetResult(true);
                }
                else
                {
                    TTSDK.TT.OpenSettingsPanel((isAuth2) =>
                    {
                        auth.SetResult(isAuth2);
                    }, (errMsg) =>
                    {
                        Log.Error(errMsg);
                        auth.SetResult(false);
                    });
                }
            }, (errMsg) =>
            {
                Log.Error(errMsg);
                auth.SetResult(false);
            });
            if (await auth)
            {
                ETTask task = ETTask.Create(true);
                TTSDK.TT.GetUserInfo((ref TTSDK.TTUserInfo userInfo) =>
                {
                    nick = userInfo.nickName;
                    avatar = userInfo.avatarUrl;
                    task.SetResult();
                }, (errMsg) =>
                {
                    Log.Error(errMsg);
                    task.SetResult();
                });
                await task;
            }
#elif UNITY_WEBGL_TAPTAP
            ETTask<bool> auth = ETTask<bool>.Create(true);
            TapTapMiniGame.Tap.GetSetting(new TapTapMiniGame.GetSettingOption()
            {
                success = (res) =>
                {
                    if (res.authSetting.ContainsKey("scope.userInfo") && res.authSetting["scope.userInfo"])
                    {
                        auth.SetResult(true);
                    }
                    else
                    {
                        auth.SetResult(false);
                    }
                },
                fail = (res) =>
                {
                    auth.SetResult(false);
                    Log.Error(res.errMsg);
                }
            });
            if (await auth)
            {
                ETTask task = ETTask.Create(true);
                TapTapMiniGame.Tap.GetUserInfo(new TapTapMiniGame.GetUserInfoOption
                {
                    success = (res) =>
                    {
                        nick = res.userInfo.nickName;
                        avatar = res.userInfo.avatarUrl;
                        task.SetResult();
                    },
                    fail = (res) =>
                    {
                        Log.Error(res.errMsg);
                        task.SetResult();
                    },
                });
                await task;
            }
#elif UNITY_WEBGL_WeChat || UNITY_WEBGL_BILIGAME
            ETTask<bool> auth = ETTask<bool>.Create(true);
            WeChatWASM.WX.GetSetting(new WeChatWASM.GetSettingOption()
            {
                success = (res) =>
                {
                    if (res.authSetting.ContainsKey("scope.userInfo") && res.authSetting["scope.userInfo"])
                    {
                        auth.SetResult(true);
                    }
                    else
                    {
                        auth.SetResult(false);
                    }
                },
                fail = (res) =>
                {
                    auth.SetResult(false);
                    Log.Error(res.errMsg);
                }
            });
            if (await auth)
            {
                ETTask task = ETTask.Create(true);
                WeChatWASM.WX.GetUserInfo(new WeChatWASM.GetUserInfoOption
                {
                    success = (res) =>
                    {
                        nick = res.userInfo.nickName;
                        avatar = res.userInfo.avatarUrl;
                        task.SetResult();
                    },
                    fail = (res) =>
                    {
                        Log.Error(res.errMsg);
                        task.SetResult();
                    },
                });
                await task;
            }
#elif UNITY_WEBGL_ALIPAY
            ETTask<bool> auth = ETTask<bool>.Create(true);
          
            AlipaySdk.AlipaySDK.API.GetAuthCode(new []{"auth_user"}, res =>
            {
                var authCode = JsonHelper.FromJson<AlipaySdk.AuthCodeResult>(res);
                auth.SetResult(authCode?.error == 0);
            });
            if (await auth)
            {
                ETTask task = ETTask.Create(true);
                AlipaySdk.AlipaySDK.API.GetAuthUserInfo((res) =>
                {
                    var loginData = JsonHelper.FromJson<AlipaySdk.LoginData>(res);
                    if (loginData?.nickName != null)
                    {
                        nick = loginData.nickName;
                    }
                    else
                    {
                        Log.Error("no nick name");
                    }

                    if (loginData?.avatar != null)
                    {
                        avatar = loginData.avatar;
                    }
                    else
                    {
                        Log.Error("no avatar");
                    }
                    task.SetResult();
                });
                await task;
            }
#elif UNITY_WEBGL_KS
            ETTask<bool> auth = ETTask<bool>.Create(true);
            KSWASM.KS.GetSetting(new KSWASM.GetSettingOption()
            {
                success = (res) =>
                {
                    if (res.authSetting.ContainsKey("scope.userInfo") && res.authSetting["scope.userInfo"])
                    {
                        auth.SetResult(true);
                    }
                    else
                    {
                        auth.SetResult(false);
                    }
                },
                fail = (res) =>
                {
                    auth.SetResult(false);
                    Log.Error(res.msg);
                }
            });
            if (await auth)
            {
                ETTask task = ETTask.Create(true);
                KSWASM.KS.GetUserInfo(new KSWASM.GetUserInfoOption
                {
                    success = (res) =>
                    {
                        nick = res.userInfo.nickName;
                        avatar = res.userInfo.avatarUrl;
                        task.SetResult();
                    },
                    fail = (res) =>
                    {
                        Log.Error(res.msg);
                        task.SetResult();
                    },
                });
                await task;
            }
#elif UNITY_WEBGL_MINIHOST
            ETTask<bool> auth = ETTask<bool>.Create(true);
            minihost.TJ.GetSetting(new minihost.GetSettingOption()
            {
                success = (res) =>
                {
                    if (res.authSetting.ContainsKey("scope.userInfo") && res.authSetting["scope.userInfo"])
                    {
                        auth.SetResult(true);
                    }
                    else
                    {
                        auth.SetResult(false);
                    }
                },
                fail = (res) =>
                {
                    auth.SetResult(false);
                    Log.Error(res.errMsg);
                }
            });
            if (await auth)
            {
                ETTask task = ETTask.Create(true);
                minihost.TJ.GetUserInfo(new minihost.GetUserInfoOption
                {
                    success = (res) =>
                    {
                        nick = res.userInfo.nickName;
                        avatar = res.userInfo.avatarUrl;
                        task.SetResult();
                    },
                    fail = (res) =>
                    {
                        Log.Error(res.errMsg);
                        task.SetResult();
                    },
                });
                await task;
            }
#endif
#endif
            PlayerDataManager.Instance.UpdateUserInfo(nick, avatar);
            return true;
        }

        public void SetImRankData(string val)
        {
#if !UNITY_EDITOR
#if UNITY_WEBGL_TT
            TTSDK.TT.SetImRankData(new TTSDK.UNBridgeLib.LitJson.JsonData()
            {
                ["dataType"] = 0,
                ["value"] = val,
                ["priority"] = 0,
                ["zoneId"] ="default"
            });
#elif UNITY_WEBGL_WeChat || UNITY_WEBGL_BILIGAME
            WeChatWASM.WX.SetUserCloudStorage(new WeChatWASM.SetUserCloudStorageOption()
            {
                KVDataList = new WeChatWASM.KVData[]
                {
                    new WeChatWASM.KVData {key = "value", value = val}
                }
            });
#elif  UNITY_WEBGL_TAPTAP
            BigNumber data = val;
            long score;
            if (data >= long.MaxValue)
            {
                score = long.MaxValue;
            }
            else
            {
                score = (long) (decimal.Parse(data));
            }
            TapTapMiniGame.Tap.SubmitScore(new TapTapMiniGame.SubmitScoreOption()
            {
                scores = new List<TapTapMiniGame.ScoreItem>(){new TapTapMiniGame.ScoreItem
                {
                    leaderboardId = "1ae3b9iclmdpcqv31k",
                    score = score
                }}
            });
#endif
#endif
        }
        
        public bool GetImRankList()
        {
#if !UNITY_EDITOR
#if UNITY_WEBGL_TT
            TTSDK.TT.GetImRankList(new TTSDK.UNBridgeLib.LitJson.JsonData()
            {
                ["rankType"] = "all",
                ["dataType"] = 0,
                ["relationType"] = "default",
                ["suffix"] = null,
                ["rankTitle"] = I18NManager.Instance.I18NGetText(I18NKey.Text_Title_Rank),
                ["zoneId"] ="default",
            });
            return true;
#elif  UNITY_WEBGL_TAPTAP
            TapTapMiniGame.Tap.OpenLeaderboard(new TapTapMiniGame.OpenLeaderboardOption()
            {
                leaderboardId = "1ae3b9iclmdpcqv31k"
            });
            return true;
#endif
#endif
            return false;
        }

        public async ETTask ShareGlobal(int scene = 0)
        {
            var list = ShareConfigCategory.Instance.GetShareConfig(scene);
            if (list == null)
            {
                Log.Error("未配置场景"+scene);
                return;
            }
#if !UNITY_EDITOR
#if UNITY_WEBGL_WeChat
            list.RandomSort();
            ShareConfig sc = null;
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].Type == 0 || list[i].Type == 1)
                {
                    sc = list[i];
                    break;
                }
            }

            if (sc == null)
            {
                Log.Error("未配置微信分享场景"+scene);
                return;
            }

            var option = new WeChatWASM.ShareAppMessageOption();
            if (!string.IsNullOrEmpty(sc.Title))
            {
                option.title = sc.Title;
            }
            if (sc.Type == 0)
            {
                option.imageUrl = sc.Value;
            }
            else if (sc.Type == 1)
            {
                option.imageUrlId = sc.Value;
            }
            else
            {
                UIToast.ShowToast(I18NKey.Global_Unknow);
                return;
            }
            WeChatWASM.WX.ShareAppMessage(option);
#elif UNITY_WEBGL_TT
            list.RandomSort();
            ShareConfig sc = null;
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].Type == 2 || list[i].Type == 3)
                {
                    sc = list[i];
                    break;
                }
            }

            if (sc == null)
            {
                Log.Error("未配置抖音分享场景"+scene);
                return;
            }
            TTSDK.UNBridgeLib.LitJson.JsonData shareJson = new TTSDK.UNBridgeLib.LitJson.JsonData();
            if (sc.Type == 3)
            {
                ETTask<string> task = ETTask<string>.Create();
                TTSDK.TT.ChooseImage(new TTSDK.TTChooseImageParam()
                {
                    sourceType = new string[]{"album"},
                    count = 1,
                    success = (res =>
                    {
                        if (res.tempFilePaths.Length > 0)
                        {
                            task.SetResult(res.tempFilePaths[0]);
                        }
                        else
                        {
                            Log.Error("ChooseImage No Paths");
                            task.SetResult(null);
                        }
                        
                    }),
                    fail = (res) =>
                    {
                        Log.Error("ChooseImage Fail. "+res);
                        task.SetResult(null);
                    }
                });
                var path = await task;
                if (string.IsNullOrEmpty(path))
                {
                    return;
                }
                shareJson["channel"] = "picture";
                if(!string.IsNullOrEmpty(sc.Title))
                    shareJson["title"] = sc.Title;
                if(!string.IsNullOrEmpty(sc.Content))
                    shareJson["desc"] = sc.Content;
                shareJson["extra"] = new TTSDK.UNBridgeLib.LitJson.JsonData();
                shareJson["extra"]["picturePath"] = path;
            }
            else if (sc.Type == 2)
            {
                shareJson["channel"] = "invite";
                if(!string.IsNullOrEmpty(sc.Title))
                    shareJson["title"] = sc.Title;
                if(!string.IsNullOrEmpty(sc.Content))
                    shareJson["desc"] = sc.Content;
                if (sc.Type == 2)
                {
                    shareJson["templateId"] = sc.Value;
                }
            }
            else
            {
                UIToast.ShowToast(I18NKey.Global_Unknow);
                return;
            }
            TTSDK.TT.ShareAppMessage(shareJson);
#elif UNITY_WEBGL_TAPTAP
            list.RandomSort();
            ShareConfig sc = null;
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].Type == 4)
                {
                    sc = list[i];
                    break;
                }
            }

            if (sc == null)
            {
                Log.Error("未配置TapTap分享场景"+scene);
                return;
            }
            
            if (sc.Type == 4)
            {
                var options = new TapTapMiniGame.ShowShareboardOption();
                options.templateId = sc.Value;
                TapTapMiniGame.Tap.ShowShareboard(options);
            }
            else
            {
                UIToast.ShowToast(I18NKey.Global_Unknow);
                return;
            }
#endif
#endif
            await ETTask.CompletedTask;
        }

        public void Recommend()
        {
#if !UNITY_EDITOR
#if UNITY_WEBGL_WeChat
            WeChatWASM.WX.OpenPage(new WeChatWASM.OpenPageOption()
            {
                openlink = "TWFRCqV5WeM2AkMXhKwJ03MhfPOieJfAsvXKUbWvQFQtLyyA5etMPabBehga950uzfZcH3Vi3QeEh41xRGEVFw"
            });
#endif
#endif
            Log.Info("推荐");
        }
        
        public void GameGroup()
        {
#if !UNITY_EDITOR
#if UNITY_WEBGL_WeChat 
			WeChatWASM.WX.OpenPage(new WeChatWASM.OpenPageOption()
			{
				openlink = "-SSEykJvFV3pORt5kTNpS1h1dUJtq-BGMx1fakBJCa1nzCNfq9dAp6aCJsGGW4XIitr1sktaByZ2-NjQTBNXpOINvM5Kbh7RT7n4-kQDEHHQtHJUXOEDw0ebCSbJBqBe6pPffISLGe_rPEN3zkbgym8i1VAbwK4wUW2t6Y_gBP5zWhoWr6_9y3ODogiZWt0yudPb_3TqUJx3H5i_P2cfdalsUcDSjb2t1mMCUWZoNyp9_6eu7u5oAs5B0doZTp_QDOUafC1N2lJEEUdw7NtbzMX5LUfiQsfjrPQk5RJzh0gkCmvClLukhwlNmazDCaNYyptwkO9AX8u7w7BtrZ0aiw",
			});
#elif UNITY_WEBGL_TT
            TTSDK.TT.NavigateToMiniProgram(new TTSDK.NavigateToMiniProgramParam()
            {
                AppId = "ttc9e5bf5856eff54001",
                Fail = (errInfo) =>
                {
                    Log.Error($"NavigateToMiniProgram Fail, ErrMsg : ${errInfo.ErrMsg},ErrCode: ${errInfo.ErrorCode},ErrType: ${errInfo.ErrorType}");
                },
            });
#elif UNITY_WEBGL_KS
            KSWASM.KS.JumpToGameClub(new KSWASM.JumpToGameClubOption()
            {
                fail = (errInfo)=>{
                    Log.Error($"JumpToGameClub Fail, ErrMsg : ${errInfo.msg},ErrCode: ${errInfo.code}");
                }
            });
#endif
#endif
            Log.Info("点击游戏圈");
        }

        public void Desktop()
        {
#if !UNITY_EDITOR
#if UNITY_WEBGL_TT
            TTSDK.TT.AddShortcut((res) =>
            {
                if (!res)
                {
                    UIToast.ShowToast(I18NKey.Global_Control_Fail);
                }
            });
#elif  UNITY_WEBGL_TAPTAP
            TapTapMiniGame.Tap.CreateHomeScreenWidget(new TapTapMiniGame.CreateHomeScreenWidgetOption());
#elif  UNITY_WEBGL_BILIGAME
            WeChatWASM.WX.AddShortcut(new WeChatWASM.AddShortcutOption());
#elif  UNITY_WEBGL_KS
            KSWASM.KS.AddShortcut(new KSWASM.AddShortcutOption());
#endif
#endif
            Log.Info("添加桌面");
        }

        public void Collect()
        {
#if !UNITY_EDITOR
#if UNITY_WEBGL_TT
            TTSDK.TT.ShowFavoriteGuide();
#elif UNITY_WEBGL_KS
            KSWASM.KS.AddCommonUse(new KSWASM.AddCommonUseOption());
#endif
#endif
            Log.Info("收藏");
        }

        public void Follow()
        {
#if !UNITY_EDITOR
#if UNITY_WEBGL_TT
            TTSDK.TT.OpenAwemeUserProfile(
                () =>
                {
                    Log.Info("OpenAwemeUserProfile Call Back Success!");
                },
                (errorCode, errorMsg) =>
                {
                    Log.Info("OpenAwemeUserProfile errorCode:" + errorCode + "errorMsg:" + errorMsg);
                });
#elif UNITY_WEBGL_KS
            KSWASM.KS.OpenUserProfile(new KSWASM.OpenUserProfileOption()
            {
                accountType = "MiniGameOfficialAccount"
            });
#endif
#endif
            Log.Info("关注");
        }

        /// <summary>
        /// 是否可以分享
        /// </summary>
        /// <returns></returns>
        public bool CanShare()
        {
#if UNITY_WEBGL_TT || UNITY_WEBGL_WeChat || UNITY_WEBGL_TAPTAP || UNITY_EDITOR
            return true;
#else
            return false;
#endif
        }
        
        /// <summary>
        /// 侧边栏是否可用
        /// </summary>
        /// <returns></returns>
        public bool CanSliderBar()
        {
#if UNITY_EDITOR
            return true;
#elif UNITY_WEBGL_TT || UNITY_WEBGL_BILIGAME || UNITY_WEBGL_KS
            return canSliderBar;
#else
            return false;
#endif
        }
    }
}