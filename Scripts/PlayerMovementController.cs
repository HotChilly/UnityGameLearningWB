using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
//Sam Armstrong

public class PlayerMovementController : NetworkBehaviour
{
    [SerializeField] private float movementSpeed = 1000f;
    [SerializeField] private CharacterController controller = null;



    private Vector2 previousInput;
    private float gravity = -9.81f;
    private float jumpHeight = 10f;
    private Vector3 velocity;

    private Controls controls;
    private Controls Controls
    {
        get
        {
            if (controls != null) { return controls; }
            return controls = new Controls();
        }
    }

    //hold position and wall object
    public bool firstWallRespawn = true;
    public Transform wallSpawnPosition;
    public GameObject wallObjectSpawnee;
    public float rateOfRespawn = 2f;
    internal float respawnDelay;

    public bool firstWallRespawnGrassland = true;
    public Transform wallSpawnPositionGrassland;
    public GameObject wallObjectSpawneeGrassland;

    public GameObject instantiatedWall;
    public GameObject instantiatedGrasslandWall;



    public Transform cannonVisual;
    public Transform shootPos;
    public GameObject sphere;
    public GameObject sphereHot;
    public GameObject sphereNoAmmo;
    public float shotPower;
    public float rateOfFire = .5f;
    internal float fireDelay;


    public Transform aimCannonPoint;

    public Rigidbody playerVisual;
    public MeshRenderer visorVisual;
    public bool alive = true;
    public Transform respawnPointOne;
    public Transform respawnPointOneVisual;

    public Transform respawnPointTwo;
    public Transform respawnPointTwoVisual;

    public GameObject sphereClone;
    public GameObject playerObject;
    public Canvas canvas;
    public GameObject respawnText;

    //player UI stuff
    public Text redTeamLives = null;
    public Text blueTeamLives = null;
    public Canvas roundOverCanvas = null;
    public Canvas escapeCanvas = null;
    public Text winningTeamText = null;
    public bool redWon = false;
    public bool blueWon = false;
    public int redDownTracker;
    public int blueDownTracker;
    public bool redOut = false;
    public bool blueOut = false;
    public bool redPlayerAllDead = false;
    public bool bluePlayerAllDead = false;
    //rename these two variables when u adjust the number of lives each team gets in order to have the server get a new number as well
    public int redTeamLivesIntNew6 = 20;
    public int blueTeamLivesIntNew6 = 20;
    public Text ammoNumberTextBox = null;
    public Text shotsTextBox = null;
    public Text whoShotMeText = null;
    

    public float delayPotentialScoreChange = 5f;
    internal float scoreChangeDelay;

    public float delayPotentialAddLifeChange = 5f;
    internal float addLifeChangeDelay;

    public float delayPotentialMegaReload = 5f;
    internal float MegaReloadChangeDelay;


    public bool cannonSpawnedChecker = false;
    public Rigidbody CannonRigidBodyToFly;
    public float lavaTransform;
    public float controlUp;
    public bool respawnImmunity = false;


    public bool doubleJumpCheck = true;
    private bool firstJump;
    public AudioClip kekw;

    public AudioClip cannonShotSound;
    public AudioClip outOfAmmonShotSound;
    public AudioClip respawnSound;
    public AudioClip reloadSound;
    public AudioClip sizzlingSound;
    public AudioClip clickClackSound;
    public AudioClip addLifeSound;
    public AudioClip lostAddLifeSound;
    public AudioClip wonGameSound;
    public AudioClip lostGameSound;
    public AudioClip jumpSound;
    public AudioClip jumpSound2;

    private bool firstUpdateOnScene = true;

    public Transform cam;
    public List<Canvas> NameCanvases = new List<Canvas>();
    public List<NetworkConnection> NWConnections = new List<NetworkConnection>();

    public bool resetGameCheck = false;
    public int playerSpawnIndex;

    //TRYING TO GET LIST OF SPAWNPOINTS INTO INDIIDUAL PLAYERS PMC's. THIS WONT WORK THIS WILL BE A CLEAN SLATE PSS. Maybe send over the information to the PMC when the PSS is spawning the PMC? 
    public List<Transform> spawnPoints;
    public List<Transform> spawnPointsVisual;

    private bool optionMenuOpen = false;

    public int normalShotsNew1 = 0;
    public int hotShots = 0;
    public bool hotShotMode = false;
    private bool emptySoundDone = true;
    private bool endGameSoundPlayed = false;

    public Material redTeamColor;
    public Material blueTeamColor;

    public Dictionary<int, string> rektwords;
    bool startGame = true;
    private bool WaitForPlayerConnectStart = true;

    //first person or follow
    private bool firstPersonMode = false;

    string playerNameListBuilder;
    //string playerKillsListBuilder;
    string playerDeathsListBuilder;
    string playerEventsListBuilder;

    private bool aliveCheck = true;

    public int deaths = 0;

    public int events = 0;
    public Text playerNameList = null;
    //public Text playerKillsList = null;
    public Text playerDeathList = null;
    public Text playerEventList = null;

