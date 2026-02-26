using System;
using System.Collections.Generic;
using UnityEngine;

namespace TaoTie
{
    public partial class GlobalConfigCategory
    {
        private Dictionary<string, GlobalConfig> maps;

        public override void AfterEndInit()
        {
            base.AfterEndInit();
            maps = new Dictionary<string, GlobalConfig>();
            for (int i = 0; i < list.Count; i++)
            {
                maps.Add(list[i].Key, list[i]);
            }
        }

        public float GetFloat(string key, float defaultValue = 0)
        {
            if (maps.TryGetValue(key, out var data) && float.TryParse(data.Value, out float value))
            {
                return value;
            }

            return defaultValue;
        }


        public int GetInt(string key, int defaultValue = 0)
        {
            if (maps.TryGetValue(key, out var data) && int.TryParse(data.Value, out int value))
            {
                return value;
            }

            return defaultValue;
        }

        public long GetLong(string key, long defaultValue = 0)
        {
            if (maps.TryGetValue(key, out var data) && long.TryParse(data.Value, out long value))
            {
                return value;
            }

            return defaultValue;
        }

        public string GetString(string key, string defaultValue = null)
        {
            if (maps.TryGetValue(key, out var data))
            {
                return data.Value;
            }

            return defaultValue;
        }

        public bool TryGetFloat(string key, out float value)
        {
            if (maps.TryGetValue(key, out var data) && float.TryParse(data.Value, out value))
            {
                return true;
            }

            value = 0;
            return false;
        }


        public bool TryGetInt(string key, out int value)
        {
            if (maps.TryGetValue(key, out var data) && int.TryParse(data.Value, out value))
            {
                return true;
            }

            value = 0;
            return false;
        }

        public bool TryGetLong(string key, out long value)
        {
            if (maps.TryGetValue(key, out var data) && long.TryParse(data.Value, out value))
            {
                return true;
            }

            value = 0;
            return false;
        }

        public bool TryGetString(string key, out string value)
        {
            if (maps.TryGetValue(key, out var data))
            {
                value = data.Value;
                return true;
            }

            value = null;
            return false;
        }

        public bool TryGetColor(string key, out Color value)
        {
            if (maps.TryGetValue(key, out var data) && ColorUtility.TryParseHtmlString(data.Value, out value))
            {
                return true;
            }

            value = Color.white;
            return false;
        }

        public bool TryGetArray<T>(string key, out T[] value)
        {
            if (maps.TryGetValue(key, out var data))
            {
                if (typeof(T) == typeof(string))
                {
                    var vs = data.Value.Split(",");
                    value = vs as T[];
                }
                else
                {
                    JsonHelper.TryFromJson("[" + data.Value + "]", out value);
                }

                return true;
            }

            value = Array.Empty<T>();
            return false;
        }
    }
}