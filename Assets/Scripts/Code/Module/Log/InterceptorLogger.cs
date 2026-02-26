using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace TaoTie
{
    public class InterceptorLogger : ILog
    {
        private const int TraceLevel = 1;
        private const int DebugLevel = 2;
        private const int InfoLevel = 3;
        private const int WarningLevel = 4;
        
        public int CheckLv;
        public static string dir = Application.persistentDataPath + "/log/";
#if !UNITY_EDITOR
#if UNITY_WEBGL_TT
        private TTSDK.TTRealtimeLogManager logManager;
#elif UNITY_WEBGL_WeChat 
        private WeChatWASM.WXLogManager logManager;
        private string[] errorMsg = new string[1];
#elif UNITY_WEBGL_TAPTAP
        private TapTapMiniGame.TapLogManager logManager;
#endif
#endif
        public InterceptorLogger()
        {
            CheckLv = Define.LogLevel;
            CleanDirectory(dir,
                new List<string>()
                    {DateTime.Now.ToString("yyyy-MM-dd"), DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd")});
            WriteText("-------------------------华丽的分割线--------------------------------");
#if !UNITY_EDITOR
#if UNITY_WEBGL_TT
            logManager = TTSDK.TT.GetRealtimeLogManager(new TTSDK.GetRealtimeManagerParam());
#elif UNITY_WEBGL_WeChat
            logManager = WeChatWASM.WX.GetLogManager(new WeChatWASM.GetLogManagerOption());
#elif UNITY_WEBGL_TAPTAP
            logManager = TapTapMiniGame.Tap.GetLogManager(new TapTapMiniGame.GetLogManagerOption());
#endif
#endif
        }

        private static void CleanDirectory(string dir, List<string> ignoreList = null)
        {
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
                return;
            }
            List<string> ignorePaths = new List<string>();
            if (ignoreList != null)
            {
                ignorePaths = ignoreList.Select(fileName => Path.Combine(dir, fileName)).ToList();
            }

            foreach (string subFile in Directory.GetFiles(dir))
            {
                if (!ignorePaths.Contains(subFile))
                {
                    File.Delete(subFile);
                }
            }
        }
        
        private void WriteText(string log)
        {
            log += "\r\n";
            try
            {
                var logPath = dir + DateTime.Now.ToString("yyyy-MM-dd");
                File.AppendAllText(logPath,log);
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.LogError(ex);
            }
        }
        
        private bool CheckLogLevel(int level)
        {
            return CheckLv <= level;
        }

        public void Trace(string msg)
        {
            if (CheckLogLevel(TraceLevel)) UnityEngine.Debug.Log(msg);
            WriteText($"[Trace] {DateTime.Now} {msg}");
        }

        public void Debug(string msg)
        {
            if (CheckLogLevel(DebugLevel))UnityEngine.Debug.Log(msg);
            WriteText($"[Debug] {DateTime.Now} {msg}");
        }

        public void Info(string msg)
        {
            if (CheckLogLevel(InfoLevel))UnityEngine.Debug.Log(msg);
            WriteText($"[Info] {DateTime.Now} {msg}");
        }

        public void Warning(string msg)
        {
            if (CheckLogLevel(WarningLevel))UnityEngine.Debug.LogWarning(msg);
            WriteText($"[Warning] {DateTime.Now} {msg}");
        }

        public void Error(string msg)
        {
            UnityEngine.Debug.LogError(msg);
            WriteText($"[Error] {DateTime.Now} {msg}");
#if !UNITY_EDITOR
#if UNITY_WEBGL_TT
            logManager?.Error(msg);
#elif UNITY_WEBGL_WeChat
            errorMsg[0] = msg;
            logManager?.Log(errorMsg);
#elif UNITY_WEBGL_TAPTAP
            logManager?.Log(msg);
#endif
#endif
        }

        public void Error(Exception e)
        {
            UnityEngine.Debug.LogException(e);
            WriteText($"[Error] {DateTime.Now} {e}");
#if !UNITY_EDITOR
#if UNITY_WEBGL_TT
            logManager?.Error(e.ToString());
#elif UNITY_WEBGL_WeChat
            errorMsg[0] = e.ToString();
            logManager?.Log(errorMsg);
#elif UNITY_WEBGL_TAPTAP
            logManager?.Log(e.ToString());
#endif
#endif
        }

        public void Trace(string message, params object[] args)
        {
            if (CheckLogLevel(TraceLevel))UnityEngine.Debug.LogFormat(message, args);
            try
            {
                WriteText($"[Trace] {DateTime.Now} {string.Format(message, args)}");
            }
            catch
            {
            }
        }

        public void Warning(string message, params object[] args)
        {
            if (CheckLogLevel(WarningLevel)) UnityEngine.Debug.LogWarningFormat(message, args);
            try
            {
                WriteText($"[Warning] {DateTime.Now} {string.Format(message, args)}");
            }
            catch
            {
            }
        }

        public void Info(string message, params object[] args)
        {
            if (CheckLogLevel(InfoLevel)) UnityEngine.Debug.LogFormat(message, args);
            try
            {
                WriteText($"[Info] {DateTime.Now} {string.Format(message, args)}");
            }
            catch
            {
            }
        }

        public void Debug(string message, params object[] args)
        {
            if (CheckLogLevel(DebugLevel)) UnityEngine.Debug.LogFormat(message, args);
            try
            {
                WriteText($"[Debug] {DateTime.Now} {string.Format(message, args)}");
            }
            catch
            {
            }
        }

        public void Error(string message, params object[] args)
        {
            UnityEngine.Debug.LogErrorFormat(message, args);
            try
            {
                var text = string.Format(message, args);
                WriteText($"[Error] {DateTime.Now} {text}");
#if !UNITY_EDITOR
#if UNITY_WEBGL_TT
                logManager?.Error(text);
#elif UNITY_WEBGL_WeChat
                errorMsg[0] = text;
                logManager?.Log(errorMsg);
#elif UNITY_WEBGL_TAPTAP
                logManager?.Log(text);
#endif
#endif
            }
            catch
            {
            }
        }
    }
}