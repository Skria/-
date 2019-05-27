using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UITweenPosition : TweenPosition
{
    public RectTransform m_Transform;

    private void Update()
    {
        if (tweener != null && tweener.IsActive() && tweener.IsPlaying())
        {
            m_Transform.anchoredPosition3D = curPos;
        }
    }

    protected override void OnComplete()
    {
        if (loopType == LoopType.Restart) curPos = to;
        m_Transform.anchoredPosition3D = curPos;
        if (onComplete != null)
        {
            onComplete.Call();
        }
    }
}
