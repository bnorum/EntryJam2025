using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public enum GameState
    {
        FREE,
        MENU,
        BATTLE
    }
    public GameState gameState = GameState.FREE;

    public int currentMap = 0; // Example map index, adjust as needed

    public Transform overworldPlayerPosition;
    public BasePlayer player;
    public GameObject playerPrefab; // Reference to the player prefab
    public Camera mainCamera;

    public GameObject BattleStartCanvasPrefab;

    public AudioClip battleStartSound;

    void Start()
    {
#if UNITY_EDITOR
        if (overworldPlayerPosition == null && SceneManager.GetActiveScene().name == "Overworld")
        {
            mainCamera = Camera.main;
            overworldPlayerPosition = Instantiate(playerPrefab, Vector2.zero, Quaternion.identity).GetComponent<Transform>();

            player = new BasePlayer(1, 0, "Fisherman", 100, 30, 10, 5);

        }
#endif
    }

    public void NewGame()
    {
        SceneTransition("Overworld");
        // Wait for the scene to load before instantiating the player
        StartCoroutine(WaitForSceneAndInstantiate());

        player = new BasePlayer(1, 0, "Fisherman", 100, 30, 10, 5); // Example player creation
        currentMap = 0;
        gameState = GameState.FREE;
        Debug.Log("New game started.");
    }

    // Coroutine to wait for scene load
    IEnumerator WaitForSceneAndInstantiate()
    {
        // Wait until the scene is fully loaded
        while (SceneManager.GetActiveScene().name != "Overworld")
            yield return null;

        mainCamera = Camera.main;
        overworldPlayerPosition = Instantiate(playerPrefab, Vector2.zero, Quaternion.identity).GetComponent<Transform>();
    }

    public void LoadGame()
    {
        SceneManager.LoadScene("Overworld");

        // Wait for the scene to load before instantiating the player
        StartCoroutine(LoadCoroutine());


    }

    IEnumerator LoadCoroutine()
    {
        // Wait until the scene is fully loaded
        while (SceneManager.GetActiveScene().name != "Overworld")
            yield return null;

        mainCamera = Camera.main;
        overworldPlayerPosition = Instantiate(playerPrefab, Vector2.zero, Quaternion.identity).GetComponent<Transform>();
        SaveData data = SaveManager.Load();
        if (data != null)
        {
            overworldPlayerPosition.position = new Vector3(data.posX, data.posY, 0);
            currentMap = data.mapNumber;
            player = new BasePlayer(data.level, data.exp, data.playerName, data.maxExhaustion, data.maxMP, data.attack, data.defense);
            for (int i = 0; i < data.itemIDs.Length; i++)
            {
                if (ItemData.items.TryGetValue(data.itemIDs[i], out IItem item))
                {
                    // Add item to player's inventory or handle it as needed
                    Debug.Log($"Loaded item: {item.Name}");
                    player.Inventory.Add(item); // Assuming player has an Inventory property
                }
            }
            //set EXH and MP values from data somehow
        }

    }

#if UNITY_EDITOR
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            SaveManager.Load();
        }

        if (Input.GetKeyDown(KeyCode.N))
        {
            NewGame();
        }

        if (Input.GetKeyDown(KeyCode.G))
        {
            player.Inventory.Add(new WaterBottle()); // Example of adding a WaterBottle item
        }
    }
#endif

    public void StartBattle(IFish fish, GameObject backgroundPrefab = null)
    {
        gameState = GameState.BATTLE;
        StartCoroutine(WaitForBattle(fish, backgroundPrefab));
    }

    public IEnumerator WaitForBattle(IFish fish, GameObject backgroundPrefab)
    {
        GameObject BattleStartCanvas = Instantiate(BattleStartCanvasPrefab); // Show battle start UI
        GameObject BattleStar = BattleStartCanvas.transform.GetChild(0).gameObject;
        AudioSource.PlayClipAtPoint(battleStartSound, mainCamera.transform.position);


        while (BattleStar.transform.localScale.x < 300)
        {
            BattleStar.transform.localScale += new Vector3(0.3f, 0.3f, 0);
            BattleStar.transform.Rotate(new Vector3(0, 0, 1));
            yield return null;
        }
        yield return new WaitForSeconds(0.5f);

        // fade to black
        Image fadeImage = BattleStartCanvas.transform.GetChild(1).GetComponent<Image>();
        while (fadeImage.color.a < 1)
        {
            fadeImage.color += new Color(0, 0, 0, 0.02f);
            yield return null;
        }


        yield return new WaitForSeconds(1f);

        SceneManager.LoadSceneAsync("BattleScene", LoadSceneMode.Additive); // loads scene on top of overworld
        while (!SceneManager.GetSceneByName("BattleScene").isLoaded)
            yield return null;

        Camera battleCam = GameObject.FindGameObjectWithTag("BattleCamera").GetComponent<Camera>(); // Battle camera

        mainCamera.enabled = false;
        battleCam.enabled = true;

        StartCoroutine(BattleManager.Instance.SetupBattle(player, fish, backgroundPrefab));

        while (fadeImage.color.a > 0)
        {
            fadeImage.color -= new Color(0, 0, 0, 0.02f);
            yield return null;
        }

        Destroy(BattleStartCanvas);




    }

    public IEnumerator EndBattle()
    {
        Camera battleCam = GameObject.FindGameObjectWithTag("BattleCamera").GetComponent<Camera>(); // Battle camera

        mainCamera.enabled = true;
        battleCam.enabled = false;

        GameObject BattleStartCanvas = Instantiate(BattleStartCanvasPrefab); // Show battle start UI
        GameObject BattleStar = BattleStartCanvas.transform.GetChild(0).gameObject;
        BattleStar.SetActive(false);
        GameObject fadeImageObj = BattleStartCanvas.transform.GetChild(1).gameObject;
        Image fadeImage = fadeImageObj.GetComponent<Image>();
        fadeImage.color = new Color(0, 0, 0, 0);

        while (fadeImage.color.a < 1)
        {
            fadeImage.color += new Color(0, 0, 0, 0.02f);
            yield return null;
        }

        SceneManager.UnloadSceneAsync("BattleScene");
        yield return new WaitUntil(() => !SceneManager.GetSceneByName("BattleScene").isLoaded);
        yield return new WaitForSeconds(0.5f);

        while (fadeImage.color.a > 0)
        {
            fadeImage.color -= new Color(0, 0, 0, 0.02f);
            yield return null;
        }

        gameState = GameState.FREE;


    }

    public void SceneTransition(string sceneName)
    {
        StartCoroutine(TransitionToScene(sceneName));
    }

    public IEnumerator TransitionToScene(string sceneName)
    {
        GameObject BattleStartCanvas = Instantiate(BattleStartCanvasPrefab); // Show battle start UI
        DontDestroyOnLoad(BattleStartCanvas);
        GameObject fadeImageObj = BattleStartCanvas.transform.GetChild(1).gameObject;
        Image fadeImage = fadeImageObj.GetComponent<Image>();
        fadeImage.color = new Color(0, 0, 0, 0);

        while (fadeImage.color.a < 1)
        {
            fadeImage.color += new Color(0, 0, 0, 0.02f);
            yield return null;
        }

        SceneManager.LoadScene(sceneName);
        yield return new WaitForSeconds(0.5f);

        while (fadeImage.color.a > 0)
        {
            fadeImage.color -= new Color(0, 0, 0, 0.02f);
            yield return null;
        }


    }

}
