using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class TooltipController : MonoBehaviour {
    public RectTransform m_Rect = null;
    public TextMeshProUGUI TooltipText = null;
    private CanvasGroup CanvasGroup;
    private bool m_Show = false;

    void Awake()
    {
        instance = this;
        CanvasGroup = GetComponent<CanvasGroup>();
    }
    Vector2 mp = Vector2.zero;
    string text = "";
    public void ShowToolTip(bool b, Vector2 pos, float wait = 0.0f, Sprite s = null, string t = "")
    {
        m_Show = b;
        if (m_Show)
        {
            mp = MousePosition(pos);
            text = t;
        }
        if (b)
        {
            Invoke("Tooltip", wait);
        }
        else
        {
            CancelInvoke("Tooltip");
            AnimatedState();
        }
    }

    void Tooltip()
    {
        m_Rect.position = mp;
        AnimatedState();
    }
    
    public void AnimatedState()
    {
        if (m_Show)
        {
            TooltipText.text = text;
            CanvasGroup.DOKill();
            CanvasGroup.DOFade(1, 0.2f);
        }
        else
        {
            CanvasGroup.DOKill();
            CanvasGroup.DOFade(0, 0.2f);
        }
    }

    private Vector2 MousePosition(Vector2 v)
    {
#if UNITY_ANDROID
        Vector2 OffSet = new Vector3(200, 100);
#else
        Vector2 OffSet = new Vector3(20, 10);
#endif

        if ((v.x - (m_Rect.sizeDelta.x)) > 0)
        {
            v.x -= (m_Rect.sizeDelta.x/2) + OffSet.x;
        }
        else
        {
            v.x += (m_Rect.sizeDelta.x/2) + OffSet.x;
        }

        if ((v.y - (m_Rect.sizeDelta.y/2)) < 0)
        {
            v.y = (v.y - (m_Rect.sizeDelta.y / 2)) - OffSet.y;
        }
        else if ((v.y + (m_Rect.sizeDelta.y / 2)) > Screen.height)
        {
            v.y -= (v.y + (m_Rect.sizeDelta.y / 2)) - Screen.height;
        }

        return v;
    }
    
    private static TooltipController instance;
    public static TooltipController Instance
    {
        get
        {
            if (instance == null)
                instance = GameObject.FindObjectOfType<TooltipController>();
            return instance;
        }
    }
}