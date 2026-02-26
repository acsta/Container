namespace TaoTie
{
    public class UIRareAnim: UIBaseContainer,IOnCreate
    {
        private UIImage bg;
        private UIImage inner;
        public void OnCreate()
        {
            bg = AddComponent<UIImage>();
            inner = AddComponent<UIImage>("Bg");
        }

        public void SetColor(string colorStr)
        {
            bg.SetColor(colorStr);
            inner.SetColor(colorStr);
        }
    }
}