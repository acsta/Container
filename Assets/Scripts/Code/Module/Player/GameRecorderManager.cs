
namespace TaoTie
{
    public class GameRecorderManager: IManager
    {
        public static GameRecorderManager Instance;
#if UNITY_WEBGL_TT
        private TTSDK.TTGameRecorder recorder;
        private string videoPath;
#elif UNITY_WEBGL_WeChat
        private WeChatWASM.WXGameRecorder recorder;
#elif UNITY_WEBGL_KS
        private KSWASM.KSGameRecorder recorder;
#endif
        public void Init()
        {
            Instance = this;
            if (PerformanceManager.Instance.Level < PerformanceManager.DevicePerformanceLevel.Mid) return;
#if UNITY_WEBGL_TT && !UNITY_EDITOR
            recorder = TTSDK.TT.GetGameRecorder();
#elif UNITY_WEBGL_WeChat && !UNITY_EDITOR
            recorder = WeChatWASM.WX.GetGameRecorder();
#elif UNITY_WEBGL_KS && !UNITY_EDITOR
            recorder = KSWASM.KS.GetGameRecorder();
#endif
        }

        public void Destroy()
        {
            Instance = null;
        }

        public bool Support()
        {
#if UNITY_WEBGL_TT || UNITY_WEBGL_WeChat || UNITY_WEBGL_KS
            return recorder != null;
#else
            return false;
#endif
        }

        public async ETTask<bool> StartRecorder(int tryCount = 3)
        {
#if UNITY_WEBGL_TT
            if (recorder == null) return false;
            videoPath = null;
            for (int i = 0; i < tryCount - 1; i++)
            {
                if(recorder.Start(true, 0)) return true;
                await TimerManager.Instance.WaitAsync(100);
            }
            return recorder.Start(true, 0);
#elif UNITY_WEBGL_WeChat
            if (recorder == null) return false;
            recorder.Start(new WeChatWASM.GameRecorderStartOption()
            {
                hookBgm = true,
            });
            await TimerManager.Instance.WaitAsync(100);
            return true;
#elif UNITY_WEBGL_KS
            if (recorder == null) return false;
            recorder.Start(new KSWASM.GameRecorderStartOption()
            {
                hookBgm = true,
            });
            await TimerManager.Instance.WaitAsync(100);
            return true;
#else
            await ETTask.CompletedTask;
            return false;
#endif
        }
        
        public bool PauseRecorder(bool pause)
        {
#if UNITY_WEBGL_TT
            if (recorder == null) return false;
            recorder.SetEnabled(!pause);
            return !recorder.GetEnabled();
#elif UNITY_WEBGL_WeChat
            if (recorder == null) return false;
            if (pause)
            {
                recorder.Pause();
            }
            else
            {
                recorder.Resume();
            }
            return !pause;
#elif UNITY_WEBGL_KS
            if (recorder == null) return false;
            if (pause)
            {
                recorder.Pause();
            }
            else
            {
                recorder.Resume();
            }
            return !pause;
#else
            return false;
#endif

        }
        public async ETTask<bool> StopRecorder()
        {
#if UNITY_WEBGL_TT
            if (recorder == null) return false;
            for (int i = 0; i < 3; i++)
            {
                ETTask<bool> res = ETTask<bool>.Create();
                if (recorder.Stop((path) =>
                    {
                        videoPath = path;
                        res.SetResult(true);
                    }, (code, msg) =>
                    {
                        Log.Error(msg);
                        res.SetResult(false);
                    }))
                {
                    return await res;
                }
                await TimerManager.Instance.WaitAsync(100);
            }

            return false;
#elif UNITY_WEBGL_WeChat
            if (recorder == null) return false;
            recorder.Stop();
            await TimerManager.Instance.WaitAsync(100);
            return true;
#elif UNITY_WEBGL_KS
            if (recorder == null) return false;
            recorder.Stop();
            await TimerManager.Instance.WaitAsync(100);
            return true;
#else
            await ETTask.CompletedTask;
            return false;
#endif

        }

        public bool CanShareVideo()
        {
#if UNITY_WEBGL_TT
            if (recorder == null) return false;
            return recorder.GetVideoRecordState() == TTSDK.TTGameRecorder.VideoRecordState.RECORD_COMPLETED
                || recorder.GetVideoRecordState() == TTSDK.TTGameRecorder.VideoRecordState.RECORD_STOPED;
#elif UNITY_WEBGL_WeChat
            if (recorder == null) return false;
            return true;
#elif UNITY_WEBGL_KS
            if (recorder == null) return false;
            return true;
#else
            return false;
#endif
        }

        public void PublishVideo()
        {
#if UNITY_WEBGL_TT
            if (recorder == null || recorder.GetVideoRecordState() != TTSDK.TTGameRecorder.VideoRecordState.RECORD_COMPLETED
                && recorder.GetVideoRecordState() != TTSDK.TTGameRecorder.VideoRecordState.RECORD_STOPED) 
                return;
            recorder.ShareVideo((success) =>
            {
                Log.Info("Share Success");
            }, (fail) =>
            {
                Log.Error(fail);
            }, () =>
            {
                Log.Error("Cancel");
            });
#elif UNITY_WEBGL_WeChat
            if (recorder == null) return;
            WeChatWASM.WX.OperateGameRecorderVideo(new WeChatWASM.OperateGameRecorderVideoOption());
#elif UNITY_WEBGL_KS
            if (recorder == null) return;
            recorder.PublishVideo(new KSWASM.GameRecorderPublishVideoOption()
            {
                callback = (res) =>
                {
                    if (!string.IsNullOrEmpty(res.msg))
                    {
                        Log.Error(res.msg);
                    }
                },
            });
#endif
        }
    }
}