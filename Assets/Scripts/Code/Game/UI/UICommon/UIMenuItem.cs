using System;
using UnityEngine;

namespace TaoTie
{
    public class UIMenuItem: UIBaseContainer,IOnCreate
    {
        public MenuPara Para;
        public int Index;
        public Action<int, int> onClick;

        public UITextmesh Text;
        public UIImage TabFocus;
        public UIPointerClick Btn;
        public UIRedDot RedDot;
        public UIImage Icon;

        #region override

        public void OnCreate()
        {
            Icon = AddComponent<UIImage>("Content/Icon");
            Text = AddComponent<UITextmesh>("Content/Text");
            TabFocus = AddComponent<UIImage>("TabFocus");
            Btn = AddComponent<UIPointerClick>();
            Btn.SetOnClick(() =>
            {
                onClick?.Invoke(Para.Id, Index);
            });
            RedDot = AddComponent<UIRedDot>("RedDot");
        }

        #endregion

        private MenuPara para;
        public void SetData(MenuPara para, int index, Action<int, int> onClick, bool isActive = false, bool changeScale = true)
        {
            this.para = para;
            if (!string.IsNullOrEmpty(para.ImgPath))
            {
                Icon.SetSpritePath(para.ImgPath).Coroutine();
            }
            this.onClick = onClick;
            Index = index;
            Para = para;
            Text.SetActive(!string.IsNullOrEmpty(para.Name));
            Text.SetText(para.Name);
            SetIsActive(isActive, changeScale);
            RedDot.ReSetTarget(para.RedDot);
        }
        /// <summary>
        /// 设置是否选择状态
        /// </summary>
        /// <param name="isActive"></param>
        public void SetIsActive(bool isActive, bool changeScale)
        {
            TabFocus.SetActive(isActive);
            GetRectTransform().localScale = isActive && changeScale ? Vector3.one * 1.2f : Vector3.one;
            if (isActive)
            {
                if (!string.IsNullOrEmpty(para.ImgPath)) Icon.SetColor(para.ActiveColor);
                Text.SetTextColor(para.ActiveColor);
            }
            else
            {
                if (!string.IsNullOrEmpty(para.ImgPath)) Icon.SetColor(para.UnActiveColor);
                Text.SetTextColor(para.UnActiveColor);
            }
        }
    }
}