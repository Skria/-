using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using LuaInterface;
using Framework;
public class TweenBulletParabolicPosition : MonoBehaviour
{
    
    public bool isGlobal = false;
    public bool playOnEnable = false;
    public Vector3 gVector = new Vector3(0, 1, 0);
    Vector3 from_;
    public Vector3 from {
        set
        {
            from_ = value;
            if (isGlobal)
            {
                transform.position = from_;
            }
            else
            {
                transform.localPosition = from_;
            }
        }
        get
        {
            return from_;
        }
    }//点A
    public Vector3 to;//点B
    public float g = -10;//重力加速度
    public bool autoRatate = false;
    public float duration = 1;//时长


    private Vector3 speed;//初速度向量
    private Vector3 gravity;//重力向量
    private float dTime = 0;
    private bool isPlaying = false;
    protected TweenLuaCallback onComplete;

    private void OnEnable()
    {
        if (playOnEnable)
        {
            Restart();
        }
    }

    public void SetOnComplete(LuaTable super, LuaFunction call)
    {
        if (onComplete != null)
        {
            onComplete.Release();
        }
        onComplete = new TweenLuaCallback(super, call);
    }

    public void RemoveOnComplete()
    {
        if (onComplete != null)
        {
            onComplete.Release();
        }
        onComplete = null;
    }

    protected virtual void OnComplete()
    {
        if (onComplete != null)
        {
            onComplete.Call();
        }
    }

    public void Restart()
    {
        if (isGlobal)
        {
            transform.position = from;
        }
        else
        {
            transform.localPosition = from;
        }
        
        //通过一个式子计算初速度
        speed =  new Vector3((to.x - from.x) / duration,
            (to.y - from.y) / duration, (to.z - from.z) / duration) - 0.5f * g * duration * gVector;
        gravity = Vector3.zero;//重力初始速度为0
        dTime = 0;
        AutoRatated(speed);
        isPlaying = true;
    }

    void AutoRatated(Vector3 dV)
    {
        if (autoRatate)
        {
            if (isGlobal)
            {
                gameObject.transform.rotation = Quaternion.LookRotation(dV);
            }
            else
            {
                gameObject.transform.localRotation = Quaternion.LookRotation(dV);
            }
        }

    }

    public void Pause()
    {
        isPlaying = false;
    }

    
    // Update is called once per frame
    void FixedUpdate()
    {
        if (isPlaying)
        {
            if (dTime < duration)
            {
                gravity = g * (dTime += Time.fixedDeltaTime) * gVector;//v=at
                Vector3 vec = speed * Time.fixedDeltaTime + gravity * Time.fixedDeltaTime;
                if (isGlobal)
                {
                    transform.position = transform.position + vec;
                }
                else
                {
                    transform.localPosition = transform.localPosition + vec;
                }
                AutoRatated(vec);
            }
            else
            {
                if (isGlobal)
                {
                    transform.position = to;
                }
                else
                {
                    transform.localPosition = to;
                }
                isPlaying = false;
                OnComplete();
            }
        }
        
    }
    private void OnDestroy()
    {
        if (onComplete != null)
        {
            onComplete.Release();
        }
    }


}
