using System.Collections.Generic;

namespace TaoTie
{
    public partial class StageConfigCategory
    {
        private UnOrderDoubleKeyDictionary<int, int, StageConfig> lvStageMapData;
        public override void AfterEndInit()
        {
            base.AfterEndInit();
            lvStageMapData = new UnOrderDoubleKeyDictionary<int, int, StageConfig>();
            for (int i = 0; i < list.Count; i++)
            {
                lvStageMapData.Add(list[i].Level,list[i].Stage,list[i]);
            }
        }
        
        public StageConfig GetLevelConfigByLvAndStage(int lv,int stage)
        {
            lvStageMapData.TryGetValue(lv, stage, out StageConfig res);
            return res;
        }
        public bool TryGetStageByLevel(int lv,out Dictionary<int,StageConfig> stages)
        {
            return lvStageMapData.TryGetDic(lv, out stages);
        }
    }
}