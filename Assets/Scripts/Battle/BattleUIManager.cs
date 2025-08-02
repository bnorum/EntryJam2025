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

    public Transform buttonHideTarget;
    public Transform buttonShowTarget;
    public GameObject buttonPanel;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        moveAction = InputSystem.actions.FindAction("Move");
        confirmAction = InputSystem.actions.FindAction("Jump");
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
        if (moveInput.x > 0 && moveAction.triggered)
        {
            selectedButtonIndex = (selectedButtonIndex + 1) % buttons.Count; // Move right
        }
        else if (moveInput.x < 0 && moveAction.triggered)
        {
            selectedButtonIndex = (selectedButtonIndex - 1 + buttons.Count) % buttons.Count; // Move left
        }

        if (confirmAction.triggered && BattleManager.Instance.isShowingInput)
        {
            ExecuteSelectedButton();
        }
        if (BattleManager.Instance.isShowingInput)
        {
            buttonPanel.transform.position = Vector2.Lerp(buttonPanel.transform.position, buttonShowTarget.position, Time.deltaTime * 10f);
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



    }

    private void ExecuteSelectedButton()
    {
        // Look, this is a bad implementation. But look at the bright side! There's only three buttons right now.
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
            BattleManager.Instance.OnSnapLineButton();
        }

    }



}
