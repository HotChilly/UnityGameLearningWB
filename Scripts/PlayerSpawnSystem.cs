using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System.Linq;
//Sam Armstrong
public class PlayerSpawnSystem : NetworkBehaviour
{
    //[SerializeField] private GameObject playerPrefab = null;
    [SerializeField] private PlayerMovementController playerPrefab = null;


    public static List<Transform> spawnPoints = new List<Transform>();
    //TODO make list of spawnpoints for other wall here, check connections in dictrionary list for connect vs teamchoice comparerr.
    public static List<Transform> spawnPointsTeamTwo = new List<Transform>();
    
    private int nextIndex = 0;
    private int nextIndexTeamTwo = 0;

    public GameObject normalReloadObject = null;
    public bool firstUpdate = true;
    //public Material redTeamColor;
    //public Material blueTeamColor;

    private WallBrawlNetworkManager room;

    private WallBrawlNetworkManager Room
    {
        get
        {
            if (room != null) { return room; }
            return room = NetworkManager.singleton as WallBrawlNetworkManager;
        }
    }

    public static void AddSpawnPoint(Transform transform)
    {
        spawnPoints.Add(transform);

        spawnPoints = spawnPoints.OrderBy(x => x.GetSiblingIndex()).ToList();
    }

    //TODO PLUG THIS IN 
    public static void AddSpawnPointTeamTwo(Transform transform)
    {
        spawnPointsTeamTwo.Add(transform);

        spawnPointsTeamTwo = spawnPointsTeamTwo.OrderBy(x => x.GetSiblingIndex()).ToList();
    }

    public static void RemoveSpawnPoint(Transform transform) => spawnPoints.Remove(transform);

    public static void RemoveSpawnPointTeamTwo(Transform transform) => spawnPointsTeamTwo.Remove(transform);

    public override void OnStartServer() => WallBrawlNetworkManager.OnServerReadied += SpawnPlayer;

    [ServerCallback]
    private void OnDestroy() => WallBrawlNetworkManager.OnServerReadied -= SpawnPlayer;

    [Server]
    public void SpawnPlayer(NetworkConnection conn)
    {

        //what team is this guy on? Do some logic probably using dictionary NetworkConnection check...
        if (WallBrawlNetworkManager.DicTeamChoices.TryGetValue(conn, out bool value)) {
        if(value == false)
            {

                Transform spawnPoint = spawnPoints.ElementAtOrDefault(nextIndex);

                if (spawnPoint == null)
                {
                    Debug.LogError("Missing spawnpoint for player");
                    return;
                }


                var playerInstance = Instantiate(playerPrefab, spawnPoints[nextIndex].position, spawnPoints[nextIndex].rotation);
                playerInstance.SetTeamChoice(value);
                playerInstance.spawnPoints = spawnPoints;
                playerInstance.playerSpawnIndex = nextIndex;

                if(nextIndex == 0 && nextIndexTeamTwo ==0)
                {
                    playerInstance.SetIsLeader(true);
                }
                //playerInstance.playerVisual.GetComponent<Renderer>().material = redTeamColor;
                foreach (KeyValuePair<NetworkConnection, string> kvp in WallBrawlNetworkManager.DicDisplayNames)
                {
                    if (WallBrawlNetworkManager.DicDisplayNames.TryGetValue(conn, out string name))
                    {
                        playerInstance.SetDisplayName(name);
                    }
                }
                NetworkServer.ReplacePlayerForConnection(conn, playerInstance.gameObject, true);
                Room.GamePlayers.Add(playerInstance);
               // WallBrawlNetworkManager.redTeamPlayerCount++;
                nextIndex++;

            }
        if(value == true)
            {
                Transform spawnPoint = spawnPointsTeamTwo.ElementAtOrDefault(nextIndexTeamTwo);
                if (spawnPoint == null)
                {
                    Debug.LogError("Missing spawnpoint for player");
                    return;
                }

                var playerInstance = Instantiate(playerPrefab, spawnPointsTeamTwo[nextIndexTeamTwo].position, spawnPointsTeamTwo[nextIndexTeamTwo].rotation);
                playerInstance.SetTeamChoice(value);
                playerInstance.spawnPoints = spawnPointsTeamTwo;
                playerInstance.playerSpawnIndex = nextIndexTeamTwo;

                if (nextIndex == 0 && nextIndexTeamTwo == 0)
                {
                    playerInstance.SetIsLeader(true);
                }
                //playerInstance.playerVisual.GetComponent<Renderer>().material = blueTeamColor;
                foreach (KeyValuePair<NetworkConnection, string> kvp in WallBrawlNetworkManager.DicDisplayNames)
                {
                    if (WallBrawlNetworkManager.DicDisplayNames.TryGetValue(conn, out string name))
                    {
                        playerInstance.SetDisplayName(name);
                        //playerInstance.playerNameText.text = value;
                    }
                }
                NetworkServer.ReplacePlayerForConnection(conn, playerInstance.gameObject, true);
                Room.GamePlayers.Add(playerInstance);
                // WallBrawlNetworkManager.blueTeamPlayerCount++;
                nextIndexTeamTwo++;
            }
        }
    }
}
