using UnityEngine;
using TMPro;

public class BuffPrefab : MonoBehaviour {
    public Spell spell;
    public SpriteRenderer icon;
    [SerializeField]
    private TextMeshPro text;

    public void SetCooldown(int duration)
    {
        if (spell.AbilitySettings.maxDuration == 0 || spell.AbilitySettings.maxDuration == duration)
        {
            text.gameObject.SetActive(false);
        }
        else
        {
            text.text = (spell.AbilitySettings.maxDuration - duration).ToString();
        }
    }
}