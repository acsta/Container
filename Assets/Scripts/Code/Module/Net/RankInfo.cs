using System.Collections.Generic;
using TaoTie.LitJson.Extensions;

namespace TaoTie
{
    public class RankInfo
    {
        public int uid;
        public string Avatar;
        public string NickName;
        public string RankValue;
        [JsonIgnore] 
        public BigNumber Money => RankValue;
    }

    public class RankList
    {
        public int my;
        public RankInfo[] list;
    }
}