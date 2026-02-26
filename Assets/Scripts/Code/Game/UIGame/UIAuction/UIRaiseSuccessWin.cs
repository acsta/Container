using System.Collections;
using System.Collections.Generic;
using System;
using SuperScrollView;
using UnityEngine;
using UnityEngine.UI;

namespace TaoTie
{
	public class UIRaiseSuccessWin : UIBaseView, IOnCreate, IOnEnable<BigNumber,float, float, bool>, IOnEnable<BigNumber,float>, IOnDisable, IOnDestroy
	{
		public static string PrefabPath => "UIGame/UIAuction/Prefabs/UIRaiseSuccessWin.prefab";
		public UIPointerClick Mask;
		public UITextmesh Text;
		public UITextmesh RatioText;
		public UITextmesh IntelligenceRatioText;

		private BigNumber val;
		public UIAnimator Win;
		private float mul;
		private bool isMul;
		private float IntelligenceMul;
		private ETCancellationToken cancel;

		private Vector3 RatioTextPos;
		private Vector2 IntelligenceTextPos;
		#region override
		public void OnCreate()
		{
			Win = AddComponent<UIAnimator>();
			this.Mask = this.AddComponent<UIPointerClick>("Mask");
			this.Text = this.AddComponent<UITextmesh>("Win/T/Text");
			this.IntelligenceRatioText = this.AddComponent<UITextmesh>("Win/T/Text/Ratio (1)");
			this.RatioText = this.AddComponent<UITextmesh>("Win/T/Text/Ratio");
			//Text.SetI18NKey(I18NKey.Text_Raise_Success);
			
			IntelligenceTextPos = IntelligenceRatioText.GetRectTransform().anchoredPosition;
			RatioTextPos = RatioText.GetRectTransform().anchoredPosition;
		}
		public void OnEnable(BigNumber val,float mul)
		{
			Text.GetRectTransform().localScale = Vector3.one;
			this.val = val;
			this.mul = mul;
			
			cancel = null;
			cancel = new ETCancellationToken();
			this.Mask.SetOnClick(OnClickMask);
			
			PlayOpenAnim(val, mul).Coroutine();
		}
		public void OnEnable(BigNumber val,float mul, float IntelligenceMul, bool isMul)
		{
			Text.GetRectTransform().localScale = Vector3.one;
			this.val = val;
			this.mul = mul;
			this.IntelligenceMul = IntelligenceMul;
			this.isMul = isMul;
			
			cancel = null;
			cancel = new ETCancellationToken();
			this.Mask.SetOnClick(OnClickMask);
			
			PlayOpenAnim(val, mul).Coroutine();
		}
		public void OnDisable()
		{
			cancel = null;
		}
		public void OnDestroy()
		{
			cancel = null;
		}

		public override async ETTask CloseSelf()
		{
			await Win.Play("UIRaise_Close");
			await base.CloseSelf();
		}

		#endregion

		#region 事件绑定

		public void OnClickMask()
		{
			OnClickMaskAsync().Coroutine();
		}

		public async ETTask OnClickMaskAsync()
		{
			if (cancel != null && cancel.IsCancel())
			{
				var result = val * mul;
				//抬价获得的奖励直接加
				PlayerDataManager.Instance.RecordWinToday(result);

				CloseSelf().Coroutine();
				UIGameView view = UIManager.Instance.GetView<UIGameView>(1);
				if (view != null)
				{
					await view.CashGroup.DoMoneyMoveAnim(result, Text.GetRectTransform().position, 10);
				}
				UIGuideGameView view2 = UIManager.Instance.GetView<UIGuideGameView>(1);
				if (view2 != null)
				{
					await view2.CashGroup.DoMoneyMoveAnim(result, Text.GetRectTransform().position, 10);
				}
				PlayerDataManager.Instance.ChangeMoney(result, false);

				RatioText.GetRectTransform().anchoredPosition = RatioTextPos;
				Text.SetText("0");
				return;
			}
			else
			{
				cancel?.Cancel();
				CompleteImmediately();
			}
		}
		#endregion

