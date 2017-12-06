using UnityEngine;

public class CreatureLoader : MonoBehaviour {
    private static ClassMatrix _matrix;
    public static ClassMatrix matrix {
        get
        {
            if (!_matrix)
            {
                _matrix = Resources.Load("StrongAgainstMatrix") as ClassMatrix;                
            }
            return _matrix;
        }
    }
    

    private static CreatureSettings LoadCreatureBase(string name)
    {
        CreatureSettings creatureBase = Resources.Load("Settings/Creatures/" + name) as CreatureSettings;
        return creatureBase;
    }

    public static Creature SpawnCreature(string name, Player owner, Transform parent = null)
    {
        CreatureSettings creatureBase = LoadCreatureBase(name);
        GameObject go = Instantiate(creatureBase.prefab);
        Creature creature = go.AddComponent<Creature>();
        creature.owner = owner;
        creature.creatureBase = creatureBase;
        if (parent)
        {
            creature.Positioning(parent);
        }
        return creature;
    }
}