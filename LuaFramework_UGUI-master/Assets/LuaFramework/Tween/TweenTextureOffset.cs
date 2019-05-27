using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class TweenTextureOffset : TweenBase
{
    public Vector2 from; 
    public Vector2 to;


    protected Vector2 curPos = Vector2.zero;

    public Renderer renderer;

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
        if (renderer!=null && tweener != null && tweener.IsActive() && tweener.IsPlaying())
        {
            renderer.material.mainTextureOffset = curPos;
        }
    }

    protected override void OnComplete()
    {
        if(loopType == LoopType.Restart)curPos = to;
        if (renderer != null)
        {
            renderer.material.mainTextureOffset = curPos;
        }
        base.OnComplete();
    }
}
