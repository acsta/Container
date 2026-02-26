using System.Collections;
using System.Collections.Generic;
using System;
using SuperScrollView;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace TaoTie
{
	public class UITurntableView : UIBaseView, IOnCreate, IOnEnable,IUpdate,IOnWidthPaddingChange
	{
		public static string PrefabPath => "UIGame/UIMiniGame/Prefabs/UITurntableView.prefab";
		public UICopyGameObject Panel;
		public UIAnimator Light;
		public UIButton btn_start;
		public UIButton Close;
		public UIAnimator UICommonView;
		public UITextmesh Count;
		public UIImage Ad;
		public UICashGroup CashGroup;

		private List<TurntableRewardsConfig> data;
		private bool isRotate = false;
		private int adCount;
		private int overAD;
		#region override
		public void OnCreate()
		{
			this.CashGroup = this.AddComponent<UICashGroup>("CashGroup");
			Ad = AddComponent<UIImage>("UICommonView/Bg/Content/StartBtn/Ad");
			UICommonView = AddComponent<UIAnimator>("UICommonView");
			this.Panel = this.AddComponent<UICopyGameObject>("UICommonView/Bg/Content/Panel");
			this.Panel.InitListView(0,GetPanelItemByIndex);
			this.Light = this.AddComponent<UIAnimator>("UICommonView/Bg/Content/Light");
			this.btn_start = this.AddComponent<UIButton>("UICommonView/Bg/Content/StartBtn");
			Close = AddComponent<UIButton>("UICommonView/Bg/Close");
			Count = AddComponent<UITextmesh>("UICommonView/Bg/Content/StartBtn/Text");
		}
		public void OnEnable()
		{
			OnEnableAsync().Coroutine();
			isRotate = false;
			Light.Play("Turntable_LightNormal").Coroutine();
			Close.SetOnClick(OnClickClose);
			btn_start.SetActive(true);
			this.btn_start.SetOnClick(OnClickbtn_start);
			var lv = IAuctionManager.Instance == null ? 1 : IAuctionManager.Instance.Level;
			if (!TurntableRewardsConfigCategory.Instance.TryGet(lv, PlayerDataManager.Instance.RestaurantLv, out var list))
			{
				Log.Error("未配置大转盘" + lv);
				CloseSelf().Coroutine();
				return;
			}
			data = list;
			var transform = Panel.GetTransform();
			transform.localEulerAngles = Vector3.one;
			Panel.SetListItemCount(list.Count);
			Panel.RefreshAllShownItem();
			adCount = PlayerDataManager.Instance.GetTurnTableNeedAdCount();
			Ad.SetActive(adCount > 0);
			overAD = 0;
			if (adCount > 1)
			{
				Count.SetText(I18NManager.Instance.I18NGetText(I18NKey.Text_Turntable_Start)+$"(0/{adCount})");
			}
			else
			{
				Count.SetI18NKey(I18NKey.Text_Turntable_Start);
			}
		}
		private async ETTask OnEnableAsync()
		{
			var mainCamera = CameraManager.Instance.MainCamera();
			if (mainCamera == null) return;
			await UICommonView.Play("UIView_Open");
			CameraManager.Instance.MainCamera().cullingMask = Define.UILayer;
		}

		public void Update()
		{
			if(isRotate) return;
			var transform = Panel.GetTransform();
			transform.localEulerAngles += Time.deltaTime * Vector3.forward * 10;
		}
		#endregion

		#region 事件绑定

		private void OnClickClose()
		{
			OnClickCloseAsync().Coroutine();
		}
		public async ETTask OnClickCloseAsync()
		{
			var mainCamera = CameraManager.Instance.MainCamera();
			if (mainCamera != null)
			{
				mainCamera.cullingMask = Define.AllLayer;
			}
			await UICommonView.Play("UIView_Close");
			CloseSelf().Coroutine();
			GameTimerManager.Instance.SetTimeScale(1);
		}
		public void GetPanelItemByIndex(int index, GameObject obj)
		{
			TurntableItem item = Panel.GetUIItemView<TurntableItem>(obj);
			if (item == null)
			{
				item = Panel.AddItemViewComponent<TurntableItem>(obj);
			}
			item.SetData(data[index]);
			item.GetTransform().localEulerAngles = new Vector3(0, 0, -45 * index);
		}
		public void OnClickbtn_start()
		{
			OnClickBtnStartAsync().Coroutine();
		}
		#endregion
		
		public async ETTask OnClickBtnStartAsync()
		{
			try
			{
				Close.SetInteractable(false);
				btn_start.SetActive(false);
				bool over = true;
				while (overAD < adCount)
				{
					var res = await AdManager.Instance.PlayAd();
					if (res)
					{
						overAD++;
						Count.SetText(I18NManager.Instance.I18NGetText(I18NKey.Text_Turntable_Start) +
						              $"({overAD}/{adCount})");
					}
					else
					{
						over = false;
						break;
					}
				}

				if (over)
				{
					PlayerDataManager.Instance.OnTurnTableAd();
					await DoRotate();
				}
				else
				{
					btn_start.SetActive(true);
				}
			}
			catch (Exception ex)
			{
				Log.Error(ex);
			}
			finally
			{
				Close.SetInteractable(true);
			}
		}

		private async ETTask DoRotate()
		{
			isRotate = true;
			Light.Play("Turntable_LightRotate").Coroutine();
			var total = 0;
			for (int i = 0; i < data.Count; i++)
			{
				total += data[i].Weight;
			}
			var weight = Random.Range(0, total);
			int index = 0;
			for (int i = 0; i < data.Count; i++)
			{
				weight -= data[i].Weight;
				if (weight <= 0)
				{
					index = i;
					break;
				}
			}
			var config = data[index];
			var timeStart = TimerManager.Instance.GetTimeNow();
			var transform = Panel.GetTransform();
			var v = 1000f;
			var a = 2000f;
			var startPos = transform.localEulerAngles.z;
			while (true)
			{
				await TimerManager.Instance.WaitAsync(1);
				var timeNow = TimerManager.Instance.GetTimeNow();
				var during = Mathf.Clamp((timeNow - timeStart) / 1000f, 0, 0.5f);
				transform.localEulerAngles = new Vector3(0, 0, startPos + a * during * during);
				if (timeNow - timeStart > 500)
				{
					timeStart = timeNow;
					break;
				}
			}

			startPos = transform.localEulerAngles.z;
			while (true)
			{
				await TimerManager.Instance.WaitAsync(1);
				var timeNow = TimerManager.Instance.GetTimeNow();
				var during = (timeNow - timeStart) / 1000f;
				transform.localEulerAngles = new Vector3(0, 0, startPos + v * during);
				if (timeNow - timeStart > 1000)
				{
					timeStart = timeNow;
					break;
				}
			}
			startPos = transform.localEulerAngles.z;
			startPos %= 360;
			transform.localEulerAngles = new Vector3(0, 0, startPos);
			var endPos = 360 * 5 + 45 * index + Random.Range(-20f, 20f);
			var t = 2 * (endPos - startPos) / v;
			long endTime = (int) (t * 1000) + timeStart;
			a = v / t;
			while (true)
			{
				await TimerManager.Instance.WaitAsync(1);
				var timeNow = TimerManager.Instance.GetTimeNow();
				var during = (timeNow - timeStart) / 1000f;
				transform.localEulerAngles = new Vector3(0, 0, startPos + v * during - a * during * during / 2);
				if (timeNow > endTime)
				{
					break;
				}
			}

			transform.localEulerAngles = new Vector3(0, 0, endPos);
			Light.Play("Turntable_LightFlash").Coroutine();
			UIManager.Instance.OpenBox<UIToast,string>(UIToast.PrefabPath,
				I18NManager.Instance.I18NGetParamText(I18NKey.Text_Turntable_Res,
					I18NManager.Instance.TranslateMoneyToStr(config.RewardCount))).Coroutine();
			var uiItem = Panel.GetUIItemView<TurntableItem>(Panel.GetItemByIndex(index));
			await CashGroup.DoMoneyMoveAnim(config.RewardCount, uiItem.Image.GetRectTransform().position, 5);
			PlayerDataManager.Instance.ChangeMoney(config.RewardCount);
		}
	}
}
