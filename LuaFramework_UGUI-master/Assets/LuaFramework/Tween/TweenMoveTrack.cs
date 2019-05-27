using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TweenMoveTrack : MonoBehaviour
{
    public enum PlayType
    {
        Once = 1,
        Loop = 2,
    }

    public PlayType loopType = PlayType.Once;
    public int loopCount = 1;
    public bool playEnable = false;
    public bool IsGlobal = false;
    public float delayTime = 0;

    public float time = 10;
    public Vector3 fromPosition = Vector3.zero;
    public Vector3 toPosition = Vector3.zero;

    public AnimationCurve positionXCurve;
    public AnimationCurve positionYCurve;
    public AnimationCurve positionZCurve;

    public float offsetPositionX = 0;
    public float offsetPositionY = 0;
    public float offsetPositionZ = 0;

    private float standardPositionX = 0;
    private float standardPositionY = 0;
    private float standardPositionZ = 0;

    public Vector3 fromRotation = Vector3.zero;
    public Vector3 toRotation = Vector3.zero;

    public AnimationCurve rotationXCurve;
    public AnimationCurve rotationYCurve;
    public AnimationCurve rotationZCurve;

    public float offsetRotationX = 0;
    public float offsetRotationY = 0;
    public float offsetRotationZ = 0;

    private float standardRotationX = 0;
    private float standardRotationY = 0;
    private float standardRotationZ = 0;


    public Vector3 fromScale = Vector3.one;
    public Vector3 toScale = Vector3.one;

    public AnimationCurve scaleXCurve;
    public AnimationCurve scaleYCurve;
    public AnimationCurve scaleZCurve;

    public float offsetScaleX = 0;
    public float offsetScaleY = 0;
    public float offsetScaleZ = 0;

    private float standardScaleX = 0;
    private float standardScaleY = 0;
    private float standardScaleZ = 0;



    private bool isStart = false;
    private float startTime = 0;
    public void Awake()
    {

    }

    private void Start()
    {
        if (playEnable)
        {
            Play();
        }
    }

    private void OnEnable()
    {
        if (playEnable)
        {
            Play();
        }
    }

    public void Update()
    {
        if (isStart)
        {
            if (Time.time - startTime < delayTime)
            {

            }
            else
            {
                if (Time.time - startTime > time + delayTime)
                {
                    if (loopType == PlayType.Once)
                    {
                        isStart = false;
                    }
                    else if (loopType == PlayType.Loop)
                    {
                        loopCount--;
                        if (loopCount <= 0)
                        {
                            isStart = false;
                        }
                        else
                        {
                            startTime = Time.time;
                        }
                    }
                }
                else
                {
                    float nowTime = (Time.time - startTime) / (time + delayTime);


                    float tempPositionX = positionXCurve.Evaluate(nowTime) * (standardPositionX + offsetPositionX);
                    float tempPositionY = positionYCurve.Evaluate(nowTime) * (standardPositionY + offsetPositionY);
                    float tempPositionZ = positionZCurve.Evaluate(nowTime) * (standardPositionZ + offsetPositionZ);
                    if (IsGlobal)
                    {
                        transform.position = fromPosition + new Vector3(tempPositionX, tempPositionY, tempPositionZ);
                    }
                    else
                    {
                        transform.localPosition = fromPosition + new Vector3(tempPositionX, tempPositionY, tempPositionZ);
                    }

                    float tempScaleX = scaleXCurve.Evaluate(nowTime) * (standardScaleX + offsetScaleX);
                    float tempScaleY = scaleYCurve.Evaluate(nowTime) * (standardScaleY + offsetScaleY);
                    float tempScaleZ = scaleZCurve.Evaluate(nowTime) * (standardScaleZ + offsetScaleZ);
                    transform.localScale = fromScale + new Vector3(tempScaleX, tempScaleY, tempScaleZ);

                    float tempRotationX = rotationXCurve.Evaluate(nowTime) * (standardRotationX + offsetRotationX);
                    float tempRotationY = rotationYCurve.Evaluate(nowTime) * (standardRotationY + offsetRotationY);
                    float tempRotationZ = rotationZCurve.Evaluate(nowTime) * (standardRotationZ + offsetRotationZ);
                    transform.localRotation = Quaternion.Euler(fromRotation + new Vector3(tempRotationX, tempRotationY, tempRotationZ));
                }
            }
        }
    }

    public void Play()
    {
        if (isStart)
        {
            Stop();
        }
        startTime = Time.time;
        isStart = true;
        if (IsGlobal)
        {
            transform.position = fromPosition;
        }
        else
        {
            transform.localPosition = fromPosition;
        }

        standardPositionX = toPosition.x - fromPosition.x;
        standardPositionY = toPosition.y - fromPosition.y;
        standardPositionZ = toPosition.z - fromPosition.z;

        standardRotationX = toRotation.x - fromRotation.x;
        standardRotationY = toRotation.y - fromRotation.y;
        standardRotationY = toRotation.y - fromRotation.y;

        standardScaleX = toScale.x - fromScale.x;
        standardScaleY = toScale.y - fromScale.y;
        standardScaleZ = toScale.z - fromScale.z;
    }

    public void Stop()
    {
        isStart = false;
    }
}
