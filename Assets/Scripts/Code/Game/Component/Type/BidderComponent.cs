using UnityEngine;
namespace TaoTie
{
    public class BidderComponent: Component, IComponent<int>
    {
        public int ConfigId { get; private set; }
        public AIConfig Config => AIConfigCategory.Instance.Get(ConfigId);

        public void Init(int id)
        {
            ConfigId = id;
        }

        public void Destroy()
        {
            ConfigId = 0;
        }
    }
}