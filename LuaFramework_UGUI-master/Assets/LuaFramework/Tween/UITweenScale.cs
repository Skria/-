using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UITweenScale : TweenScale
{
    public RectTransform m_Transform;

    private void Update()
    {
        if (tweener != null && tweener.IsActive() && tweener.IsPlaying())
        {
            m_Transform.localScale = curPos;
        }
    }

    protected override void OnComplete()
    {
        if (loopType == LoopType.Restart) curPos = to;
        m_Transform.localScale = curPos;
        if (onComplete != null)
        {
            onComplete.Call();
        }
    }
}
