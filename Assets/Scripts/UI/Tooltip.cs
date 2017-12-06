using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

using UnityEngine.UI;

public class Tooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField, TextArea(3, 10)]
    private string _m_Text = string.Empty;
    [Space(10)]
    [Range(0.0f, 5.0f)]
    public float waitForShow = 0.2f;
    public Sprite m_Icon = null;

    public virtual string Content()
    {
        return "";
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (m_Icon == null)
        {
            if (GetComponent<Image>() != null)
            {
                m_Icon = GetComponent<Image>().sprite;
            }
        }

        TooltipController.Instance.ShowToolTip(true, transform.position, waitForShow, m_Icon, _m_Text == string.Empty? Content() : _m_Text);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        TooltipController.Instance.ShowToolTip(false, transform.position);
    }
}
