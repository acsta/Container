using System.Collections;
using System.Collections.Generic;
using System;
using SuperScrollView;
using UnityEngine;
using UnityEngine.UI;

namespace TaoTie
{
	public class UIWashDishView : UIBaseView, IOnCreate, IOnEnable
	{
		[Timer(TimerType.UIRestaurantViewUpdate)]
		public class UIRestaurantViewUpdateTimer : ATimer<UIWashDishView>
		{
			public override void Run(UIWashDishView self)
			{
				try
				{
					self.Update();
				}
				catch (Exception e)
				{
					Log.Error($"move timer error: UIRestaurantView\n{e}");
				}
			}
		}
		[Timer(TimerType.UIWashDishViewUpdate)]
		public class UIWashDishViewUpdateTimer : ATimer<UIWashDishView>
		{
			public override void Run(UIWashDishView self)
			{
				try
				{
					self.UpdateAdd();
				}
				catch (Exception e)
				{
					Log.Error($"move timer error: UIRestaurantView\n{e}");
				}
			}
		}
		
		
		private const int DISH_COUNT = 6;
		private const int DURING = 100;
		private static int DishAdd = 10;
		private static int DishAddDuring;
		public static string PrefabPath => "UIGame/UILobby/Prefabs/UIWashDishView.prefab";
		public UIAnimator UICommonView;
		public UITextmesh Talk;
		public UICopyGameObject Dishes;
		public UIButton Close;
		public UIEmptyView Wash;
		public UIButton BtnAdd;
		public UITextmesh BtnAddText;
		public UIButton BtnAuto;

		public UITextmesh TextLv;
		public UITextmesh Cost;
		public UIEmptyView LevelUp;
		public UIEmptyView LevelMax;
		public UITextmesh From;
		public UITextmesh From2;
		public UITextmesh To;
		public UIButton BtnLevelUp;
		public UIPointerClick PointerDish;
		private ParticleSystem Bubble;

		public UIEmptyView MoneyGroup;
		public UIEmptyView Effect;
		public UIButton GetButton;
		public UISlider Slider;
		public UITextmesh ValueText;
		public UITextmesh Speed;
		public UITextmesh ValueText2;
		public UIEmptyView Mask;
		public UIAnimator SliderAnim;
		public UIImage Fill;
		private Material FillMat;
		
