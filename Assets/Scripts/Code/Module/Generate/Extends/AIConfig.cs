namespace TaoTie
{
    public partial class AIConfig
    {
        public int TotalWight;
        public int[] NegativeBehaviorArray;
        public int[] NegativeBehaviorWight;
        public override void AfterEndInit()
        {
            base.AfterEndInit();
            TotalWight = 0;
            NegativeBehaviorArray = new int[NegativeBehavior.Length];
            NegativeBehaviorWight = new int[NegativeBehavior.Length];
            for (int i = 0; i < NegativeBehavior.Length; i++)
            {
                NegativeBehaviorArray[i] = NegativeBehavior[i][0];
                TotalWight += NegativeBehavior[i][1];
                NegativeBehaviorWight[i] = TotalWight;
            }
        }
    }
}