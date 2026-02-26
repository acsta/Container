using UnityEngine;

namespace TaoTie
{
    public class UILoadingView:UIBaseView,IOnCreate, IOnEnable,IOnDisable,IUpdate
    {
        public static string PrefabPath => "UI/UILoading/Prefabs/UILoadingView.prefab";
        public UISlider slider;
        public UITextmesh text;
        protected float progress;
        public bool isAnime = false;
        #region override

        public virtual void OnCreate()
        {
            text = AddComponent<UITextmesh>("Loadingscreen/Text");
            this.slider = this.AddComponent<UISlider>("Loadingscreen/Slider");
        }

        public virtual void OnEnable()
        {
            progress = 0;
            text.SetI18NKey(I18NKey.Loading_Tip_0);
        }
        
        public void OnDisable()
        {
            progress = 0;
        }
        
        public void Update()
        {
            if (isAnime) return;
            SetProgress(progress + Time.deltaTime / Mathf.Max(5, progress*100));
        }

        #endregion
        public virtual void SetProgress(float value)
        {
            if (value > progress)
            {
                progress = value;
            }
            this.slider.SetValue(progress);
        }

        public void SetTipText(I18NKey key)
        {
            text.SetI18NKey(key);
        }
    }
}