/*********************************************************
	文件：WindowRoot
	作者：Administrator
	邮箱：630276388@qq.com
	日期：2020/8/12 15:44:19
	功能：UI界面基类
***********************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class WindowRoot : MonoBehaviour
{
    protected ResSev resSev = null;
    protected AudioSev audioSev = null;
    protected NetSev netSev = null;
    protected TimerSev timerSev = null;

    public void SetWindowState(bool isActive = true)
    {
        if(gameObject.activeSelf != isActive)
        {
            SetActive(gameObject ,isActive);
        }
        if (isActive) InitWindow(); else ClearWindow();
    }

    protected void SetSprite(Image img, string path)
    {
        Sprite sp = resSev.LoadSprite(path, true);
        img.sprite = sp;
    }

    protected virtual void InitWindow()
    {
        resSev = ResSev.Instance;
        audioSev = AudioSev.Instance;
        netSev = NetSev.Instance;
        timerSev = TimerSev.Instance;
    }

    protected virtual void ClearWindow()
    {
        resSev = null;
        audioSev = null;
        netSev = null;
        timerSev = null;
    }

    #region Tool Functions
    protected void SetText(Text txt, string contex)
    {
        txt.text = contex;
    }
    protected void SetText(Text txt, int num)
    {
        SetText(txt, num.ToString());
    }
    protected void SetText(Transform tans, string contex)
    {
        transform.GetComponent<Text>().text = contex;
    }
    protected void SetText(Transform trans, int num)
    {
        SetText(trans, num.ToString());
    }


    protected void SetActive(GameObject go, bool isActive)
    {
        go.SetActive(isActive);
    }
    protected void SetActive(Component ct, bool isActive)
    {
        ct.gameObject.SetActive(isActive);
    }


    protected T FindComponent<T>(string path) where T : Component
    {
        Transform trans = transform.Find(path);
        if(trans == null)
        {
            Debug.LogError("该路径无法查找到物体，路径为：" + path);
            return null;
        }

        T res;
        trans.TryGetComponent(out res);
        if (res == null) Debug.Log("该路径无法找到 " + typeof(T).Name + " 组件，路径为：" + path);
        return res;
    }

    protected T[] FindComponentsInChildren<T>(string path) where T : Component
    {
        Transform trans = transform.Find(path);
        return trans.GetComponentsInChildren<T>();
    }

    protected T GetOrCreateComponent<T>(GameObject go) where T : Component
    {
        T t = null;
        if (go.TryGetComponent(out t)) return t;
        t = go.AddComponent<T>();
        return t;
    }
    #endregion

    #region Click Events
    protected void OnClickDown(GameObject go, Action<PointerEventData> cb)
    {
        ListenerUtil listener = GetOrCreateComponent<ListenerUtil>(go);
        listener.onClickDown = cb;
    }
    protected void OnClickDown(Component ct, Action<PointerEventData> cb)
    {
        ListenerUtil listener = GetOrCreateComponent<ListenerUtil>(ct.gameObject);
        listener.onClickDown = cb;
    }

    protected void OnClickUp(GameObject go, Action<PointerEventData> cb)
    {
        ListenerUtil listener = GetOrCreateComponent<ListenerUtil>(go);
        listener.onClickUp = cb;
    }
    protected void OnClickUp(Component ct, Action<PointerEventData> cb)
    {
        ListenerUtil listener = GetOrCreateComponent<ListenerUtil>(ct.gameObject);
        listener.onClickUp = cb;
    }

    protected void OnClickDrag(GameObject go, Action<PointerEventData> cb)
    {
        ListenerUtil listener = GetOrCreateComponent<ListenerUtil>(go);
        listener.onDrag = cb;
    }
    protected void OnClickDrag(Component ct, Action<PointerEventData> cb)
    {
        ListenerUtil listener = GetOrCreateComponent<ListenerUtil>(ct.gameObject);
        listener.onDrag = cb;
    }
    #endregion
}
