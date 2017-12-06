using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

/// <summary>
/// HP, energy and buff indicators
/// </summary>
public class CreatureBar : MonoBehaviourEx {
    private int fullHP;
    private int currentHP;
    private int currentPower;
    [SerializeField]
    private TextMeshPro hpText;
    [SerializeField]
    private TextMeshPro powerText;
    [SerializeField]
    private SpriteMask hpMask;
    [SerializeField]
    private SpriteMask powerMask;
    [SerializeField]
    private BuffPrefab buffPrefab;
    public List<BuffPrefab> buffs = new List<BuffPrefab>();

    public void Init(int _fullHP)
    {
        fullHP = _fullHP;
        currentHP = fullHP;
        currentPower = 100;
        SetHP(currentHP);
        SetPower(currentPower);
    }

    public void SetHP(int val)
    {
        float scaledValue = ((0.9f / fullHP) * val);
        hpText.text = val + "/" + fullHP;
        SetIndicator(1-scaledValue, hpMask);
    }

    public void SetPower(int val)
    {
        float scaledValue = ((0.9f / 100) * val);
        powerText.text = val + "/100";
        SetIndicator(1-scaledValue, powerMask);
    }

    private void SetIndicator(float val, SpriteMask mask)
    {
        DOTween.To(() => mask.alphaCutoff, x=> mask.alphaCutoff = x, val, 0.3f);
    }

    public void AddBuff(Spell spell)
    {
        BuffPrefab b = Instantiate(buffPrefab, buffPrefab.transform.parent);
        b.spell = spell;
        b.icon.sprite = spell.AbilitySettings.icon;
        b.transform.localRotation = Quaternion.Euler(0,0, 73);
        b.gameObject.SetActive(true);
        buffs.Add(b);
        UpdateBuffPosition();
    }

    public BuffPrefab FindBuff(Spell spell)
    {
        foreach (var item in buffs)
        {
            if (item.spell == spell)
            {
                return item;
            }
        }
        return null;
    }

    public BuffPrefab FindBuff(string name)
    {
        foreach (var item in buffs)
        {
            if (item.spell.AbilitySettings.name == name)
            {
                return item;
            }
        }
        return null;
    }

    public void RemoveBuff(Spell spell)
    {
        foreach (var item in buffs)
        {
            if (item.spell == spell)
            {
                buffs.Remove(item);
                item.transform.DOScale(0, 0.1f).OnComplete(()=> {
                    Destroy(item.gameObject);
                    UpdateBuffPosition();
                });
                return;
            }
        }
    }

    float radius = 1.6f;
    void UpdateBuffPosition()
    {
        for (int i = 0; i < buffs.Count; i++)
        {
            float angle = 12-i * Mathf.PI * 2f / 12;
            Vector2 newPos = new Vector2(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius);
            buffs[i].transform.localPosition = newPos;
        }
    }
}