		private async ETTask PlayOpenAnim(BigNumber val, float mul)
		{
			await Win.Play("UIRaise_Open");
			if(!ActiveSelf) return;
			var animTime = 400f;
			var startTime = TimerManager.Instance.GetTimeNow();
			while(true)
			{
				await TimerManager.Instance.WaitAsync(1);
				if (cancel.IsCancel())
				{
					return;
				}
				if(!ActiveSelf) return;
				var timeNow = TimerManager.Instance.GetTimeNow();
				Text.SetText(((int)Mathf.Lerp(0, val, (timeNow - startTime) / animTime)).ToString());
				if (timeNow - startTime > animTime)
				{
					break;
				}
			}

			RatioText.GetRectTransform().localScale = Vector3.zero;
			RatioText.SetText($"{I18NManager.Instance.I18NGetText("Text_Reward_Ratio")}：x{mul}");
			if (IntelligenceMul != 0)
			{
				IntelligenceRatioText.GetRectTransform().localScale = Vector3.zero;
				IntelligenceRatioText.SetText(isMul ? $"{I18NManager.Instance.I18NGetText("Text_Intelligence_Ratio")}：x{IntelligenceMul}" : $"{I18NManager.Instance.I18NGetText("Text_Intelligence_Ratio")}：+{IntelligenceMul}");
			}
			else
			{
				IntelligenceRatioText.SetText("");
			}
			
			await TimerManager.Instance.WaitAsync(300);
			
			DoRatioTextAnimStep1().Coroutine();
			if (IntelligenceMul != 0)
			{
				await TimerManager.Instance.WaitAsync(100);
				DoIntelligenceRatioTextAnimStep1().Coroutine();
			}

			await TimerManager.Instance.WaitAsync(400);
			if(!ActiveSelf) return;
			if (IntelligenceMul != 0)
			{
				DoIntelligenceRatioTextAnimStep2().Coroutine();
				await TimerManager.Instance.WaitAsync(100);
			}
			DoRatioTextAnimStep2().Coroutine();

			if(IntelligenceMul != 0 && isMul) mul *= IntelligenceMul;
			animTime = 50f;
			for (int i = 0; i < 6; ++i)
			{
				startTime = TimerManager.Instance.GetTimeNow();
				var originText = val * Mathf.Lerp(1, mul, (float)i / 6);
				var targetText = val * Mathf.Lerp(1, mul, (float)(i + 1) / 6);
				var biggest = this.Text.GetRectTransform().localScale * 1.2f;
				var isNeg = false;
				while (true) 
				{
					await TimerManager.Instance.WaitAsync(1);
					if (cancel.IsCancel())
					{
						return;
					}
					if(!ActiveSelf) return;
					var timeNow = TimerManager.Instance.GetTimeNow();
					bool isBigger = timeNow - startTime <= animTime / 2;
					if (isBigger)
					{
						this.Text.GetRectTransform().localScale = Vector3.Lerp(Vector3.one, biggest, (timeNow - startTime) / (animTime * 0.5f));
					}
					else
					{
						if (!isNeg)
						{
							isNeg = true;
							startTime = timeNow;
						}
						this.Text.GetRectTransform().localScale = Vector3.Lerp(biggest, Vector3.one, (timeNow - startTime) / (animTime * 0.5f));
					}
					
					this.Text.SetText(I18NManager.Instance.TranslateMoneyToStr(originText + (targetText - originText) * Mathf.Clamp01((timeNow - startTime) / animTime)));

					if (timeNow - startTime >= animTime)
					{
						break;
					}
				}
					
			}
			if(IntelligenceMul != 0 && !isMul)
			{
				startTime = TimerManager.Instance.GetTimeNow();
				var originText = val * mul;
				var targetText = originText + IntelligenceMul;
				var biggest = this.Text.GetRectTransform().localScale * 1.2f;
				var isNeg = false;
				
				while (true) 
				{
					await TimerManager.Instance.WaitAsync(1);
					if (cancel.IsCancel())
					{
						return;
					}
					if(!ActiveSelf) return;
					var timeNow = TimerManager.Instance.GetTimeNow();
					bool isBigger = timeNow - startTime <= animTime / 2;
					if (isBigger)
					{
						this.Text.GetRectTransform().localScale = Vector3.Lerp(Vector3.one, biggest, (timeNow - startTime) / (animTime * 0.5f));
					}
					else
					{
						if (!isNeg)
						{
							isNeg = true;
							startTime = timeNow;
						}
						this.Text.GetRectTransform().localScale = Vector3.Lerp(biggest, Vector3.one, (timeNow - startTime) / (animTime * 0.5f));
					}
					
					this.Text.SetText(I18NManager.Instance.TranslateMoneyToStr(originText + (targetText - originText) * Mathf.Clamp01((timeNow - startTime) / animTime)));

					if (timeNow - startTime >= animTime)
					{
						break;
					}
				}
			}

			this.Text.GetRectTransform().localScale = Vector3.one;
			await TimerManager.Instance.WaitAsync(200);
			if(!ActiveSelf) return;
			animTime = 200f;
			startTime = TimerManager.Instance.GetTimeNow();
			var oldScale = Text.GetRectTransform().localScale;
			while (true)
			{
				await TimerManager.Instance.WaitAsync(1);
				if (cancel.IsCancel())
				{
					return;
				}
				if(!ActiveSelf) return;
				var timeNow = TimerManager.Instance.GetTimeNow();
				Text.GetRectTransform().localScale = Vector3.Lerp(oldScale, oldScale * 1.2f, (timeNow - startTime) / animTime);
				if (timeNow - startTime > animTime)
				{
					break;
				}
			}
			
			cancel?.Cancel();
		}
		
