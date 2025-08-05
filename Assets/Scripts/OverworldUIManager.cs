using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

public class OverworldUIManager : MonoBehaviour
{

    public static OverworldUIManager Instance;

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


    public GameObject buttonPanel;
    public Transform buttonHideTarget;
    public Transform buttonShowTarget;

    public GameObject statsPanel;
    public Transform statsHideTarget;
    public Transform statsShowTarget;

    public bool isInItemMenu = false;
    public int selectedItemIndex = 0;
    public GameObject inventoryPanel;
    public TextMeshProUGUI inventoryText;

    public bool isInEquipmentMenu = false;
    public GameObject equipmentPanel;
    public TextMeshProUGUI equipmentText;

    public InputAction toggleMenuAction;
    public InputAction moveAction;
    public InputAction confirmAction;
    public InputAction cancelAction;

    public List<Image> buttons;
    public int selectedButtonIndex = 0;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        toggleMenuAction = InputSystem.actions.FindAction("ToggleMenu");
        moveAction = InputSystem.actions.FindAction("Navigate");
        confirmAction = InputSystem.actions.FindAction("Submit");
        cancelAction = InputSystem.actions.FindAction("Cancel");

        buttonPanel.transform.position = buttonHideTarget.position;
        statsPanel.transform.position = statsHideTarget.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (toggleMenuAction.triggered)
        {
            if (GameManager.Instance.gameState == GameManager.GameState.FREE)
            {
                GameManager.Instance.gameState = GameManager.GameState.MENU;
            }
            else if (GameManager.Instance.gameState == GameManager.GameState.MENU)
            {
                GameManager.Instance.gameState = GameManager.GameState.FREE;
                isInItemMenu = false;
                isInEquipmentMenu = false;
                inventoryPanel.SetActive(false);
                equipmentPanel.SetActive(false);
            }
        }

        if (cancelAction.triggered && isInItemMenu)
        {
            isInItemMenu = false;
            inventoryPanel.SetActive(false);
        }
        else if (cancelAction.triggered && isInEquipmentMenu)
        {
            isInEquipmentMenu = false;
            equipmentPanel.SetActive(false);
        }

        if (GameManager.Instance.gameState == GameManager.GameState.MENU)
        {
            buttonPanel.transform.position = Vector3.Lerp(buttonPanel.transform.position, buttonShowTarget.position, Time.deltaTime * 5f);
            statsPanel.transform.position = Vector3.Lerp(statsPanel.transform.position, statsShowTarget.position, Time.deltaTime * 5f);
        }
        else
        {
            buttonPanel.transform.position = Vector3.Lerp(buttonPanel.transform.position, buttonHideTarget.position, Time.deltaTime * 5f);
            statsPanel.transform.position = Vector3.Lerp(statsPanel.transform.position, statsHideTarget.position, Time.deltaTime * 5f);
        }

        if (GameManager.Instance.gameState == GameManager.GameState.MENU)
        {
            for (int i = 0; i < buttons.Count; i++)
            {
                if (i == selectedButtonIndex)
                {
                    buttons[i].color = Color.yellow; // Highlight selected button
                }
                else
                {
                    buttons[i].color = Color.white; // Reset other buttons
                }
            }

            Vector2 moveInput = moveAction.ReadValue<Vector2>();

            // Handle button navigation
            if (moveInput.x > 0 && moveAction.triggered && !isInItemMenu && !isInEquipmentMenu)
            {
                selectedButtonIndex = (selectedButtonIndex + 1) % buttons.Count; // Move right
            }
            else if (moveInput.x < 0 && moveAction.triggered && !isInItemMenu && !isInEquipmentMenu)
            {
                selectedButtonIndex = (selectedButtonIndex - 1 + buttons.Count) % buttons.Count; // Move left
            }

            // Handle item and technique menu navigation
            if (moveInput.y > 0 && moveAction.triggered && isInItemMenu)
            {

            }
            else if (moveInput.y < 0 && moveAction.triggered && isInItemMenu)
            {

            }

            if (moveInput.y > 0 && moveAction.triggered && isInEquipmentMenu)
            {
            }
            else if (moveInput.y < 0 && moveAction.triggered && isInEquipmentMenu)
            {

            }

            // Handle button confirmation
            if (confirmAction.triggered && !isInEquipmentMenu && !isInItemMenu)
            {
                ExecuteSelectedButton();
            }
        }

    }

    private void ExecuteSelectedButton()
    {
        // Look, this is a bad implementation. But look at the bright side! There's only three buttons right now.
        if (selectedButtonIndex == 0)
        {
            if (isInItemMenu)
            {
                isInItemMenu = false;
                inventoryPanel.SetActive(false);
            }
            else
            {
                isInItemMenu = true;
                inventoryPanel.SetActive(true);
            }
        }
        else if (selectedButtonIndex == 1)
        {
            if (isInEquipmentMenu)
            {
                isInEquipmentMenu = false;
                equipmentPanel.SetActive(false);
            }
            else
            {
                isInEquipmentMenu = true;
                equipmentPanel.SetActive(true);
            }
        }
        else if (selectedButtonIndex == 2)
        {
            SaveManager.Save(new SaveData(
                GameManager.Instance.overworldPlayerPosition.position.x,
                GameManager.Instance.overworldPlayerPosition.position.y,
                GameManager.Instance.currentMap,
                GameManager.Instance.player
                ));
        }
    }

}
