using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Serialization;
using UnityEngine;

[Serializable]
public class Profile
{
    public List<ProfileCreature> creatures = new List<ProfileCreature>();

    public void Save()
    {
        string json = JsonUtility.ToJson(this);

        using (FileStream fs = new FileStream(Application.persistentDataPath + "/save.xml", FileMode.Create))
        {
            using (StreamWriter writer = new StreamWriter(fs))
            {
                writer.Write(json);
            }
        }
    }
}

[Serializable]
public class ProfileCreature
{
    public string pet;
    public string petName;
    public int experience = 0;

    public ProfileCreature(string pet, string petName = "", int experience = 0)
    {
        this.pet = pet;
        this.petName = petName;
        this.experience = experience;
    }

    public int level
    {
        get
        {
            for (int i = 1; i < 10; i++)
            {
                if ((300 * 2 ^ (i - 1)) > experience)
                {
                    return i;
                }
            }
            return 1;
        }
    }
}