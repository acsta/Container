using System.Collections;
using System.Collections.Generic;
using System;
using SuperScrollView;
using UnityEngine;
using UnityEngine.UI;

namespace TaoTie
{
    public class UITargetView : UIBaseView, IOnCreate, IOnEnable
    {
        public static string PrefabPath => "UIGame/UIAuction/Prefabs/UITargetView.prefab";
        public UIAnimator Aim;
		

        #region override
        public void OnCreate()
        {
            this.Aim = this.AddComponent<UIAnimator>("Aim");
        }
        public void OnEnable()
        {
            Aim.SetActive(false);
        }
        #endregion

        public async ETTask EnterTarget(GameObject target)
        {
            if (target != null)
            {
                if (target.GetComponent<RectTransform>() == null)
                {
                    var mainCamera = CameraManager.Instance.MainCamera();
                    Vector2 pt = UIManager.Instance.ScreenPointToUILocalPoint(GetRectTransform(),
                        mainCamera.WorldToScreenPoint(target.transform.position));
                    Aim.GetRectTransform().anchoredPosition = pt;
                    Aim.GetRectTransform().sizeDelta = Vector2.one * 200;
                }
                else
                {
                    var size = target.GetComponent<RectTransform>()?.sizeDelta ??
                               Vector2.one * 100;
                    Aim.GetRectTransform().sizeDelta = size * 2;
                    Aim.GetRectTransform().position = target.transform.position;
                }
            }
            else
            {
                Aim.GetRectTransform().anchoredPosition = Vector2.zero;
            }
            Aim.SetActive(true);
            await Aim.Play("MaskOpen");
            CloseSelf().Coroutine();
        }
    }
}
