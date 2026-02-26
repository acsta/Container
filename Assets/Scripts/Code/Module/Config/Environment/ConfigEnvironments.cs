using Nino.Core;
using Sirenix.OdinInspector;
using UnityEngine;

namespace TaoTie
{
    [NinoType(false)]
    public partial class ConfigEnvironments
    {
        [NinoMember(1)]
        public ConfigEnvironment DefaultEnvironment;
        [NinoMember(2)]
        public ConfigEnvironment[] Environments;
        [NinoMember(3)] [HideReferenceObjectPicker]
        public ConfigBlender DefaultBlend = new ConfigBlender();
        
        [NinoMember(4)][BoxGroup("SkyDayTex")]
        public string SkyDayTexPath;
        [NinoMember(5)][BoxGroup("SkyNightTex")]
        public string SkyNightTexPath;
        [NinoMember(6)][BoxGroup("SkySunriseTex")]
        public string SkySunriseTexPath;
        [NinoMember(7)][BoxGroup("SkySunsetTex")]
        public string SkySunsetTexPath;
#if UNITY_EDITOR
        [OnValueChanged(nameof(UpdateSkyDayTexPath))][BoxGroup("SkyDayTex")]
        public Texture SkyDayTex;

        private void UpdateSkyDayTexPath()
        {
            if (SkyDayTex == null)
            {
                SkyDayTexPath = null;
                return;
            }

            var path = UnityEditor.AssetDatabase.GetAssetPath(SkyDayTex);
            if (path.StartsWith("Assets/AssetsPackage/"))
            {
                SkyDayTexPath = path.Replace("Assets/AssetsPackage/","");
            }
            else
            {
                SkyDayTexPath = null;
            }
        }
        [Button("预览SkyDayTex")][BoxGroup("SkyDayTex")]
        private void PreviewSkyDayTex()
        {
            if (!string.IsNullOrEmpty(SkyDayTexPath))
            {
                SkyDayTex = UnityEditor.AssetDatabase.LoadAssetAtPath<Texture>("Assets/AssetsPackage/" + SkyDayTexPath);
                return;
            }
            SkyDayTex = null;
        }
        
        [OnValueChanged(nameof(UpdateSkyNightTexPath))][BoxGroup("SkyNightTex")]
        public Texture SkyNightTex;

        private void UpdateSkyNightTexPath()
        {
            if (SkyNightTex == null)
            {
                SkyNightTexPath = null;
                return;
            }

            var path = UnityEditor.AssetDatabase.GetAssetPath(SkyNightTex);
            if (path.StartsWith("Assets/AssetsPackage/"))
            {
                SkyNightTexPath = path.Replace("Assets/AssetsPackage/","");
            }
            else
            {
                SkyNightTexPath = null;
            }
        }
        [Button("预览SkyNightTex")][BoxGroup("SkyNightTex")]
        private void PreviewSkyNightTex()
        {
            if (!string.IsNullOrEmpty(SkyNightTexPath))
            {
                SkyNightTex = UnityEditor.AssetDatabase.LoadAssetAtPath<Texture>("Assets/AssetsPackage/" + SkyNightTexPath);
                return;
            }
            SkyNightTex = null;
        }
        
        [OnValueChanged(nameof(UpdateSkySunriseTexPath))][BoxGroup("SkySunriseTex")]
        public Texture SkySunriseTex;

        private void UpdateSkySunriseTexPath()
        {
            if (SkySunriseTex == null)
            {
                SkySunriseTexPath = null;
                return;
            }

            var path = UnityEditor.AssetDatabase.GetAssetPath(SkySunriseTex);
            if (path.StartsWith("Assets/AssetsPackage/"))
            {
                SkySunriseTexPath = path.Replace("Assets/AssetsPackage/","");
            }
            else
            {
                SkySunriseTexPath = null;
            }
        }
        [Button("预览SkySunriseTex")][BoxGroup("SkySunriseTex")]
        private void PreviewSkySunriseTex()
        {
            if (!string.IsNullOrEmpty(SkySunriseTexPath))
            {
                SkySunriseTex = UnityEditor.AssetDatabase.LoadAssetAtPath<Texture>("Assets/AssetsPackage/" + SkySunriseTexPath);
                return;
            }
            SkySunriseTex = null;
        }
        
        [OnValueChanged(nameof(UpdateSkySunsetTexPath))][BoxGroup("SkySunsetTex")]
        public Texture SkySunsetTex;

        private void UpdateSkySunsetTexPath()
        {
            if (SkySunsetTex == null)
            {
                SkySunsetTexPath = null;
                return;
            }

            var path = UnityEditor.AssetDatabase.GetAssetPath(SkySunsetTex);
            if (path.StartsWith("Assets/AssetsPackage/"))
            {
                SkySunsetTexPath = path.Replace("Assets/AssetsPackage/","");
            }
            else
            {
                SkySunsetTexPath = null;
            }
        }
        [Button("预览SkySunsetTex")][BoxGroup("SkySunsetTex")]
        private void PreviewSkySunsetTex()
        {
            if (!string.IsNullOrEmpty(SkySunsetTexPath))
            {
                SkySunsetTex = UnityEditor.AssetDatabase.LoadAssetAtPath<Texture>("Assets/AssetsPackage/" + SkySunsetTexPath);
                return;
            }
            SkySunsetTex = null;
        }
#endif
    }
}