		private long timerId;
		private long timerId2; 
		private int step;
		private bool isAnim;
		private int index;
		#region override
		public void OnCreate()
		{
			Bubble = AddComponent<UIEmptyView>("UICommonView/Bg/Content/Dishes/Bubble").GetGameObject().GetComponent<ParticleSystem>();
			From = AddComponent<UITextmesh>("UICommonView/Bg/Content/Bottom/LevelUp/Desc/From");
			To = AddComponent<UITextmesh>("UICommonView/Bg/Content/Bottom/LevelUp/Desc/To");
			BtnLevelUp = AddComponent<UIButton>("UICommonView/Bg/Content/Bottom/LevelUp/LvUp");
			LevelUp = AddComponent<UIEmptyView>("UICommonView/Bg/Content/Bottom/LevelUp");
			LevelMax = AddComponent<UIEmptyView>("UICommonView/Bg/Content/Bottom/LevelMax");
			From2 = AddComponent<UITextmesh>("UICommonView/Bg/Content/Bottom/LevelMax/Desc/From");
			
			Cost = AddComponent<UITextmesh>("UICommonView/Bg/Content/Bottom/LevelUp/LvUp/Money/Text");
			this.UICommonView = this.AddComponent<UIAnimator>("UICommonView");
			this.Talk = this.AddComponent<UITextmesh>("UICommonView/Bg/Content/Human/Talk/Text");
			this.Dishes = this.AddComponent<UICopyGameObject>("UICommonView/Bg/Content/Dishes");
			this.Dishes.InitListView(DISH_COUNT,OnGetItemByIndex);
			this.Close = this.AddComponent<UIButton>("UICommonView/Bg/Close");
			Wash = this.AddComponent<UIEmptyView>("UICommonView/Bg/Content/Dishes/Wash");
			BtnAdd = AddComponent<UIButton>("UICommonView/Bg/Content/Top/btn_Add");
			BtnAddText = AddComponent<UITextmesh>("UICommonView/Bg/Content/Top/btn_Add/Text");
			BtnAuto = AddComponent<UIButton>("UICommonView/Bg/Content/Top/btn_Auto");
			TextLv = AddComponent<UITextmesh>("UICommonView/Bg/Content/Bottom/Lv/Text");
			PointerDish = AddComponent<UIPointerClick>("UICommonView/Bg/Content/Dishes");
			TextLv.SetI18NKey(I18NKey.Text_Dish_Lv);
			this.Close.SetOnClick(OnClickClose);
			MoneyGroup = AddComponent<UIEmptyView>("UICommonView/Bg/Content/Top/Money");
			Effect = AddComponent<UIEmptyView>("UICommonView/Bg/Content/Top/Money/money");
			this.GetButton = this.AddComponent<UIButton>("UICommonView/Bg/Content/Top/Money/Icon");
			this.Slider = this.AddComponent<UISlider>("UICommonView/Bg/Content/Top/Money/Slider");
			SliderAnim = this.AddComponent<UIAnimator>("UICommonView/Bg/Content/Top/Money/Slider");
			this.ValueText = this.AddComponent<UITextmesh>("UICommonView/Bg/Content/Top/Money/Slider/Fill Area/ValueText");
			this.ValueText2 = AddComponent<UITextmesh>("UICommonView/Bg/Content/Top/Money/Slider/Fill Area/Mask/ValueText");
			Mask = AddComponent<UIEmptyView>("UICommonView/Bg/Content/Top/Money/Slider/Fill Area/Mask");
			this.Speed = this.AddComponent<UITextmesh>("UICommonView/Bg/Content/Top/Money/Icon/Speed");
			Fill = AddComponent<UIImage>("UICommonView/Bg/Content/Top/Money/Slider/Fill Area/Fill");
			FillMat = Fill.GetMaterial();
			AddComponent<UIRedDot,string>("UICommonView/Bg/Content/Top/Money/RedDot","Restaurant_TimeOut");
		}
		public void OnEnable()
		{
			if (PlayerDataManager.Instance.RestaurantLv == 0)
			{
				PlayerDataManager.Instance.LevelUp();
			}
			Bubble.Stop();
			DishAdd = GlobalConfigCategory.Instance.GetInt("DishAdd", DishAdd);
			DishAddDuring = GlobalConfigCategory.Instance.GetInt("DishAddDuring", 600);
			DishAddDuring = DishAddDuring * 1000;
			PlayerDataManager.Instance.RefreshRestaurantProfitRedDot();
			isAnim = true;
			index = 0;
			step = 0;
			this.Dishes.RefreshAllShownItem();
			Wash.GetRectTransform().anchoredPosition = Vector2.right * 80 + new Vector2(0, 25 + 10 * DISH_COUNT);
			SoundManager.Instance.PlaySound("Audio/Sound/Common_Open.mp3");
			BtnAuto.SetActive(!PlayerDataManager.Instance.WashDishAuto);
			this.GetButton.SetOnClick(OnClickGetButton);
			Slider.SetOnValueChanged(OnSliderChange);
			PointerDish.SetOnClick(OnClickDish);
			BtnAdd.SetOnClick(OnClickAdd);
			BtnAdd.SetActive(AdManager.Instance.PlatformHasAD());
			BtnAuto.SetActive(AdManager.Instance.PlatformHasAD());
			RefreshLvInfo();
			if (!PlayerDataManager.Instance.WashDishAuto)
			{
				BtnAuto.SetOnClick(OnClickUnlockAuto);
			}
			OnEnableAsync().Coroutine();
			UpdateAdd();
			var timeNow = TimerManager.Instance.GetTimeNow();
			TimerManager.Instance.Remove(ref timerId2);
			if (timeNow - PlayerDataManager.Instance.StartAddTime < DishAddDuring)
			{
				timerId2 = TimerManager.Instance.NewRepeatedTimer(1000, TimerType.UIWashDishViewUpdate, this);
			}
			TimerManager.Instance.Remove(ref timerId);
			MoneyGroup.SetActive(PlayerDataManager.Instance.WashDishAuto);
			if (PlayerDataManager.Instance.WashDishAuto)
			{
				timerId = TimerManager.Instance.NewRepeatedTimer(PlayerDataManager.Instance.ProfitUpdateUnitTime,
					TimerType.UIRestaurantViewUpdate, this);
			}
			UpdateImmediate();
		}

		private async ETTask OnEnableAsync()
		{
			await UICommonView.Play("UIView_Open");
			Bubble.Stop();
			isAnim = false;
		}

		public override async ETTask CloseSelf()
		{
			TimerManager.Instance.Remove(ref timerId2);
			TimerManager.Instance.Remove(ref timerId);
			UIManager.Instance.OpenWindow<UILobbyView>(UILobbyView.PrefabPath).Coroutine();
			await UICommonView.Play("UIView_Close");
			await base.CloseSelf();
		}