		private async ETTask PlayOpenAnim()
		{
			await Win.Play("UIRaise_Open");
			if(!ActiveSelf) return;
			var animTime = 400f;
			var startTime = TimerManager.Instance.GetTimeNow();
			while(true)
			{
				await TimerManager.Instance.WaitAsync(1);
				if (cancel.IsCancel())
				{
					return;
				}
				if(!ActiveSelf) return;
				var timeNow = TimerManager.Instance.GetTimeNow();
				Text.SetText(((int)Mathf.Lerp(0, val, (timeNow - startTime) / animTime)).ToString());
				if (timeNow - startTime > animTime)
				{
					break;
				}
			}

			RatioText.GetRectTransform().localScale = Vector3.zero;
			RatioText.SetText($"{I18NManager.Instance.I18NGetText("Text_Reward_Ratio")}：x{mul}");
			IntelligenceRatioText.GetRectTransform().localScale = Vector3.zero;
			IntelligenceRatioText.SetText($"{I18NManager.Instance.I18NGetText("Text_Intelligence_Ratio")}：x{IntelligenceMul}");
			await DoRatioTextAnimStep1();

			await TimerManager.Instance.WaitAsync(400);
			if(!ActiveSelf) return;
			await DoRatioTextAnimStep2();

			animTime = 50f;
			for (int i = 0; i < 6; ++i)
			{
				startTime = TimerManager.Instance.GetTimeNow();
				var originText = val * Mathf.Lerp(1, mul, (float)i / 6);
				var targetText = val * Mathf.Lerp(1, mul, (float)(i + 1) / 6);
				var biggest = this.Text.GetRectTransform().localScale * 1.2f;
				var isNeg = false;
				while (true) 
				{
					await TimerManager.Instance.WaitAsync(1);
					if (cancel.IsCancel())
					{
						return;
					}
					if(!ActiveSelf) return;
					var timeNow = TimerManager.Instance.GetTimeNow();
					bool isBigger = timeNow - startTime <= animTime / 2;
					if (isBigger)
					{
						this.Text.GetRectTransform().localScale = Vector3.Lerp(Vector3.one, biggest, (timeNow - startTime) / (animTime * 0.5f));
					}
					else
					{
						if (!isNeg)
						{
							isNeg = true;
							startTime = timeNow;
						}
						this.Text.GetRectTransform().localScale = Vector3.Lerp(biggest, Vector3.one, (timeNow - startTime) / (animTime * 0.5f));
					}
					
					this.Text.SetText(I18NManager.Instance.TranslateMoneyToStr(originText + (targetText - originText) * Mathf.Clamp01((timeNow - startTime) / animTime)));

					if (timeNow - startTime >= animTime)
					{
						break;
					}
				}
					
			}

			this.Text.GetRectTransform().localScale = Vector3.one;
			await TimerManager.Instance.WaitAsync(200);
			if(!ActiveSelf) return;
			animTime = 200f;
			startTime = TimerManager.Instance.GetTimeNow();
			var oldScale = Text.GetRectTransform().localScale;
			while (true)
			{
				await TimerManager.Instance.WaitAsync(1);
				if (cancel.IsCancel())
				{
					return;
				}
				if(!ActiveSelf) return;
				var timeNow = TimerManager.Instance.GetTimeNow();
				Text.GetRectTransform().localScale = Vector3.Lerp(oldScale, oldScale * 1.2f, (timeNow - startTime) / animTime);
				if (timeNow - startTime > animTime)
				{
					break;
				}
			}
			
			cancel?.Cancel();
		}

