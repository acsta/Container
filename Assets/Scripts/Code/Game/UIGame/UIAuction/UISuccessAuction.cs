using System.Collections;
using System.Collections.Generic;
using System;
using SuperScrollView;
using UnityEngine;
using UnityEngine.UI;

namespace TaoTie
{
    public class UISuccessAuction : UIBaseView, IOnCreate, IOnEnable, IOnDisable, IOnWidthPaddingChange
    {
        public static string PrefabPath => "UIGame/UIAuction/Prefabs/UISuccessAuction.prefab";
        private UIAnimator _animator;
        private UITextmesh _textmesh;

        public void OnCreate()
        {
            _animator = this.AddComponent<UIAnimator>();
            _textmesh = this.AddComponent<UITextmesh>("Image/Text (TMP)");
        }

        public void OnEnable()
        {
            PlaySuccessAnim().Coroutine();
            
        }

        public void OnDisable()
        {
            
        }

        private async ETTask PlaySuccessAnim()
        {
            _textmesh.SetActive(false);
            await _animator.Play("SuccessAuction");

            for (int i = 0; i <= _textmesh.GetText().Length; ++i)
            {
                _textmesh.SetMaxVisibleCharacters(i);
                if(i == 0) _textmesh.SetActive(true);
                
                await TimerManager.Instance.WaitAsync(20);
            }

            await TimerManager.Instance.WaitAsync(700);

            _animator.Play("CloseAuction").Coroutine();
            for (int i = _textmesh.GetText().Length; i >= 0; --i)
            {
                _textmesh.SetMaxVisibleCharacters(i);
                
                await TimerManager.Instance.WaitAsync(50);
            }
            
            await TimerManager.Instance.WaitAsync(300);
            await this.CloseSelf();
        }
    }
}