		public void UpdateAdd()
		{
			var timeNow = TimerManager.Instance.GetTimeNow();
			var during = timeNow - PlayerDataManager.Instance.StartAddTime;
			if (during > DishAddDuring)
			{
				TimerManager.Instance.Remove(ref timerId2);
				BtnAddText.SetI18NKey(I18NKey.Text_Dish_Add, DishAdd);
			}
			else
			{
				BtnAddText.SetText((DishAddDuring - during) / 1000 + I18NManager.Instance.I18NGetText(I18NKey.Text_Time_Second));
			}
		}
		public void Update()
		{
			var max = PlayerDataManager.Instance.CalculateMaxStageProfit();
			if (max == 0 || !PlayerDataManager.Instance.WashDishAuto)
			{
				ValueText.SetText("");
				ValueText2.SetText("");
				Slider.SetNormalizedValue(0);
				return;
			}
			var current = PlayerDataManager.Instance.CalculateProfit();
			var value = (float) (current / max);
			if (value < 0.5f)
			{
				GetButton.SetSpritePath("UI/UICommon/Atlas/UI_icon_cash6.png",true).Coroutine();
			} 
			else if (value < 0.9f)
			{
				GetButton.SetSpritePath("UI/UICommon/Atlas/UI_icon_cash7.png",true).Coroutine();
			}
			else
			{
				GetButton.SetSpritePath("UI/UICommon/Atlas/UI_icon_cash4.png",true).Coroutine();
			}
			if (Slider.GetNormalizedValue() != value)
			{
				Speed.SetActive(false);
				Speed.SetActive(true);
			}
			SliderAnim.SetEnable(value >= 1);
			if (value < 0)
			{
				SliderAnim.GetRectTransform().localScale = Vector3.one;
			}
			Fill.SetMaterial(value >= 1 ? FillMat : null);
			Slider.SetNormalizedValue(value);
			var text = I18NManager.Instance.TranslateMoneyToStr(current) +
			           "/" + I18NManager.Instance.TranslateMoneyToStr(max);
			ValueText.SetText(text);
			ValueText2.SetText(text);
			var win = UIManager.Instance.GetView<UIProfitWin>(1);
			win?.Update(current);
			Effect.SetActive(UIManager.Instance.GetTopWindow(UILayerNames.NormalLayer)?.View == this);
			PlayerDataManager.Instance.RefreshRestaurantProfitRedDot();
		}
		public void UpdateImmediate()
		{
			Update();
			Speed.SetActive(false);
		}
		public void OnClickDish()
		{
			if (isAnim) return;
			ShockManager.Instance.Vibrate();
			if (step % 2 == 0)
			{
				WashDish(index).Coroutine();
			}
			else
			{
				RemoveDish(index).Coroutine();
			}
			step++;
		}

		public void OnClickAdd()
		{
			var timeNow = TimerManager.Instance.GetTimeNow();
			if (timeNow - PlayerDataManager.Instance.StartAddTime <= DishAddDuring)
			{
				return;
			}
			OnClickAddAsync().Coroutine();
		}

		public async ETTask OnClickAddAsync()
		{
			if (AdManager.Instance.PlatformHasAD())
			{
				BtnAuto.SetInteractable(false);
				BtnAdd.SetInteractable(false);
				try
				{
					var res = await AdManager.Instance.PlayAd();
					if (res)
					{
						BtnAddText.SetText(DishAddDuring / 1000 + I18NManager.Instance.I18NGetText(I18NKey.Text_Time_Second));
						PlayerDataManager.Instance.StartAddTime = TimerManager.Instance.GetTimeNow();
						TimerManager.Instance.Remove(ref timerId2);
						timerId2 = TimerManager.Instance.NewRepeatedTimer(1000, TimerType.UIWashDishViewUpdate, this);
					}
				}
				catch (Exception ex)
				{
					Log.Error(ex);
				}
				finally
				{
					BtnAuto.SetInteractable(true);
					BtnAdd.SetInteractable(true);
				}
			}
		}
		private void RefreshLvInfo()
		{
			var config =
				RestaurantConfigCategory.Instance.GetByLv(PlayerDataManager.Instance.RestaurantLv, out var next);
			LevelUp.SetActive(next != null);
			LevelMax.SetActive(next == null);
			TextLv.SetI18NText(PlayerDataManager.Instance.RestaurantLv);
			Speed.SetText("+"+I18NManager.Instance.TranslateMoneyToStr(PlayerDataManager.Instance
				.CalculateProfitUpdateUnit()));
			if (next != null)
			{
				From.SetNum(config.WashRewards);
				To.SetNum(next.WashRewards);
				Cost.SetNum(config.Cost);
				BtnLevelUp.SetOnClick(OnClickLvUp);
			}
			else
			{
				From2.SetNum(config.WashRewards);
			}
		}
		#endregion

		#region 事件绑定

		private void OnSliderChange(float val)
		{
			var rect = Mask.GetRectTransform();
			var size = (rect.parent as RectTransform).sizeDelta;
			rect.offsetMin = new Vector2(val * size.x, 0);
		}
		
