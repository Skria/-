using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UITweenTextSequence : TweenBase
{
    public Text m_Text;
    public string[] m_texts;

    private int curIndex = 0;
    // Start is called before the first frame update
    void Start()
    {
        if (m_texts.Length <= 0)
        {
            string str = m_Text.text.Replace(".", string.Empty);
            m_texts = new string[] {
                str,
                str + ".",
                str + "..",
                str + "..."
            };
        }
        OnEnable();
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

    protected override void InitTween()
    {
        if (tweener != null && tweener.IsActive())
        {
            tweener.Kill();
        }
        curIndex = 0;

        tweener = DOTween.To(() => curIndex, x => curIndex = x, m_texts.Length - 1, duration);
        tweener.SetEase(easeType);
        tweener.SetDelay(delay);
        tweener.SetLoops(loop, loopType);
        tweener.onComplete = OnComplete;
    }

    private void Update()
    {
        if (tweener != null && tweener.IsActive() && tweener.IsPlaying())
        {
            if (m_Text != null && curIndex >= 0 && curIndex < m_texts.Length)
                m_Text.text = m_texts[curIndex];
        }
    }

    protected override void OnComplete()
    {
        if (loopType == LoopType.Restart) curIndex = m_texts.Length - 1;
        if (m_Text != null) m_Text.text = m_texts[curIndex];
        base.OnComplete();
    }

    public override void Reset()
    {
        base.Reset();
        //m_texts = null;
        //if (m_texts.Length <= 0)
        //{
        //    string str = m_Text.text.Replace(".", string.Empty);
        //    m_texts = new string[] {
        //        str,
        //        str + ".",
        //        str + "..",
        //        str + "..."
        //    };
        //}
    }

}
