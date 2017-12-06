using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Nakama;
using System.Text;
using System.Linq;

public class NetManager : MonoBehaviour
{
    public static NetManager Instance;
    public EventHandler OnDisconnect = delegate { };
    public EventHandler OnMatchCreated = delegate { };
    private const string SERVERHOST = "35.202.80.4";
    private const int SERVERPORT = 7350;
    private const string SERVERKEY = "defaultkey";
    private const bool SERVERSSL = false;
    // number of players participating in the match.
    private const int MATCHPLAYERSIZE = 2;
    // A custom Opcode to distinguish different message types
    private const int MATCHOPCODE = 16;
    // Nakama client
    private readonly INClient _client;
    // Matchmaking matchmakeTicket - used to cancel matchmaking request.
    private INMatchmakeTicket _matchmakeTicket;
    // Ongoing match - used to send data
    private INMatch _match;
    List<INUserPresence> connectedOpponents;
    bool isConnected;
    public bool IsConnected
    {
        get
        {
            return isConnected;
        }
        protected set
        {
            isConnected = value;
        }
    }

    NetManager()
    {
        _client = new NClient.Builder(SERVERKEY)
                .Host(SERVERHOST)
                .Port(SERVERPORT)
                .SSL(SERVERSSL)
                .Build();
    }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            DialogManager.Instance.OpenInfoDialog("Check internet connection");
        }
        else
        {
            Authentication();            
        }
    }

    private void OnApplicationQuit()
    {
        // If no matchmaking active just disconnect.
        if (_matchmakeTicket == null)
        {
            // Lets gracefully disconnect from the server.
            _client.Disconnect();
            return;
        }

        // If matchmaking active stop matchmaking then disconnect.
        var message = NMatchmakeRemoveMessage.Default(_matchmakeTicket);
        _client.Send(message, (bool done) => {
            // The user is now removed from the matchmaker pool.
            Debug.Log("Matchmaking stopped.");

            // Lets gracefully disconnect from the server.
            _client.Disconnect();
        }, (INError error) => {
            Debug.LogErrorFormat("Send error: code '{1}' with '{0}'.", error.Message, error.Code);

            // Lets gracefully disconnect from the server.
            _client.Disconnect();
        });
        OnDisconnect.Invoke(this, new EventArgs());
    }

    public EventHandler authCallback = delegate { };    
    private void Authentication(string _userName = null, string _password = null)
    {
        string id = PlayerPrefs.GetString("deviceId");
        if (string.IsNullOrEmpty(id))
        {
            // device ID for the user
            id = SystemInfo.deviceUniqueIdentifier;
            
            // Store the identifier for next game start.
            PlayerPrefs.SetString("deviceId", id);
        }
        if (GameManager.TEST)
        {
            id = SystemInfo.deviceUniqueIdentifier + System.Diagnostics.Process.GetCurrentProcess().ProcessName;
        }
        NAuthenticateMessage message = NAuthenticateMessage.Custom(id);
        _client.Login(message, (INSession session) => {
            _client.Connect(session, (bool connected) => {
            UnityMainThreadDispatcher.Instance.Enqueue(                
                () => {
                    authCallback.Invoke(this, new EventArgs());
                    isConnected = true;
                }
            );
            });
        }, (INError err) => {
            if (err.Code == ErrorCode.UserNotFound)
            {
                Registration(id);
            }
            else
            {
                UnityMainThreadDispatcher.Instance.Enqueue(()=> {
                    DialogManager.Instance.OpenInfoDialog(err.Message);
                });
                
            }
        });
    }

    public EventHandler regCallback = delegate { };    
    private void Registration(string _userName)
    {
        // This can be DeviceID, Facebook, Google, GameCenter or a custom ID.
        // For more information have a look at: https://heroiclabs.com/docs/development/user/
        var message = NAuthenticateMessage.Custom(_userName);
        _client.Register(message, (INSession session) => {
            Debug.LogFormat("Registered user.");
            // We suggest that you cache the Session object on device storage
            // so you don't have to login each time the game starts.
            // For demo purposes - we'll ignore that.
            UnityMainThreadDispatcher.Instance.Enqueue(() => regCallback.Invoke(this, new EventArgs()));
            _client.Connect(session, (bool connected) => {
                isConnected = true;
            });
        }, (INError error) => {
            Debug.LogErrorFormat("ID register '{0}' failed: {1}", _userName, error);
        });
    }

    public EventHandler matchCreatedCallback = delegate { };
    public void FindMatch()
    {
        MessageHandler();

        // Look for a match for two participants. Yourself and one more.
        var message = NMatchmakeAddMessage.Default(2);
        _client.Send(message, (INMatchmakeTicket result) => {
            Debug.Log("Added user to matchmaker pool.");

            _matchmakeTicket = result;
        }, (INError err) => {
            Debug.LogErrorFormat("Error: code '{0}' with '{1}'.", err.Code, err.Message);
        });

        _client.OnMatchmakeMatched = (INMatchmakeMatched matched) => {
            // The match token is used to join a multiplayer match.
            var JoinMessage = NMatchJoinMessage.Default(matched.Token);
            _client.Send(JoinMessage, (INResultSet<INMatch> matches) =>
            {
                Debug.Log("Successfully joined match " + matches.Results[0].Id);
                _match = matches.Results[0];
                // a list of users who've been matched as opponents.
                connectedOpponents = new List<INUserPresence>();
                // Add list of connected opponents.
                connectedOpponents.AddRange(matches.Results[0].Presence);
                // Remove your own user from list.
                connectedOpponents.Remove(matches.Results[0].Self);

                foreach (var presence in connectedOpponents)
                {
                    var userId = presence.UserId;
                    var handle = presence.Handle;
                }
                UnityMainThreadDispatcher.Instance.Enqueue(() => matchCreatedCallback.Invoke(this, new EventArgs()));
            }, (INError error) => {
                Debug.LogErrorFormat("Error: code '{0}' with '{1}'.", error.Code, error.Message);
            });
        };

        _client.OnMatchPresence = (INMatchPresence presences) => {
            // Remove all users who left.
            foreach (var user in presences.Leave)
            {
                connectedOpponents.Remove(user);
            }

            // Add all users who joined.
            connectedOpponents.AddRange(presences.Join);
        };
    }

    void MessageHandler()
    {
        _client.OnMatchData = (INMatchData m) => {
            UnityMainThreadDispatcher.Instance.Enqueue(()=> {
                Debug.Log("MessageHandler " + m.OpCode);
                switch (m.OpCode)
                {
                    case 17:
                        RecievePlayerInfo(m);
                        break;
                    case 16:
                        RecievePlayerTurn(m);
                        break;
                    default:
                        Debug.LogFormat("User handle '{0}' sent '{1}'", m.Presence.Handle, Encoding.UTF8.GetString(m.Data));
                        break;
                };
            });            
        };
    }

    /// <summary>
    /// Send to opponent info about you creatures targets and selected spells
    /// </summary>
    /// <param name="msg"></param>
    public void SendPlayerTurn(TurnMsg msg)
    {
        SendNetworkMessage(JsonUtility.ToJson(msg), 16);
    }

    [Serializable]
    public class TurnMsg
    {
        public List<TurnMsgElement> list = new List<TurnMsgElement>();
        [Serializable]
        public class TurnMsgElement
        {
            public int targetId;
            public string spellName;

            public TurnMsgElement(int targetId, string spellName)
            {
                this.targetId = targetId;
                this.spellName = spellName;
            }
        }
    }

    void RecievePlayerTurn(INMatchData msg)
    {
        TurnMsg _msg = JsonUtility.FromJson<TurnMsg>(Encoding.UTF8.GetString(msg.Data));

        for (int i = 0; i < _msg.list.Count; i++)
        {
            Match.Instance.players[1].selectedCreatures[i].target = Match.Instance.FindCreatureById(_msg.list[i].targetId);
            
            for (int j = 0; j < Match.Instance.players[1].selectedCreatures[i].abilities.Count; j++)
            {
                if (_msg.list[i].spellName != "" && Match.Instance.players[1].selectedCreatures[i].abilities[j].AbilitySettings.spellName == _msg.list[i].spellName)
                {
                    Match.Instance.players[1].selectedCreatures[i].selectedSpell = Match.Instance.players[1].selectedCreatures[i].abilities[j];
                    break;
                }
            }
        }
        Match.Instance.text[1].text = "Opponent ready";
        Match.Instance.text[1].color = Color.green;
        Match.Instance.playersReady++;
    }

    public void SendPlayerInfo(string msg)
    {
        SendNetworkMessage(msg, 17);
    }

    void RecievePlayerInfo(INMatchData msg)
    {
        Profile profile = new Profile();
        List<string> charList = Encoding.UTF8.GetString(msg.Data).Split(',').ToList();
        foreach (var item in charList)
        {
            profile.creatures.Add(new ProfileCreature(item));
        }

        Match.Instance.AddEnemyPlayer(profile);
    }

    void SendNetworkMessage(string msg, long opCode)
    {
        string matchId = _match.Id; // an INMatch Id.
        
        byte[] data = Encoding.UTF8.GetBytes(msg);

        var message = NMatchDataSendMessage.Default(matchId, opCode, data);
        _client.Send(message, (bool done) => {
            Debug.Log("Successfully sent data message.");
        }, (INError err) => {
            Debug.LogErrorFormat("Error: code '{0}' with '{1}'.", err.Code, err.Message);
        });
    }

}