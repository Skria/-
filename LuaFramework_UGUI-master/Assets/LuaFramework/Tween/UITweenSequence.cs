using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using LuaInterface;

public class UITweenSequence : TweenBase
{
    public RectTransform m_Transform;
    public Vector3 from;
    public Vector3[] tos;
    public Sequence sequence;
    protected Vector3 curPos = Vector3.zero;

    private void Start()
    {
        OnEnable();       
    }

    protected override void InitTween()
    {
        if (sequence == null)
        {
            sequence = DOTween.Sequence();
        }
        curPos = from;
        sequence.onComplete = OnComplete;
    }
    //defective  应该支持多种类型动画
    public void AddSequences(Vector3[] tos, float duration)
    {
        InitTween();
        foreach (Vector3 to in tos)
        {
            Tweener tweener = DOTween.To(() => curPos, x => curPos = x, to, duration);
            tweener.SetEase(easeType);
            tweener.SetDelay(delay);
            tweener.SetLoops(loop, loopType);
            sequence.Append(tweener);
        }
    }

    public void ClearSequence()
    {
        sequence = null;
    }

    public override void Pause()
    {
        if (sequence != null && sequence.IsActive())
        {
            m_Transform.anchoredPosition3D = curPos;
            sequence.Pause();
        }
    }

    public override void Restart()
    {
        //InitTween();
        sequence.Play();
    }

    private void Update()
    {   
        if (sequence != null && sequence.IsActive())
        {
            m_Transform.anchoredPosition3D = curPos;
        }
    }

    private void OnEnable()
    {
        if (playOnEnable)
        {
            InitTween();
            if (sequence != null && sequence.IsActive())
            {
                sequence.Restart(true);
            }
            else
            {
                sequence.Play();
            }
        }
    }

    public override void Play()
    {
        sequence.Play();
    }

    protected override void OnComplete()
    {
        sequence = null;
        if (onComplete != null)
        {
            onComplete.Call();
        }
    }
}
