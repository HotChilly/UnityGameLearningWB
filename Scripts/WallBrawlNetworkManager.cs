using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;
using UnityEngine.SceneManagement;
//Sam Armstrong
public class WallBrawlNetworkManager : NetworkManager
{

    [SerializeField] private int minPlayers = 1;
    [Scene] [SerializeField] private string menuScene = string.Empty;
    [Header("Room")]
    [SerializeField] private NetworkRoomPlayer roomPlayer = null;

    [Header("Game")]
    [SerializeField] private PlayerMovementController gamePlayerPrefab = null;
    [SerializeField] private GameObject playerSpawnSystem = null;

    public static event Action OnClientConnected;
    public static event Action OnClientDisconnected;
    public static event Action<NetworkConnection> OnServerReadied;
    public int roomIDIndexer = 0;

    public List<NetworkRoomPlayer> roomPlayers { get; } = new List<NetworkRoomPlayer>();
    public List<PlayerMovementController> GamePlayers { get; } = new List<PlayerMovementController>();

    //disctionary of connections and player names
    public static IDictionary<NetworkConnection, string> DicDisplayNames = new Dictionary<NetworkConnection, string>();
    //dictionary of connections and team choices
    public static IDictionary<NetworkConnection, bool> DicTeamChoices = new Dictionary<NetworkConnection, bool>();


    //trying to add info to a shared list on server. Is this list shared for all connections on server? Or would it be a separateinstance of the list for each client....Might need syncvar stuff to make it happen. 
    public static List<Array> listOfPlayerStats = new List<Array>();
    

    public override void OnClientConnect(NetworkConnection conn)
    {
        base.OnClientConnect(conn);

        OnClientConnected?.Invoke();
    }

    public override void OnClientDisconnect(NetworkConnection conn)
    {
        base.OnClientDisconnect(conn);

        OnClientDisconnected?.Invoke();
    }

