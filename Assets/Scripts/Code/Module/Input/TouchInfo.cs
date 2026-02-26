using System;
using UnityEngine;

namespace TaoTie
{
    public class TouchInfo : IDisposable
    {
        public int Index;
        /// <summary>
        /// 起始点是否在UI
        /// </summary>
        public bool IsStartOverUI;
        
        public Touch? Touch
        {
            get
            {
                if (Index < Input.touchCount)
                {
                    return Input.GetTouch(Index);
                }
                return null;
            }
        }

        public bool IsScroll;
        
        public void Dispose()
        {
            Index = -1;
            IsStartOverUI = false;
            IsScroll = false;
        }
    }
}