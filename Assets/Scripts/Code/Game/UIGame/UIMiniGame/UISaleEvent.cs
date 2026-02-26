using System.Collections;
using System.Collections.Generic;
using System;
using SuperScrollView;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace TaoTie
{
    public class UISaleEvent : UIBaseView, IOnCreate, IOnEnable<BigNumber,long>, IOnDestroy, IOnDisable
    {
        private float PriceStep = 0;

        enum SaleMode
        {
            Buy,
            Sell
        }
        
        enum LastState
        {
            None = 0,
            Bad,
            Soso
        }

        enum CurrStepState
        {
            None = 0,
            Deal,
            DealFail,
            Face,
            Fixed,
        }
        
        public static string PrefabPath => "UIGame/UIMiniGame/Prefabs/UISaleEvent.prefab";
        public UITextmesh TotalSaleText;
        public UITextmesh MyScaleText;
        public UITextmesh NoticText;
        public UITextmesh LeftButtonText;
        public UITextmesh RightButtonText;
        public UITextmesh TitleText;
        public UITextmesh DealSuccessNoticeText;
        public UITextmesh DealFailNoticeText;
        public UITextmesh BattleSuccessText;
        public UITextmesh BattleFailText;
        public UITextmesh Name;
        public UITextmesh TopMaskTurnaroundText;
        public UISlider Slider;
        public UIPointerClick Add;
        public UIPointerClick Substract;
        public UIPointerClick Mask;
        public UIButton BitButton;
        public UIButton BattleButton;
        public UIButton NextLevelButton;
        public UIButton BackMainButton;
        public UIRawImage WinImage;
        public UIImage Back2Image;
        public UIImage Back3Image;
        public UIImage ADIconImg;
        public UIImage TopMaskTurnaroundImg;
        public UIImage Icon;
        public UIImage animImg;

        public UIEmptyView Buttons;
        public UIAnimator animator;
        public UICashGroup CashGroup;
        public UIPointerClick silderOffClick;
        public UIPointerClick animClick;

        private long blackId;
        public Bidder BlackBoy => EntityManager.Instance?.Get<Bidder>(blackId);
        private BigNumber totalSale = 0;
        private float otherSale = 0;
        private float mySale = 0;
        private bool isOver = false;
        private bool needChoose = false;
        private bool isBattled = false;
        private bool isBattleing = false;
        private int saleCount = 0;

        private ETTask<int> task;
        private LastState lastState;
        private SaleMode saleMode => (SaleMode) Config.Type;
        private CurrStepState currStepState;
        private SaleEventConfig Config;

        public void OnCreate()
        {
            if (!GlobalConfigCategory.Instance.TryGetFloat("PriceStep", out PriceStep)) PriceStep = 0.1f;
            Name = AddComponent<UITextmesh>("UICommonWin/Win/Content/Back1/Name/Text (TMP)");
            Icon = AddComponent<UIImage>("UICommonWin/Win/Content/Back1/character");
            this.TotalSaleText = this.AddComponent<UITextmesh>("UICommonWin/Win/Content/Back2/Text (TMP)");
            this.MyScaleText = this.AddComponent<UITextmesh>("UICommonWin/Win/Content/Back3/CoinIcon/Sale");
            this.NoticText = this.AddComponent<UITextmesh>("UICommonWin/Win/Content/NoticeBox/Text (TMP)");
            this.LeftButtonText = this.AddComponent<UITextmesh>("UICommonWin/Win/Content/Button/CommonButton/Text");
            this.RightButtonText = this.AddComponent<UITextmesh>("UICommonWin/Win/Content/Button/CommonButton (1)/Text");
            this.TitleText = this.AddComponent<UITextmesh>("UICommonWin/Win/Title");
            this.DealSuccessNoticeText = this.AddComponent<UITextmesh>("UICommonWin/DealSuccessNotice");
            this.DealFailNoticeText = this.AddComponent<UITextmesh>("UICommonWin/DealFailNotice");
            this.BattleSuccessText = this.AddComponent<UITextmesh>("UICommonWin/BattleSuccess");
            this.BattleFailText = this.AddComponent<UITextmesh>("UICommonWin/BattleFail");
            this.TopMaskTurnaroundText = this.AddComponent<UITextmesh>("TopMaskTurnaround/Text (TMP)");

            this.Add = this.AddComponent<UIPointerClick>("UICommonWin/Win/Content/Back3/Add");
            this.Substract = this.AddComponent<UIPointerClick>("UICommonWin/Win/Content/Back3/Substract");
            this.Mask = this.AddComponent<UIPointerClick>("UICommonWin/Mask");

            this.BitButton = this.AddComponent<UIButton>("UICommonWin/Win/Content/Button/CommonButton");
            this.BattleButton = this.AddComponent<UIButton>("UICommonWin/Win/Content/Button/CommonButton (1)");
            this.NextLevelButton = this.AddComponent<UIButton>("NextLevelButton");
            this.BackMainButton = this.AddComponent<UIButton>("BackMainButton");
            
            this.Slider = this.AddComponent<UISlider>("UICommonWin/Win/Content/Back3/Slider");

            this.WinImage = this.AddComponent<UIRawImage>("UICommonWin/Win");
            this.Back2Image = this.AddComponent<UIImage>("UICommonWin/Win/Content/Back2");
            this.Back3Image = this.AddComponent<UIImage>("UICommonWin/Win/Content/Back3");
            this.ADIconImg = this.AddComponent<UIImage>("UICommonWin/Win/Content/Button/CommonButton (1)/Image");
            this.animImg = this.AddComponent<UIImage>("Anim");
            TopMaskTurnaroundImg = AddComponent<UIImage>("TopMaskTurnaround");

            this.Buttons = AddComponent<UIEmptyView>("UICommonWin/Win/Content/Button");
            CashGroup = AddComponent<UICashGroup>("CashGroup");
            this.animator = this.AddComponent<UIAnimator>();

            silderOffClick = AddComponent<UIPointerClick>("UICommonWin/Win/Content/Back3/Slider");
            animClick = AddComponent<UIPointerClick>("Anim");
        }

        public void OnEnable(BigNumber totalSale, long blackId)
        {
            this.blackId = blackId;
            var bbc = BlackBoy.GetComponent<BlackBoyComponent>();
            Config = bbc.SaleEventConfig;
            this.totalSale = totalSale;
            isOver = false;
            needChoose = false;
            isBattled = false;
            isBattleing = true;
            saleCount = 0;
            lastState = LastState.None;
            currStepState = CurrStepState.None;
            this.GetGameObject().GetComponent<CanvasGroup>().alpha = 1;
            
            BitButton.SetInteractable(false);
            BattleButton.SetInteractable(false);
            BattleButton.SetActive(true);
            WinImage.SetEnabled(true);
            TitleText.SetActive(true);
            Back2Image.SetActive(true);
            Back3Image.SetActive(true);
            Buttons.SetActive(true);
            DealSuccessNoticeText.SetActive(false);
            DealFailNoticeText.SetActive(false);
            BattleSuccessText.SetActive(false);
            BattleFailText.SetActive(false);
            Mask.SetEnabled(false);
            ADIconImg.SetEnabled(false);
            Slider.SetEnable(true);
            silderOffClick.SetEnabled(false);
            animImg.SetActive(false);
            CashGroup.SetActive(true);
            NextLevelButton.SetActive(false);
            BackMainButton.SetActive(false);

            TotalSaleText.SetText(I18NManager.Instance.I18NGetText(I18NKey.Text_Game_Amount) +
                                  I18NManager.Instance.TranslateMoneyToStr(totalSale));
            MyScaleText.SetNum(totalSale);
            TitleText.SetText(I18NManager.Instance.I18NGetText(I18NKey.Text_Event));
            Slider.SetValue(totalSale / (totalSale * Config.SliderMaxValue));
            mySale = totalSale;
            
            silderOffClick.SetOnClick(OnClickCantGivePrice);
            Slider.SetOnValueChanged(OnClickSlider);
            Add.SetOnClick(OnClickAdd);
            Substract.SetOnClick(OnClickSubstract);
            LeftButtonText.SetText(I18NManager.Instance.I18NGetText(I18NKey.Text_Bid));
            RightButtonText.SetText(I18NManager.Instance.I18NGetText(I18NKey.Text_Resist));
            BitButton.SetOnClick(OnClickGivePrice);
            BattleButton.SetOnClick(OnClickBattle);
            Mask.SetOnClick(OnClickMask);
            animClick.SetOnClick(OnClickAnim);
            NextLevelButton.SetOnClick(OnClickNextLevel);
            BackMainButton.SetOnClick(OnClickBack);
            
            Name.SetText(I18NManager.Instance.I18NGetText(Config));
            Icon.SetSpritePath(Config.Icon).Coroutine();
            SaleEventAsync().Coroutine();
        }

        public void OnDestroy()
        {
            if (!task.IsCompleted)
            {
                task?.SetResult(-1);
            }
        }

        public void OnDisable()
        {
            if (!task.IsCompleted)
            {
                task?.SetResult(-1);
            }
        }

        #region OnClick
        private void OnClickSlider(float value)
        {
            var sale = Config.SliderMaxValue * totalSale * value;
            mySale = sale;
            MyScaleText.SetNum(sale);
        }
        private void OnClickAdd()
        {
            var newSliderValue = Mathf.Clamp01(Slider.GetValue() + PriceStep);
            Slider.SetValue(newSliderValue);
            MyScaleText.SetNum(Config.SliderMaxValue * totalSale * newSliderValue);
        }
        private void OnClickSubstract()
        {
            var newSliderValue = Mathf.Clamp01(Slider.GetValue() - PriceStep);
            Slider.SetValue(newSliderValue);
            MyScaleText.SetNum(Config.SliderMaxValue * totalSale * newSliderValue);
        }
        private async ETTask SaleEventAsync()
        {
            currStepState = CurrStepState.Deal;
            task = ETTask<int>.Create();
            BitButton.SetInteractable(false);
            BattleButton.SetInteractable(false);
            bool isNPCGivePrice = Random.Range(0, 2) == 1;
            isNPCGivePrice = true;
            otherSale = totalSale * SaleEventConfigCategory.Instance.Get(10001).NPCTargetValue;
            if (isNPCGivePrice)
            {
                NoticText.SetText(I18NManager.Instance.I18NGetParamText(Config.FirstStepNPCText, Mathf.Floor(otherSale)));
                MyScaleText.SetText(Mathf.Floor(otherSale).ToString());
                task.SetResult(0);
            }
            else
            {
                NoticText.SetText(I18NManager.Instance.I18NGetParamText(Config.FirstStepUserText, otherSale));
                MyScaleText.SetText(totalSale.ToString());
            }
            await task;
            
            if (isOver)
            {
                return;
            }
            task = ETTask<int>.Create();
            BitButton.SetInteractable(true);
            BattleButton.SetInteractable(true);
            BitButton.SetOnClick(OnClickGivePrice);
            BattleButton.SetOnClick(OnClickBattle);
            needChoose = true;
            await task;

            while (true)
            {
                if (isOver)
                {
                    return;
                }
                task = ETTask<int>.Create();
                BitButton.SetInteractable(false);
                BattleButton.SetInteractable(false);

                switch (currStepState)
                {
                    case CurrStepState.Deal:
                    {
                        if (DoSatisfied())
                        {
                
                        }
                        else DoSosoBad();
                        
                        BitButton.SetInteractable(true);
                        BattleButton.SetInteractable(true);
            
                        BitButton.SetOnClick(OnClickGivePrice);
                        BattleButton.SetOnClick(OnClickBattle);
                        break;
                    }
                    case CurrStepState.DealFail:
                    {
                        BitButton.SetInteractable(true);
                        BattleButton.SetInteractable(true);
                        
                        BitButton.SetActive(true);
                        BattleButton.SetActive(true);
                            
                        LeftButtonText.SetText(I18NManager.Instance.I18NGetText(Config.FailButton));
                        RightButtonText.SetText(I18NManager.Instance.I18NGetText(Config.ADButton));
                        ADIconImg.SetActive(true);
                        
                        BitButton.SetOnClick(isBattled ? OnClickFaceBattleFailed : OnClickFace);
                        BattleButton.SetOnClick(OnClickAD);
                        break;
                    }
                    case CurrStepState.Fixed:
                    {
                        BattleButton.SetActive(true);

                        BitButton.SetInteractable(true);
                        BattleButton.SetInteractable(true);
                            
                        BitButton.SetOnClick(OnClickAgree);
                        BattleButton.SetOnClick(OnClickRefuse);
                            
                        LeftButtonText.SetText(I18NManager.Instance.I18NGetText(Config.FixedPriceAgree));
                        RightButtonText.SetText(I18NManager.Instance.I18NGetText(Config.FixedPriceRefuse));
                        break;
                    }
                }
                
                await task;
            }
        }
        private void OnClickGivePrice()
        {
            //needChoose = false;
            saleCount++;
            BattleButton.SetActive(false);
            task.SetResult(0);
        }
        private void OnClickBattle()
        {
            OnClickBattleAsync().Coroutine();
        }
        private async ETTask OnClickBattleAsync()
        {
            isBattleing = true;
            bool isWin = Random.Range(0f, 1f) <= Config.BattleThreshold;
            animImg.SetActive(true);

            while (isBattleing)
            {
                await TimerManager.Instance.WaitAsync(1);
            }

            if (isWin)
            {
                DoBattleSuccess();
            }
            else
            {
                DoBattleFail();
            }
        }
        private void OnClickRefuse()
        {
            DoDealFail();
        }
        private void OnClickAgree()
        {
            isOver = true;
            DoDealSuccess(otherSale);
        }
        private void OnClickMask()
        {
            OnClickMaskAsync().Coroutine();
        }
        private async ETTask OnClickMaskAsync()
        {
            if (currStepState == CurrStepState.Face)
            {
                await animator.Play("Fadeout");
                IAuctionManager.Instance.RunNextStage();
            }
            else
            {
                IAuctionManager.Instance.RunNextStage();
            }

            await CloseSelf();
        }
        private void OnClickFace()
        {
            if (isBattled)
            {
                TopMaskTurnaroundImg.SetActive(true);
                TopMaskTurnaroundText.SetText(I18NManager.Instance.I18NGetText(Config.TurnBattleFail));
                TitleText.SetText(I18NManager.Instance.I18NGetText(I18NKey.Text_Battle_Fail));
            }
            else
            {
                currStepState = CurrStepState.Face;
                DealFailNoticeText.SetActive(true);
                Buttons.SetActive(false);
                NextLevelButton.SetActive(true);
                DealFailNoticeText.SetActive(true);
                DealFailNoticeText.SetText(I18NManager.Instance.I18NGetParamText(Config.ResultSaleFail, totalSale.ToString()));
                TitleText.SetText(I18NManager.Instance.I18NGetText(I18NKey.Text_Sale_Fail));
                //Mask.SetEnabled(true);
                isOver = true;
            }
        }
        private void OnClickAD()
        {
            isOver = true;
            DoBattleSuccess(true);
        }
        private void OnClickCantGivePrice()
        {
            UIManager.Instance.OpenBox<UIToast, I18NKey>(UIToast.PrefabPath, I18NKey.Text_FixedPriceCantGivePrice).Coroutine();
        }
        private void OnClickAnim()
        {
            isBattleing = false;
            this.animImg.SetActive(false);
        }
        private void OnClickNextLevel()
        {
            IAuctionManager.Instance.RunNextStage();
            this.CloseSelf().Coroutine();
        }
        private void OnClickBack()
        {
            OnClickBackAsync().Coroutine();
        }
        private async ETTask OnClickBackAsync()
        {
            await CloseSelf();
            var gameView = UIManager.Instance.GetView<UIGameView>(1);
            if (gameView != null)
            {
                await gameView.CloseSelf();
            }
            GameTimerManager.Instance.SetTimeScale(1);
            IAuctionManager.Instance.ForceAllOver();
        }
        private void OnClickFaceBattleFailed()
        {
            TopMaskTurnaroundImg.SetActive(true);
            TopMaskTurnaroundText.SetText(I18NManager.Instance.I18NGetText(Config.TurnBattleFail));
        }
        #endregion OnClick

        private bool DoFixedPrice(bool isRandom = true)
        {
            if (saleCount < 3)
            {
                var fixedPrice = GenerateFixedPrice(isRandom);
                fixedPrice = Mathf.Floor(fixedPrice);
                if (fixedPrice >= 0)
                {
                    currStepState = CurrStepState.Fixed;
                    otherSale = fixedPrice;
                    Slider.SetEnable(false);
                    Add.SetOnClick(OnClickCantGivePrice);
                    Substract.SetOnClick(OnClickCantGivePrice);
                    Slider.SetValue(otherSale / (totalSale * Config.SliderMaxValue));
                    silderOffClick.SetEnabled(true);
                    NoticText.SetText(I18NManager.Instance.I18NGetParamText(Config.NPCFixedPrice, fixedPrice));
                    //SetAgreeButton();
                    //SetRefuseButton();
                    task.SetResult(1);
                    return true;
                }
            }

            return false;
        }
        private float GenerateFixedPrice(bool isRandom = true)
        {
            if (isRandom)
            {
                bool isGenerated = Random.Range(0f, 1f) <= Config.FixedPriceThreshold;
                if (!isGenerated) return -1f;
            }

            var randomFactor = Random.Range(Config.NPCNewPriceRangeMinValue, Config.NPCNewPriceRangeMaxValue);
            
            var o = mySale > otherSale ? otherSale * (1 + randomFactor) : otherSale * (1 - randomFactor);
            return o;
        }
        private void DoSosoBad()
        {
            if (otherSale * Config.NPCTargetRangeMinValue < mySale && 
                mySale < otherSale * Config.NPCTargetRangeMaxValue)
            {
                if (saleCount >= 3)
                {
                    DoDealSuccess(mySale);
                }
                else
                {
                    if (lastState == LastState.Soso)
                    {
                        DoFixedPrice(false);
                        
                        return;
                    }
                    
                    if (!DoFixedPrice())
                    {
                        var factor = Random.Range(Config.NPCNewPriceRangeMinValue, Config.NPCNewPriceRangeMaxValue);
                        var newPrice = Mathf.Floor(otherSale > mySale ? otherSale * (1 - factor) : otherSale * (1 + factor));
                        otherSale = newPrice;
                        
                        Slider.SetValue(newPrice / (totalSale * Config.SliderMaxValue));
                        NoticText.SetText(I18NManager.Instance.I18NGetParamText(Config.NPCSoso, newPrice));
                        lastState = LastState.Soso;
                        needChoose = true;
                    }
                }
            }
            else
            {
                if (saleCount >= 3 || lastState == LastState.Bad || lastState == LastState.Soso)
                {
                    DoDealFail();
                }
                else
                {
                    var newPrice = Mathf.Floor(otherSale * (1 + Random.Range(Config.NPCNewPriceRangeMinValue, Config.NPCNewPriceRangeMaxValue)));
                    Slider.SetValue(newPrice / (totalSale * Config.SliderMaxValue));
                    NoticText.SetText(I18NManager.Instance.I18NGetParamText(Config.NPCBad, newPrice));
                    lastState = LastState.Bad;
                    needChoose = true;
                }
            }
        }
        private bool DoSatisfied()
        {
            bool isSatisfied = mySale >= otherSale * Config.NPCTargetRangeMaxValue;
            if (isSatisfied)
            {
                isOver = true;

                DoDealSuccess(mySale);
            }
            return isSatisfied;
        }
        private void DoDealSuccess(float salePrice)
        {
            //await ETTask.CompletedTask;
            isOver = true;
            NoticText.SetText(I18NManager.Instance.I18NGetParamText(saleCount >= 3 ? Config.DealSuccess : Config.NPCGood, Mathf.Floor(salePrice)));
            WinImage.SetEnabled(false);
            Back2Image.SetActive(false);
            Back3Image.SetActive(false);
            Buttons.SetActive(false);
            Mask.SetEnabled(true);
            DealSuccessNoticeText.SetActive(true);
            DealSuccessNoticeText.SetText(I18NManager.Instance.I18NGetParamText(Config.ResultSaleSuccess, Mathf.Floor(salePrice)));
            NextLevelButton.SetActive(true);
            TitleText.SetText(I18NManager.Instance.I18NGetText(I18NKey.Text_Sale_Success));
            task.SetResult(2);
        }
        private void DoDealFail()
        {
            //await ETTask.CompletedTask;
            currStepState = CurrStepState.DealFail;
            needChoose = true;
            NoticText.SetText(I18NManager.Instance.I18NGetText(Config.DealFail));
            WinImage.SetEnabled(false);
            Back2Image.SetActive(false);
            Back3Image.SetActive(false);
            task.SetResult(1);
        }
        private void DoBattleSuccess(bool isAD = false)
        {
            //await ETTask.CompletedTask;
            isOver = true;
            NoticText.SetText(I18NManager.Instance.I18NGetParamText(Config.BattleSuccess, totalSale));
            WinImage.SetEnabled(false);
            Back2Image.SetActive(false);
            Back3Image.SetActive(false);
            Buttons.SetActive(false);
            Mask.SetEnabled(true);
            BattleSuccessText.SetText(I18NManager.Instance.I18NGetParamText(Config.ResultBattleSuccess, isAD ? Config.ADProfit : Config.NormalProfit));
            BattleSuccessText.SetActive(true);
            NextLevelButton.SetActive(true);
            TitleText.SetText(I18NManager.Instance.I18NGetText(I18NKey.Text_Battle_Success));
            task.SetResult(1);
        }
        private void DoBattleFail()
        {
            //await ETTask.CompletedTask;
            isBattled = true;
            currStepState = CurrStepState.DealFail;
            NoticText.SetText(I18NManager.Instance.I18NGetText(Config.DealFail));
            WinImage.SetEnabled(false);
            Back2Image.SetActive(false);
            Back3Image.SetActive(false);
            Buttons.SetActive(true);
            Mask.SetEnabled(false);
            // else
            // {
            //     isOver = true;
            //     NoticText.SetText(I18NManager.Instance.I18NGetText(Config.BattleFail));
            //     WinImage.SetEnabled(false);
            //     Back2Image.SetActive(false);
            //     Back3Image.SetActive(false);
            //     Buttons.SetActive(false);
            //     Mask.SetEnabled(true);
            //     BattleFailText.SetActive(true);
            // }
            task.SetResult(1);
        }
    }
}
