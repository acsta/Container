using System;
namespace TaoTie
{
    public class MsgBoxPara
    {
        public string Content;
        public string CancelText;
        public string ConfirmText;
        public Action<UIBaseView> CancelCallback;
        public Action<UIBaseView> ConfirmCallback;
        public bool CanClose = false;
        public bool HideCancel = false;
    }
    public class UIMsgBoxWin:UIBaseView,IOnCreate,IOnEnable<MsgBoxPara>,IOnDisable
    {
        public static string PrefabPath => "UI/UIUpdate/Prefabs/UIMsgBoxWin.prefab";
        public UITextmesh Text;
        public UIButton btn_cancel;
        public UITextmesh CancelText;
        public UIButton btn_confirm;
        public UITextmesh ConfirmText;
        public UIButton CloseBtn;
        public UIAnimator Win;

        private MsgBoxPara para;
        #region overrride

        public void OnCreate()
        {
            Win = AddComponent<UIAnimator>("Win");
            CloseBtn = AddComponent<UIButton>("Win/Close");
            this.Text = this.AddComponent<UITextmesh>("Win/Text");
            this.btn_cancel = this.AddComponent<UIButton>("Win/btn_cancel");
            this.CancelText = this.AddComponent<UITextmesh>("Win/btn_cancel/Text");
            this.btn_confirm = this.AddComponent<UIButton>("Win/btn_confirm");
            this.ConfirmText = this.AddComponent<UITextmesh>("Win/btn_confirm/Text");
        }

        public void OnEnable(MsgBoxPara a)
        {
            SoundManager.Instance.PlaySound("Audio/Sound/Win_Open.mp3");
            CloseBtn.SetActive(a.CanClose);
            CloseBtn.SetOnClick(Close);
            para = a;
            this.Text.SetText(a.Content);
            if (a.HideCancel)
            {
                btn_cancel.SetActive(false);
            }
            else
            {
                btn_cancel.SetActive(true);
                this.btn_cancel.SetOnClick(OnClickCancel);
                this.CancelText.SetText(a.CancelText);
            }
            this.btn_confirm.SetOnClick(OnClickConfirm);
            this.ConfirmText.SetText(a.ConfirmText);
        }

        public override async ETTask CloseSelf()
        {
            SoundManager.Instance.PlaySound("Audio/Sound/Win_Close.mp3");
            await Win.Play("UIWin_Close");
            await base.CloseSelf();
        }

        public void OnDisable()
        {
            this.btn_cancel.RemoveOnClick();
            this.btn_confirm.RemoveOnClick();
        }
        #endregion
        
        private void OnClickConfirm()
        {
            if (para?.ConfirmCallback != null)
            {
                para.ConfirmCallback.Invoke(this);
            }
        }
        private void OnClickCancel()
        {
            if (para?.CancelCallback != null)
            {
                para.CancelCallback.Invoke(this);
            }
            else
            {
                Close();
            }
        }
        
        void Close()
        {
            CloseSelf().Coroutine();
        }

    }
}