using System.Collections.Generic;
using UnityEngine;

public class SpellTooltip : Tooltip {
    public AbilitySettings spell;
    
    public override string Content () {
        string result = "";
        string spellName = "<color=#2466D0FF><align=left>"+spell.spellName+"<line-height=0>";
        string spellCost = "\n<align=right><color=\"yellow\"><size=14>Cost: " + spell.cost + " energy<line-height=1em>";
        string spellTarget = "\n<color=\"white\"><align=\"right\"><size=12>Target: " + spell.target;
        string spellDamage = "\n<size=14><align=\"left\"><color=\"white\">" + spell.damage + " " + spell.damageType + " damage" + (spell.maxDuration>1?" per turn":"") + " (" + spell.attackCritChance + " % crit chance)";
        string spellHeal = "\n<size=14><align=\"left\"><color=\"white\">Restore " + spell.heal + " health" + (spell.maxDuration > 1 ? " per turn" : "") + " (" + spell.healCritChance + " % crit chance)";
        string spellDuration = "\n<size=14><align=\"left\"><color=\"white\">Duration: " + spell.maxDuration + " turns" + (spell.canBeInterrupted ? " (Can be interrupted)" : "");
        string spellCooldown = "\n<size=14><align=\"left\"><color=\"white\">Cooldown: " + spell.maxCooldown + " turns";
        string spellEffects = "\n<color=#37FF39FF>Effects:";
        string attackBuff = BuffString("damage", spell.attackBuff, spell.damageBuffType);
        string critBuff = BuffString("crit", spell.critBuff);
        string speedBuff = BuffString("speed", spell.speedBuff);
        string resistanceBuff = BuffString("resistance", spell.resistancePercent, spell.resistanceType);
        string clearance = "Clear all effects from target";
        string spellStun = "\nDisorient " + spell.target + " for " + (spell.maxDuration > 1 ? spell.maxDuration + "turns" : spell.maxDuration + "turn");
        string spellDescription = "\n<color=\"yellow\">" + spell.description;

        result += spellName;
        if (spell.cost>0)
        {
            result += spellCost;
        }
        result += spellTarget;
        if (spell.damage > 0)
        {
            result += spellDamage;
        }
        if (spell.heal > 0)
        {
            result += spellHeal;
        }
        if (spell.maxDuration > 0)
        {
            result += spellDuration;
        }
        if (spell.maxCooldown > 0)
        {
            result += spellCooldown;
        }
        if (spell.attackBuff != 0 || spell.critBuff != 0 || spell.resistancePercent != 0 || spell.clearance || spell.stun)
        {
            result += spellEffects;
        }
        if (spell.attackBuff != 0)
        {
            result += attackBuff;
        }
        if (spell.critBuff != 0)
        {
            result += critBuff;
        }
        if (spell.speedBuff != 0)
        {
            result += speedBuff;
        }
        if (spell.resistancePercent != 0)
        {
            result += resistanceBuff;
        }
        if (spell.clearance)
        {
            result += clearance;
        }
        if (spell.stun)
        {
            result += spellStun;
        }
        if (spell.description != "")
        {
            result += spellDescription;
        }

        return result;
    }

    string BuffString(string str, int value, List<Classification.Type> type = null)
    {
        string s ="";
        
        if (value > 0)
        {
            s += "\nIncreases ";
        }
        else if (value < 0)
        {
            s += "\nDecrease ";
        }
        if (type != null)
        {
            for (int i = 0; i < type.Count; i++)
            {
                if (type[i] != Classification.Type.All)
                {
                    s += type[i] + (i < type.Count ? ", " : " ");
                }
            }
        }        

        s += str + " by " + Mathf.Abs(value) + "%";

        return s;
    }
}