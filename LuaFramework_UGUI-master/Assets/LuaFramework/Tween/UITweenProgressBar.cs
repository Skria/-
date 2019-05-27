using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class UITweenProgressBar : TweenBase
{
    ///实现进度条动画
    public float from;
    public float to;
    public Image image;
    public Slider slider;
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
            if (image != null) image.fillAmount = curPos;
            if (slider != null)
            {
                slider.value = curPos;
            }
        }
    }

    protected override void OnComplete()
    {
        if (loopType == LoopType.Restart) curPos = to;
        if (image != null) image.fillAmount = curPos;
        if (slider != null) slider.value = curPos;
        base.OnComplete();
    }
}
