using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class TweenPosition : TweenBase
{
    public Vector3 from; 
    public Vector3 to;


    protected Vector3 curPos = Vector3.zero;

    public bool isGlobal = false;

    private void Start()
    {
        OnEnable();
    }

    protected override void InitTween()
    {
        if(tweener!=null && tweener.IsActive())
        {
            tweener.Kill();
        }
        curPos = from;
        tweener = DOTween.To(() => curPos, x => curPos = x, to, duration);
        tweener.SetEase(easeType);
        tweener.SetDelay(delay);
        tweener.SetLoops(loop, loopType);
        tweener.onComplete = OnComplete;
        //tweener.Pause();
    }

    private void OnEnable()
    {
        if (playOnEnable)
        {
            InitTween();
            if (tweener != null && tweener.IsActive())
            {
                tweener.Restart(true);
            }
            else
            {
                Play();
            }
        }
    }

    private void Update()
    {
        if (tweener != null && tweener.IsActive() && tweener.IsPlaying())
        {
            if (isGlobal)
            {
                transform.position = curPos;
            }
            else
            {
                transform.localPosition = curPos;
            }
        }
    }

    protected override void OnComplete()
    {
        if(loopType == LoopType.Restart)curPos = to;
        if (isGlobal)
        {
            transform.position = curPos;
        }
        else
        {
            transform.localPosition = curPos;
        }
        base.OnComplete();
    }
}
