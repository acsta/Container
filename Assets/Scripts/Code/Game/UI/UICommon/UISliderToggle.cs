using System;
using UnityEngine;
using UnityEngine.Events;

namespace TaoTie
{
    public class UISliderToggle: UIBaseContainer,IOnCreate,IOnEnable
    {
        public UIAnimator Animator;
        public UIButton Pointer;
        public UITextmesh Text;
        private UnityAction<bool> callBack;
        private bool isOn;

        public void OnCreate()
        {
            Animator = AddComponent<UIAnimator>();
            Pointer = AddComponent<UIButton>();
            Text = AddComponent<UITextmesh>("tog/Text");
        }

        public void OnEnable()
        {
            Pointer.SetOnClick(OnClickThis);
            SetIsOn(isOn, false);
        }
        
        public bool GetIsOn()
        {
            return isOn;
        }

        public void OnClickThis()
        {
            OnClickThisAsync(!isOn).Coroutine();
        }
        private async ETTask OnClickThisAsync(bool ison)
        {
            Animator.Play(this.isOn ? "Selected" : "Normal").Coroutine();
            this.isOn = ison;
            await TimerManager.Instance.WaitAsync(1);
            Animator.CrossFade(ison ? "Selected" : "Normal", 0.1f);
            Text.SetI18NKey(ison? I18NKey.Text_Toggle_Open: I18NKey.Text_Toggle_Close);
            callBack?.Invoke(ison);
        }
        public void SetIsOn(bool ison,bool broadcast = true)
        {
            this.isOn = ison;
            Animator.Play(ison ? "Selected" : "Normal").Coroutine();
            Text.SetI18NKey(ison? I18NKey.Text_Toggle_Open: I18NKey.Text_Toggle_Close);
            if (broadcast) callBack?.Invoke(ison);
        }
        
        public void SetOnValueChanged(Action<bool> cb)
        {
            this.callBack = (a)=>
            {
                cb?.Invoke(a);
            };
        }
        
        public void RemoveOnValueChanged()
        {
            this.callBack = null;
        }
    }
}