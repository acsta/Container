namespace TaoTie
{
    /// <summary>
    /// 其他地方不要持有对NumericChange的引用
    /// </summary>
    public struct NumericChange
    {
        public Entity Parent;
        public int NumericType;
        public decimal Old;
        public decimal New;
        
    }
}