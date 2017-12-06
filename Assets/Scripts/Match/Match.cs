using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Match : MonoBehaviour {
    public static Match Instance;
    public List<Player> players = new List<Player>();
    public List<Transform> enemySpawners;
    public List<Transform> mySpawners;
    public bool turning;
    public EventHandler onTurnEnd;
    public List<TextMeshProUGUI> text;
    public Button readyBtn;

    private void Awake()
    {
        Instance = this;
        
        if (ProfileManager.Instance == null)
        {
            new ProfileManager();
        }        
        
        players.Add(new Player());
        players[0].profile = ProfileManager.Instance.Load();
        players[0].isMe = true;
        players[0].spawners = mySpawners;
        foreach (var i in mySpawners)
        {
            i.gameObject.SetActive(false);
        }

        if (NetManager.Instance == null) // for editor test only
        {
            AddEnemyPlayer();
        }
        else
        {            
            NetManager.Instance.SendPlayerInfo(String.Join(",", players[0].profile.creatures.Select(s => s.pet.ToString()).ToArray()));
        }
    }

    public void AddEnemyPlayer(Profile profile = null)
    {        
        players.Add(new Player());
        if (profile == null)
        {
            players[1].profile = ProfileManager.Instance.CreateRandomProfile();
        }
        else
        {
            players[1].profile = profile;                     
        }
        
        players[1].spawners = enemySpawners;

        foreach (var i in enemySpawners)
        {
            i.gameObject.SetActive(false);
        }
        foreach (var i in players)
        {
            i.SelectCreatures();
        }
        foreach (var creature in players[0].selectedCreatures)
        {
            creature.AutoTargeting();
        }
    }
    

    /// <summary>
    /// Counts how many players press Ready button
    /// </summary>
    int _playersReady;
    public int playersReady
    {
        get { return _playersReady; }
        set { _playersReady = value;
            if (_playersReady == players.Count)
            {
                MakeTurn();
            }
        }
    }

    public void ReadyBtn()
    {
        readyBtn.interactable = false;        
        text[0].text = "You are ready";
        text[0].color = Color.green;
        if (NetManager.Instance == null) // for editor test only
        {
            MakeTurn();
        }
        else
        {
            playersReady++;
            NetManager.TurnMsg msg = new NetManager.TurnMsg();
            foreach (var i in players[0].selectedCreatures)
            {
                if (i.selectedSpell != null)
                {
                    msg.list.Add(new NetManager.TurnMsg.TurnMsgElement(i.target.IsMyCreature? i.target.id - 3: i.target.id + 3, i.selectedSpell.AbilitySettings.spellName));
                }
                else
                {
                    msg.list.Add(new NetManager.TurnMsg.TurnMsgElement(i.target.id, ""));
                }
            }
            NetManager.Instance.SendPlayerTurn(msg);
        }
    }
    /// <summary>
    /// Queue of creatures that should proceed on this turn
    /// </summary>
    List<Creature> queue = new List<Creature>();
    public void MakeTurn()
    {
        text[0].text = "You are ready";
        text[0].color = Color.green;
        
        turning = true;
        foreach (var player in players)
        {
            foreach (var i in player.selectedCreatures)
            {
                queue.Add(i);
            }
        }
        NextStep();
    }

    /// <summary>
    /// In each step we finding fastest creature(speed>>>) and this creature cast spells or affects by buffs/debuffs. After that creature pass turn to another creature.
    /// When creatures is over current turn is considered to be over. Match is ended when one of players have no hp;
    /// </summary>
    public void NextStep()
    {
        if (queue.Count > 0)
        {
            Creature fastest = FindFastestCreature();
            queue.Remove(fastest);
            fastest.StartStep();
        }
        else if (turning == true)
        {
            readyBtn.interactable = true;
            text[0].text = "My pets";
            text[0].color = Color.white;
            text[1].text = "Opponent pets";
            text[1].color = Color.white;

            playersReady = 0;
            turning = false;
            onTurnEnd.Invoke(this, new EventArgs());
            foreach (var player in players)
            {
                int healthSum = 0;
                foreach (var creature in player.selectedCreatures)
                {
                    creature.SetPower(creature.power + 15);
                    healthSum += creature.hp;
                }

                if (healthSum == 0)
                {
                    if (player.isMe)
                    {
                        Gui.Instance.winDialog.Show(players[0], false);
                    }
                    else
                    {
                        Gui.Instance.winDialog.Show(players[0], true);
                    }
                }
            }
        }
    }

    private Creature FindFastestCreature()
    {
        Creature fastest = queue[0];
        for (int i = 1; i < queue.Count; i++)
        {
            if (queue[i].speed > fastest.speed)
            {
                fastest = queue[i];
            }
        };
        return fastest;
    }

    public Creature FindCreatureById(int id)
    {
        foreach (var i in players)
        {
            foreach (var j in i.selectedCreatures)
            {
                if (j.id == id)
                {
                    return j;
                }
            }
        }
        return null;
    }
}