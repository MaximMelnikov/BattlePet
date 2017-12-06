using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using TMPro;

public class SpellBtn : MonoBehaviourEx, IPointerEnterHandler, IPointerExitHandler {
    [HideInInspector]
    public Creature creature;
    public Image icon;
    [SerializeField]
    private Image back;
    public TextMeshProUGUI text;
    [HideInInspector]
    public Ability ability;
    [HideInInspector]
    public SpellTooltip tooltip;
    private Button btn;
    public int cooldown;

    void Start()
    {
        btn = GetComponent<Button>();
        if (ability != null)
        {
            SetCooldown(ability.cooldown);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (btn.interactable)
        {
            creature.selectedSpell = ability;
        }
        
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (Gui.Instance.radialMenu.Interactable)
        {
            creature.selectedSpell = null;
        }
    }

    public void SetSpellTooltip()
    {
        tooltip.spell = ability.AbilitySettings;
    }

    public void SetCooldown(int cd)
    {
        cooldown = cd;
        back.fillAmount = (1f / ability.AbilitySettings.maxCooldown) * cooldown;
        if (cooldown < ability.AbilitySettings.maxCooldown)
        {
            Interactable = false;
        }else
        {
            Interactable = true;
        }
    }

    public bool Interactable
    {
        set {
            btn.GetComponent<Image>().raycastTarget = value;
            btn.interactable = value;
        }
    }
}