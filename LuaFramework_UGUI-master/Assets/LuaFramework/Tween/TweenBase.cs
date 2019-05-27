using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using LuaInterface;
using DG.Tweening;
using Framework;

public class TweenBase : MonoBehaviour
{
    public bool playOnEnable = false;
    public Ease easeType = Ease.Linear;
    public float duration = 1;
    public float delay;
    public int loop = 0;
    public LoopType loopType = LoopType.Restart;
    List<TweenLuaCallback> readyToReleaseCallback = new List<TweenLuaCallback>();

    public float tweenPosition
    {
        get {
            if (tweener != null && tweener.IsActive())
            {
                return tweener.position;
            }
            return 0;
        }
    }

    [HideInInspector]
    public bool isPlaying
    {
        get {
            if (tweener != null && tweener.IsActive())
            {
                return tweener.IsPlaying();
            }
            return false;
        }
    }

    protected Tweener tweener;
    protected TweenLuaCallback onComplete;

    protected virtual void InitTween()
    {

    }

    protected virtual void OnComplete()
    {
        if (onComplete != null)
        {
            onComplete.Call();
        }
        ReleaseUselessCallback();
    }

    public virtual void Pause()
    {
        if (tweener != null && tweener.IsActive())
        {
            tweener.Pause();
        }
    }

    public virtual void Restart()
    {
        InitTween();
        tweener.Restart();
    }

    public virtual void Reset()
    {
    }

    public virtual void Play()
    {
        if (tweener == null || !tweener.IsActive())
        {
            InitTween();
        }
        tweener.Play();
    }
    //     Plays the tween if it was paused, pauses it if it was playing
    public virtual void TogglePause()
    {
        if (tweener != null && tweener.IsActive())
        {
            tweener.TogglePause();
        }
    }

    public virtual void PlayBackwards()
    {
        if (tweener == null || !tweener.IsActive())
        {
            InitTween();
        }
        tweener.PlayBackwards();
    }

    public void SetOnComplete(LuaTable super,LuaFunction call)
    {
        if (onComplete != null)
        {
            readyToReleaseCallback.Add(onComplete);
            //onComplete.Release();
        }
        onComplete = new TweenLuaCallback(super, call);
    }

    public void RemoveOnComplete()
    {
        if (onComplete != null)
        {
            readyToReleaseCallback.Add(onComplete);
            //onComplete.Release();
        }
        onComplete = null;
    }

    void ReleaseUselessCallback()
    {
        for(int i = 0; i < readyToReleaseCallback.Count; i++)
        {
            readyToReleaseCallback[i].Release();
        }
        readyToReleaseCallback.Clear();
    }

    private void OnDestroy()
    {
        RemoveOnComplete();
        ReleaseUselessCallback();
    }

    public void SetLoopType(int type)
    {
        loopType = (LoopType)type;
    }
}