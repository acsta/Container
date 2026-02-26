using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace TaoTie
{
#if UNITY_EDITOR
    using PlayerPrefs = UnityEngine.PlayerPrefs;
#endif
    public class CacheManager: IManager
    {
        public static CacheManager Instance;

        private Dictionary<string, object> cacheObj;

        public void Init()
        {
            cacheObj = new Dictionary<string, object>();
            Instance = this;
        }

        public void Destroy()
        {
            Save();
            Instance = null;
        }


        public string GetString(string key, string defaultValue = null)
        {
#if UNITY_WEBGL_QG
            return QGMiniGame.QG.StorageGetStringSync(key, defaultValue);
#else
            return PlayerPrefs.GetString(key, defaultValue);
#endif
        }
        
        public long GetLong(string key, long defaultValue = 0)
        {
#if UNITY_WEBGL_QG
            var text = QGMiniGame.QG.StorageGetStringSync(key);
#else
            var text = PlayerPrefs.GetString(key);
#endif
            if (!string.IsNullOrWhiteSpace(text) && long.TryParse(text, out var res))
            {
                return res;
            }

            return defaultValue;
        }
        
        public int GetInt(string key, int defaultValue = 0)
        {
#if UNITY_WEBGL_QG
            return QGMiniGame.QG.StorageGetIntSync(key, defaultValue);
#else
            return PlayerPrefs.GetInt(key, defaultValue);
#endif
        }
        
        public T GetValue<T>(string key) where T: class
        {
            if (cacheObj.TryGetValue(key, out var data))
            {
                return data as T;
            }
#if UNITY_WEBGL_QG
            var jStr = QGMiniGame.QG.StorageGetStringSync(key, null);
#else
            var jStr = PlayerPrefs.GetString(key, null);
#endif
            if (jStr == null) return null;
            var res = JsonHelper.FromJson<T>(jStr);
            cacheObj[key] = res;
            return res;
        }
        
        public void SetString(string key, string value)
        {
#if UNITY_WEBGL_QG
            QGMiniGame.QG.StorageSetStringSync(key, value);
#else
            PlayerPrefs.SetString(key, value);
#endif
        }
        
        public void SetLong(string key, long value)
        {
#if UNITY_WEBGL_QG
            QGMiniGame.QG.StorageSetStringSync(key, value.ToString());
#else
            PlayerPrefs.SetString(key, value.ToString());
#endif
        }
        
        public void SetInt(string key, int value)
        {
#if UNITY_WEBGL_QG
            QGMiniGame.QG.StorageSetIntSync(key, value);
#else
            PlayerPrefs.SetInt(key, value);
#endif
        }
        
        public void SetValue<T>(string key, T value) where T: class
        {
            cacheObj[key] = value;
            var jStr = JsonHelper.ToJson(value);
#if UNITY_WEBGL_QG
            QGMiniGame.QG.StorageSetStringSync(key, jStr);
#else
            PlayerPrefs.SetString(key, jStr);
#endif
        }

        public void Save()
        {
#if UNITY_WEBGL_QG
            //do nothing
#else
            PlayerPrefs.Save();
#endif
        }

        public void DeleteKey(string key)
        {
            if (cacheObj.ContainsKey(key))
            {
                cacheObj.Remove(key);
            }
#if UNITY_WEBGL_QG
            QGMiniGame.QG.StorageDeleteKeySync(key);
#else
            PlayerPrefs.DeleteKey(key);
#endif
        }
    }
}