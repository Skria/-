using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UITweenArcMove : TweenBase
{
    public RectTransform m_Transform;

    public Vector3 from; 
    public Vector3 to;
    public float moveDis;  //需要偏移的总距离
    public Boolean reverse = false;
    private float k = 0;  //垂线斜率
    private Vector3 centerPoint;
    private Boolean verticalFlag = false;

    private Vector3 curPos = Vector3.zero;
    private float Pi = 3.14f;
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
        verticalFlag = false;

        centerPoint = (to + from) / 2;
        if(to.y == from.y)
        {
            verticalFlag = true;
            k = 0;
        }
        else
        {
            k = (to.x - from.x) / (to.y - from.y);
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
            if (m_Transform != null)
            {
                float tempX = 0;
                float tempY = 0;
                Vector3 tempDis = curPos - centerPoint;
                float tempRate =1 - Math.Abs(tempDis.magnitude / (centerPoint - from).magnitude);
                float tempMoveDis = (float)(moveDis * Math.Sin(tempRate * Pi / 4));
                if (verticalFlag)
                {
                    tempX = 0;
                    tempY = tempMoveDis;
                }
                else
                {
                    tempX = (float)(tempMoveDis / Math.Sqrt(1 + k * k));
                    tempY = tempX * k;
                }
                float tempReverse = 1;
                if (reverse)
                {
                    tempReverse = -1;
                }
                Vector3 moveV3 = new Vector3(curPos.x + tempX * tempReverse, curPos.y + tempY * tempReverse, curPos.z) ;
                m_Transform.anchoredPosition = moveV3;
            }
        }
    }

    protected override void OnComplete()
    {
        if (loopType == LoopType.Restart) curPos = to;
        
        base.OnComplete();
    }
}
