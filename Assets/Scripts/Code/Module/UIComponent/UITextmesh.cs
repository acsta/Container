using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace TaoTie
{
    public class UITextmesh : UIBaseContainer,II18N
    {
        private TMPro.TMP_Text text;
        private I18NText i18nCompTouched;
        private I18NKey textKey;
        private object[] keyParams;

        public BigNumber lastNum = 0;
        private ETCancellationToken cancel;
        
        void ActivatingComponent()
        {
            if (this.text == null)
            {
                this.text = this.GetGameObject().GetComponent<TMPro.TMP_Text>();
                if (this.text == null)
                {
                    this.text = this.GetGameObject().AddComponent<TMPro.TMP_Text>();
                    Log.Info($"添加UI侧组件UIText时，物体{this.GetGameObject().name}上没有找到Text组件");
                }
                this.i18nCompTouched = this.GetGameObject().GetComponent<I18NText>();
            }
        }

        //当手动修改text的时候，需要将mono的i18textcomponent给禁用掉
        void DisableI18Component(bool enable = false)
        {
            this.ActivatingComponent();
            if (this.i18nCompTouched != null)
            {
                this.i18nCompTouched.enabled = enable;
                if (!enable)
                    Log.Warning($"组件{this.GetGameObject().name}, text在逻辑层进行了修改，所以应该去掉去预设里面的I18N组件，否则会被覆盖");
            }
        }

        public string GetText()
        {
            this.ActivatingComponent();
            return this.text.text;
        }

        public void SetText(string text)
        {
            this.DisableI18Component();
            this.textKey = default;
            this.text.text = text;
            lastNum = BigNumber.Zero;
        }
        public void SetI18NKey(I18NKey key)
        {
            if (key == default)
            {
                this.SetText("");
                return;
            }
            this.DisableI18Component();
            this.textKey = key;
            this.SetI18NText(null);
        }
        public void SetI18NKey(I18NKey key, params object[] paras)
        {
            if (key == default)
            {
                this.SetText("");
                return;
            }

            this.DisableI18Component();
            this.textKey = key;
            cancel?.Cancel();
            cancel = null;
            this.SetI18NText(paras);
            lastNum = BigNumber.Zero;
        }

        public void SetI18NText(params object[] paras)
        {
            if (textKey == default)
            {
                Log.Error("there is not key ");
            }
            else
            {
                this.DisableI18Component();
                this.keyParams = paras;
                if (I18NManager.Instance.I18NTryGetText(this.textKey, out var text) && paras != null)
                    text = string.Format(text, paras);
                this.text.text = text;
            }
        }

        public void OnLanguageChange()
        {
            this.ActivatingComponent();
            {
                if (textKey != default)
                {
                    if (I18NManager.Instance.I18NTryGetText(this.textKey, out var text) && this.keyParams != null)
                        text = string.Format(text, this.keyParams);
                    this.text.text = text;
                }
            }
        }

        public void SetTextColor(Color color)
        {
            this.ActivatingComponent();
            this.text.color = color;
        }
        public void SetTextColor(string colorstr)
        {
            this.ActivatingComponent();
            if(ColorUtility.TryParseHtmlString(colorstr, out var color))
            {
                this.text.color = color;
            }
        }
        
        public Color GetTextColor()
        {
            this.ActivatingComponent();
            return this.text.color;
        }
        public void SetTextWithColor(string text, string colorstr)
        {
            if (string.IsNullOrEmpty(colorstr))
                this.SetText(text);
            else
                this.SetText($"<color={colorstr}>{text}</color>");
        }
        
                
        public int GetCharacterCount()
        {
            ActivatingComponent();
            return text.CharacterCount;
        }

        public void SetMaxVisibleCharacters(int count)
        {
            ActivatingComponent();
            text.maxVisibleCharacters = count;
        }
        /// <summary>
        /// 获取最后一个字符右下角坐标
        /// </summary>
        /// <returns></returns>
        public Vector3 GetLastCharacterLocalPosition()
        {
            ActivatingComponent();
            if (text.m_textInfo.characterInfo != null && text.m_textInfo.characterInfo.Length > 0)
            {
                var info = text.m_textInfo.characterInfo[text.m_textInfo.characterCount - 1];
                return info.vertex_BR.position;
            }

            var rect = text.rectTransform.rect;
            return new Vector3(-rect.width / 2, -rect.height / 2, 0);
        }
        /// <summary>
        /// 获取指定字符右下角坐标
        /// </summary>
        /// <returns></returns>
        public Vector3 GetCharacterLocalPosition(int index)
        {
            ActivatingComponent();
            if (text.m_textInfo.characterInfo != null && text.m_textInfo.characterInfo.Length > index)
            {
                var info = text.m_textInfo.characterInfo[index];
                return info.vertex_BR.position;
            }

            var rect = text.rectTransform.rect;
            return new Vector3(-rect.width / 2, -rect.height / 2, 0);
        }
        
        public async ETTask DoI18NNum(BigNumber number, bool showFlag = true)
        {
            if (textKey == default)
            {
                Log.Error("there is not key ");
            }
            else
            {
                this.DisableI18Component();
                if (!I18NManager.Instance.I18NTryGetText(this.textKey, out var text)) return;
                var dis = number - lastNum;
                cancel?.Cancel();
                if(dis == BigNumber.Zero) return;
                var thisCancel = cancel = new ETCancellationToken();
                long startTime = TimerManager.Instance.GetTimeNow();
                var lastN = new BigNumber(lastNum);
                while (true)
                {
                    var timeNow = TimerManager.Instance.GetTimeNow();
                    var progress = Mathf.Clamp01((timeNow - startTime) / 500f);
                    this.lastNum = lastN + progress * dis;
                    var num = I18NManager.Instance.TranslateMoneyToStr(this.lastNum, true);
                    if (!showFlag)
                    {
                        num = num.Remove('-');
                    }
                    this.text.text = string.Format(text, num);
                    if (timeNow > startTime + 500)
                    {
                        this.lastNum = number;
                        break;
                    }

                    await TimerManager.Instance.WaitAsync(35, thisCancel);//30帧
                    if (thisCancel.IsCancel())
                    {
                        return;
                    }
                }
            }
        }
        /// <summary>
        /// 跳字动画
        /// </summary>
        /// <param name="number"></param>
        /// <param name="showFlag">是否显示负号</param>
        /// <param name="onFlagChange">当符号改变,动画开始和结束时也会调用一下</param>
        public async ETTask DoNum(BigNumber number, bool showFlag = true, Action<bool> onFlagChange = null)
        {
            this.DisableI18Component();
            var dis = number - lastNum;
            cancel?.Cancel();
            if (dis == BigNumber.Zero && number != BigNumber.Zero)
            {
                return;
            }
            var thisCancel = cancel = new ETCancellationToken();
            long startTime = TimerManager.Instance.GetTimeNow();
            var lastN = new BigNumber(lastNum);
            onFlagChange?.Invoke(lastNum > 0);
            while (true)
            {
                var timeNow = TimerManager.Instance.GetTimeNow();
                var progress = Mathf.Clamp01((timeNow - startTime) / 500f);
                var num = lastN + progress * dis;
                if (lastNum > 0 != num > 0) onFlagChange?.Invoke(num > 0);
                this.lastNum = num;
                if (showFlag)
                {
                    this.text.text = I18NManager.Instance.TranslateMoneyToStr(this.lastNum, true);
                }
                else
                {
                    this.text.text = I18NManager.Instance.TranslateMoneyToStr(!lastNum.Value.StartsWith("-") ? 
                        lastNum : lastNum.Value.Substring(1,lastNum.Value.Length-1), true);
                }
                if (timeNow > startTime + 500)
                {
                    this.lastNum = number;
                    break;
                }

                await TimerManager.Instance.WaitAsync(35, thisCancel);//30帧
                if (thisCancel.IsCancel())
                {
                    return;
                }
            }
            onFlagChange?.Invoke(number > 0);
        }
        public void SetNum(BigNumber number, bool showFlag = true)
        {
            this.DisableI18Component();
            cancel?.Cancel();
            if (showFlag)
            {
                this.text.text = I18NManager.Instance.TranslateMoneyToStr(number, true);
            }
            else
            {
                this.text.text = I18NManager.Instance.TranslateMoneyToStr(!number.Value.StartsWith("-") ? 
                    number : number.Value.Substring(1,number.Value.Length-1), true);
            }
            this.lastNum = number;
        }

        public void SetTextGray(bool isGray)
        {
            var uITextColorCtrl = TextColorCtrl.Get(GetGameObject());
            if (isGray)
            {
                uITextColorCtrl.SetTextColor(new Color(89 / 255f, 93 / 255f, 93 / 255f));
            }
            else
            {
                uITextColorCtrl.ClearTextColor();
            }
        }

        public int GetLineCount()
        {
            ActivatingComponent();
            return text.m_textInfo.lineCount;
        }
    }
}
