using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ScrollViewEventRaycast : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{

    /// <summary>
    /// 上层的ScrollRect
    /// </summary>
    public ScrollRect[] parentScrollRect;
    //事件传递
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (parentScrollRect!=null)
        {
            for (int i = 0; i < parentScrollRect.Length; i++)
            {
                if(parentScrollRect[i]==null) continue;
                parentScrollRect[i].OnBeginDrag(eventData);
            }
            
        }
    }
    //事件传递
    public void OnDrag(PointerEventData eventData)
    {
        if (parentScrollRect != null)
        {
            for (int i = 0; i < parentScrollRect.Length; i++)
            {
                if(parentScrollRect[i]==null) continue;
                parentScrollRect[i].OnDrag(eventData);
            }
        }
    }
    //事件传递
    public void OnEndDrag(PointerEventData eventData)
    {
        if (parentScrollRect != null)
        {
            for (int i = 0; i < parentScrollRect.Length; i++)
            {
                if(parentScrollRect[i]==null) continue;
                parentScrollRect[i].OnEndDrag(eventData);
            }
        }
    }
}