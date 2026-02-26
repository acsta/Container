using System.Collections.Generic;
using UnityEngine;

namespace TaoTie
{
    public class APIManager: IManager
    {
        const string GAME = "p4";
        public static APIManager Instance;

        private LoginPlatform platform;
        private string baseurl;
        public void Init()
        {
            Instance = this;
            if (Define.Debug)
            {
                baseurl = "http://192.168.0.6:29527/";
            }
            else
            {
                baseurl = "https://wgsh.hxwgame.cn/dish1/";
            }
        }
        
        public void Destroy()
        {
            Instance = null;
        }
        private async ETTask<bool> HttpGet(string url, Dictionary<string,string> param, int tryCount = 3, bool retryNotice = false)
        {
            param["game"] = GAME;
            Log.Info($"http <color=green>get request</color>: {url} \r\n{JsonHelper.ToJson(param)}");
            var res = await HttpManager.Instance.HttpGetResult<HttpResult>(url, null, param);
            Log.Info($"http <color=red>get result</color>: {url} \r\n{JsonHelper.ToJson(res)}");
            if (res!=null && res.code == 0 && res.status)
            {
                return true;
            }

            if (tryCount > 0)
            {
                tryCount--;
                return await HttpGet(url, param, tryCount, retryNotice);
            }

            if (retryNotice)
            {
                ETTask<bool> task = ETTask<bool>.Create(true);
                string content = res?.msg;
                if (string.IsNullOrEmpty(content))
                {
                    content = I18NManager.Instance.I18NGetText(I18NKey.Net_Error);
                }
                await UIManager.Instance.OpenBox<UIMsgBoxWin,MsgBoxPara>(UIMsgBoxWin.PrefabPath, new MsgBoxPara()
                {
                    Content = content + (res != null?$"({res.code})":""),
                    CancelText = I18NManager.Instance.I18NGetText(I18NKey.Btn_Exit),
                    ConfirmText = I18NManager.Instance.I18NGetText(I18NKey.Global_Btn_Confirm),
                    CancelCallback = (win) =>
                    {
                        UIManager.Instance.CloseBox(win).Coroutine();
                        BridgeHelper.Quit();
                        task.SetResult(false);
                    },
                    ConfirmCallback = (win) =>
                    {
                        task.SetResult(true);
                        UIManager.Instance.CloseBox(win).Coroutine();
                    }
                });
                if (await task)
                {
                    return await HttpGet(url, param, 3, retryNotice);
                }
            }

            if (res == null)
            {
                Log.Error("res == null");
            }
            else if(!string.IsNullOrEmpty(res.msg))
            {
                Log.Error(res.msg);
                UIManager.Instance.OpenBox<UIToast, string>(UIToast.PrefabPath, res.msg).Coroutine();
            }
            return false;
        }
        private async ETTask<T> HttpGet<T>(string url, Dictionary<string,string> param, int tryCount = 3, bool retryNotice = false,int logResDetails = 3)
        {
            param["game"] = GAME;
            Log.Info($"http <color=green>get request</color>: {url} \r\n{((logResDetails & 2) != 0 ?JsonHelper.ToJson(param):"hide details")}");
            var res = await HttpManager.Instance.HttpGetResult<HttpResult<T>>(url, null, param);
            Log.Info($"http <color=red>get result</color>: {url} \r\n{((logResDetails & 1) != 0 ?JsonHelper.ToJson(res):"hide details")}");
            if (res!=null && res.code == 0 && res.status)
            {
                return res.data;
            }

            if (tryCount > 0)
            {
                tryCount--;
                return await HttpGet<T>(url, param, tryCount, retryNotice);
            }

            if (retryNotice)
            {
                ETTask<bool> task = ETTask<bool>.Create(true);
                string content = res?.msg;
                if (string.IsNullOrEmpty(content))
                {
                    content = I18NManager.Instance.I18NGetText(I18NKey.Net_Error);
                }
                await UIManager.Instance.OpenBox<UIMsgBoxWin,MsgBoxPara>(UIMsgBoxWin.PrefabPath, new MsgBoxPara()
                {
                    Content = content + (res != null?$"({res.code})":""),
                    CancelText = I18NManager.Instance.I18NGetText(I18NKey.Btn_Exit),
                    ConfirmText = I18NManager.Instance.I18NGetText(I18NKey.Global_Btn_Confirm),
                    CancelCallback = (win) =>
                    {
                        UIManager.Instance.CloseBox(win).Coroutine();
                        BridgeHelper.Quit();
                        task.SetResult(false);
                    },
                    ConfirmCallback = (win) =>
                    {
                        task.SetResult(true);
                        UIManager.Instance.CloseBox(win).Coroutine();
                    }
                });
                if (await task)
                {
                    return await HttpGet<T>(url, param, 3, retryNotice);
                }
            }

            if (res == null)
            {
                Log.Error("res == null");
            }
            else if(!string.IsNullOrEmpty(res.msg))
            {
                Log.Error(res.msg);
                UIManager.Instance.OpenBox<UIToast, string>(UIToast.PrefabPath, res.msg).Coroutine();
            }
            return default;
        }
        
