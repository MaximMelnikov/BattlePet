using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

/// <summary>
/// Shows creature abilities
/// </summary>
public class RadialMenu : MonoBehaviour
{
    [SerializeField]
    private SpellBtn buttonPrefab;
    [HideInInspector]
    public List<SpellBtn> buttons;
    [SerializeField]
    private CanvasGroup buttonsCanvasGroup;
    public bool Interactable { get { return buttonsCanvasGroup.interactable; } }

    private void Start()
    {
        buttonPrefab.gameObject.SetActive(false);
        gameObject.SetActive(false);
    }

    private float distance;
    private void Update()
    {
        if (gameObject.activeSelf)
        {
            Vector2 mPos = new Vector2(Input.mousePosition.x - Screen.width/2, Screen.height - Input.mousePosition.y - Screen.height / 2);
            distance = Vector2.Distance(mPos, transform.localPosition);
            Vector3 v = new Vector3(Mathf.Clamp((transform.position.y - Input.mousePosition.y) * -0.1f, -10, 10), Mathf.Clamp((transform.position.x - Input.mousePosition.x) * -0.1f, - 10, 10), 0);
            transform.rotation = Quaternion.Euler(v);
        }
    }
    
    public void Show (Creature _creature) {
        Gui.Instance.creatureTooltip.Hide();
        _creature.selectedSpell = null;
        gameObject.SetActive(true);
        transform.position = Camera.main.WorldToScreenPoint(_creature.center);

        buttonsCanvasGroup.alpha = 0;
        buttonsCanvasGroup.interactable = false;
        buttonPrefab.transform.parent.transform.localScale = Vector3.zero;
        buttonPrefab.transform.parent.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, 90f));
        float radius = 50f;
        for (int i = 0; i < _creature.abilities.Count; i++)
        {
            float angle = i * Mathf.PI * 2f / _creature.abilities.Count;
            Vector2 newPos = new Vector2(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius);
            GameObject go = Instantiate(buttonPrefab.gameObject, buttonPrefab.transform.parent);
            SpellBtn spellBtn = go.GetComponent<SpellBtn>();            
            spellBtn.creature = _creature;
            spellBtn.ability = _creature.abilities[i];
            go.SetActive(true);
            go.transform.localPosition = newPos;
            go.transform.localRotation = Quaternion.Euler(new Vector3(0,0, (angle * Mathf.Rad2Deg)-90));
            spellBtn.icon.transform.rotation = Quaternion.Euler(new Vector3(0,0,90));
            spellBtn.icon.sprite = _creature.abilities[i].AbilitySettings.icon;
            spellBtn.text.text = _creature.abilities[i].AbilitySettings.spellName;
            spellBtn.SetSpellTooltip();
            buttons.Add(spellBtn);

            if (_creature.abilities[i].AbilitySettings.cost > _creature.power)
            {
                spellBtn.Interactable = false;
            }
        }

        //animation
        buttonPrefab.transform.parent.DOKill();
        buttonPrefab.transform.parent.DOScale(Vector3.one, 0.3f);
        buttonPrefab.transform.parent.transform.DOLocalRotate(Vector3.zero, 0.3f);
        buttonsCanvasGroup.DOFade(1, 0.3f).OnComplete(()=> {
            buttonsCanvasGroup.interactable = true;
        });
    }
    
    public void Hide()
    {
        if (gameObject.activeSelf)
        {
            buttonPrefab.transform.parent.DOKill();
            buttonPrefab.transform.parent.DOScale(Vector3.zero, 0.2f);
            buttonPrefab.transform.parent.transform.DOLocalRotate(new Vector3(0, 0, -90f), 0.2f);
            buttonsCanvasGroup.interactable = false;
            foreach (var i in buttons)
            {
                i.Interactable = false;
            }
            buttonsCanvasGroup.DOFade(0, 0.2f).OnComplete(()=> {
                gameObject.SetActive(false);
                foreach (var i in buttons)
                {
                    Destroy(i.gameObject);
                }
                buttons.Clear();
            });
        }
    }
}
