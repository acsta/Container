using UnityEngine;

namespace TaoTie
{
    public partial class CasualActionConfigCategory
    {
        private int total;
        public override void AfterEndInit()
        {
            base.AfterEndInit();
            for (int i = 0; i < list.Count; i++)
            {
                total += list[i].Widget;
            }
        }

        public string RandomAction()
        {
            int state = Random.Range(0, total * 10) % total;
            for (int i = 0; i < list.Count; i++)
            {
                if (state < list[i].Widget)
                {
                    return list[i].ActionName;
                }

                state -= list[i].Widget;
            }

            return list[0].ActionName;
        }
    }
}