		public void OnClickGetButton()
		{
			UIManager.Instance.OpenWindow<UIProfitWin>(UIProfitWin.PrefabPath).Coroutine();
		}
		
		public void OnGetItemByIndex(int index, GameObject obj)
		{
			var trans = obj.GetComponent<RectTransform>();
			trans.anchoredPosition = index * 10 * Vector2.up;
			trans.GetChild(0).GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
		}
		public void OnClickClose()
		{
			CloseSelf().Coroutine();
		}

		public void OnClickUnlockAuto()
		{
			OnClickUnlockAutoAsync().Coroutine();
		}

		public async ETTask OnClickUnlockAutoAsync()
		{
			if(PlayerDataManager.Instance.WashDishAuto) return;
			if (AdManager.Instance.PlatformHasAD())
			{
				BtnAuto.SetInteractable(false);
				BtnAdd.SetInteractable(false);
				try
				{
					var res = await AdManager.Instance.PlayAd();
					if (res)
					{
						PlayerDataManager.Instance.UnlockAutoWash();
						BtnAuto.SetActive(false);
						MoneyGroup.SetActive(PlayerDataManager.Instance.WashDishAuto);
						timerId = TimerManager.Instance.NewRepeatedTimer(PlayerDataManager.Instance.ProfitUpdateUnitTime,
							TimerType.UIRestaurantViewUpdate, this);
					}
				}
				catch (Exception ex)
				{
					Log.Error(ex);
				}
				finally
				{
					BtnAuto.SetInteractable(true);
					BtnAdd.SetInteractable(true);
				}
			}
		}
		#endregion

		private async ETTask RemoveDish(int index)
		{
			isAnim = true;
			var curIndex = DISH_COUNT - 1 - index;
			var curDish = Dishes.GetItemByIndex(curIndex).transform.GetChild(0).GetComponent<RectTransform>();
			var timeStart = TimerManager.Instance.GetTimeNow();
			var config = RestaurantConfigCategory.Instance.GetByLv(PlayerDataManager.Instance.RestaurantLv, out _);
			BigNumber rewards;
			int count = 1;
			if (timeStart - PlayerDataManager.Instance.StartAddTime <= DishAddDuring)
			{
				rewards = config.WashRewards * DishAdd;
				count = DishAdd;
			}
			else
			{
				rewards = config.WashRewards;
			}
			var top = UIManager.Instance.GetView<UITopView>(1);
			if (top != null)
			{
				top.Top.DoMoneyMoveAnim(rewards, curDish.position, count).Coroutine();
			}
			float during = DURING;
			while (true)
			{
				await TimerManager.Instance.WaitAsync(1);
				var timeNow = TimerManager.Instance.GetTimeNow();
				var delta = timeNow - timeStart;
				var progress = delta / during;
				curDish.anchoredPosition = Vector2.Lerp(Vector2.zero,
					((curIndex % 2 == 1 ? Vector2.left : Vector2.right) + Vector2.up * 0.2f) * Define.DesignScreenWidth, progress);
				if (delta > during)
				{
					break;
				}
			}
			
			PlayerDataManager.Instance.ChangeMoney(rewards);
			isAnim = false;
			this.index++;
			if (this.index >= DISH_COUNT)
			{
				this.index = 0;
				this.Dishes.RefreshAllShownItem();
				Wash.GetRectTransform().anchoredPosition = Vector2.right * 80 + new Vector2(0, 25 + 10 * DISH_COUNT);
			}
			else
			{
				curIndex--;
				Wash.GetRectTransform().anchoredPosition =
					(curIndex % 2 == 1 ? Vector2.left : Vector2.right) * -80 + new Vector2(0, 25 + 10 * curIndex);
			}
		}

		private async ETTask WashDish(int index)
		{
			Bubble.Play();
			isAnim = true;
			var curIndex = DISH_COUNT - index;
			float during = DURING;
			var timeStart = TimerManager.Instance.GetTimeNow();
			while (true)
			{
				await TimerManager.Instance.WaitAsync(1);
				var timeNow = TimerManager.Instance.GetTimeNow();
				float washTime = timeNow - timeStart;
				float progress = Mathf.Clamp01(washTime / during);
				Wash.GetRectTransform().anchoredPosition =
					(curIndex % 2 == 1 ? Vector2.left : Vector2.right) * (80 - progress * 160) + new Vector2(0, 25 + 10 * curIndex);
				if(washTime >= during) break;
			}
			// Wash.GetRectTransform().anchoredPosition =
			// 	(curIndex % 2 == 1 ? Vector2.left : Vector2.right) * -80 + new Vector2(0, 25 + 10 * (curIndex - 1));
			isAnim = false;
			Bubble.Stop();
		}
		

		private void OnClickLvUp()
		{
			if (PlayerDataManager.Instance.LevelUp())
			{
				RefreshLvInfo();
			}
		}
	}
}
