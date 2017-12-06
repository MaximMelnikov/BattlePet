using System.Collections.Generic;
using UnityEngine;

public class CreatureSettings : ScriptableObject
{
    public string title; //unique creature name
    public string petName; //player can rename pet
    public int fullHp;
    public int fullPower = 100;
    public int speed;
    public int crit;
    public int firstLevelExp = 300;
    public List<AbilitySettings> abilities;
    public List<AbilitySettings> auras;
    public List<Classification.Type> type;
    public Classification.Quality quality;

    #region visualVars
    public Sprite icon;
    public GameObject prefab;
    public List<string> idleAnims;
    public List<string> attackAnims;
    public List<string> hitAnims;
    public List<string> dieAnims;
    public List<string> disorientationAnims;
    #endregion

    /// <summary>
    /// Clone object. I use it to make ScriptableObject clone in memory. I can modify it without asset changing.
    /// </summary>
    /// <returns></returns>
    public CreatureSettings GetCopy()
    {
        return MemberwiseClone() as CreatureSettings;
    }
}