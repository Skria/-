using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class TweenHumanRun : TweenBase
{
    public Vector3 from; 
    public Vector3 to;
    public float legLength = 2;
    public float stepAngle = 45;
    public float stepDuration = 0.2f;
    public float rotAngle = 4;
    public float rotDuration = 0.4f;



    protected Vector3 curPos = Vector3.zero;

    float curStepAngle = 0;
    float curRotAngle = 0;

    Tweener rotTweener;
    Tweener stepTweener;
    float initY;

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
        if (rotTweener != null && rotTweener.IsActive())
        {
            rotTweener.Kill();
        }
        if (stepTweener != null && stepTweener.IsActive())
        {
            stepTweener.Kill();
        }
        curPos = from;
        initY = from.y;

        tweener = DOTween.To(() => curPos, x => curPos = x, to, duration);
        tweener.SetEase(easeType);
        tweener.SetDelay(delay);
        tweener.SetLoops(loop, loopType);
        tweener.onComplete = OnComplete;
        //tweener.Pause();

        stepTweener = DOTween.To(() => 0, x => curStepAngle = x, stepAngle, stepDuration);
        stepTweener.SetEase(Ease.InOutQuad);
        stepTweener.SetDelay(delay);
        stepTweener.SetLoops(-1,LoopType.Yoyo);
        //stepTweener.Pause();

        rotTweener = DOTween.To(() => 0, x => curRotAngle = x, rotAngle, rotDuration);
        rotTweener.SetEase(Ease.InOutQuad);
        rotTweener.SetDelay(delay);
        rotTweener.SetLoops(-1, LoopType.Yoyo);
        //rotTweener.Pause();


    }

    private void OnEnable()
    {
        if (playOnEnable)
        {
            InitTween();
            if (tweener != null && tweener.IsActive() && rotTweener != null && rotTweener.IsActive() && stepTweener != null && stepTweener.IsActive())
            {
                tweener.Restart(true);
                rotTweener.Restart(true);
                stepTweener.Restart(true);
            }
            else
            {
                Play();
            }
        }
    }

    public override void Restart()
    {
        InitTween();
        if (tweener != null && tweener.IsActive())
        {
            tweener.Restart();
        }
        if (rotTweener != null && rotTweener.IsActive())
        {
            rotTweener.Restart();
        }
        if (stepTweener != null && stepTweener.IsActive())
        {
            stepTweener.Restart();
        }
    }

    public override void Play()
    {
        if (tweener != null && tweener.IsActive())
        {
            tweener.Play();
        }
        if (rotTweener != null && rotTweener.IsActive())
        {
            rotTweener.Play();
        }
        if (stepTweener != null && stepTweener.IsActive())
        {
            stepTweener.Play();
        }
    }

    public override void Pause()
    {
        if (tweener != null && tweener.IsActive())
        {
            tweener.Pause();
        }
        if (rotTweener != null && rotTweener.IsActive())
        {
            rotTweener.Pause();
        }
        if (stepTweener != null && stepTweener.IsActive())
        {
            stepTweener.Pause();
        }
    }

    public override void TogglePause()
    {
        if (tweener != null && tweener.IsActive())
        {
            tweener.TogglePause();
        }
        if (rotTweener != null && rotTweener.IsActive())
        {
            rotTweener.TogglePause();
        }
        if (stepTweener != null && stepTweener.IsActive())
        {
            stepTweener.TogglePause();
        }
    }

    public override void PlayBackwards()
    {
        if (tweener != null && tweener.IsActive())
        {
            tweener.PlayBackwards();
        }
        if (rotTweener != null && rotTweener.IsActive())
        {
            rotTweener.PlayBackwards();
        }
        if (stepTweener != null && stepTweener.IsActive())
        {
            stepTweener.PlayBackwards();
        }
    }

    private void Update()
    {
        if (rotTweener != null && rotTweener.IsActive() && rotTweener.IsPlaying())
        {
            transform.localRotation = Quaternion.Euler(0, 0, curRotAngle - rotAngle / 2);
        }
        if ((tweener != null && tweener.IsActive() && tweener.IsPlaying())||(stepTweener != null && stepTweener.IsActive() && stepTweener.IsPlaying()))
        {
            Vector3 pos = new Vector3(curPos.x, curPos.y - legLength *(1- Mathf.Cos(Mathf.Deg2Rad * curStepAngle / 2)), curPos.z);
            transform.localPosition = pos;
        }
    }
    
    protected override void OnComplete()
    {
        if (loopType == LoopType.Restart) curPos = to;

        if (rotTweener != null && rotTweener.IsActive())
        {
            rotTweener.Kill();
            rotTweener = DOTween.To(() => curRotAngle, x => curRotAngle = x, rotAngle/2, 0.1f);
            rotTweener.SetEase(Ease.InOutQuad);
            rotTweener.Play();
        }
        if (stepTweener != null && stepTweener.IsActive())
        {
            stepTweener.Kill();
            stepTweener = DOTween.To(() => curStepAngle, x => curStepAngle = x, 0, 0.1f);
            stepTweener.SetEase(Ease.InOutQuad);
            stepTweener.Play();
        }
        base.OnComplete();
    }
}
