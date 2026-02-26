using UnityEngine;

namespace TaoTie
{
    public partial class RareConfigCategory
    {
        public RareConfig GetRare(int rare)
        {
            return dict[Mathf.Clamp(rare, 1, list.Count)];
        }
    }
}