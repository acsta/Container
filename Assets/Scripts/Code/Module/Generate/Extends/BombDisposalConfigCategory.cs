namespace TaoTie
{
    public partial class BombDisposalConfigCategory
    {
        private int totalWeight = -1; 
        public int TotalWeight
        {
            get
            {
                if (totalWeight == -1)
                {
                    totalWeight = 0;
                    for (int i = 0; i < list.Count; i++)
                    {
                        totalWeight += list[i].Weight;
                    }
                }

                return totalWeight;
            }
        }
    }
}