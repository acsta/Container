#if ODIN_INSPECTOR
using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;

namespace TaoTie
{
    [Serializable]
    public class LabelConfig
    {
        [LabelText("大类")] public string Label;

        [LabelText("小类")] [HideReferenceObjectPicker]
        public List<CollectConfig> Collects = new List<CollectConfig>();
    }
}

#endif