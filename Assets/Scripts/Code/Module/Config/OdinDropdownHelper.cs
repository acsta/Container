#if UNITY_EDITOR

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using Sirenix.Utilities;
using UnityEditor;
using UnityEngine;

namespace TaoTie
{
    public static class OdinDropdownHelper
    {
        public class StringComparer :IComparer<ValueDropdownItem<string>>
        {
            public int Compare(ValueDropdownItem<string> x, ValueDropdownItem<string> y)
            {
                return string.Compare(x.Text??x.Value,y.Text??y.Value);
            }
        }

        public static StringComparer DefaultStringComparer = new StringComparer();
        
        #region AI

        /// <summary>
        /// 过滤类型
        /// </summary>
        /// <returns></returns>
        public static IEnumerable GetAIDecisionInterface()
        {
            var methods = typeof(AIDecisionInterface).GetMethods();
            ValueDropdownList<string> list = new ValueDropdownList<string>();
            if (methods.Length > 0)
            {
                for (int i = 0; i < methods.Length; i++)
                {
                    if (!methods[i].IsStatic)
                    {
                        continue;
                    }

                    var attrs = methods[i].GetCustomAttributes(TypeInfo<LabelTextAttribute>.Type,false);
                    string val = methods[i].Name;
                    string text;
                    if (attrs.Length > 0)
                    {
                        text = $"{(attrs[0] as LabelTextAttribute).Text}({val})";
                       
                        if(methods[i].GetCustomAttribute(typeof(TooltipAttribute)) is TooltipAttribute toolTip)
                        {
                            text += "   " + toolTip.tooltip;
                        }
                    }
                    else
                    {
                        text = val;
                    }
                    list.Add(text, val);
                }
               
            }
            list.Sort(DefaultStringComparer);
            return list;
        }

        /// <summary>
        /// 表情
        /// </summary>
        /// <returns></returns>
        public static IEnumerable GetEmoji()
        {
            ValueDropdownList<string> list = new ValueDropdownList<string>();
            list.Add("无", "");
            list.Add("0", "0");
            list.Add("1", "1");
            list.Add("2", "2");
            list.Add("3", "3");
            list.Add("4", "4");
            list.Add("5", "5");
            list.Add("6", "6");
            return list;
        }
        #endregion
        
        /// <summary>
        /// 数值类型
        /// </summary>
        /// <returns></returns>
        public static IEnumerable GetNumericFinalTypeId()
        {
            var fields = typeof(NumericType).GetFields();
            ValueDropdownList<int> list = new ValueDropdownList<int>();
            if (fields.Length > 0)
            {
                for (int i = 0; i < fields.Length; i++)
                {
                    if (!fields[i].IsStatic)
                    {
                        continue;
                    }
                    var val = (int) fields[i].GetValue(null);
                    if (val >= NumericType.Max) continue;
                    list.Add($"{fields[i].Name}({val})", val);
                }
            }
            return list;
        }
    }
}
#endif