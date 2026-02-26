using System;
using System.Collections.Generic;
using UnityEngine;

namespace TaoTie
{
    public class I18NManager : IManager
    {
        public event Action OnLanguageChangeEvt;

        public static I18NManager Instance;
        //语言类型枚举

        public LangType CurLangType { get; private set; }
        private Dictionary<int, string> i18nTextKeyDic;
        private bool addFonts = false;

        private Dictionary<int, I18NKey> numTemp;
        #region override

        public void Init()
        {
            Instance = this;
            numTemp = new Dictionary<int, I18NKey>();
            I18NBridge.Instance.GetValueByKey = I18NGetText;
            var lang = CacheManager.Instance.GetInt(CacheKeys.CurLangType, -1);
            if (lang < 0)
            {
                this.CurLangType = LangType.Chinese;
            }
            else
            {
                this.CurLangType = (LangType) lang;
            }

            this.i18nTextKeyDic = new Dictionary<int, string>();
            InitAsync().Coroutine();
        }

        private async ETTask InitAsync()
        {
            var res = await ConfigManager.Instance.LoadOneConfig<I18NConfigCategory>(this.CurLangType.ToString());
            for (int i = 0; i < res.GetAllList().Count; i++)
            {
                var item = res.GetAllList()[i];
                this.i18nTextKeyDic.Add(item.Id, item.Value);
            }
        }

        public void Destroy()
        {
            numTemp = null;
            OnLanguageChangeEvt = null;
            Instance = null;
            this.i18nTextKeyDic.Clear();
            this.i18nTextKeyDic = null;
        }

        #endregion

        /// <summary>
        /// 取不到返回key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string I18NGetText(I18NKey key)
        {
            if (!this.i18nTextKeyDic.TryGetValue((int) key, out var result))
            {
                Log.Error("多语言key未添加！ " + key);
                result = key.ToString();
                return result;
            }

            return result;
        }

        /// <summary>
        /// 取不到返回key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string I18NGetText(string key)
        {
            if (!I18NKey.TryParse(key, out I18NKey i18nKey) ||
                !this.i18nTextKeyDic.TryGetValue((int) i18nKey, out var result))
            {
                Log.Error("多语言key未添加！ " + key);
                result = key;
                return result;
            }

            return result;
        }

        /// <summary>
        /// 根据key取多语言取不到返回key
        /// </summary>
        /// <param name="key"></param>
        /// <param name="paras"></param>
        /// <returns></returns>
        public string I18NGetParamText(I18NKey key, params object[] paras)
        {
            if (!this.i18nTextKeyDic.TryGetValue((int) key, out var value))
            {
                Log.Error("多语言key未添加！ " + key);
                return key.ToString();
            }

            if (paras != null)
                return string.Format(value, paras);
            else
                return value;
        }
        
        /// <summary>
        /// 根据key取多语言取不到返回key
        /// </summary>
        /// <param name="str"></param>
        /// <param name="paras"></param>
        /// <returns></returns>
        public string I18NGetParamText(string str, params object[] paras)
        {
            if (!Enum.TryParse(str,out I18NKey key) || !this.i18nTextKeyDic.TryGetValue((int)key, out var value))
            {
                Log.Error("多语言key未添加！ " + key);
                return key.ToString();
            }

            if (paras != null)
                return string.Format(value, paras);
            else
                return value;
        }

        /// <summary>
        /// 取配置多语言
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public string I18NGetText(II18NConfig config)
        {
            return config.GetI18NText(CurLangType);
        }

        /// <summary>
        /// 取配置多语言
        /// </summary>
        /// <param name="config"></param>
        /// <param name="pos"></param>
        /// <returns></returns>
        public string I18NGetText(II18NSwitchConfig config, int pos = 0)
        {
            return config.GetI18NText(CurLangType, pos);
        }

        /// <summary>
        /// 取不到返回key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool I18NTryGetText(I18NKey key, out string result)
        {
            if (!this.i18nTextKeyDic.TryGetValue((int) key, out result))
            {
                Log.Info("多语言key未添加！ " + key);
                result = key.ToString();
                return false;
            }

            return true;
        }

