#if ODIN_INSPECTOR
using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEditor;
using Object = UnityEngine.Object;

namespace TaoTie
{
    [Serializable]
    public class CollectConfig
    {
        [LabelText("小类")] public string Label;

        [HideReferenceObjectPicker] [LabelText("包含的文件夹")]
        public List<Object> Objects = new List<Object>();
    }
}
#endif