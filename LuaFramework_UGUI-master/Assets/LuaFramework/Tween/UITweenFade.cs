using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

///实现一组UI的渐入减出效果
public class UITweenFade : TweenBase
{
    public float from;
    public float to;
    public CanvasGroup canvasGroup;
    protected float curPos = 0;

    private void Start()
    {
        OnEnable();
    }

    protected override void InitTween()
    {
        if (tweener != null && tweener.IsActive())
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
            if (canvasGroup != null) canvasGroup.alpha = curPos;
        }
    }

    protected override void OnComplete()
    {
        if (loopType == LoopType.Restart) curPos = to;
        if (canvasGroup != null) canvasGroup.alpha = curPos;
        base.OnComplete();
    }

    public override void Reset()
    {
        base.Reset();
        canvasGroup.alpha = from;
    }
}

