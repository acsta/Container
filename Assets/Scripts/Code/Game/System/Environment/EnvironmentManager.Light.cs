using UnityEngine;

namespace TaoTie
{
    public partial class EnvironmentManager
    {
        private Light dirLight;
        public Transform SceneLight;
        private partial void ApplyLight(EnvironmentInfo info)
        {
            dirLight.enabled = info.UseDirLight;
            if (info.UseDirLight)
            {
                dirLight.color = info.LightColor;
                dirLight.transform.eulerAngles = info.LightDir;
                dirLight.intensity = info.LightIntensity;
                dirLight.shadows = info.LightShadows;
            }
            else if(SceneLight != null)
            {
                SceneLight.eulerAngles = info.LightDir;
            }
        }
    }
}