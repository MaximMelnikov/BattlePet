using UnityEngine;
using TMPro;
using System.Collections.Generic;
using DG.Tweening;

public class ScrollLog : MonoBehaviour {
    [SerializeField]
    private TextMeshProUGUI textPrefab;
    Queue<TextMeshProUGUI> cache = new Queue<TextMeshProUGUI>();
    private const float SHOWTIME = 1.5f;
    private const int CACHECOUNT = 5;

    void Start () {
        for (int i = 0; i < CACHECOUNT; i++)
        {
            TextMeshProUGUI text = Instantiate(textPrefab, transform);
            cache.Enqueue(text);
        }
	}

    public void Show(string text, Color color, Vector3 position, float time = SHOWTIME)
    {
        TextMeshProUGUI tm = cache.Peek();
        tm.text = text;
        tm.color = color;
        cache.Dequeue();
        cache.Enqueue(tm);
        tm.rectTransform.position = Camera.main.WorldToScreenPoint(position);
        tm.transform.DOMoveY(tm.rectTransform.position.y + 50, 2);
        tm.transform.DOScale(1.5f, 0.2f).OnComplete(()=> tm.transform.DOScale(1, 0.3f));
        tm.DOFade(1, 0);
        tm.DOFade(1, 0.5f).OnComplete(()=> {
            tm.DOFade(0, time);
        });
    }
}

