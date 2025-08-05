using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

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
    void Start()
    {

    }

    public void NewGame()
    {
        SceneManager.LoadScene("Overworld"); // Replace with your overworld scene name
        // Wait for the scene to load before instantiating the player
        StartCoroutine(WaitForSceneAndInstantiate());

        player = new BasePlayer(1, 0, 0, "Fisherman", 100, 30, 10, 5); // Example player creation
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
            player = new BasePlayer(data.level, data.exp, data.expToNext, data.playerName, data.maxExhaustion, data.maxMP, data.attack, data.defense);
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
        SceneManager.LoadSceneAsync("BattleScene", LoadSceneMode.Additive); // loads scene on top of overworld
        StartCoroutine(WaitForBattle(fish, backgroundPrefab));
    }

    public IEnumerator WaitForBattle(IFish fish, GameObject backgroundPrefab)
    {
        while (!SceneManager.GetSceneByName("BattleScene").isLoaded)
            yield return null;

        Camera battleCam = GameObject.FindGameObjectWithTag("BattleCamera").GetComponent<Camera>(); // Battle camera

        mainCamera.enabled = false;
        battleCam.enabled = true;

        StartCoroutine(BattleManager.Instance.SetupBattle(player, fish, backgroundPrefab));

    }

    public void EndBattle()
    {
        Camera battleCam = GameObject.FindGameObjectWithTag("BattleCamera").GetComponent<Camera>(); // Battle camera

        mainCamera.enabled = true;
        battleCam.enabled = false;

        gameState = GameState.FREE;
        SceneManager.UnloadSceneAsync("BattleScene");
    }
}
