using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class TweenRotation : TweenBase
{
    public Vector3 from; 
    public Vector3 to;


    protected Vector3 curPos = Vector3.zero;

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
            transform.localRotation = Quaternion.Euler(curPos);
        }
    }
    
    protected override void OnComplete()
    {
        if (loopType == LoopType.Restart) curPos = to;
        transform.localRotation = Quaternion.Euler(curPos);
        base.OnComplete();
    }
}
