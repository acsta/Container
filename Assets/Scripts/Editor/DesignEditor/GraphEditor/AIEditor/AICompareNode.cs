using DaGenGraph;
using DaGenGraph.Editor;
using Sirenix.OdinInspector;
using UnityEngine;


namespace TaoTie
{
    public class AICompareNode: JsonNodeBase
    {
        [NotNull][PropertyOrder(1)][LabelText("左值")]
        public BaseValue LeftValue = new SingleValue();
        [PropertyOrder(2)][LabelText("判断符号")]
        public CompareMode CompareMode;
        [NotNull][PropertyOrder(3)][LabelText("右值")]
        public BaseValue RightValue = new SingleValue();
        public override void InitNode(Vector2 pos, string nodeName, int minInputPortsCount = 0, int minOutputPortsCount = 0)
        {
            base.InitNode(pos, nodeName, minInputPortsCount, minOutputPortsCount);
            SetName("Compare");
        }

        public override void AddDefaultPorts()
        {
            AddInputPort("输入", EdgeMode.Multiple, false, false);
            AddOutputPort("True" , EdgeMode.Override, false, false);
            AddOutputPort("False" ,EdgeMode.Override, false, false);
        }
    }
}