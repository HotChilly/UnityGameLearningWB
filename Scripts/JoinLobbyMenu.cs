using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
//Sam Armstrong
public class JoinLobbyMenu : MonoBehaviour
{
    [SerializeField] private WallBrawlNetworkManager networkManager = null;

    [Header("UI")]
    [SerializeField] private GameObject landingPagePanel = null;
    [SerializeField] private TMP_InputField ipAddressInputField = null;
    [SerializeField] private Button joinButton = null;

    
    private void OnEnable()
    {
        WallBrawlNetworkManager.OnClientConnected += HandleClientConnected;
        WallBrawlNetworkManager.OnClientDisconnected += HandleClientDisconnected;
    }

    private void OnDisable()
    {
        WallBrawlNetworkManager.OnClientConnected -= HandleClientConnected;
        WallBrawlNetworkManager.OnClientDisconnected -= HandleClientDisconnected;
    }


    public void JoinLobby()
    {
        string ipAddress = ipAddressInputField.text;
        Debug.Log("IP is: " + ipAddressInputField.text);
        networkManager.networkAddress = ipAddress;
        networkManager.StartClient();
        Debug.Log("in manager.startclient");
    }

    public void JoinAWS_Lobby()
    {
        string ipAddress = "ec2-100-20-122-171.us-west-2.compute.amazonaws.com";


        networkManager.networkAddress = ipAddress;
        networkManager.StartClient();
        
    }
    private void HandleClientConnected()
    {
        joinButton.interactable = true;
        gameObject.SetActive(false);
        landingPagePanel.SetActive(false);
    }
    private void HandleClientDisconnected()
    {
        joinButton.interactable = true;
    }
}