    //public bool jetPack = true;

    



    [SyncVar]
    public bool teamChoice;

    public bool TeamChoice
    {
        get
        {
            return teamChoice;
        }
    }

    [SyncVar]
    public string displayName = "Loading...";

    public string DisplayName
    {
        get
        {
            return displayName;
        }
    }
    public TMP_Text playerNameText;

    private WallBrawlNetworkManager room;

    private WallBrawlNetworkManager Room
    {
        get
        {
            if (room != null) { return room; }
            return room = NetworkManager.singleton as WallBrawlNetworkManager;
        }
    }
    public override void OnStartClient()
    {
        DontDestroyOnLoad(gameObject);
        Room.GamePlayers.Add(this);

    }
    public override void OnStopClient()
    {
        Room.GamePlayers.Remove(this);
    }
    [Server]
    public void SetDisplayName(string displayname)
    {
        this.displayName = displayname;
        playerNameText.text = displayName;
    }
    [Server]
    public void SetTeamChoice(bool teamChoice)
    {
        this.teamChoice = teamChoice;

    }
    [Server]
    public void SetIsLeader(bool isThisTheLeader)
    {
        this.isLeader = isThisTheLeader;
    }

    [SyncVar]
    public bool isLeader = false;
    [SyncVar]
    public bool allPlayersReady = false;
    public bool startGameNow = false;

    public ParticleSystem hotShotModeParticles;
    public ParticleSystem normalShotEmitParticles;
    public ParticleSystem hotShotEmitParticles;
    public ParticleSystem yellowFireEmitParticles;

    public override void OnStartAuthority()
    {
        controller.detectCollisions = true;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        enabled = true;
        Controls.Player.Move.performed += ctx => SetMovement(ctx.ReadValue<Vector2>());
        Controls.Player.Move.canceled += ctx => ResetMovement();
        alive = false;

        Rigidbody rbpv = playerVisual;
        Canvas canvas = rbpv.GetComponentInChildren<Canvas>();
        canvas.enabled = false;

        lavaTransform = wallSpawnPositionGrassland.position.y + 9;
        controller.enabled = false;
        rektwords = new Dictionary<int, string>()
        {
            {1, "rekt by: " },
            {2, "blasted by: " },
            {3, "dunked on by: " },
            {4, "annihilated by: " },
            {5, "pewpewed by: " }
        };

        if (SceneManager.GetActiveScene().name == "IceBrawl")
        {
            respawnPointOne = GameObject.Find("IceRespawnPointOne").GetComponent<Transform>();
            respawnPointOneVisual = GameObject.Find("IceRespawnPointOneV").GetComponent<Transform>();
            respawnPointTwo = GameObject.Find("IceRespawnPointTwo").GetComponent<Transform>();
            respawnPointTwoVisual = GameObject.Find("IceRespawnPointTwoV").GetComponent<Transform>();
        }
        if (SceneManager.GetActiveScene().name == "SkyBrawl")
        {
            respawnPointOne = GameObject.Find("SkyRespawnPointOne").GetComponent<Transform>();
            respawnPointOneVisual = GameObject.Find("SkyRespawnPointOneV").GetComponent<Transform>();
            respawnPointTwo = GameObject.Find("SkyRespawnPointTwo").GetComponent<Transform>();
            respawnPointTwoVisual = GameObject.Find("SkyRespawnPointTwoV").GetComponent<Transform>();
        }
    }



