using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Sam Armstrong
public class MainMenu : MonoBehaviour
{
    [SerializeField] private WallBrawlNetworkManager networkManager = null;

    [Header("UI")]
    [SerializeField] private GameObject landingPagePanel = null;

    public void HostLobby()
    {
        networkManager.StartHost();

        landingPagePanel.SetActive(false);
    }
}
