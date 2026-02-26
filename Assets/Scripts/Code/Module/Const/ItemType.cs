namespace TaoTie
{
    public enum ItemType
    {
        Container = -2,
        Const = -1,
        None = 0,
        Story = 1,//剧情
        Appraisal = 10,//鉴定（转盘）
        AppraisalResult = 11,//鉴定结果
        Repair = 20,//修复（拼图验证码）
        GoodsCheck = 30,//验货（答题）
        Quarantine = 40,//检疫（刮刮乐）
        BombDisposal = 50,//拆弹（线迷宫）
        
        MAX = 10000,//大于此表示全局玩法
        GodOfWealth = 10010,//财神
    }
}