    [ClientCallback]
    private void OnEnable() => Controls.Enable();
    [ClientCallback]
    private void OnDisable() => Controls.Disable();
    [ClientCallback]
    private void Update()
    {
        
        controlUp = playerVisual.position.y - 1;
        //Get canvas and text boxes to feed UI information
        if (firstUpdateOnScene)
        {
            if (teamChoice == true)
            {
                playerVisual.GetComponent<Renderer>().material = blueTeamColor;
            }
            if (teamChoice == false)
            {
                playerVisual.GetComponent<Renderer>().material = redTeamColor;
            }
            //redTeamLivesInt = 1;
            canvas = GameObject.Find("playerUI").GetComponent<Canvas>();
            //hotShotModeParticles = GameObject.Find("HotShotNozzleParticleEmit").GetComponent<ParticleSystem>();
            hotShotModeParticles.Stop();
            //normalShotEmitParticles = GameObject.Find("NormalShotParticleEmit").GetComponent<ParticleSystem>();
            //hotShotEmitParticles = GameObject.Find("HotShotParticleEmit").GetComponent<ParticleSystem>();
            canvas.GetComponent<PlayerUIScript>().pmc = this;
            foreach (Text txt in FindObjectsOfType<Text>())
            {
                if (txt.name == "RedTeamLivesNumber")
                {
                    redTeamLives = txt;
                }
                if (txt.name == "BlueTeamLivesNumber")
                {
                    blueTeamLives = txt;
                }
                if (txt.name == "WinTeamText")
                {
                    winningTeamText = txt;
                }
                if (txt.name == "ShotsText")
                {
                    shotsTextBox = txt;
                }
                if(txt.name == "AmmoNumberText")
                {
                    ammoNumberTextBox = txt;
                }
                if(txt.name == "WhoShotMe")
                {
                    whoShotMeText = txt;
                }
                if(txt.name == "PlayersNames")
                {
                    playerNameList = txt;
                }
                if (txt.name == "PlayerDeaths")
                {
                    playerDeathList = txt;
                }
                if (txt.name == "PlayerEventsWon")
                {
                    playerEventList = txt;
                }
            }
            
            roundOverCanvas = GameObject.Find("RoundOverCanvas").GetComponent<Canvas>();
            escapeCanvas = GameObject.Find("EscapeCanvas").GetComponent<Canvas>();
            roundOverCanvas.enabled = false;
            escapeCanvas.enabled = false;

            StartCoroutine(DelayGetPlayerNames());
            //StartCoroutine(StartGameCountDown());
            firstUpdateOnScene = false;
        }
        if (startGame)
        {
            Text respawnTextt = canvas.GetComponentInChildren<Text>();
            respawnTextt.text = "Waiting for all players to load...";
            if (WaitForPlayerConnectStart)
            {
                StartCoroutine(WaitForPlayerConnect());
                WaitForPlayerConnectStart = false;
            }
            //go through list of all pmc's, check a bool that sees if they are ready...
            if (isLeader)
            {
                if (Room.GamePlayers.Count == WallBrawlNetworkManager.DicDisplayNames.Count)
                {
                    allPlayersReady = true;
                    startGameNow = true;
                }
            }
            if(!isLeader)
            {
                foreach (PlayerMovementController pcm in Room.GamePlayers)
                {
                    if(pcm.isLeader == true)
                    {
                        if (pcm.allPlayersReady == true)
                        {

                            startGameNow = true;
                        }
                    }
                }
            }
            if(startGameNow)
            {
                startGame = false;
                StartCoroutine(StartGameCountDown());
            }
        }

        if (redTeamLivesIntNew6 > 1)
        {
            redOut = false;
        }
        if(blueTeamLivesIntNew6 > 1)
        {
            blueOut = false;
        }
        if (controlUp <= lavaTransform && alive)
        {
            alive = false;
            //deaths++;
            cmdPlayerRigid(-velocity * (1 / 3));
            CmdChangeScore();
        }
        
        if (alive)
        {
            ammoNumberTextBox.text = normalShotsNew1.ToString();
            if (!controller.isGrounded)
            {
                velocity.y += gravity * Time.deltaTime;
            }
            if (controller.isGrounded)
            {
                velocity.y = gravity;
                if (doubleJumpCheck == false)
                {
                    doubleJumpCheck = true;
                }
            }
            if (Input.GetButton("Fire1") && Time.time > fireDelay)
            {
                
                if (normalShotsNew1 > 0)
                {
                    if (!hotShotMode)
                    {
                        fireDelay = Time.time + rateOfFire;
                        cmdShootCannon();
                    }
                    if(hotShotMode)
                    {
                        fireDelay = Time.time + rateOfFire;
                        cmdShootCannonHot();
                    }
                    if(normalShotsNew1 == 1)
                    {
                        rateOfFire = .5f;
                        CmdTurnOffHotShotMode();
                        ammoNumberTextBox.color = Color.white;
                        shotsTextBox.color = Color.white;
                    }
                    normalShotsNew1--;
                    ammoNumberTextBox.text = normalShotsNew1.ToString();
                }
                else
                {
                    /*
                    if (emptySoundDone == true)
                    {
                        StartCoroutine(EmptyAmmoSound());
                        emptySoundDone = false;
                    }
                    */
                    
                    fireDelay = Time.time + rateOfFire;
                    CmdShootCannonNoAmmo();
                }
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (!optionMenuOpen)
                {
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;
                    //PlayerCameraController.SetAliveC(false);
                    
                    controller.enabled = false;
                    escapeCanvas.enabled = true;
                    optionMenuOpen = true;
                }
                else
                {
                    controller.enabled = true;
                    Cursor.lockState = CursorLockMode.Locked;
                    Cursor.visible = false;

                    //PlayerCameraController.SetAliveC(false);
                    escapeCanvas.enabled = false;
                    optionMenuOpen = false;
                }
            }

            if (Input.GetButtonDown("Jump"))
            {
                if (controller.isGrounded)
                {
                    velocity.y += Mathf.Sqrt(jumpHeight * -3f * gravity);
                    GetComponent<AudioSource>().spatialBlend = 1.0f;
                    GetComponent<AudioSource>().volume = .5f;
                    GetComponent<AudioSource>().priority = 128;
                    GetComponent<AudioSource>().PlayOneShot(jumpSound, .2f);
                }
                
                else if (!controller.isGrounded && doubleJumpCheck)
                {
                    velocity.y = 0;
                    velocity.y += Mathf.Sqrt(jumpHeight * -1f * gravity);
                    GetComponent<AudioSource>().spatialBlend = 1.0f;
                    GetComponent<AudioSource>().volume = .5f;
                    GetComponent<AudioSource>().priority = 128;
                    GetComponent<AudioSource>().PlayOneShot(jumpSound, .2f);
                    doubleJumpCheck = false;
                } 
            }
            //working on jetpack event.  
            /*
            if (Input.GetButton("Jump") && jetPack == true)
            {
                if (!controller.isGrounded)
                {
                    velocity.y += Mathf.Sqrt(jumpHeight* .0001f * -1f * gravity);
                }
            }
            */
            cmdaimCannon();
            Move();
        }

    }
    //billboard name update and scoreboard update
    private void LateUpdate()
    {
        if (isLocalPlayer)
        {
            foreach (Canvas canvas in NameCanvases)
            {

                canvas.transform.LookAt(cam);
                canvas.transform.Rotate(0, 180, 0);

            }
        }
        if(redTeamLivesIntNew6 <= 0)
        {
            bool redTeamLost = true;
            foreach(PlayerMovementController pcm in Room.GamePlayers)
            {
                if(pcm.teamChoice == false)
                {
                    if(pcm.playerVisual.isKinematic)
                    {
                        redTeamLost = false;
                    }
                }
            }
            blueWon = redTeamLost;
        }
        if(blueTeamLivesIntNew6 <= 0)
        {
            bool blueTeamLost = true;
            foreach(PlayerMovementController pcm in Room.GamePlayers)
            {
                if(pcm.teamChoice == true)
                {
                    if(pcm.playerVisual.isKinematic)
                    {
                        blueTeamLost = false;
                    }
                }
            }
            redWon = blueTeamLost;
        }
        if (blueWon == true)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            winningTeamText.color = Color.blue;
            winningTeamText.text = "BLUE VICTORY";
            roundOverCanvas.enabled = true;
            if (endGameSoundPlayed == false)
            {
                //make lists of stats
                StartCoroutine(DelayGetScore());

                //play sound
                if (teamChoice == true)
                {
                    GetComponent<AudioSource>().PlayOneShot(wonGameSound);
                }
                else
                {
                    GetComponent<AudioSource>().PlayOneShot(lostGameSound);
                }
                endGameSoundPlayed = true;
            }
        }
        if (redWon == true)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            winningTeamText.color = Color.red;
            winningTeamText.text = "RED VICTORY";
            roundOverCanvas.enabled = true;
            if (endGameSoundPlayed == false)
            {
                Debug.Log("RUNNING ENDGAMEPROCEEDURE");
                //make lists of stats 
                StartCoroutine(DelayGetScore());
                if (teamChoice == false)
                {
                    GetComponent<AudioSource>().PlayOneShot(wonGameSound);
                }
                else
                {
                    GetComponent<AudioSource>().PlayOneShot(lostGameSound);
                }
                endGameSoundPlayed = true;
            }
        }
        redTeamLives.text = redTeamLivesIntNew6.ToString();
        blueTeamLives.text = blueTeamLivesIntNew6.ToString();
    }

    public void CallReadyNewRound()
    {
        CmdReadyNewRound();
    }
    [Command(ignoreAuthority = true)]
    public void CmdReadyNewRound()
    {  
        Room.ServerChangeScene("Lobby2"); 
    }

    public void ChangeCameraFirstPerson()
    {
        
        playerVisual.GetComponent<Renderer>().enabled = false;
        visorVisual.enabled = false;
        PlayerCameraController.SetCameraFirstperson(true);
    }
    public void ChangeCameraThirdPerson()
    {
        playerVisual.GetComponent<Renderer>().enabled = true;
        visorVisual.enabled = true;
        PlayerCameraController.SetCameraFirstperson(false);
    }
    IEnumerator DelayRegisterPlayerShot()
    {
        yield return new WaitForSeconds(1f);
        
    }


    IEnumerator WaitForPlayerConnect()
    {
        yield return new WaitForSeconds(3f);
        if(startGame)
        {
            StartCoroutine(StartGameCountDown());
        }
    }

    IEnumerator DelayGetScore()
    {
        yield return new WaitForSeconds(1f);
        foreach (PlayerMovementController pmc in Room.GamePlayers)
        {
            playerNameListBuilder += pmc.displayName + "\n";
            //playerKillsListBuilder += pmc.kills + "\n";
            playerDeathsListBuilder += pmc.deaths + "\n";
            playerEventsListBuilder += pmc.events + "\n";
        }
        playerNameList.text = playerNameListBuilder;
        //playerKillsList.text = playerKillsListBuilder;
        playerDeathList.text = playerDeathsListBuilder;
        playerEventList.text = playerEventsListBuilder;
    }
    [Command]
    public void CmdTurnOffHotShotMode()
    {
        RpcTurnOffHotShotMode();
    }
    [ClientRpc]
    public void RpcTurnOffHotShotMode()
    {
        hotShotModeParticles.Stop();
    } 
    [Command]
    public void CmdTurnOnHotShotMode()
    {
        RpcTurnOnHotShotMode();
    }
    [ClientRpc]
    public void RpcTurnOnHotShotMode()
    {
        hotShotModeParticles.Play();
    }
    //The server should probably be doing this and sending it to all clients to agree on start time.
    IEnumerator StartGameCountDown()
    {
        CmdTurnOffHotShotMode();
        //canvas.enabled = true;
        Text respawnTextt = canvas.GetComponentInChildren<Text>();
        respawnTextt.text = "Three";
        yield return new WaitForSeconds(1f);
        respawnTextt.text = "Two";
        yield return new WaitForSeconds(1f);
        respawnTextt.text = "One";
        yield return new WaitForSeconds(1f);
        respawnTextt.text = "BRAWL";
        alive = true;
        controller.enabled = true;
        startGame = false;
        yield return new WaitForSeconds(1f);
        respawnTextt.text = "";
    }

    IEnumerator EmptyAmmoSound()
    {

        GetComponent<AudioSource>().PlayOneShot(clickClackSound);
        yield return new WaitForSeconds(.2f);
        emptySoundDone = true;
    }

    public void PlayAddLifeSound()
    {
        GetComponent<AudioSource>().PlayOneShot(addLifeSound);
    }
    public void PlayLostAddLifeSound()
    {
        GetComponent<AudioSource>().PlayOneShot(lostAddLifeSound);
    }
    public void WonAddLivesEvent()
    {
        controller.enabled = false;
        CmdAddLivesScore();
        CmdRespawnPlayer();
    }
    [Command]
    public void CmdAddLivesScore()
    {

        if (Time.time > addLifeChangeDelay)
        {
            RpcAddLivesScore();
        }
        Debug.Log("About to call CmdRespawn;");
        addLifeChangeDelay = Time.time + delayPotentialAddLifeChange;
        //CmdRespawnPlayer();
    }
    [ClientRpc]
    public void RpcAddLivesScore()
    {
        events++;
        if (teamChoice == false)
        {
            foreach (PlayerMovementController pmc in Room.GamePlayers)
            {
                if(pmc.teamChoice == false)
                {
                    pmc.GetComponent<AudioSource>().PlayOneShot(addLifeSound);
                }
                else
                {
                    pmc.GetComponent<AudioSource>().PlayOneShot(lostAddLifeSound);
                }
                pmc.redTeamLivesIntNew6 += 5;
            }
        }
        if (teamChoice == true)
        {
            foreach (PlayerMovementController pmc in Room.GamePlayers)
            {
                if(pmc.teamChoice == true)
                {
                    pmc.GetComponent<AudioSource>().PlayOneShot(addLifeSound);
                }
                else
                {
                    pmc.GetComponent<AudioSource>().PlayOneShot(lostAddLifeSound);
                }
                pmc.blueTeamLivesIntNew6 += 5;
            }
        }
    }
    public void WonAddMegaReloadEvent()
    {
        controller.enabled = false;
        CmdMegaReload();
        CmdRespawnPlayer();
    }
    [Command]
    public void CmdMegaReload()
    {
        if (Time.time > MegaReloadChangeDelay)
        {
            RpcMegaReload();
        }
        
        MegaReloadChangeDelay = Time.time + delayPotentialMegaReload;
    }
    [ClientRpc]
    public void RpcMegaReload()
    {
        events++;
        if (teamChoice == false)
        {
            foreach (PlayerMovementController pmc in Room.GamePlayers)
            {
                if (pmc.teamChoice == false)
                {
                    pmc.MegaReloadEvent();

                }
            }
        }
        if (teamChoice == true)
        {
            foreach (PlayerMovementController pmc in Room.GamePlayers)
            {
                if (pmc.teamChoice == true)
                {
                    pmc.MegaReloadEvent();
                }
            }
        }
    }
    public void MegaReloadEvent()
    {
        normalShotsNew1 = 3;
        rateOfFire = 1f;

        hotShotMode = false;
        //shotsTextBox.color = Color.white;
        //ammoNumberTextBox.color = Color.white;
        //ammoNumberTextBox.text = normalShotsNew1.ToString();
        //GetComponent<AudioSource>().PlayOneShot(reloadSound);
    }

    public void NormalReload()
    {
        
        normalShotsNew1 = 1;
        rateOfFire = 1f;
        
        hotShotMode = false;
        shotsTextBox.color = Color.white;
        ammoNumberTextBox.color = Color.white;
        ammoNumberTextBox.text = normalShotsNew1.ToString();
        GetComponent<AudioSource>().PlayOneShot(reloadSound);
    }

    public void HotReload()
    {
        normalShotsNew1 = 2;
        rateOfFire = 1f;

        CmdTurnOnHotShotMode();
        //hotShotModeParticles.Play();
        hotShotMode = true;
        shotsTextBox.color = Color.red;
        ammoNumberTextBox.text = normalShotsNew1.ToString();
        ammoNumberTextBox.color = Color.red;
        GetComponent<AudioSource>().PlayOneShot(reloadSound);
    }

    //function to wait 3 seconds upon loading into game so that chances are all players have loaded up by then, so that you can load player names into array of canvases to be written and then turned towards camera every frame
    public IEnumerator DelayGetPlayerNames()
    {

        if (isLocalPlayer)
        {
            yield return new WaitForSeconds(3f);
            foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player"))
            {
                Rigidbody rbpv = player.GetComponentInChildren<Rigidbody>();
                Canvas canvas = rbpv.GetComponentInChildren<Canvas>();
                TextMeshProUGUI textMesh = canvas.GetComponentInChildren<TextMeshProUGUI>();
                PlayerMovementController pmcs = player.GetComponent<PlayerMovementController>();
                string nameFromScript = pmcs.DisplayName;
                textMesh.text = nameFromScript;
                NameCanvases.Add(canvas);
                //assign team color
                if (pmcs.teamChoice == true)
                {
                    rbpv.GetComponent<Renderer>().material = blueTeamColor;
                }
                if (pmcs.teamChoice == false)
                {
                    rbpv.GetComponent<Renderer>().material = redTeamColor;
                }
            }
        }
    }
    public IEnumerator DisplayWhoShot(GameObject whoShotMe)
    {
        int random = Random.Range(1, 5);
        if(rektwords.TryGetValue(random, out string rektword)) {
            whoShotMeText.text = rektword + whoShotMe.GetComponent<PlayerMovementController>().DisplayName;
        }
        yield return new WaitForSeconds(5f);
        whoShotMeText.text = "";
    }

    //Destructable wall spawn and unspawning
    [Command]
    public void cmdUnspawnGrasslandWall()
    {
        RpcUnspawnGrasslandWall();
    }
    [ClientRpc]
    public void RpcUnspawnGrasslandWall()
    {
        Destroy(GameObject.FindGameObjectWithTag("GrasslandWall"));
        NetworkServer.UnSpawn(instantiatedGrasslandWall);
    }
    [Command]
    public void cmdRespawnGrasslandWall()
    {
        instantiatedGrasslandWall = (GameObject)Instantiate(wallObjectSpawneeGrassland, wallSpawnPositionGrassland.position, wallSpawnPositionGrassland.rotation);
        NetworkServer.Spawn(instantiatedGrasslandWall);
    }
    [Command]
    public void cmdUnspawnDesertWall()
    {
        RpcUnspawnDesertWall();
    }
    [ClientRpc]
    public void RpcUnspawnDesertWall()
    {
        Destroy(GameObject.FindGameObjectWithTag("DesertWall"));
        NetworkServer.UnSpawn(instantiatedWall);
    }
    [Command]
    public void cmdRespawnDesertWall()
    {
        instantiatedWall = (GameObject)Instantiate(wallObjectSpawnee, wallSpawnPosition.position, wallSpawnPosition.rotation);
        NetworkServer.Spawn(instantiatedWall);
    }
    //End of wall spawn and unspawn


    //client character controll for cannon
    [Client]
    public void cmdaimCannon()
    {
        if (isLocalPlayer)
        {
            shootPos.LookAt(aimCannonPoint);
            cannonVisual.LookAt(aimCannonPoint);
        }
    }
    [Command]
    public void cmdShootCannon()
    {
        
        RpcShootCannon();
    }
    [ClientRpc]
    void RpcShootCannon()
    {
        yellowFireEmitParticles.Play();
        normalShotEmitParticles.Play();
        GetComponent<AudioSource>().spatialBlend = 1.0f;
        GetComponent<AudioSource>().priority = 128;
        GetComponent<AudioSource>().PlayOneShot(cannonShotSound, .7f);
        sphereClone = (GameObject)Instantiate(sphere, shootPos.position, shootPos.rotation);
        sphereClone.GetComponent<CannonBallScript>().playerShooter = playerObject;
        sphereClone.GetComponent<Rigidbody>().AddRelativeForce(Vector3.forward * shotPower);
    }

    [Command]
    public void cmdShootCannonHot()
    {

        RpcShootCannonHot();
    }
    [ClientRpc]
    void RpcShootCannonHot()
    {
        yellowFireEmitParticles.Play();
        hotShotEmitParticles.Play();
        GetComponent<AudioSource>().spatialBlend = 1.0f;
        GetComponent<AudioSource>().priority = 128;
        GetComponent<AudioSource>().PlayOneShot(cannonShotSound, 1f);
        sphereClone = (GameObject)Instantiate(sphereHot, shootPos.position, shootPos.rotation);
        sphereClone.GetComponent<CannonBallScript>().playerShooter = playerObject;
        sphereClone.GetComponent<Rigidbody>().AddRelativeForce(Vector3.forward * shotPower*4);
    }
    [Command]
    public void CmdShootCannonNoAmmo()
    {
        RpcShootCannonNoAmmor();
    }
    [ClientRpc]
    public void RpcShootCannonNoAmmor()
    {
        GetComponent<AudioSource>().spatialBlend = 1.0f;
        GetComponent<AudioSource>().priority = 128;
        GetComponent<AudioSource>().PlayOneShot(outOfAmmonShotSound, .15f);
        sphereClone = (GameObject)Instantiate(sphereNoAmmo, shootPos.position, shootPos.rotation);
        sphereClone.GetComponent<CannonBallScript>().playerShooter = playerObject;
        sphereClone.GetComponent<Rigidbody>().AddRelativeForce(Vector3.forward * shotPower*.0025f);
    }


    //respawn player
    [Command]
    public void CmdRespawnPlayer()
    {
        Debug.Log("InsideCmdRespawn");
        cannonSpawnedChecker = false;
        playerVisual.GetComponent<Rigidbody>().isKinematic = true;
        if (teamChoice == true)
        {
            if (blueTeamLivesIntNew6 <= 0) return;
            playerObject.transform.SetPositionAndRotation(respawnPointOne.position, respawnPointOne.rotation);
            playerVisual.transform.SetPositionAndRotation(respawnPointOneVisual.position, respawnPointOneVisual.rotation);
        }
        if(teamChoice == false)
        {
            if (redTeamLivesIntNew6 <= 0) return;
            playerObject.transform.SetPositionAndRotation(respawnPointTwo.position, respawnPointTwo.rotation);
            playerVisual.transform.SetPositionAndRotation(respawnPointTwoVisual.position, respawnPointTwoVisual.rotation);
        }
        if (isLocalPlayer)
        {
            //alive = true;
            controller.enabled = true;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            PlayerCameraController.SetAliveC(true);
            doubleJumpCheck = true;
        }
        playerObject.GetComponent<CharacterController>().enabled = true;
        RpcRespawnPlayer();
    }
    [ClientRpc]
    public void RpcRespawnPlayer()
    {
        
        playerVisual.GetComponent<Rigidbody>().isKinematic = true;
        if (teamChoice == true)
        {
            if (blueTeamLivesIntNew6 <= 0) return;
            playerObject.transform.SetPositionAndRotation(respawnPointOne.position, respawnPointOne.rotation);
            playerVisual.transform.SetPositionAndRotation(respawnPointOneVisual.position, respawnPointOneVisual.rotation);
        }
        if (teamChoice == false)
        {
            if (redTeamLivesIntNew6 <= 0) return;
            playerObject.transform.SetPositionAndRotation(respawnPointTwo.position, respawnPointTwo.rotation);
            playerVisual.transform.SetPositionAndRotation(respawnPointTwoVisual.position, respawnPointTwoVisual.rotation);
        }
        MeshRenderer[] playerCannonMeshRenders =
        playerObject.GetComponentsInChildren<MeshRenderer>();
        foreach (var MeshRendererX in playerCannonMeshRenders)
        {
            if (MeshRendererX.name == "Cannon Visual" || MeshRendererX.name == "CannonNozzle")
            {
                MeshRendererX.enabled = true;
            }
        }
        if (isLocalPlayer)
        {
            //alive = true;
            controller.enabled = true;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            PlayerCameraController.SetAliveC(true);
            //canvas.enabled = false;
            doubleJumpCheck = true;
        }
        playerObject.GetComponent<CharacterController>().enabled = true;
    }

    public void setAlive(bool tf)
    {
        alive = tf;
    }
    //player is DED
    [Command (ignoreAuthority = true)]
    public void cmdPlayerRigid(Vector3 hit)
    {
        if (Time.time > scoreChangeDelay)
        {
            RpcPlayerRigid(hit);
        }
        
    }
    //respawn counter user interface
    public IEnumerator TenSecondDelay()
    {
        if (isLocalPlayer)
        {
            GetComponent<AudioSource>().Stop();
            GetComponent<AudioSource>().spatialBlend = 0f;
            GetComponent<AudioSource>().priority = 128;
            GetComponent<AudioSource>().PlayOneShot(kekw, .05f);

            if (teamChoice == false && redOut) yield break;
            if (teamChoice == true && blueOut) yield break;

            canvas.enabled = true;
            Text respawnTextt = canvas.GetComponentInChildren<Text>();
            respawnTextt.text = "Five";
            yield return new WaitForSeconds(1f);
            respawnTextt.text = "Four";
            yield return new WaitForSeconds(1f);
            respawnTextt.text = "Three";
            yield return new WaitForSeconds(1f);
            respawnTextt.text = "Two";
            yield return new WaitForSeconds(1f);
            respawnTextt.text = "One";
            yield return new WaitForSeconds(.5f);
            velocity.y = 0;
            respawnTextt.text = "";
            /*
            if (resetGameCheck == true)
            {
                CmdRespawnPlayerRestartGame();
                resetGameCheck = false;
            }
            else
            {
                Debug.Log("In normal respawn");
                CmdRespawnPlayer();
            }
            */
            CmdRespawnPlayer();
            yield return new WaitForSeconds(.5f);
            GetComponent<AudioSource>().spatialBlend = 0f;
            GetComponent<AudioSource>().priority = 128;
            GetComponent<AudioSource>().PlayOneShot(respawnSound, .05f);
            aliveCheck = true;
            setAlive(true);
            doubleJumpCheck = true;
            if (teamChoice == false && redTeamLivesIntNew6 == 1)
            {
                foreach(PlayerMovementController pmc in Room.GamePlayers)
                {
                    pmc.redOut = true;
                }
                
            }
            if (teamChoice == true && blueTeamLivesIntNew6 == 1)
            {
                foreach (PlayerMovementController pmc in Room.GamePlayers)
                {
                    pmc.blueOut = true;
                }
            }
        }

    }
    public void ChangeScorePlayerDied()
    {
        if(aliveCheck)
        {
            CmdChangeScore();
            aliveCheck = false;
        }
    }

    [Command(ignoreAuthority = true)]
    public void CmdChangeScore()
    {
        if (Time.time > scoreChangeDelay)
        {
            RpcSetScore();
        }
        scoreChangeDelay = Time.time + delayPotentialScoreChange;
    }
    [ClientRpc]
    public void RpcSetScore()
    {

        if (teamChoice == false && redTeamLivesIntNew6 > 0)
        {
            foreach (PlayerMovementController pmc in Room.GamePlayers)
            {
                
                pmc.redTeamLivesIntNew6--;
            }
        }
        if (teamChoice == true && blueTeamLivesIntNew6 > 0)
        {
            foreach (PlayerMovementController pmc in Room.GamePlayers)
            {
                pmc.blueTeamLivesIntNew6--;
            }
        }
    }
    [Command]
    public void CmdAddMoreLivesEventWonPersonalScore()
    {
        RpcAddMoreLivesEventWonPersonalScore();
    }
    [ClientRpc]
    public void RpcAddMoreLivesEventWonPersonalScore()
    {
        events++;
    }
    [ClientRpc]
    public void RpcPlayerRigid(Vector3 hit) 
    {
        
        if (isLocalPlayer)
        {
            
            //Cursor.lockState = CursorLockMode.None;
            //Cursor.visible = true;
            PlayerCameraController.SetAliveC(false);
            controller.enabled = false;
        }
        deaths++;
        MeshRenderer[] playerCannonMeshRenders = 
        playerObject.GetComponentsInChildren<MeshRenderer>();
        foreach(var MeshRendererX in playerCannonMeshRenders)
        {
            if (MeshRendererX.name == "Cannon Visual" || MeshRendererX.name == "CannonNozzle")
            {
                MeshRendererX.enabled = false;
            }
        }
        
        playerVisual.GetComponent<Rigidbody>().isKinematic = false;
        playerObject.GetComponent<CharacterController>().enabled = false;
        playerVisual.GetComponent<Rigidbody>().AddForce(hit*40);
        /*
        if (!cannonSpawnedChecker)
        {
           
            Instantiate(CannonRigidBodyToFly, cannonVisual.position, cannonVisual.rotation);
            CannonRigidBodyToFly.AddForce(hit * 450);
            cannonSpawnedChecker = true;
        }
        */
        StartCoroutine(TenSecondDelay());
    }
    [Client]
    private void SetMovement(Vector2 movement) => previousInput = movement;
    [Client]
    private void ResetMovement() => previousInput = Vector2.zero;
    [Client]
    private void Move()
    {
        Vector3 right = controller.transform.right;
        Vector3 forward = controller.transform.forward;
        Vector3 down = controller.transform.up;
        right.y = 0f;
        forward.y = 0f;
        Vector3 movement = right * previousInput.x + forward * previousInput.y;
        controller.Move(movement * movementSpeed * 10* Time.deltaTime);
        controller.Move(velocity * Time.deltaTime);
    }
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        //if (hit.collider.name == "CannonBallReload")
        //{
         //   cmdShootCannon();
        //}
        if (hit.collider.name == "respawnDesertWallObject" && GameObject.FindGameObjectWithTag("DesertWall") == null && Time.time > respawnDelay)
        {
            respawnDelay = Time.time + rateOfRespawn;
            cmdRespawnDesertWall();    
        }
        if (hit.collider.name == "UnspawnDesertWallObject" && GameObject.FindGameObjectWithTag("DesertWall") != null)
        {
            cmdUnspawnDesertWall();
        }

        if (hit.collider.name == "respawnGrasslandWallObject" && GameObject.FindGameObjectWithTag("GrasslandWall") == null && Time.time > respawnDelay)
        {
            respawnDelay = Time.time + rateOfRespawn;
            cmdRespawnGrasslandWall();
        }
        if (hit.collider.name == "UnspawnGrasslandWallObject" && GameObject.FindGameObjectWithTag("GrasslandWall") != null)
        {
            cmdUnspawnGrasslandWall();
        }
        
    }
}
