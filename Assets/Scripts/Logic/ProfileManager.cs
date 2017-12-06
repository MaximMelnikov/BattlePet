using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ProfileManager {
    public static ProfileManager Instance;
    public Profile lastLoadedProfile;

    public ProfileManager()
    {
        Instance = this;
        if (!File.Exists(Application.persistentDataPath + "/save.xml"))
        {
            CreateRandomProfile().Save();
        }
    }

    public Profile CreateRandomProfile()
    {
        Profile profile = new Profile();

        List<string> charList = new List<string>();
        charList.Add("Wolf");
        charList.Add("Dragon");
        charList.Add("Ghost");
        charList.Add("Mummy");
        charList.Add("Orc");
        charList.Add("Skeleton");
        charList.Add("Zombie");
        int rand = UnityEngine.Random.Range(0, charList.Count);
        profile.creatures.Add(new ProfileCreature(charList[rand]));
        charList.RemoveAt(rand);
        rand = UnityEngine.Random.Range(0, charList.Count);
        profile.creatures.Add(new ProfileCreature(charList[rand]));
        charList.RemoveAt(rand);
        rand = UnityEngine.Random.Range(0, charList.Count);
        profile.creatures.Add(new ProfileCreature(charList[rand]));

        return profile;
    }

    public Profile Load(string json = null)
    {
        try
        {
            if (String.IsNullOrEmpty(json))
            {
                using (FileStream fs = new FileStream(Application.persistentDataPath + "/save.xml", FileMode.Open))
                {
                    using (StreamReader reader = new StreamReader(fs))
                    {
                        Profile p = Deserealize(reader.ReadToEnd());
                        lastLoadedProfile = p;
                        return p;
                    }
                }
            }
            else
            {
                return Deserealize(json);
            }
            
        }
        catch (Exception)
        {
            Debug.LogError("ProfileCorrupted");

            throw;
        }        
    }

    Profile Deserealize(string json)
    {
        return JsonUtility.FromJson<Profile>(json);
    }
}