		private void CompleteImmediately()
		{
			Win.Play("UIRaise_ImmOpen").Coroutine();
			
			RatioText.SetText("");
			RatioText.GetRectTransform().anchoredPosition = RatioTextPos;
			RatioText.GetRectTransform().localScale = Vector3.zero;
			
			this.Text.GetRectTransform().localScale = this.Text.GetRectTransform().localScale * 1.2f;
			this.Text.SetText(isMul ? I18NManager.Instance.TranslateMoneyToStr(val * mul * IntelligenceMul) : I18NManager.Instance.TranslateMoneyToStr(val * mul + IntelligenceMul));
			
			IntelligenceRatioText.SetText("");
			IntelligenceRatioText.GetRectTransform().anchoredPosition = IntelligenceTextPos;
		}

		private async ETTask DoRatioTextAnimStep1()
		{
			var animTime = 300f;
			var startTime = TimerManager.Instance.GetTimeNow();
			
			while(true)
			{
				await TimerManager.Instance.WaitAsync(1);
				if (cancel.IsCancel())
				{
					return;
				}
				if(!ActiveSelf) return;
				var timeNow = TimerManager.Instance.GetTimeNow();
				RatioText.GetRectTransform().localScale = Vector3.Lerp(Vector3.zero, Vector3.one, Mathf.Clamp01((timeNow - startTime) / animTime));
				if (timeNow - startTime >= animTime)
				{
					break;
				}
			}
		}
		private async ETTask DoIntelligenceRatioTextAnimStep1()
		{
			var animTime = 300f;
			
			var startTime = TimerManager.Instance.GetTimeNow();
			while(true)
			{
				await TimerManager.Instance.WaitAsync(1);
				if (cancel.IsCancel())
				{
					return;
				}
				if(!ActiveSelf) return;
				var timeNow = TimerManager.Instance.GetTimeNow();
				IntelligenceRatioText.GetRectTransform().localScale = Vector3.Lerp(Vector3.zero, Vector3.one, Mathf.Clamp01((timeNow - startTime) / animTime));
				if (timeNow - startTime >= animTime)
				{
					break;
				}
			}
		}

		private async ETTask DoRatioTextAnimStep2()
		{
			var animTime = 300f;
			var startTime = TimerManager.Instance.GetTimeNow();
			var oldPos = RatioText.GetRectTransform().anchoredPosition;
			while (true)
			{
				await TimerManager.Instance.WaitAsync(1);
				if (cancel.IsCancel())
				{
					return;
				}
				if(!ActiveSelf) return;
				var timeNow = TimerManager.Instance.GetTimeNow();
				RatioText.GetRectTransform().anchoredPosition = Vector3.Lerp(oldPos, Vector3.zero, Mathf.Clamp01((timeNow - startTime) / animTime));
				if (timeNow - startTime >= animTime)
				{
					break;
				}
			}
			RatioText.SetText("");
			RatioText.GetRectTransform().anchoredPosition = oldPos;
		}
		private async ETTask DoIntelligenceRatioTextAnimStep2()
		{
			var animTime = 300f;
			var startTime = TimerManager.Instance.GetTimeNow();
			while (true)
			{
				await TimerManager.Instance.WaitAsync(1);
				if (cancel.IsCancel())
				{
					return;
				}
				if(!ActiveSelf) return;
				var timeNow = TimerManager.Instance.GetTimeNow();
				IntelligenceRatioText.GetRectTransform().anchoredPosition = Vector3.Lerp(IntelligenceTextPos, Vector3.zero, Mathf.Clamp01((timeNow - startTime) / animTime));
				if (timeNow - startTime >= animTime)
				{
					break;
				}
			}
			IntelligenceRatioText.SetText("");
			IntelligenceRatioText.GetRectTransform().anchoredPosition = IntelligenceTextPos;
		}
	}
}
