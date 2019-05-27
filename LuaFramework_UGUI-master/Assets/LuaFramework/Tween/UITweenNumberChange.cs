using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UITweenNumberChange : TweenBase
{
    public Text m_Text;

    public float from;
    public float to;


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

            if (m_Text != null)
            {
                int show = (int)(curPos);
                m_Text.text = show.ToString();
            }
        }
    }

    protected override void OnComplete()
    {
        if (loopType == LoopType.Restart) curPos = to;
        if (m_Text != null) m_Text.text = curPos.ToString();
        base.OnComplete();
    }
}
