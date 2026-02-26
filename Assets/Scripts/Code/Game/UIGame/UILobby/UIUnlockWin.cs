using System.Collections;
using System.Collections.Generic;
using System;
using SuperScrollView;
using UnityEngine;
using UnityEngine.UI;

namespace TaoTie
{
	public class UIUnlockWin : UIBaseView, IOnCreate, IOnEnable<TechnologyTreeConfig>
	{
		public static string PrefabPath => "UIGame/UILobby/Prefabs/UIUnlockWin.prefab";
		public UIButton Close;
		public UITextmesh Title;
		public UIButton Button;
		public UIImage Icon;
		public UITextmesh Content;
		public UIEmptyView Coin;
		public UITextmesh Text;
		public UIPointerClick Mask;
		private TechnologyTreeConfig config;
		public UIAnimator UICommonWin;


		#region override
		public void OnCreate()
		{
			UICommonWin = AddComponent<UIAnimator>("UICommonWin");
			Mask = AddComponent<UIPointerClick>("UICommonWin/Mask");
			this.Close = this.AddComponent<UIButton>("UICommonWin/Win/Close");
			this.Title = this.AddComponent<UITextmesh>("UICommonWin/Win/Content/IconBg/Table/Text");
			this.Button = this.AddComponent<UIButton>("UICommonWin/Win/Content/Button");
			this.Icon = this.AddComponent<UIImage>("UICommonWin/Win/Content/IconBg/Rare/Icon");
			this.Content = this.AddComponent<UITextmesh>("UICommonWin/Win/Content/Content");
			this.Coin = this.AddComponent<UIEmptyView>("UICommonWin/Win/Content/Coin");
			this.Text = this.AddComponent<UITextmesh>("UICommonWin/Win/Content/Coin/Text");
		}
		public void OnEnable(TechnologyTreeConfig config)
		{
			SoundManager.Instance.PlaySound("Audio/Sound/Win_Open.mp3");
			this.config = config;
			this.Close.SetOnClick(OnClickClose);
			Mask.SetOnClick(OnClickClose);
			this.Button.SetOnClick(OnClickButton);
			Title.SetText(I18NManager.Instance.I18NGetText(config));
			Icon.SetSpritePath(config.Icon).Coroutine();
			Content.SetText(I18NManager.Instance.I18NGetText(config, 1));
			bool isUnlock = PlayerDataManager.Instance.IsUnlock(config.Id);
			Coin.SetActive(!isUnlock);
			Button.SetActive(!isUnlock);
			if (!isUnlock)
			{
				if (config.UnlockType == 1)
				{
					Text.SetText(I18NManager.Instance.TranslateMoneyToStr(config.UnlockValue));
				}
				else
				{
					Log.Error("解锁类型不对 TechnologyTreeConfig id=" + config.Id);
				}
			}
		}
		public override async ETTask CloseSelf()
		{
			SoundManager.Instance.PlaySound("Audio/Sound/Win_Close.mp3");
			await UICommonWin.Play("UIWin_Close");
			await base.CloseSelf();
		}
		#endregion

		#region 事件绑定
		public void OnClickClose()
		{
			CloseSelf().Coroutine();
		}
		public void OnClickButton()
		{
			bool isUnlock = PlayerDataManager.Instance.IsUnlock(config.Id);
			if(isUnlock) return;
			var res = PlayerDataManager.Instance.UnlockTreeNode(config.Id);
			if (res)
			{
				UIManager.Instance.OpenBox<UIToast,I18NKey>(UIToast.PrefabPath,I18NKey.Text_Unlock_Success).Coroutine();
				Messager.Instance.Broadcast(0,MessageId.UnlockTreeNode,config.Id);
				SoundManager.Instance.PlaySound("Audio/Sound/Unlock.mp3");
				CloseSelf().Coroutine();
			}
		}
		#endregion
	}
}