    public override void OnServerConnect(NetworkConnection conn)
    {
        if(numPlayers >= maxConnections)
        {
            conn.Disconnect();
            return;
        }
        if("Assets/Scenes/" + SceneManager.GetActiveScene().name + ".unity" != menuScene)
        {
            conn.Disconnect();
            return;
        }
        
    }

    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        
        if("Assets/Scenes/" + SceneManager.GetActiveScene().name + ".unity" == menuScene)
        {

            bool isLeader = roomPlayers.Count == 0;

            NetworkRoomPlayer roomPlayerInstance = Instantiate(roomPlayer);
            roomPlayerInstance.IsLeader = isLeader;
            NetworkServer.AddPlayerForConnection(conn, roomPlayerInstance.gameObject);
            //ADDING ROOMPLAYERS TO LIST ON NETWORKMANAGER HERE NOW FOR SERVER BUILDS TO WORK
            roomPlayers.Add(roomPlayerInstance);
        }
    }

    public override void OnServerDisconnect(NetworkConnection conn)
    {
        if (conn.identity != null)
        {
            var player = conn.identity.GetComponent<NetworkRoomPlayer>();
            var playerInGame = conn.identity.GetComponent<PlayerMovementController>();

            roomPlayers.Remove(player);
            GamePlayers.Remove(playerInGame);
            NotifyPlayersOfReadyState();

            //setting server to lobby if all players exit game
            Debug.Log("roomPlayers.Count ===== "+roomPlayers.Count);
            Debug.Log("gamePlayers.Count ===== "+GamePlayers.Count);
            if(roomPlayers.Count == 0 && GamePlayers.Count == 0)
            {
                ServerChangeScene("Lobby2");
            }
            
        }
        base.OnServerDisconnect(conn);
    }

    public override void OnStopServer()
    {
        roomPlayers.Clear();
        
    }

    public void NotifyPlayersOfReadyState()
    {
        foreach (var player in roomPlayers)
        {
            player.HandleReadyToStart(IsReadyToStart());
        }
    }

    private bool IsReadyToStart()
    {
        if (numPlayers < minPlayers) { return false; }

        foreach(var player in roomPlayers)
        {

            //getting rid of this so server"player" doesnt stop button interaction
            if(!player.IsReady) { return false; }
        }
        return true;
    }
    public void StartGame()
    {
        if("Assets/Scenes/" + SceneManager.GetActiveScene().name + ".unity" == menuScene)
        {
            if(!IsReadyToStart()) { return; }

            ServerChangeScene("WallBrawl");
        }
    }
    public void StartIceGame()
    {
        if ("Assets/Scenes/" + SceneManager.GetActiveScene().name + ".unity" == menuScene)
        {
            if (!IsReadyToStart()) { return; }

            ServerChangeScene("IceBrawl");
        }
    }
    public void StartSkyGame()
    {
        if ("Assets/Scenes/" + SceneManager.GetActiveScene().name + ".unity" == menuScene)
        {
            if (!IsReadyToStart()) { return; }

            ServerChangeScene("SkyBrawl");
        }
    }

    //not the problem
    public override void ServerChangeScene(string newSceneName)
    {
        
        if ("Assets/Scenes/" + SceneManager.GetActiveScene().name + ".unity" == menuScene && newSceneName.StartsWith("WallBrawl")) {
            
            for (int i = roomPlayers.Count - 1; i>=0; i--)
            {
                var conn = roomPlayers[i].connectionToClient;
                //var gameplayerInstance = Instantiate(gamePlayerPrefab);
                //gameplayerInstance.SetDisplayName(roomPlayers[i].DisplayName);
                DicDisplayNames.Add(conn, roomPlayers[i].DisplayName);
                DicTeamChoices.Add(conn, roomPlayers[i].TeamChoice);
                Debug.Log(conn.identity.gameObject);
                NetworkServer.Destroyimmediate(conn.identity.gameObject);
                
                //NetworkServer.ReplacePlayerForConnection(conn, gameplayerInstance.gameObject, true);
            }
            roomPlayers.Clear();

        }
        if (newSceneName.StartsWith("Lobby2"))
        {
            for (int i = GamePlayers.Count - 1; i >= 0; i--)
            {
                var conn = GamePlayers[i].connectionToClient;
                DicDisplayNames.Clear();
                DicTeamChoices.Clear();
                Debug.Log(conn.identity.gameObject);
                NetworkServer.Destroyimmediate(conn.identity.gameObject);
            }
            GamePlayers.Clear();
        }
        
        if(newSceneName.StartsWith("IceBrawl"))
        {
            for (int i = roomPlayers.Count - 1; i >= 0; i--)
            { 
                var conn = roomPlayers[i].connectionToClient;
                //var gameplayerInstance = Instantiate(gamePlayerPrefab);
                //gameplayerInstance.SetDisplayName(roomPlayers[i].DisplayName);
                DicDisplayNames.Add(conn, roomPlayers[i].DisplayName);
                DicTeamChoices.Add(conn, roomPlayers[i].TeamChoice);
                NetworkServer.Destroyimmediate(conn.identity.gameObject);
            }
            roomPlayers.Clear();
        }

        if (newSceneName.StartsWith("SkyBrawl"))
        {
            for (int i = roomPlayers.Count - 1; i >= 0; i--)
            {
                var conn = roomPlayers[i].connectionToClient;
                //var gameplayerInstance = Instantiate(gamePlayerPrefab);
                //gameplayerInstance.SetDisplayName(roomPlayers[i].DisplayName);
                DicDisplayNames.Add(conn, roomPlayers[i].DisplayName);
                DicTeamChoices.Add(conn, roomPlayers[i].TeamChoice);
                NetworkServer.Destroyimmediate(conn.identity.gameObject);
            }
            roomPlayers.Clear();
        }
        base.ServerChangeScene(newSceneName);
    }

    public override void OnServerSceneChanged(string sceneName)
    {
        if(sceneName.StartsWith("WallBrawl"))
        {
            GameObject playerSpawnSystemInstance = Instantiate(playerSpawnSystem);
            NetworkServer.Spawn(playerSpawnSystemInstance);
        }
        if (sceneName.StartsWith("IceBrawl"))
        {
            GameObject playerSpawnSystemInstance = Instantiate(playerSpawnSystem);
            NetworkServer.Spawn(playerSpawnSystemInstance);
        }
        if (sceneName.StartsWith("SkyBrawl"))
        {
            GameObject playerSpawnSystemInstance = Instantiate(playerSpawnSystem);
            NetworkServer.Spawn(playerSpawnSystemInstance);
        }
    }

    public override void OnServerReady(NetworkConnection conn)
    {
        base.OnServerReady(conn);

        OnServerReadied?.Invoke(conn);
    }

}
