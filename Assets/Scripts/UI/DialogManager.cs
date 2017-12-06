using DG.Tweening;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogManager : MonoBehaviour {
    public static DialogManager Instance;
    [SerializeField]
    private CanvasGroup Fader;
    [SerializeField]
    private RectTransform infoDialog;
    private List<RectTransform> openedDialogs = new List<RectTransform>();

    DialogManager()
    {
        Instance = this;
    }

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void OpenDialog(RectTransform rect)
    {
        openedDialogs.Add(rect);
        Fader.DOKill();
        Fader.GetComponent<Canvas>().sortingOrder = openedDialogs.Count + 2000;
        Fader.DOFade(1, 0.3f);
        Fader.blocksRaycasts = true;
        rect.DOKill();
        rect.GetComponent<Canvas>().sortingOrder = openedDialogs.Count + 2001;
        rect.gameObject.SetActive(true);
        rect.GetComponent<CanvasGroup>().alpha = 0;
        rect.GetComponent<CanvasGroup>().DOFade(1, 0.3f);
        rect.anchoredPosition = new Vector2(0, 0 - Screen.height / 2);
        rect.DOAnchorPosY(0, 0.3f);
    }

    public void OpenInfoDialog(string text, string ok = "Ok", string cancel = null, Action okAction = null, Action cancelAction = null)
    {
        GameObject gObj = Instantiate(infoDialog.gameObject, transform);
        TextMeshProUGUI info = gObj.transform.Find("InfoText").GetComponent<TextMeshProUGUI>();
        Button okBtn = gObj.transform.Find("ButtonsLayout").Find("Ok").GetComponent<Button>();
        TextMeshProUGUI okBtnText = okBtn.transform.Find("Text").GetComponent<TextMeshProUGUI>();
        Button cancelBtn = gObj.transform.Find("ButtonsLayout").Find("Cancel").GetComponent<Button>();
        TextMeshProUGUI cancelBtnText = cancelBtn.transform.Find("Text").GetComponent<TextMeshProUGUI>();
        okBtn.gameObject.SetActive(!string.IsNullOrEmpty(ok));
        cancelBtn.gameObject.SetActive(!string.IsNullOrEmpty(cancel));
        info.text = text;
        if (okAction == null)
        {
            okAction = delegate { CloseDialog(); };
        }
        if (cancelAction == null)
        {
            cancelAction = delegate { CloseDialog(); };
        }
        okBtn.onClick.AddListener(() => okAction());
        cancelBtn.onClick.AddListener(() => cancelAction());
        OpenDialog(gObj.GetComponent<RectTransform>());
    }

    public void CloseDialog()
    {
        RectTransform rect = openedDialogs[openedDialogs.Count-1];
        CloseDialog(rect);
    }

    public void CloseDialog(RectTransform rect)
    {
        openedDialogs.Remove(rect);
        rect.DOKill();

        if (openedDialogs.Count == 0)
        {
            Fader.DOKill();
            Fader.DOFade(0, 0.3f).OnComplete(() => { Fader.blocksRaycasts = false; });
        }
        rect.GetComponent<CanvasGroup>().DOFade(0, 0.3f);
        rect.DOAnchorPosY(Screen.height / 2, 0.3f).OnComplete(() => {
            rect.gameObject.SetActive(false);

            Fader.GetComponent<Canvas>().sortingOrder = openedDialogs.Count + 2000;
        });
    }
}