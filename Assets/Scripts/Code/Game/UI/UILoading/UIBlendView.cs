using System.Collections;
using System.Collections.Generic;
using System;
using SuperScrollView;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

namespace TaoTie
{
	public class UIBlendView : UIBaseView, IOnCreate, IOnEnable, IOnDisable
	{
		public static string PrefabPath => "UI/UILoading/Prefabs/UIBlendView.prefab";
		public UIRawImage RawImage;
		public Material Material;
		public UIImage Loading;
		private int Progress = Shader.PropertyToID("_Progress");

		private static Texture2D screenShotTemp;

		#region override
		public void OnCreate()
		{
			this.RawImage = this.AddComponent<UIRawImage>("RawImage");
			Loading = AddComponent<UIImage>("Loading");
			Material = RawImage.GetMaterial();
		}
		public void OnEnable()
		{
			Loading.SetActive(false);
			RawImage.SetEnabled(false);
			Material.SetFloat(Progress, 0);
		}

		public void OnDisable()
		{
			if (screenShotTemp != null)
			{
				RawImage.SetTexture(null);
				GameObject.Destroy(screenShotTemp);
				screenShotTemp = null;
			}
		}
		#endregion
		
		public async ETTask CaptureBg(bool ignoreUI = false)
        {
	        await UIManager.Instance.CloseWindow<UIGuidanceView>();
	        if (screenShotTemp != null)
	        {
		        RawImage.SetTexture(null);
		        GameObject.Destroy(screenShotTemp);
		        screenShotTemp = null;
	        }
	        
	        if (screenShotTemp == null)
	        {
		        GameObject uiCamera = null;
		        if (ignoreUI)
		        {
			        var mainCamera = CameraManager.Instance.MainCamera();
			        if (mainCamera != null)
			        {
				        if (PlatformUtil.IsWebGl1())
				        {
					        mainCamera.cullingMask -= LayerMask.GetMask("UI");
				        }
				        else
				        {
					        var cd = mainCamera.GetUniversalAdditionalCameraData();
					        for (int i = 0; i < cd.cameraStack.Count; i++)
					        {
						        if (cd.cameraStack[i].GetUniversalAdditionalCameraData().isUICamera)
						        {
							        uiCamera = cd.cameraStack[i].gameObject;
							        break;
						        }
					        }
					        uiCamera?.SetActive(false);
				        }
			        }
		        }
		        await UnityLifeTimeHelper.WaitFrameFinish();
		        // 先创建一个的空纹理，大小可根据实现需要来设置  
		        var rect = new Rect(0, 0, Screen.width, Screen.height);
		        screenShotTemp = new Texture2D((int) rect.width, (int) rect.height, TextureFormat.RGB24, false);
		        // 读取屏幕像素信息并存储为纹理数据，  
		        screenShotTemp.ReadPixels(rect, 0, 0);
		        screenShotTemp.Apply();
		        RawImage.SetTexture(screenShotTemp);
		        RawImage.SetEnabled(true);
		        if (PlatformUtil.IsWebGl1())
		        {
			        var mainCamera = CameraManager.Instance.MainCamera();
			        if (mainCamera != null)
			        {
				        mainCamera.cullingMask += LayerMask.GetMask("UI");
			        }
		        }
		        else
		        {
			        if (uiCamera != null)
			        {
				        uiCamera.SetActive(true);
			        }
		        }
		        
	        }
	        Loading.SetActive(true);
	        await UIManager.Instance.OpenWindow<UIGuidanceView>(UIGuidanceView.PrefabPath, UILayerNames.TipLayer);
        }

		public async ETTask DoFade(float during = 1000)
		{
			Loading.SetActive(false);
			var startTime = TimerManager.Instance.GetTimeNow();
			while (true)
			{
				await TimerManager.Instance.WaitAsync(1);
				var timeNow = TimerManager.Instance.GetTimeNow();
				var progress = Mathf.Clamp01((timeNow - startTime) / during);
				if(progress >= 1) break;
				progress = -0.1f + 2.1f * progress;
				Material.SetFloat(Progress, progress);
			}
			Material.SetFloat(Progress, 2.1f);
		}
	}
}
