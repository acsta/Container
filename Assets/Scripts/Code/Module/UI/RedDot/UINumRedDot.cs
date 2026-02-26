namespace TaoTie
{
    public class UINumRedDot: UIRedDot
    {
        
        public override void RefreshRedDot()
        {
            var count = RedDotManager.Instance.GetRedDotViewCount(target);
            this.SetActive(count > 0);
            var text = GetTransform().GetComponentInChildren<TMPro.TMP_Text>();
            if (text != null)
            {
                if (count < 100)
                {
                    text.text = count.ToString();
                }
                else
                {
                    text.text = "99+";
                }
            }
        }
    }
}