        /// <summary>
        /// 切换语言,外部接口
        /// </summary>
        /// <param name="langType"></param>
        public async ETTask SwitchLanguage(int langType)
        {
            numTemp.Clear();
            //修改当前语言
            CacheManager.Instance.SetInt(CacheKeys.CurLangType, langType);
            this.CurLangType = (LangType) langType;
            var res = await ConfigManager.Instance.LoadOneConfig<I18NConfigCategory>(this.CurLangType.ToString());
            this.i18nTextKeyDic.Clear();
            for (int i = 0; i < res.GetAllList().Count; i++)
            {
                var item = res.GetAllList()[i];
                this.i18nTextKeyDic.Add(item.Id, item.Value);
            }

            I18NBridge.Instance.OnLanguageChangeEvt?.Invoke();
            OnLanguageChangeEvt?.Invoke();
        }

        public void RegisterI18NEntity(II18N entity)
        {
            OnLanguageChangeEvt += entity.OnLanguageChange;
        }

        public void RemoveI18NEntity(II18N entity)
        {
            OnLanguageChangeEvt -= entity.OnLanguageChange;
        }


        #region 添加系统字体
        
        /// <summary>
        /// 需要就添加
        /// </summary>
        public async ETTask<bool> AddSystemFonts()
        {
            await ETTask.CompletedTask;
            if(addFonts) return false;
            addFonts = true;
#if UNITY_WEBGL_TT && !UNITY_EDITOR
            ETTask task = ETTask.Create(true);
            TTSDK.TT.GetSystemFont(font =>
            {
                if (font != null)
                {
                    TextMeshFontAssetManager.Instance.AddFontAssetByFont(font);
                    Log.Info("加载抖音系统字体成功");
                } 
                else 
                {
                    Log.Error("加载抖音系统字体失败");
                }
                task.SetResult();
            });
            await task;
#elif UNITY_WEBGL_WeChat && !UNITY_EDITOR
            ETTask task = ETTask.Create(true);
            string fallbackFont = "https://cdn.hxwgame.cn/FantasyHouse/fzcyjt.ttf";
            WeChatWASM.WX.GetWXFont(fallbackFont, (font) =>
            {
                TextMeshFontAssetManager.Instance.AddFontAssetByFont(font);
                Log.Info("加载微信系统字体成功");
                 task.SetResult();
            });
            await task;
#else
            var font = await ResourcesManager.Instance.LoadAsync<Font>("FontsAddon/fzcyjt.ttf");
            TextMeshFontAssetManager.Instance.AddFontAssetByFont(font);
#endif
            return true;
        }
        public void RemoveSystemFonts()
        {
            if (!addFonts) return;
            addFonts = false;
            TextMeshFontAssetManager.Instance.RemoveAllAddFont();
        }

        #endregion

        public enum PointType
        {
            None,
            One,
            Two,
        }
        public enum ApproximateType
        {
            Cell,
            Floor,
        }
        /// <summary>
        /// 转换缩写
        /// </summary>
        /// <param name="num">值</param>
        /// <param name="onePoint">保留小数类型</param>
        /// <param name="minUseInteger">未缩写时是否保留整数</param>
        /// <returns></returns>
        public string TranslateMoneyToStr(BigNumber num, bool minUseInteger = false, PointType onePoint = PointType.Two)
        {
            if (CurLangType == LangType.Chinese)
            {
                return GetTransNum(num, onePoint, 10000, "Cn", minUseInteger);
            }
            else
            {
                return GetTransNum(num, onePoint, 1000, "En", minUseInteger);
            }
        }

        private string GetTransNum(BigNumber num, PointType onePoint, int progress, string transFlag, bool minUseInteger)
        {
            if (num.IntegerLength < 25 && decimal.TryParse(num, out var d))
            {
                return GetTransNum(d, onePoint, progress, transFlag, minUseInteger);
            }

            int index = 0;
            bool hasFlag = num < 0;
            BigNumber numStr = num.Value;
            if (hasFlag)
            {
                numStr = -numStr;
            }
            if (numStr < progress)
            {
                if (minUseInteger || onePoint == PointType.None)
                {
                    if (hasFlag) return "-" + numStr.ToString(0);
                    return numStr.ToString(0);
                }
                if (onePoint == PointType.One)
                {
                    if(hasFlag) return "-"+numStr.ToString(1);
                    return numStr.ToString(1);
                }
                if(hasFlag) return "-"+numStr.ToString(2);
                return numStr.ToString(2);
            }
            
            I18NKey unitKey = I18NKey.Global_Unknow;
            while (numStr >= progress)
            {
                index++;
                if (numTemp.TryGetValue(index,out I18NKey key))
                {
                    unitKey = key;
                }
                else if(Enum.TryParse("Text_Common_Unit" + transFlag + index, out key))
                {
                    unitKey = key;
                    numTemp[index] = key;
                }
                else
                {
                    break;
                }

                numStr /= progress;
            }
            
            if (numStr > progress * 10)
            {
                if (CurLangType == LangType.Chinese)
                {
                    if (hasFlag) return "-" + I18NGetParamText(unitKey,
                            GetTransNum(numStr, onePoint, progress, transFlag, minUseInteger));
                    return I18NGetParamText(unitKey, GetTransNum(numStr, onePoint, progress, transFlag, minUseInteger));
                }

                if (hasFlag)
                    return "-" + I18NGetParamText(unitKey, numStr.Value[0] + "x10^" + (numStr.IntegerLength - 1));
                return I18NGetParamText(unitKey, numStr.Value[0] + "x10^" + (numStr.IntegerLength - 1));
            }
            if (onePoint == PointType.None)
            {
                if(hasFlag) return "-"+I18NGetParamText(unitKey, numStr.ToString(0));
                return I18NGetParamText(unitKey, numStr.ToString(0));
            }
            if (onePoint == PointType.One)
            {
                if(hasFlag) return "-"+I18NGetParamText(unitKey, numStr.ToString(1));
                return I18NGetParamText(unitKey, numStr.ToString(1));
            }
            if(hasFlag) return "-"+I18NGetParamText(unitKey, numStr.ToString(2));
            return I18NGetParamText(unitKey, numStr.ToString(2));
        }

