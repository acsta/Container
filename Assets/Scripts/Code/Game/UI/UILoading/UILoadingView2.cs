using UnityEngine;

namespace TaoTie
{
    public class UILoadingView2 : UILoadingView, IOnDestroy, IOnBeforeCloseWin
    {
        public new static string PrefabPath => "UI/UILoading/Prefabs/UILoadingView2.prefab";

        public UITextmesh Text;
        public UIImage MaskImage;
        public UIEmptyView LoadingScreen;
        private const float animTime = 200f;

        public override void OnCreate()
        {
            base.OnCreate();
            Text = AddComponent<UITextmesh>("Loadingscreen/Slider/Text");
            LoadingScreen = AddComponent<UIEmptyView>("Loadingscreen");
            MaskImage = AddComponent<UIImage>("Mask");
        }

        public override void OnEnable()
        {
            SoundManager.Instance.PlaySound("Audio/Sound/dockEnter.mp3");
            progress = 0;
        }

        public void OnDestroy()
        {
        }

        public async ETTask OnBeforeDisable()
        {
            var starTime = TimerManager.Instance.GetTimeNow();
            while (true)
            {
                await TimerManager.Instance.WaitAsync(1);
                
                var timeNow = TimerManager.Instance.GetTimeNow();
                MaskImage.SetImageAlpha(Mathf.Lerp(0f, 1f, (timeNow - starTime) / animTime));
                if (timeNow - starTime >= animTime)
                {
                    break;
                }
            }
            
            LoadingScreen.SetActive(false);
            
            starTime = TimerManager.Instance.GetTimeNow();
            while (true)
            {
                await TimerManager.Instance.WaitAsync(1);
                
                var timeNow = TimerManager.Instance.GetTimeNow();
                MaskImage.SetImageAlpha(Mathf.Lerp(1f, 0f, (timeNow - starTime) / animTime));
                if (timeNow - starTime >= animTime)
                {
                    break;
                }
            }
        }
        
        public override void SetProgress(float val)
        {
            base.SetProgress(val);
            Text.SetText(Mathf.Clamp(val*100, 0 ,100).ToString("0")+"%");
        }

        public async ETTask LoadingAnim(bool isToBlack)
        {
            LoadingScreen.SetActive(!isToBlack);
            MaskImage.SetImageAlpha(isToBlack ? 0f : 1f);

            var starTime = TimerManager.Instance.GetTimeNow();
            while (true)
            {
                await TimerManager.Instance.WaitAsync(1);
                
                var timeNow = TimerManager.Instance.GetTimeNow();
                MaskImage.SetImageAlpha(Mathf.Lerp(isToBlack ? 0f : 1f, isToBlack ? 1f : 0f, (timeNow - starTime) / animTime));
                if (timeNow - starTime >= animTime)
                {
                    break;
                }
            }
            
            if(isToBlack) LoadingScreen.SetActive(true);
        }
    }
}