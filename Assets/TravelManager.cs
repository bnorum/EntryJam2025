using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class TravelManager : MonoBehaviour
{
    public static TravelManager Instance { get; private set; }


    public WaterTeleport waterEntrance;
    public WaterTeleport waterExit;

    public Transform waterWaypointLeft;
    public Transform waterWaypointRight;

    public List<Dock> docks = new List<Dock>();

    public Canvas travelCanvas;
    public TextMeshProUGUI locationText;
    public List<Image> locationImages = new List<Image>();
    public int currentLocationIndex = 0;

    public InputAction NavigationAction;
    public InputAction SelectAction;



    void Awake()
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


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        NavigationAction = InputSystem.actions.FindAction("Navigate");
        SelectAction = InputSystem.actions.FindAction("Submit");
        InitializeDocks();
    }

    // Update is called once per frame
    void Update()
    {
        foreach (Image locationImage in locationImages)
        {
            locationImage.color = (locationImages.IndexOf(locationImage) == currentLocationIndex) ? Color.yellow : Color.gray;
        }

        if (SelectAction.triggered && currentLocationIndex != GameManager.Instance.currentMap && travelCanvas.gameObject.activeSelf)
        {
            SelectDock();
            CloseTravelCanvas();
        }

        if (NavigationAction.triggered && NavigationAction.ReadValue<Vector2>().x != 0)
        {
            currentLocationIndex += (int)NavigationAction.ReadValue<Vector2>().x;
            if (currentLocationIndex < 0) currentLocationIndex = docks.Count - 1;
            if (currentLocationIndex >= docks.Count) currentLocationIndex = 0;

            locationText.text = docks[currentLocationIndex].dockName;
        }
    }

    void InitializeDocks()
    {
        docks.Clear();
        Dock[] foundDocks = FindObjectsByType<Dock>(FindObjectsSortMode.None);
        foreach (Dock dock in foundDocks)
        {
            docks.Add(dock);
        }
        docks.Sort((a, b) => a.dockIndex.CompareTo(b.dockIndex)); //not that it matters
    }

    public void TravelToDock(int entranceIndex, int destinationIndex)
    {
        SetWaterEntranceDock(entranceIndex);
        SetWaterExitDock(destinationIndex);

        GameManager.Instance.overworldPlayerPosition.position = waterWaypointLeft.position;
    }

    public void SetWaterEntranceDock(int index)
    {
        //travelling offscreen left will make the player go to this dock
        waterEntrance.outLocation = docks[index].transform.position;
    }

    public void SetWaterExitDock(int index)
    {
        //travelling offscreen right will make the player go to this dock
        waterExit.outLocation = docks[index].transform.position;
    }

    public void OpenTravelCanvas()
    {
        travelCanvas.gameObject.SetActive(true);
    }

    public void CloseTravelCanvas()
    {
        travelCanvas.gameObject.SetActive(false);
    }

    public void SelectDock()
    {
        TravelToDock(GameManager.Instance.currentMap, currentLocationIndex);
        GameManager.Instance.currentMap = currentLocationIndex;
    }
}
