using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Mirror;
//Sam Armstrong
public class PlayerUIScript : NetworkBehaviour
{

    [SerializeField] private WallBrawlNetworkManager networkManager = null;

    [Header("UI")]
    [SerializeField] private Canvas roundOverCanvas = null;
    [SerializeField] private Button readyButton = null;
    [SerializeField] private Button quitButton = null;
    [SerializeField] public PlayerMovementController pmc = null;


    private WallBrawlNetworkManager room;

    private WallBrawlNetworkManager Room
    {
        get
        {
            if (room != null) { return room; }
            return room = NetworkManager.singleton as WallBrawlNetworkManager;
        }
    }


    public void ReadyNewRound()
    {
        pmc.CallReadyNewRound();
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void ChangeViewFirstPersonButton()
    {
        pmc.ChangeCameraFirstPerson();
    }
    public void ChangeViewThirdPerson()
    {
        pmc.ChangeCameraThirdPerson();
    }


}
