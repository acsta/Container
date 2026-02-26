using UnityEngine;

namespace TaoTie
{
    public partial class CameraManager:IManager
    {
        #region IManager
        public static CameraManager Instance { get; private set; }
        public void Init()
        {
            Instance = this;
        }

        public void Destroy()
        {
            Instance = null;
        }

        #endregion
        
        private bool isShake;
        public async ETTask Shake(float force = 1, int during = 1000, float hz = 50)
        {
            if(isShake) return;
            if(MainCamera()== null) return;
            var startTime = TimerManager.Instance.GetTimeNow();
            force *= 10;
            var startPos = MainCamera().transform.localPosition;
            isShake = true;
            hz = Mathf.PI * hz / 500;
            while (MainCamera() != null)
            {
                await TimerManager.Instance.WaitAsync(1);
                var timeNow = TimerManager.Instance.GetTimeNow();
                var deltaTime = timeNow - startTime;
                var addon = Mathf.Max(1, deltaTime * 100f / during);
                MainCamera().transform.localPosition = startPos + force * Vector3.up * Mathf.Sin(deltaTime * hz) / addon;

                if (deltaTime >= during)
                {
                    MainCamera().transform.localPosition = startPos;
                    break;
                }
            }

            isShake = false;
        }
        
    }
}