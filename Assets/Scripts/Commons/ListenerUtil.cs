/*********************************************************
	文件：ListenerUtil
	作者：Administrator
	邮箱：630276388@qq.com
	日期：2020/8/16 6:46:51
	功能：待定
***********************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ListenerUtil : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    public Action<PointerEventData> onClickDown;
    public Action<PointerEventData> onClickUp;
    public Action<PointerEventData> onDrag;

    public void OnDrag(PointerEventData eventData)
    {
        if (onDrag != null) onDrag.Invoke(eventData);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (onClickDown != null) onClickDown.Invoke(eventData);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (onClickUp != null) onClickUp.Invoke(eventData);
    }
}
