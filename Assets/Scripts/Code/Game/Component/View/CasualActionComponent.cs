using UnityEngine;

namespace TaoTie
{
    public class CasualActionComponent: Component,IComponent,IComponent<int,int>,IUpdate
    {
        private Animator animator => parent.GetComponent<GameObjectHolderComponent>()?.Animator;
        private long nextPlayTime;
        private bool enableAutoAction;
        private int start;
        private int end;

        public void Init()
        {
            this.start = 3000;
            this.end = 5000;
            nextPlayTime = GameTimerManager.Instance.GetTimeNow() + Random.Range(start, end);
        }

        public void Init(int start,int end)
        {
            this.start = start;
            this.end = end;
            nextPlayTime = GameTimerManager.Instance.GetTimeNow() + Random.Range(start, end);
        }

        public void Destroy()
        {
            
        }

        public void Update()
        {
            if(!enableAutoAction) return;
            var timeNow = GameTimerManager.Instance.GetTimeNow();
            if (timeNow > nextPlayTime)
            {
                PlayAnim(CasualActionConfigCategory.Instance.RandomAction());
            }
        }
        
        public void PlayAnim(string name, int during = -1)
        {
            animator?.CrossFade(name, 0.2f);
            if (during < 0)
            {
                nextPlayTime = GameTimerManager.Instance.GetTimeNow() + Random.Range(start, end);
            }
            else
            {
                nextPlayTime = GameTimerManager.Instance.GetTimeNow() + during;
            }
        }

        public void SetEnable(bool enable)
        {
            this.enableAutoAction = enable;
            if (enable)
            {
                nextPlayTime = GameTimerManager.Instance.GetTimeNow() + Random.Range(start, end);
            }
        }

        public void SetWinLoss(int val)
        {
            if(animator==null) return;
            animator.SetInteger("WinLoss",val);
        }
        
        public void SetMove(int val)
        {
            if(animator==null) return;
            animator.Play("Idle");
            animator.SetInteger("MotionFlag",val);
        }
    }
}