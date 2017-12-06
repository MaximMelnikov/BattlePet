using System;
using UnityEngine;

public class Ability
{
    [HideInInspector]
    public int cooldown;
    [HideInInspector]
    public AbilitySettings AbilitySettings;

    public Ability(AbilitySettings abilitySettings)
    {
        Match.Instance.onTurnEnd += OnTurnEnd;
        AbilitySettings = abilitySettings;
        cooldown = abilitySettings.maxCooldown;
    }

    void OnTurnEnd(object sender, EventArgs e)
    {
        if (cooldown < AbilitySettings.maxCooldown)
        {
            cooldown++;
        }
    }

    public void Cast(Creature caster, Creature target, Action onEnd)
    {
        if (target.hp > 0)
        {
            //if ((target.IsMyCreature && (AbilitySettings.friendCast)) || (!target.IsMyCreature && AbilitySettings.enemieCast))
            //{
            cooldown = 0;

            if (AbilitySettings.target == AbilitySettings.TargetType.enemyPets)
            {
                foreach (var i in Match.Instance.players[caster.IsMyCreature ? 1 : 0].selectedCreatures)
                {
                    Spell spell = new Spell(caster, AbilitySettings);
                    Match.Instance.StartCoroutine(spell.AffectTarget(i, onEnd, true));
                }
            }
            else if (AbilitySettings.target == AbilitySettings.TargetType.myPets)
            {
                foreach (var i in Match.Instance.players[caster.IsMyCreature ? 0 : 1].selectedCreatures)
                {
                    Spell spell = new Spell(caster, AbilitySettings);
                    Match.Instance.StartCoroutine(spell.AffectTarget(i, onEnd, true));
                }
            }
            else
            {
                Spell spell = new Spell(caster, AbilitySettings);
                Match.Instance.StartCoroutine(spell.AffectTarget(target, onEnd, true));
            }
            //}
        }
        else
        {
            onEnd.Invoke();
        }
    }
}