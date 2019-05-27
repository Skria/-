using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class TweenGuideShader : TweenBase
{
    public float from = 0;
    public float to = 0;
    public bool isMultiple = false;

    protected float cur = 0;

    private Material material;
    private Vector4[] centerList;
    private float[] radiusList;
    private float number;

    private Vector4 center;
    private float radius;

    private void Awake()
    {
        Image image = GetComponent<Image>();
        if (image != null)
        {
            material = image.material;
        }
    }

    protected override void InitTween()
    {
        if (tweener != null && tweener.IsActive())
        {
            tweener.Kill();
        }
        cur = from;
        tweener = DOTween.To(() => cur, x => cur = x, to, duration);
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
        if (tweener != null && tweener.IsActive() && tweener.IsPlaying() && material != null)
        {
            SetMaterial();
        }
    }

    public override void Restart()
    {
        if (material != null)
        {
            if (isMultiple)
            {
                centerList = material.GetVectorArray("_CenterArray");
                radiusList = material.GetFloatArray("_RadiusArray");
                number = material.GetFloat("_Number");
            }
            else
            {
                center = material.GetVector("_Pos");
                radius = material.GetFloat("_Radius");
            }
        }

        base.Restart();
    }

    protected override void OnComplete()
    {
        if (loopType == LoopType.Restart) cur = to;

        SetMaterial();
        centerList = null;

        base.OnComplete();
    }

    private void SetMaterial()
    {
        if (material != null)
        {
            if (isMultiple)
            {
                if (centerList == null)
                {
                    return;
                }

                float[] temp = new float[radiusList.Length];
                for (int i = 0; i < radiusList.Length; i++)
                {
                    temp[i] = cur;
                }

                material.SetVectorArray("_CenterArray", centerList);
                material.SetFloatArray("_RadiusArray", temp);
                material.SetFloat("_Number", number);
            }
            else
            {
                if (center == null)
                {
                    return;
                }

                material.SetVector("_Pos", center);
                material.SetFloat("_Radius", cur);
            }
        }
    }
}
