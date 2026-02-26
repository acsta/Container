using System.Collections;
using System.Collections.Generic;
using System;
using SuperScrollView;
using UnityEngine;
using UnityEngine.UI;

namespace TaoTie
{
	public class UISettingWin : UIBaseView, IOnCreate, IOnEnable, IOnDisable ,IOnEnable<bool>
	{
		public static string PrefabPath => "UIGame/UILobby/Prefabs/UISettingWin.prefab";
		public UIButton Close;
		public UISliderToggle Sound;
		public UISliderToggle Music;
		public UISliderToggle Vibrate;
		public UITextmesh Version;
		public UITextmesh UID;
		public UIButton Back;
		public UIButton Copy;
		public UIEmptyView Stage;
		public UITextmesh StageText;

		public UIAnimator UICommonWin;

		private bool canBack;
		#region override
		public void OnCreate()
		{
			UICommonWin = AddComponent<UIAnimator>("UICommonSmallWin");
			Stage = AddComponent<UIEmptyView>("UICommonSmallWin/Win/Stage");
			StageText = AddComponent<UITextmesh>("UICommonSmallWin/Win/Stage/Text");
			Copy = AddComponent<UIButton>("UICommonSmallWin/Win/Content/Uid/Copy");
			this.Close = this.AddComponent<UIButton>("UICommonSmallWin/Win/Close");
			this.Sound = this.AddComponent<UISliderToggle>("UICommonSmallWin/Win/Content/Sound/SliderToggle");
			this.Music = this.AddComponent<UISliderToggle>("UICommonSmallWin/Win/Content/Music/SliderToggle");
			this.Vibrate = this.AddComponent<UISliderToggle>("UICommonSmallWin/Win/Content/Vir/SliderToggle");
			this.Version = this.AddComponent<UITextmesh>("UICommonSmallWin/Win/Content/Version");
			this.UID = this.AddComponent<UITextmesh>("UICommonSmallWin/Win/Content/Uid");
			this.Back = this.AddComponent<UIButton>("UICommonSmallWin/Win/Content/Bottom/Back");
			StageText.SetI18NKey(I18NKey.Text_Game_Stage);
			UID.SetI18NKey(I18NKey.Text_UID);
			Version.SetI18NKey(I18NKey.Text_Version);
		}

		public void OnEnable()
		{
			OnEnable(false);
		}

		public void OnEnable(bool showBack)
		{
			canBack = showBack;
			SoundManager.Instance.PlaySound("Audio/Sound/Win_Open.mp3");
			this.Sound.SetIsOn(SoundManager.Instance.SoundVolume != 0, false);
			this.Music.SetIsOn(SoundManager.Instance.MusicVolume != 0, false);
			this.Vibrate.SetIsOn(ShockManager.Instance.IsOpen, false);
			this.Close.SetOnClick(OnClickContinue);
			this.Sound.SetOnValueChanged(OnSoundToggleChange);
			this.Music.SetOnValueChanged(OnMusicToggleChange);
			this.Vibrate.SetOnValueChanged(OnVibrateToggleChange);
			this.Back.SetOnClick(OnClickBack);
			this.Back.SetActive(IAuctionManager.Instance != null);
			this.Back.SetInteractable(true);
			Stage.SetActive(IAuctionManager.Instance != null);
			if (IAuctionManager.Instance != null)
			{
				StageText.SetI18NText(IAuctionManager.Instance.Stage);
			}
			this.Version.SetI18NText(Application.version);
			this.UID.SetI18NText(PlayerManager.Instance.Uid);
			Copy.SetOnClick(OnClickCopy);
		}

		public override async ETTask CloseSelf()
		{
			SoundManager.Instance.PlaySound("Audio/Sound/Win_Close.mp3");
			await UICommonWin.Play("UIWin_Close");
			await base.CloseSelf();
		}
		public void OnDisable()
		{
			CacheManager.Instance.Save();
		}
		#endregion

		#region 事件绑定

		public void OnSoundToggleChange(bool val)
		{
			var data = val ? SoundManager.DEFAULTVALUE : 0;
			SoundManager.Instance.SetSoundVolume(data);
			CacheManager.Instance.SetInt(CacheKeys.SoundVolume, data);
		}
		public void OnMusicToggleChange(bool val)
		{
			var data = val ? SoundManager.DEFAULTVALUE : 0;
			SoundManager.Instance.SetMusicVolume(val ? SoundManager.DEFAULTVALUE : 0);
			CacheManager.Instance.SetInt(CacheKeys.MusicVolume, data);
		}
		public void OnVibrateToggleChange(bool val)
		{
			ShockManager.Instance.SetOpen(val);
		}
		private void OnClickBack()
		{
			OnClickBackAsync().Coroutine();
		}

		private async ETTask OnClickBackAsync()
		{
			if (IAuctionManager.Instance == null)
			{
				CloseSelf().Coroutine();
				return;
			}

			if (!canBack)
			{
				UIManager.Instance.OpenBox<UIToast,I18NKey>(UIToast.PrefabPath,I18NKey.Text_Notice_Back_Wait).Coroutine();
				return;
			}
			this.Back.SetInteractable(false);
			
			await UIManager.Instance.OpenWindow<UIReportWin, AuctionReport[], int, bool>(UIReportWin.PrefabPath,
				IAuctionManager.Instance.AuctionReports, IAuctionManager.Instance.Level, false);
			CloseSelf().Coroutine();
			// await UIManager.Instance.OpenWindow<UIBlendView>(UIBlendView.PrefabPath,UILayerNames.TopLayer);
			// SceneManager.Instance.PreloadScene<HomeScene>().Coroutine();
			// await CloseSelf();
			// var gameView = UIManager.Instance.GetView<UIGameView>(1);
			// if (gameView != null)
			// {
			// 	await gameView.CloseSelf();
			// }
			// GameTimerManager.Instance.SetTimeScale(1);
			// IAuctionManager.Instance.ForceAllOver();
		}
		public void OnClickContinue()
		{
			CloseSelf().Coroutine();
			GameTimerManager.Instance.SetTimeScale(1);
		}

		public void OnClickCopy()
		{
			BridgeHelper.CopyBuffer(PlayerManager.Instance.Uid.ToString());
			UIManager.Instance.OpenBox<UIToast, I18NKey>(UIToast.PrefabPath, I18NKey.Text_Copy_Over).Coroutine();
		}
		#endregion
	}
}
