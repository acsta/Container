using System.Collections;
using System.Collections.Generic;
using System;
using SuperScrollView;
using UnityEngine;
using UnityEngine.UI;

namespace TaoTie
{
    public class UICopyWin : UIBaseView, IOnCreate, IOnEnable<string, ETTask>
    {
        public static string PrefabPath => "UI/UICommon/Prefabs/UICopyWin.prefab";
        public UIButton BtnLogin;
        public UIButton BtnCopy;
        public UIInputTextmesh InputField;

        private ETTask task;
        #region override
        public void OnCreate()
        {
            this.BtnLogin = this.AddComponent<UIButton>("Win/Jump");
            this.BtnCopy = this.AddComponent<UIButton>("Win/Confirm");
            this.InputField = this.AddComponent<UIInputTextmesh>("Win/Psw/UserName");
        }
        public void OnEnable(string code, ETTask task)
        {
            this.task = task;
            SoundManager.Instance.PlaySound("Audio/Sound/Win_Open.mp3");
            InputField.SetText(code);
            this.BtnLogin.SetOnClick(OnClickBtnLogin);
            this.BtnCopy.SetOnClick(OnClickBtnCopy);
        }
        #endregion

        #region 事件绑定

        public void OnClickBtnCopy()
        {
            var text = InputField.GetText();
            if (!string.IsNullOrEmpty(text))
            {
                BridgeHelper.CopyBuffer(text);
                UIToast.ShowToast(I18NKey.Text_Copy_Over);
            }
        }
        
        public void OnClickBtnLogin()
        {
            var text = InputField.GetText();
            if (!string.IsNullOrEmpty(text))
            {
                AdManager.NONE_AD_LINK = false;
                this.task.SetResult();
                this.task = null;
                CloseSelf().Coroutine();
            }
        }
        #endregion
    }
}