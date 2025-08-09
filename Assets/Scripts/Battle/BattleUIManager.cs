using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class BattleUIManager : MonoBehaviour
{

    public static BattleUIManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public List<Image> buttons;
    public int selectedButtonIndex = 0;
    public InputAction moveAction;
    public InputAction confirmAction;
    public InputAction cancelAction;

    public Transform buttonHideTarget;
    public Transform buttonShowTarget;
    public GameObject buttonPanel;

    public GameObject itemMenu;
    public TextMeshProUGUI itemMenuText;
    public int selectedItemIndex = 0;
    public bool isInItemMenu = false;

    public GameObject techniqueMenu;
    public TextMeshProUGUI techniqueMenuText;
    public int selectedTechniqueIndex = 0;
    public bool isInTechniqueMenu = false;

    public AudioClip menuNavigateSound;
    public AudioClip menuSelectSound;
    public AudioClip menuCancelSound;

    public AudioSource battleMusicSource;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        moveAction = InputSystem.actions.FindAction("Navigate");
        confirmAction = InputSystem.actions.FindAction("Submit");
        cancelAction = InputSystem.actions.FindAction("Cancel");
        HideTechniqueMenu();
        HideItemMenu();
        battleMusicSource.Play();
    }

    // Update is called once per frame
    void Update()
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
        if (moveInput.x > 0 && moveAction.triggered && !isInItemMenu && !isInTechniqueMenu)
        {
            selectedButtonIndex = (selectedButtonIndex + 1) % buttons.Count; // Move right
            AudioSource.PlayClipAtPoint(menuNavigateSound, Vector3.zero);
        }
        else if (moveInput.x < 0 && moveAction.triggered && !isInItemMenu && !isInTechniqueMenu)
        {
            selectedButtonIndex = (selectedButtonIndex - 1 + buttons.Count) % buttons.Count; // Move left
            AudioSource.PlayClipAtPoint(menuNavigateSound, Vector3.zero);
        }

        // Handle item and technique menu navigation
        if (moveInput.y > 0 && moveAction.triggered && isInItemMenu)
        {
            selectedItemIndex = (selectedItemIndex - 1 + GameManager.Instance.player.Inventory.Count) % GameManager.Instance.player.Inventory.Count; // Move up in item menu
            AudioSource.PlayClipAtPoint(menuNavigateSound, Vector3.zero);
        }
        else if (moveInput.y < 0 && moveAction.triggered && isInItemMenu)
        {
            selectedItemIndex = (selectedItemIndex + 1) % GameManager.Instance.player.Inventory.Count; // Move down in item menu
            AudioSource.PlayClipAtPoint(menuNavigateSound, Vector3.zero);
        }

        if (moveInput.y > 0 && moveAction.triggered && isInTechniqueMenu)
        {
            selectedTechniqueIndex = (selectedTechniqueIndex - 1 + GameManager.Instance.player.Techniques.Count) % GameManager.Instance.player.Techniques.Count; // Move up in technique menu
            AudioSource.PlayClipAtPoint(menuNavigateSound, Vector3.zero);
        }
        else if (moveInput.y < 0 && moveAction.triggered && isInTechniqueMenu)
        {
            selectedTechniqueIndex = (selectedTechniqueIndex + 1) % GameManager.Instance.player.Techniques.Count; // Move down in technique menu
            AudioSource.PlayClipAtPoint(menuNavigateSound, Vector3.zero);
        }

        // Handle button confirmation
        if (confirmAction.triggered && BattleManager.Instance.isShowingInput && !isInItemMenu && !isInTechniqueMenu)
        {
            ExecuteSelectedButton();
            AudioSource.PlayClipAtPoint(menuSelectSound, Vector3.zero);
        }

        else if (confirmAction.triggered && BattleManager.Instance.isShowingInput && isInItemMenu)
        {
            ExecuteItem();
            AudioSource.PlayClipAtPoint(menuSelectSound, Vector3.zero);
        }

        else if (confirmAction.triggered && BattleManager.Instance.isShowingInput && isInTechniqueMenu)
        {
            ExecuteTechnique();
            AudioSource.PlayClipAtPoint(menuSelectSound, Vector3.zero);
        }

        // Handle cancel action
        if (cancelAction.triggered && BattleManager.Instance.isShowingInput)
        {
            HideItemMenu();
            isInItemMenu = false;
            HideTechniqueMenu();
            isInTechniqueMenu = false;
            AudioSource.PlayClipAtPoint(menuCancelSound, Vector3.zero);
        }

        // Show or hide the button panel based on the battle state
        if (BattleManager.Instance.isShowingInput)
        {
            buttonPanel.transform.position = Vector2.Lerp(buttonPanel.transform.position, buttonShowTarget.position, Time.deltaTime * 10f);

            // Highlight the selected button
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
        }
        else
        {
            buttonPanel.transform.position = Vector2.Lerp(buttonPanel.transform.position, buttonHideTarget.position, Time.deltaTime * 10f);
        }

        // Ideally, we wouldn't do this. Whatever.
        ShowSelectedItem();
        ShowSelectedTechnique();



    }

    private void ExecuteSelectedButton()
    {
        // Look, this is a bad implementation. But look at the bright side! There's only five buttons right now.
        if (selectedButtonIndex == 0)
        {
            BattleManager.Instance.OnReelButton();
        }
        else if (selectedButtonIndex == 1)
        {
            BattleManager.Instance.OnHoldSteadyButton();
        }
        else if (selectedButtonIndex == 2)
        {
            BattleManager.Instance.OnItemButton();
            isInItemMenu = true;
        }
        else if (selectedButtonIndex == 3)
        {
            BattleManager.Instance.OnTechniqueButton();
            isInTechniqueMenu = true;
        }
        else if (selectedButtonIndex == 4)
        {
            BattleManager.Instance.OnSnapLineButton();
        }

    }

    private void ExecuteItem()
    {
        if (selectedItemIndex < 0 || selectedItemIndex >= GameManager.Instance.player.Inventory.Count)
        {
            Debug.LogWarning("Invalid item selection.");
            return;
        }

        BattleManager.Instance.OnUseItem(selectedItemIndex);
        HideItemMenu();
        isInItemMenu = false;
    }

    private void ExecuteTechnique()
    {
        if (selectedTechniqueIndex < 0 || selectedTechniqueIndex >= GameManager.Instance.player.Techniques.Count)
        {
            Debug.LogWarning("Invalid technique selection.");
            return;
        }

        bool success = BattleManager.Instance.OnUseTechnique(selectedTechniqueIndex);
        if (success) HideTechniqueMenu();
        isInTechniqueMenu = false;
    }

    public void ShowItemMenu()
    {
        itemMenu.SetActive(true);
        itemMenuText.text = "Select an item to use:";
        selectedItemIndex = 0;

        // Populate the item menu with items from the player's inventory
        IPlayer player = GameManager.Instance.player;
        if (player.Inventory.Count == 0)
        {
            itemMenuText.text = "No items available.";
            return;
        }
    }

    public void HideItemMenu()
    {
        itemMenu.SetActive(false);
        itemMenuText.text = "";
        selectedItemIndex = 0;
    }

    public void ShowTechniqueMenu()
    {
        techniqueMenu.SetActive(true);
        techniqueMenuText.text = "Select a technique to use:";
        selectedTechniqueIndex = 0;

        // Populate the technique menu with techniques from the player's arsenal
        IPlayer player = GameManager.Instance.player;
        if (player.Techniques.Count == 0)
        {
            techniqueMenuText.text = "No techniques available.";
            return;
        }
    }

    public void HideTechniqueMenu()
    {
        techniqueMenu.SetActive(false);
        techniqueMenuText.text = "";
        selectedTechniqueIndex = 0;
    }

    public void ShowSelectedItem()
    {
        itemMenuText.text = "";
        //Highlight the selected item in the item menu
        IPlayer player = GameManager.Instance.player;
        for (int i = 0; i < player.Inventory.Count; i++)
        {
            IItem item = player.GetItem(i);
            if (i == selectedItemIndex)
            {
                itemMenuText.text += $"\n<color=yellow>{item.Name}</color>";
            }
            else
            {
                itemMenuText.text += $"\n{item.Name}";
            }
        }


    }

    public void ShowSelectedTechnique()
    {
        techniqueMenuText.text = "";
        // Highlight the selected technique in the technique menu
        IPlayer player = GameManager.Instance.player;
        for (int i = 0; i < player.Techniques.Count; i++)
        {
            ITechnique technique = player.GetTechnique(i);
            if (i == selectedTechniqueIndex)
            {
                techniqueMenuText.text += $"\n<color=yellow>{technique.Name} - {technique.Cost} MP</color>";
            }
            else
            {
                techniqueMenuText.text += $"\n{technique.Name} - {technique.Cost} MP";
            }
        }
    }

    public System.Collections.IEnumerator FadeOutBattleMusic()
    {
        while (battleMusicSource.volume > 0)
        {
            battleMusicSource.volume -= 0.01f;
            yield return new WaitForSeconds(0.1f);
        }
        battleMusicSource.Stop();
        battleMusicSource.volume = 1f;
    }
}



