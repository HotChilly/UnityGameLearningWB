using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;
using UnityEngine.UI;
//Sam Armstrong
public class NetworkRoomPlayer : NetworkBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject lobbyUI = null;
    [SerializeField] private TMP_Text[] playerNameTexts = new TMP_Text[8];
    [SerializeField] private TMP_Text[] playerReadyTexts = new TMP_Text[8];
    [SerializeField] private TMP_Text[] playerTeamTexts = new TMP_Text[8];


    [SerializeField] private Button startGameButton = null;
    [SerializeField] private Button startGameButtonIce = null;


    [SyncVar(hook = nameof(HandleDisplayNameChanged))]
    public string DisplayName = "Loading...";
    [SyncVar(hook = nameof(HandleReadyStatusChanged))]
    public bool IsReady = false;
    [SyncVar(hook = nameof(HandleTeamChoiceChanged))]
    public bool TeamChoice = false;


    private bool isLeader;
    public bool IsLeader
    {
        set
        {
            isLeader = value;
            startGameButton.gameObject.SetActive(value);
            startGameButtonIce.gameObject.SetActive(value);
        }
    }
    private WallBrawlNetworkManager room;

    private WallBrawlNetworkManager Room
    {
        get
        {
            if (room != null) { return room; }
            return room = NetworkManager.singleton as WallBrawlNetworkManager;
        }
    }
    public override void OnStartAuthority()
    {
        CmdSetDisplayName(PlayNameInput.DisplayName);
        
        lobbyUI.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    public override void OnStartClient()
    {
        Room.roomPlayers.Add(this);
        UpdateDisplay();
    }
    public override void OnStopClient()
    {
        Room.roomPlayers.Remove(this);

        UpdateDisplay();
    }
    public void HandleReadyStatusChanged(bool oldValue, bool newValue) => UpdateDisplay();
    public void HandleDisplayNameChanged(string oldValue, string newValue) => UpdateDisplay();

    public void HandleTeamChoiceChanged(bool oldValue, bool newValue) => UpdateDisplay();

    //make two spawner lists. Add players to each list as they join. Make UI move player seperately on list swap. 

    private void UpdateDisplay()
    {
        if(!hasAuthority)
        {
            foreach(var player in Room.roomPlayers)
            {
                if (player.hasAuthority)
                {
                    player.UpdateDisplay();
                    break;
                }
            }
            return;
        }
        Debug.Log("Going through all texts");
        for (int i = 0; i < playerNameTexts.Length; i++)
        {
            //Debug.Log("Going through all texts");
            playerNameTexts[i].text = "Waiting For Player...";
            playerReadyTexts[i].text = string.Empty;
            playerTeamTexts[i].text = string.Empty;
}
        for(int i = 0; i < Room.roomPlayers.Count; i++)
        {
            //Debug.Log("Going through all existing players in Room.roomPlayers");
            playerNameTexts[i].text = Room.roomPlayers[i].DisplayName;
            playerReadyTexts[i].text = Room.roomPlayers[i].IsReady ? "<color=white>Ready</color>" : "<color=black>Not Ready</color>";
            playerTeamTexts[i].text = Room.roomPlayers[i].TeamChoice ? "<color=blue>Team Blue</color>" : "<color=red>Team Red</color>";
        }
    }
    public void HandleReadyToStart(bool readyToStart)
    {

        //TODO Make this work again
        //getting rid of this temporarily cause the leader cant be found temporarily
        if(!isLeader) { return;  }
        startGameButtonIce.interactable = readyToStart;
        startGameButton.interactable = readyToStart;
    }

    [Command]
    private void CmdSetDisplayName(string displayName)
    {
        DisplayName = displayName;
    }
    [Command]
    public void CmdSwapTeam()
    {
        TeamChoice = !TeamChoice;
        //make it so this can happen  copy how they did the ready command
        Room.NotifyPlayersOfReadyState();
    }
    [Command]
    public void CmdReadyUp()
    {
        Debug.Log("Number in Room.roomplayers: "+Room.roomPlayers.Count);
        IsReady = !IsReady;

        Room.NotifyPlayersOfReadyState();
    }
    [Command]
    public void CmdStartGame()
    {

        if (Room.roomPlayers[0].connectionToClient != connectionToClient) { return; }

        Room.StartGame();
    }

    [Command]
    public void CmdStartGameIce()
    {

        if (Room.roomPlayers[0].connectionToClient != connectionToClient) { return; }

        Room.StartIceGame();
    }
    [Command]
    public void CmdStartGameSky()
    {
        if (Room.roomPlayers[0].connectionToClient != connectionToClient) { return; }

        Room.StartSkyGame();
    }
}
