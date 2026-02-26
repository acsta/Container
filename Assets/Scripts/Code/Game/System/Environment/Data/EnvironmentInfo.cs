using System;
using UnityEngine;

namespace TaoTie
{
    public class EnvironmentInfo: IDisposable
    {
        private bool isDispose;
        public bool Changed;
        public bool IsBlender;
        public bool IsDayNight;

        public float Progress;

        public Color TintColor;
        public Color TintColor2;
        
        public Color LightColor;
        public float LightIntensity;
        public Vector3 LightDir;
        public bool UseDirLight;
        public LightShadows LightShadows;

        public float StarSpeed;
        public float NebulaSpeed;

        public static EnvironmentInfo Create(ConfigEnvironment config)
        {
            EnvironmentInfo res = ObjectPool.Instance.Fetch<EnvironmentInfo>();
            res.isDispose = false;
            

            res.UseDirLight = config.UseDirLight;
            res.TintColor = config.TintColor;
            res.LightColor = config.LightColor;
            res.LightIntensity = config.LightIntensity;
            res.LightDir = config.LightDir;
            res.LightShadows = config.LightShadows;
            res.StarSpeed = config.StarSpeed;
            res.NebulaSpeed = config.NebulaSpeed;
            return res;
        }

        public static EnvironmentInfo DeepClone(EnvironmentInfo other)
        {
            EnvironmentInfo res = ObjectPool.Instance.Fetch<EnvironmentInfo>();
            res.isDispose = false;
            res.Progress = other.Progress;
            
            
            res.TintColor = other.TintColor;
            res.TintColor2 = other.TintColor2;
            res.LightColor = other.LightColor;
            res.LightIntensity = other.LightIntensity;
            res.LightDir = other.LightDir;
            res.UseDirLight = other.UseDirLight;
            res.LightShadows = other.LightShadows;
            res.StarSpeed = other.StarSpeed;
            res.NebulaSpeed = other.NebulaSpeed;
            return res;
        }

        public void Lerp(EnvironmentInfo from, EnvironmentInfo to, float val)
        {
            Progress = val;
            

            TintColor = from.TintColor;
            TintColor2 = to.TintColor;
            UseDirLight = from.UseDirLight || to.UseDirLight;
            if (from.UseDirLight && to.UseDirLight)
            {
                LightColor = Color.Lerp(from.LightColor,to.LightColor,val);
                LightIntensity = Mathf.Lerp(from.LightIntensity,to.LightIntensity,val);
                LightShadows = to.LightShadows;
            }
            else if (from.UseDirLight)
            {
                LightColor = from.LightColor;
                LightIntensity = from.LightIntensity;
                LightShadows = from.LightShadows;
            }
            else if (to.UseDirLight)
            {
                LightColor = to.LightColor;
                LightIntensity = to.LightIntensity;
                LightShadows = to.LightShadows;
            }
            if (from.LightDir.x > 0 && to.LightDir.x < from.LightDir.x)
            {
                to.LightDir.x += 360;
                if (from.LightDir.x > 360 && to.LightDir.x > 360)
                {
                    from.LightDir.x -= 360;
                    to.LightDir.x -= 360;
                }
            }
            LightDir = Vector3.Lerp(from.LightDir,to.LightDir,val);
            StarSpeed = Mathf.Lerp(from.StarSpeed,to.StarSpeed,val);
            NebulaSpeed = Mathf.Lerp(from.NebulaSpeed,to.NebulaSpeed,val);
        }
        public void Lerp(ConfigEnvironment from, ConfigEnvironment to, float val)
        {
            Progress = val;
            
            TintColor = from.TintColor;
            TintColor2 = to.TintColor;
            UseDirLight = from.UseDirLight || to.UseDirLight;
            if (from.UseDirLight && to.UseDirLight)
            {
                LightColor = Color.Lerp(from.LightColor,to.LightColor,val);
                LightIntensity = Mathf.Lerp(from.LightIntensity,to.LightIntensity,val);
                LightShadows = to.LightShadows;
            }
            else if (from.UseDirLight)
            {
                LightColor = from.LightColor;
                LightIntensity = from.LightIntensity;
                LightShadows = from.LightShadows;
            }
            else if (to.UseDirLight)
            {
                LightColor = to.LightColor;
                LightIntensity = to.LightIntensity;
                LightShadows = to.LightShadows;
            }

            if (from.LightDir.x > 0 && to.LightDir.x < from.LightDir.x)
            {
                to.LightDir.x += 360;
                if (from.LightDir.x > 360 && to.LightDir.x > 360)
                {
                    from.LightDir.x -= 360;
                    to.LightDir.x -= 360;
                }
            }
            LightDir = Vector3.Lerp(from.LightDir,to.LightDir,val);
            StarSpeed = Mathf.Lerp(from.StarSpeed,to.StarSpeed,val);
            NebulaSpeed = Mathf.Lerp(from.NebulaSpeed,to.NebulaSpeed,val);
        }
        public void Dispose()
        {
            if (isDispose)
            {
                return;
            }
            Progress = default;

          
            IsDayNight = default;
            TintColor = default;
            TintColor2 = default;
            LightColor = default;
            LightIntensity = default;
            LightDir = default;
            UseDirLight = default;
            StarSpeed = 0;
            NebulaSpeed = 0;
            this.isDispose = true;
            ObjectPool.Instance.Recycle(this);
        }
    }
}