        private string GetTransNum(decimal num, PointType onePoint, int progress, string transFlag, bool minUseInteger)
        {
            int index = 0;
            bool hasFlag = num < 0;
            decimal numStr = num;
            if (hasFlag)
            {
                numStr = -numStr;
            }
            if (numStr < progress)
            {
                if (minUseInteger || onePoint == PointType.None)
                {
                    if(hasFlag) return (-numStr).ToString("0");
                    return numStr.ToString("0");
                }
                if (onePoint == PointType.One)
                {
                    BigNumber one = numStr;
                    if(hasFlag) return "-"+one.ToString(1);
                    return one.ToString(1);
                }
                BigNumber two = numStr;
                if(hasFlag) return "-"+two.ToString(2);
                return two.ToString(2);
            }
            
            I18NKey unitKey = I18NKey.Global_Unknow;
            while (numStr >= progress)
            {
                index++;
                if (numTemp.TryGetValue(index,out I18NKey key))
                {
                    unitKey = key;
                }
                else if(Enum.TryParse("Text_Common_Unit" + transFlag + index, out key))
                {
                    unitKey = key;
                    numTemp[index] = key;
                }
                else
                {
                    break;
                }

                numStr /= progress;
            }
            
            if (onePoint == PointType.None)
            {
                if(hasFlag) return I18NGetParamText(unitKey, (-numStr).ToString("0"));
                return I18NGetParamText(unitKey, numStr.ToString("0"));
            }
            if (onePoint == PointType.One)
            {
                BigNumber one = numStr;
                if(hasFlag) return I18NGetParamText(unitKey, "-"+one.ToString(1));
                return I18NGetParamText(unitKey, one.ToString(1));
            }
            BigNumber res = numStr;
            if(hasFlag) return I18NGetParamText(unitKey, "-"+res.ToString(2));
            return I18NGetParamText(unitKey, res.ToString(2));
        }
        /// <summary>
        /// 转换缩写并取整
        /// </summary>
        /// <param name="num"></param>
        /// <param name="approximateType"></param>
        /// <returns></returns>
        public string ApproximateMoneyToStr(BigNumber num, ApproximateType approximateType)
        {
            if (CurLangType == LangType.Chinese)
            {
                return GetAppNum(num,  10000, "Cn", approximateType);
            }
            else
            {
                return GetAppNum(num,  1000, "En", approximateType);
            }
        }
        
        private string GetAppNum(BigNumber num, int progress, string transFlag, ApproximateType approximateType)
        {
            int index = 0;
            var numStr = num;
            if (num < 10000)
            {
                return numStr.ToLString(2, approximateType == ApproximateType.Cell);
            }
            
            I18NKey unitKey = I18NKey.Global_Unknow;
            while (numStr >= progress)
            {
                index++;
                if (numTemp.TryGetValue(index,out I18NKey key))
                {
                    unitKey = key;
                }
                else if(Enum.TryParse("Text_Common_Unit" + transFlag + index, out key))
                {
                    unitKey = key;
                    numTemp[index] = key;
                }
                else
                {
                    break;
                }

                numStr /= progress;
            }
            
            return I18NGetParamText(unitKey, numStr.ToLString(3,approximateType == ApproximateType.Cell));
        }
    }
}