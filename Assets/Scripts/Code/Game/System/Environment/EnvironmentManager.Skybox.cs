using UnityEngine;

namespace TaoTie
{
    public partial class EnvironmentManager
    {
        private int StarSpeed = Shader.PropertyToID("_StarSpeed");
        private int NebulaSpeed = Shader.PropertyToID("_NebulaSpeed");
        private int DayCycleProgress = Shader.PropertyToID("_DayCycleProgress");
        private int DayLength = Shader.PropertyToID("_DayLength");
        private int NightLength = Shader.PropertyToID("_NightLength");
        private int CurrTime = Shader.PropertyToID("_CurrTime");

        private partial void ApplySkybox(EnvironmentInfo info)
        {
            Shader.SetGlobalFloat(StarSpeed, info.StarSpeed);
            Shader.SetGlobalFloat(NebulaSpeed, info.NebulaSpeed);
            
            if (info.IsBlender)
            {
                if (info.IsDayNight)
                {
                    Shader.SetGlobalFloat(DayCycleProgress, info.Progress);
                    Shader.SetGlobalFloat(DayLength, mNightTimeStart - mNoonTimeStart);
                    Shader.SetGlobalFloat(NightLength, mDayTimeCount - mNightTimeStart);
                    Shader.SetGlobalFloat(CurrTime, info.Progress * mDayTimeCount);
                }
                else
                {
                    Shader.SetGlobalFloat(DayCycleProgress, info.Progress);
                    Shader.SetGlobalFloat(DayLength, mNightTimeStart - mNoonTimeStart);
                    Shader.SetGlobalFloat(NightLength, mDayTimeCount - mNightTimeStart);
                    Shader.SetGlobalFloat(CurrTime, info.Progress * 1000);
                }
            }
            else
            {
                Shader.SetGlobalFloat(DayCycleProgress, 0.5f);
                Shader.SetGlobalFloat(DayLength, mNightTimeStart - mNoonTimeStart);
                Shader.SetGlobalFloat(NightLength, mDayTimeCount - mNightTimeStart);
                Shader.SetGlobalFloat(CurrTime, 500);
            }
        }
    }
}