using UnityEngine;

namespace TaoTie
{
    public class UIRankBtn: UIButton,IOnCreate,IOnEnable,IOnDisable
    {
#if UNITY_WEBGL_WeChat || UNITY_WEBGL_BILIGAME
        WeChatWASM.WXUserInfoButton btn;
#elif UNITY_WEBGL_TAPTAP
        TapTapMiniGame.TapUserInfoButton btn;
#elif UNITY_WEBGL_MINIHOST
        minihost.TJUserInfoButton btn;
#elif UNITY_WEBGL_KS
        KSWASM.KSUserInfoButton btn;
#endif
        public void OnCreate()
        {
            OnCreateAsync().Coroutine();
        }

        public void OnEnable()
        {
#if UNITY_WEBGL_WeChat || UNITY_WEBGL_TAPTAP || UNITY_WEBGL_BILIGAME || UNITY_WEBGL_MINIHOST || UNITY_WEBGL_KS
            btn?.Show();
#endif
        }
        
        public void OnDisable()
        {
#if UNITY_WEBGL_WeChat || UNITY_WEBGL_TAPTAP || UNITY_WEBGL_BILIGAME || UNITY_WEBGL_MINIHOST || UNITY_WEBGL_KS
            btn?.Hide();
#endif
        }

        public override void OnDestroy()
        {
#if UNITY_WEBGL_WeChat || UNITY_WEBGL_TAPTAP || UNITY_WEBGL_BILIGAME || UNITY_WEBGL_MINIHOST || UNITY_WEBGL_KS
            btn?.Destroy();
            btn = null;
#endif
            base.OnDestroy();
        }

        protected override void OnClickBtn()
        {
            OnClickBtnAsync().Coroutine();
        }

        async ETTask OnClickBtnAsync()
        {
            await ETTask.CompletedTask;
#if UNITY_WEBGL_WeChat || UNITY_WEBGL_TAPTAP || UNITY_WEBGL_BILIGAME || UNITY_WEBGL_MINIHOST || UNITY_WEBGL_KS || UNITY_WEBGL_TT || UNITY_WEBGL_KS || UNITY_WEBGL_ALIPAY
            if (!await SDKManager.Instance.Auth())
            {
                if(SDKManager.Instance.GetImRankList()) return;
            }
#endif
            base.OnClickBtn();
        }

        private async ETTask OnCreateAsync()
        {
            if(SDKManager.Instance.IsAuth()) return;
            await ETTask.CompletedTask;
#if !UNITY_EDITOR
            var rectTransform = GetRectTransform();
            Vector3 lastPos = rectTransform.position;
            while (true)
            {
                await TimerManager.Instance.WaitAsync(1);
                if (rectTransform.position == lastPos)
                {
                    break;
                }

                lastPos = rectTransform.position;
            }
            Vector3[] worldCorners = new Vector3[4];
            rectTransform.GetWorldCorners(worldCorners);
            Vector2 min = RectTransformUtility.WorldToScreenPoint(UIManager.Instance.UICamera, worldCorners[0]);
            Vector2 max = RectTransformUtility.WorldToScreenPoint(UIManager.Instance.UICamera, worldCorners[2]);
            // 转换为屏幕左上角坐标系
            min.y = Screen.height - min.y;
            max.y = Screen.height - max.y;
            var width = Mathf.Abs(max.x - min.x);
            var height = Mathf.Abs(min.y - max.y);
            var x = min.x - width;
            var y = min.y - height;
            width *= 2;
            height *= 2;
            Log.Info(width + " " + height);
            Log.Info(x + " " + y);
#if UNITY_WEBGL_WeChat || UNITY_WEBGL_BILIGAME
            btn = WeChatWASM.WX.CreateUserInfoButton((int) x,(int) y,(int) height,(int) width, "zh_CN", true);
            btn.Show();
            Log.Error("CreateUserInfoButton");
            btn.OnTap(res =>
            {
                btn.Destroy();
                btn = null;
                if (res.errCode != 0 && !string.IsNullOrEmpty(res.errMsg))
                {
                    Log.Error(res.errMsg);
                }
                OnClickBtn();
            });
#elif UNITY_WEBGL_TAPTAP
            btn = TapTapMiniGame.Tap.CreateUserInfoButton((int) x,(int) y,(int) height,(int) width, "zh_CN", true);
            btn.Show();
            Log.Error("CreateUserInfoButton");
            btn.OnTap(res =>
            {
                btn.Destroy();
                btn = null;
                if (res.errCode != 0 && !string.IsNullOrEmpty(res.errMsg))
                {
                    Log.Error(res.errMsg);
                }
                OnClickBtn();
            });
#elif UNITY_WEBGL_MINIHOST
            btn = minihost.TJ.CreateUserInfoButton((int) x,(int) y,(int) height,(int) width, "zh_CN", true);
            btn.Show();
            Log.Error("CreateUserInfoButton");
            btn.OnTap(res =>
            {
                btn.Destroy();
                btn = null;
                if (res.errCode != 0 && !string.IsNullOrEmpty(res.errMsg))
                {
                    Log.Error(res.errMsg);
                }
                OnClickBtn();
            });
#elif UNITY_WEBGL_KS
            btn = KSWASM.KS.CreateUserInfoButton((int) x,(int) y,(int) height,(int) width, "zh_CN", true);
            btn.Show();
            Log.Error("CreateUserInfoButton");
            btn.OnTap(res =>
            {
                btn.Destroy();
                btn = null;
                if (res.errCode != 0 && !string.IsNullOrEmpty(res.errMsg))
                {
                    Log.Error(res.errMsg);
                }
                OnClickBtn();
            });
#endif
#endif
        }
    }
}