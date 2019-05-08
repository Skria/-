using LuaInterface;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LuaEvent : MonoBehaviour
{
    LuaEventCall mClick;
    EventTrigger mEvent;
    Button mButton;

    public static LuaEvent GetEvent(GameObject obj)
    {
        LuaEvent e = obj.GetComponent<LuaEvent>();
        if (e == null)
        {
            e = obj.AddComponent<LuaEvent>();
        }
        return e;
    }

    public void BindClick(LuaTable self, LuaFunction call)
    {
        initClick();
        RemoveClick();
        mClick = new LuaEventCall(gameObject, this, self, call);
        if (mEvent != null)
        {
            mClick.entry = new EventTrigger.Entry();
            mClick.entry.eventID = EventTriggerType.PointerClick;
            mClick.entry.callback.AddListener(mClick.OnClickEvent);
            mEvent.triggers.Add(mClick.entry);
        }
        if (mButton != null)
        {
            mButton.onClick.AddListener(mClick.OnClick);
        }
    }

    public void RemoveClick()
    {
        if (mClick != null)
        {
            if (mEvent != null) mEvent.triggers.Remove(mClick.entry);
            if (mButton != null) mButton.onClick.RemoveListener(mClick.OnClick);
            if (mClick.call != null) mClick.Release();
        }
        mClick = null;
    }

    void initClick()
    {
        mButton = GetComponent<Button>();
        if (mButton == null)
        {
            EventTrigger trigger = GetComponent<EventTrigger>();
            if (trigger == null)
            {
                trigger = gameObject.AddComponent<EventTrigger>();
            }
            mEvent = trigger;
        }
    }
}

public class LuaEventCall
{
    public LuaEventCall(GameObject obj, LuaEvent eventSuper, LuaTable self, LuaFunction call)
    {
        this.obj = obj;
        this.self = self;
        this.call = call;
        this.eventSuper = eventSuper;
    }
    GameObject obj;
    public LuaTable self = null;
    public LuaFunction call = null;
    public EventTrigger.Entry entry = null;
    public EventTrigger.Entry entry1 = null;
    public EventTrigger.Entry entry2 = null;
    public EventTrigger.Entry entry3 = null;

    private LuaEvent eventSuper;
    private float lastCall = 0;


    private float lastTime = 0;
    public float longClickTime = 0.5f;

    protected bool longClicked = false;

    public void OnClickEvent(BaseEventData ed)
    {
        float tNow = Time.time;
        float d = tNow - lastTime;
        if (d < 0.5f) return; //点击间隔限制
        OnClick();
        lastTime = tNow;
    }

    public void OnClick()
    {
        call.Call(self, obj);
    }

    public void Release()
    {
        if (call != null) call.Dispose();
        call = null;
        self = null;
    }

}
