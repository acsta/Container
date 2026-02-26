using DaGenGraph;
using DaGenGraph.Editor;
using Sirenix.OdinInspector;
using UnityEngine;

namespace TaoTie
{
   
    public class AIRootNode:JsonNodeBase
    {
        [LabelText("AI类型")]
        public string Type; 
        
        public override void InitNode(Vector2 pos, string nodeName, int minInputPortsCount = 0, int minOutputPortsCount = 0)
        {
            base.InitNode(pos, nodeName, minInputPortsCount, minOutputPortsCount);
            canBeDeleted = false;
            SetName("Root");
        }

        public override void AddDefaultPorts()
        {
            AddOutputPort("Root", EdgeMode.Override, false, false);
        }
    }
}