using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
//Sam Armstrong
public class PlayNameInput : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TMP_InputField nameInputField = null;
    [SerializeField] private Button continueButton = null;
    public GameObject cannonBall;
    public GameObject cannonBallClone;
    public Transform confirmbuttonCannonSpawnLocation;
    public Transform joinLobbyCannonSpawnLocation;
    

    public static string DisplayName { get; private set; }

    private const string PlayerPrefsNameKey = "PlayerName";

    private void Start() => SetUpInputField();

    private void SetUpInputField()
    {
        if (!PlayerPrefs.HasKey(PlayerPrefsNameKey)) { return; }

        string defaultName = PlayerPrefs.GetString(PlayerPrefsNameKey);

        nameInputField.text = defaultName;

        SetPlayerName(defaultName);
    }

    public void SetPlayerName(string name)
    {
        continueButton.interactable = !string.IsNullOrEmpty(name);
    }

    public void SavePlayerName()
    {
        DisplayName = nameInputField.text;

        PlayerPrefs.SetString(PlayerPrefsNameKey, DisplayName);


        cannonBallClone = (GameObject)Instantiate(cannonBall, confirmbuttonCannonSpawnLocation.position, confirmbuttonCannonSpawnLocation.rotation);
        cannonBallClone.gameObject.transform.localScale += new Vector3(10, 10, 10);
        cannonBallClone.GetComponent<Rigidbody>().AddRelativeForce(Vector3.forward * 10000000); 

    }
    public void joinLobbyButtonCannonSpawn()
    {
        cannonBallClone = (GameObject)Instantiate(cannonBall, joinLobbyCannonSpawnLocation.position, joinLobbyCannonSpawnLocation.rotation);
        cannonBallClone.gameObject.transform.localScale += new Vector3(10, 10, 10);
        
        cannonBallClone.GetComponent<Rigidbody>().AddRelativeForce(Vector3.forward * 10000000);
    }
}