        private async ETTask<bool> HttpPost(string url, Dictionary<string,object> param, int tryCount = 3, bool retryNotice = false)
        {
            param["game"] = GAME;
            Log.Info($"http <color=green>post request</color>: {url} \r\n{JsonHelper.ToJson(param)}");
            var res = await HttpManager.Instance.HttpPostResult<HttpResult>(url, null, param);
            Log.Info($"http <color=red>post result</color>: {url} \r\n{JsonHelper.ToJson(res)}");
            if (res!=null && res.code == 0 && res.status)
            {
                return true;
            }

            if (tryCount > 0)
            {
                tryCount--;
                return await HttpPost(url, param, tryCount, retryNotice);
            }

            if (retryNotice)
            {
                ETTask<bool> task = ETTask<bool>.Create(true);
                string content = res?.msg;
                if (string.IsNullOrEmpty(content))
                {
                    content = I18NManager.Instance.I18NGetText(I18NKey.Net_Error);
                }
                await UIManager.Instance.OpenBox<UIMsgBoxWin,MsgBoxPara>(UIMsgBoxWin.PrefabPath, new MsgBoxPara()
                {
                    Content = content + (res != null?$"({res.code})":""),
                    CancelText = I18NManager.Instance.I18NGetText(I18NKey.Btn_Exit),
                    ConfirmText = I18NManager.Instance.I18NGetText(I18NKey.Global_Btn_Confirm),
                    CancelCallback = (win) =>
                    {
                        UIManager.Instance.CloseBox(win).Coroutine();
                        BridgeHelper.Quit();
                        task.SetResult(false);
                    },
                    ConfirmCallback = (win) =>
                    {
                        task.SetResult(true);
                        UIManager.Instance.CloseBox(win).Coroutine();
                    }
                });
                if (await task)
                {
                    return await HttpPost(url, param, 3, retryNotice);
                }
            }

            if (res == null)
            {
                Log.Error("res == null");
            }
            else if(!string.IsNullOrEmpty(res.msg))
            {
                Log.Error(res.msg);
                UIManager.Instance.OpenBox<UIToast, string>(UIToast.PrefabPath, res.msg).Coroutine();
            }
            return false;
        }
        
        private async ETTask<T> HttpPost<T>(string url, Dictionary<string,object> param, int tryCount = 3, bool retryNotice = false,int logResDetails = 3)
        {
            param["game"] = GAME;
            Log.Info(
                $"http <color=green>post request</color>: {url} \r\n{((logResDetails & 2) != 0 ? JsonHelper.ToJson(param) : "hide details")}");
            var res = await HttpManager.Instance.HttpPostResult<HttpResult<T>>(url, null, param);
            Log.Info(
                $"http <color=red>post result</color>: {url} \r\n{((logResDetails & 1) != 0 ? JsonHelper.ToJson(res) : "hide details")}");
            if (res!=null && res.code == 0 && res.status)
            {
                return res.data;
            }

            if (tryCount > 0)
            {
                tryCount--;
                return await HttpPost<T>(url, param, tryCount, retryNotice);
            }

            if (retryNotice)
            {
                ETTask<bool> task = ETTask<bool>.Create(true);
                string content = res?.msg;
                if (string.IsNullOrEmpty(content))
                {
                    content = I18NManager.Instance.I18NGetText(I18NKey.Net_Error);
                }
                await UIManager.Instance.OpenBox<UIMsgBoxWin,MsgBoxPara>(UIMsgBoxWin.PrefabPath, new MsgBoxPara()
                {
                    Content = content + (res != null?$"({res.code})":""),
                    CancelText = I18NManager.Instance.I18NGetText(I18NKey.Btn_Exit),
                    ConfirmText = I18NManager.Instance.I18NGetText(I18NKey.Global_Btn_Confirm),
                    CancelCallback = (win) =>
                    {
                        UIManager.Instance.CloseBox(win).Coroutine();
                        BridgeHelper.Quit();
                        task.SetResult(false);
                    },
                    ConfirmCallback = (win) =>
                    {
                        task.SetResult(true);
                        UIManager.Instance.CloseBox(win).Coroutine();
                    }
                });
                if (await task)
                {
                    return await HttpPost<T>(url, param, 3, retryNotice);
                }
            }

            if (res == null)
            {
                Log.Error("res == null");
            }
            else if(!string.IsNullOrEmpty(res.msg))
            {
                Log.Error(res.msg);
                UIManager.Instance.OpenBox<UIToast, string>(UIToast.PrefabPath, res.msg).Coroutine();
            }
            return default;
        }

        public async ETTask<LoginResult> MiniGameLogin(LoginPlatform platform, string token)
        {
            string url = baseurl + "MiniGameLogin";
            this.platform = platform;
            using (DictionaryComponent<string, object> param = DictionaryComponent<string, object>.Create())
            {
                param["platform"] = platform;
                param["token"] = token;
                return await HttpPost<LoginResult>(url, param);
            }
            
        }
        

        public async ETTask<bool> SaveData(int uid,PlayerData data)
        {
            string url = baseurl + "MiniGameSaveData";
            using (DictionaryComponent<string, object> param = DictionaryComponent<string, object>.Create())
            {
                param["uid"] = uid.ToString();
                param["data"] = data;
                return await HttpPost(url, param);
            }
        }

        public async ETTask<RankList> GetRankInfo(int uid)
        {
            string url = baseurl + "MiniGameGetRank";
            using (DictionaryComponent<string, object> param = DictionaryComponent<string, object>.Create())
            {
                param["platform"] = platform;
                param["uid"] = uid.ToString();
                return await HttpPost<RankList>(url, param);
            }
        }
    }
}