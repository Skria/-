using LuaInterface;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class View : MonoBehaviour
{
    protected bool m_isInited = false;

    public string moduleName;
    public string viewName;

    protected int handle = 0;

    public bool isInited
    {
        get
        {
            return m_isInited;
        }
    }



    [NoToLua]
    public virtual void InitView()
    {
    }
    [NoToLua]
    public virtual void OpenView(params object[] args)
    {
    }
    [NoToLua]
    public virtual void CloseView()
    {
    }
    [NoToLua]
    public virtual void DestroyView()
    {
        handle = 0;
    }

    [NoToLua]
    public virtual void ReopenView(params object[] args)
    {
    }

    [NoToLua]
    public virtual void NotifyInitOver()
    {
        m_isInited = true;
    }
    [NoToLua]
    public void OnNotifyInitOver()
    {
        if (!isInited && CheckChildrenInitOver())
        {
            NotifyInitOver();
        }
    }
    [NoToLua]
    public virtual void NotifyChildrenInitOver()
    {
        View[] views = GetComponentsInChildren<View>(true);
        for (int i = 0; i < views.Length; i++)
        {
            if (views[i] != this) views[i].NotifyInitOver();
        }
    }
    [NoToLua]
    public virtual bool CheckChildrenInitOver()
    {
        View[] views = GetComponentsInChildren<View>(true);
        for (int i = 0; i < views.Length; i++)
        {
            if (views[i] != this)
            {
                if (!views[i].isInited) return false;
            }
        }
        return true;
    }




}
