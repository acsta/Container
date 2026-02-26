using UnityEngine;

namespace TaoTie
{
    public class UserItem: UIBaseContainer,IOnCreate
    {
        public UIRawImage Head;
        public void OnCreate()
        {
            Head = AddComponent<UIRawImage>("NameBg/RawImage");
        }

        public void RefreshHead()
        {
            Head.SetOnlineTexturePath("https://cdn.hxwgame.cn/head/13 (" + Random.Range(1, 1888) + ").jpg").Coroutine();
        }
    }
}