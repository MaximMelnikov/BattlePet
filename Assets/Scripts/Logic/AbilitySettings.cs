using System.Collections.Generic;
using UnityEngine;

public class AbilitySettings : ScriptableObject {

    #region Spell
    public string spellName;
    public string description;
    public Sprite icon;
    public bool selfCastOnly = false;
    public bool friendCast = true;
    public bool enemieCast = true;
    public bool removeOnTurnEnd = false;
    #endregion

    #region Buffs
    [Space(10)]
    public int attackBuff;
    public int critBuff;
    public int speedBuff;
    public List<Classification.Type> damageBuffType;
    #endregion

    #region SpellTimings
    [Space(10)]
    public int maxCastingTime;
    public bool castingCanBeInterrupted;    
    public int maxDuration; // 0 is infinity
    public bool canBeInterrupted;    
    public int maxCooldown = 1;
    #endregion

    #region SpellAttack
    [Space(10)]
    public int damage;
    public int attackCritChance;
    public Classification.Type damageType;
    #endregion

    #region SpellHeal
    [Space(10)]
    public int heal;
    public int healCritChance;
    #endregion

    #region SpellResistance
    [Space(10)]
    public int resistancePercent;
    public List<Classification.Type> resistanceType;
    #endregion

    #region SpellClearance
    [Space(10)]
    public bool clearance;
    #endregion
    [Space(10)]
    public bool stun;
    public List<AbilitySettings> linkedSpells;

    #region VisualEffect
    public GameObject effect;
    public bool casterEffect;
    #endregion

    public TargetType target;
    public int cost;

    public enum TargetType
    {
        myPet,
        myPets,
        friendlyPets,
        enemyPet,
        enemyPets,
        enemiesPets
    }

    public AbilitySettings GetCopy()
    {
        return MemberwiseClone() as AbilitySettings;
    }
}