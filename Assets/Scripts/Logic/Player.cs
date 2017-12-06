using System.Collections.Generic;
using UnityEngine;

public class Player {
    public Profile profile;
    public bool isMe;
    public List<Creature> selectedCreatures = new List<Creature>();
    public List<Transform> spawners;

    public void SelectCreatures()
    {
        if (profile.creatures.Count <= 3)
        {
            for (int i = 0; i < profile.creatures.Count; i++)
            {
                selectedCreatures.Add(CreatureLoader.SpawnCreature(profile.creatures[i].pet, this, spawners[i]));
                selectedCreatures[i].id = isMe ? 3 + i : i;
            }
        }
    }
}