using System;
using System.Collections;
using UnityEngine;

public class Spell {
    public AbilitySettings AbilitySettings;
    public int castingTime;
    public int duration;
    public Creature caster;
    private const float LEVELMODIFICATOR = 0.8f;
    private const float STEPTIME = 2f;
    private const int CRIT_MODIFICATOR = 1; //change to x2. Dont work with networking at the moment
    private GameObject effect;

    public Spell(Creature _caster, AbilitySettings _spellBase)
    {
        caster = _caster;
        AbilitySettings = _spellBase.GetCopy();
        AbilitySettings.damage = LevelModificator(AbilitySettings.damage);
        AbilitySettings.heal = LevelModificator(AbilitySettings.heal);
        duration = 0;
        Match.Instance.onTurnEnd += OnTurnEnd;
    }
    
    /// <summary>
    /// increases spell damage by level
    /// </summary>
    /// <param name="level"></param>
    /// <returns></returns>
    int LevelModificator(int level)
    {
        int value = level;
        if (caster.level > 1)
        {
            value = (int)(level * caster.level * LEVELMODIFICATOR);
        }
        return value;
    }

    public bool usedInThisStep = false;
    Creature _target;
    public IEnumerator AffectTarget(Creature target, Action onEnd, bool firstStep = false)
    {
        _target = target;
        float timer = STEPTIME;
        if (!usedInThisStep)
        {
            usedInThisStep = true;
            duration++;
            AbilitySettings targetCalculations = CalculateTarget(target);
            AbilitySettings casterCalculations = CalculateTarget(caster);

            if (firstStep)
            {
                BuffPrefab s = target.creatureBar.FindBuff(AbilitySettings.spellName);
                if (s != null)
                {
                    s.spell.AbilitySettings = AbilitySettings;
                    s.spell.caster = caster;
                    s.spell.duration = 0;
                    s.SetCooldown(duration);
                    onEnd.Invoke();
                    timer += STEPTIME;
                    yield break; //if same effect is on target - renew duration
                }
                else if (AbilitySettings.maxDuration != 1)
                {
                    target.creatureBar.AddBuff(this);
                    target.creatureBar.FindBuff(this).SetCooldown(duration);
                }

                if (AbilitySettings.effect)
                {
                    effect = MonoBehaviour.Instantiate(AbilitySettings.effect);
                    if (AbilitySettings.casterEffect)
                    {
                        effect.transform.position = caster.transform.position;                                               
                    }
                    else
                    {
                        effect.transform.position = target.transform.position;
                    }
                    effect.transform.LookAt(target.transform);
                    effect.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
                }
            }
            else
            {
                target.creatureBar.FindBuff(this).SetCooldown(duration);
            }
            
            if (AbilitySettings.clearance && firstStep)
            {
                timer += STEPTIME;
                Gui.Instance.scrollLog.Show(AbilitySettings.spellName + "(clearance)", new Color(173, 0, 0), new Vector3(target.transform.position.x, target.transform.position.y + 2, target.transform.position.z), timer);
            }
            if (AbilitySettings.stun && firstStep)
            {
                timer += STEPTIME;
                Gui.Instance.scrollLog.Show(AbilitySettings.spellName + "(disoriented)", new Color(173, 0, 0), new Vector3(target.transform.position.x, target.transform.position.y + 2, target.transform.position.z), timer);
            }
            if (AbilitySettings.heal > 0)
            {
                int critMod = 1;
                if (casterCalculations.critBuff > 0 && UnityEngine.Random.Range(0, 100) < casterCalculations.critBuff + AbilitySettings.healCritChance)
                {
                    critMod = CRIT_MODIFICATOR;
                }
                timer += STEPTIME;
                int heal = AbilitySettings.healCritChance * critMod;
                target.Heal(heal);
                Gui.Instance.scrollLog.Show(AbilitySettings.spellName + " restore " + heal + "hp", new Color(66, 184, 36), new Vector3(target.transform.position.x, target.transform.position.y + 2, target.transform.position.z), timer);
                
            }
            if (AbilitySettings.damage > 0)
            {
                timer += STEPTIME;
                int critMod = 1;
                if (casterCalculations.critBuff > 0 && UnityEngine.Random.Range(0, 100) < casterCalculations.critBuff + AbilitySettings.attackCritChance)
                {
                    critMod = CRIT_MODIFICATOR;
                }
                int damage = (int)(AbilitySettings.damage + (AbilitySettings.damage * casterCalculations.attackBuff * 0.01f)) * critMod;
                if (caster.strongAgainst.Contains(target.creatureBase.type[0]))
                {
                    damage = Mathf.RoundToInt(damage * 1.25f);
                }
                damage -= (damage / 100 * targetCalculations.resistancePercent);
                target.Damage(damage);
                if (firstStep)
                {
                    caster.anim.Play(caster.creatureBase.attackAnims[0]);
                    caster.anim.PlayQueued(caster.creatureBase.idleAnims[0], QueueMode.CompleteOthers);
                }
                Gui.Instance.scrollLog.Show(AbilitySettings.spellName + " deals " + damage + " damage", new Color(173, 0, 0), new Vector3(target.transform.position.x, target.transform.position.y + 2, target.transform.position.z), timer);                
            }
        }

        yield return new WaitForSeconds(timer);
        if (!AbilitySettings.removeOnTurnEnd)
        {
            CheckForRemmove();
        }
        onEnd.Invoke();
    }

    void OnTurnEnd(object sender, EventArgs e)
    {
        usedInThisStep = false;
        if (AbilitySettings.removeOnTurnEnd)
        {
            CheckForRemmove();
        }
        MonoBehaviour.Destroy(effect);
    }

    void CheckForRemmove()
    {
        if (duration >= AbilitySettings.maxDuration)
        {
            _target.creatureBar.RemoveBuff(this);
        }
    }

    /// <summary>
    /// Calculat target resistances and how buffs/debuffs modifies target stats
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
    AbilitySettings CalculateTarget(Creature target)
    {
        AbilitySettings temp = AbilitySettings.GetCopy();
        foreach (var i in target.creatureBar.buffs)
        {
            int attackBuff = 0;
            foreach (var item in i.spell.AbilitySettings.damageBuffType)
            {
                if (HasAnyFlagInCommon(item, temp.damageType) && i.spell.AbilitySettings.attackBuff > attackBuff)
                {
                    attackBuff = i.spell.AbilitySettings.attackBuff;
                }
            }
            temp.attackBuff += attackBuff;
            temp.critBuff += i.spell.AbilitySettings.critBuff;
            temp.speedBuff += i.spell.AbilitySettings.speedBuff;

            int resistancePercent = 0;
            foreach (var item in i.spell.AbilitySettings.resistanceType)
            {
                if (HasAnyFlagInCommon(item, temp.damageType) && i.spell.AbilitySettings.resistancePercent > resistancePercent)
                {
                    resistancePercent = i.spell.AbilitySettings.resistancePercent;
                }
            }
            temp.resistancePercent += resistancePercent;
        }
        return temp;
    }
    /// <summary>
    /// Bollean and return true if param "type" contains param "value"
    /// </summary>
    /// <param name="type"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    bool HasAnyFlagInCommon(Classification.Type type, Classification.Type value)
    {
        bool b = ((type) & (value)) == value;
        return b;
    }
}