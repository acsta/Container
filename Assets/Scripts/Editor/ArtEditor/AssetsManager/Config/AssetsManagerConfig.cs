#if ODIN_INSPECTOR
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace TaoTie
{
    [CreateAssetMenu(fileName = "AssetsManagerConfig", menuName = "Create AssetsManagerConfig", order = 1)]
    public class AssetsManagerConfig : SerializedScriptableObject
    {
        public const string ConfigPath =
            "Assets/Scripts/Editor/ArtEditor/AssetsManager/Config/AssetsManagerConfig.asset";

        [HideReferenceObjectPicker] [LabelText("目录结构")]
        public List<LabelConfig> Labels = new List<LabelConfig>();
    }
}
#endif