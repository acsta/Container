using System.Collections;
using System.Collections.Generic;
using System;
using SuperScrollView;
using UnityEngine;
using UnityEngine.UI;

namespace TaoTie
{
	public class UIGameInfoView : UIBaseView, IOnCreate, IOnEnable<GameInfoConfig,GameInfoConfig,GameInfoConfig>
	{
		public static string PrefabPath => "UIGame/UIAuction/Prefabs/UIGameInfoView.prefab";
		public GameInfoItem[] GameInfoItem = new GameInfoItem[3];
		public UIAnimator UIAnimator;

		#region override
		public void OnCreate()
		{
			UIAnimator = AddComponent<UIAnimator>();
			for (int i = 0; i < 3; i++)
			{
				this.GameInfoItem[i] = this.AddComponent<GameInfoItem>("Win/Content/GameInfoItem"+i);
			}
		}

		public void OnEnable(GameInfoConfig a, GameInfoConfig b, GameInfoConfig c)
		{
			this.GameInfoItem[0].SetData(a,OnClickItem);
			this.GameInfoItem[1].SetData(b,OnClickItem);
			this.GameInfoItem[2].SetData(c,OnClickItem);
		}

		#endregion

		public void OnClickItem(GameInfoConfig config)
		{
			OnClickItemAsync(config).Coroutine();
		}
		public async ETTask OnClickItemAsync(GameInfoConfig config)
		{
			IAuctionManager.Instance.SelectGameInfo(config.Id);
			await UIAnimator.Play("GameInfo_Close");
			CloseSelf().Coroutine();
		}
	}
}
