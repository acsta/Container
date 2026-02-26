namespace TaoTie
{
    public class UIRedDot: UIBaseContainer,IOnCreate<string>,IOnDestroy,IOnCreate
    {
        protected string target;
        public void OnCreate()
        {
            
        }
        public void OnCreate(string p1)
        {
            ReSetTarget(p1);
        }

        public void OnDestroy()
        {
            if (!string.IsNullOrEmpty(target))
            {
                RedDotManager.Instance.RemoveUIRedDotComponent(target, this);
                target = null;
            }
        }

        public void ReSetTarget(string p1)
        {
            if (target == p1) return;
            if (!string.IsNullOrEmpty(target))
            {
                RedDotManager.Instance.RemoveUIRedDotComponent(target,this);
            }
            target = p1;
            if (!string.IsNullOrEmpty(p1))
            {
                RedDotManager.Instance.AddUIRedDotComponent(p1,this);
                RefreshRedDot();
            }
            else
            {
                SetActive(false);
            }
        }

        public virtual void RefreshRedDot()
        {
            this.SetActive(RedDotManager.Instance.GetRedDotViewCount(target) > 0);
        }
    }
}