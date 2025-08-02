using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BattleManager : MonoBehaviour
{

    public static BattleManager Instance;
    // This object is a singleton. This is not the most efficient way of setting up the system,
    // but because we are running on such a low profile, it shouldn't be an issue.

    // Don't put more than one of these in a scene. It will blow up your computer.

    void Awake()
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


    public IPlayer player;
    public IFish fish;
    public IStrategy strategy;

    public SpriteRenderer FishSpriteRenderer;

    // Queue of messages that will be displayed in the UI.
    // I use a queue in case two messages are sent at the same time, so that we can display them one after the other.
    private Queue<(string text, bool showInput)> messageQueue = new Queue<(string, bool)>();
    public bool isShowingMessage = false;
    public bool isShowingInput = false;
    public TMP_Text logText;

    public TMP_Text playerStats;



    // Needs to be expanded if we decide to have a party system.
    public enum BattleState { START, PLAYERTURN, FISHTURN, WIN, LOSE, RUN}
    public BattleState state;

    //DUMMY SPRITE FOR NOW :))))
    public Sprite dummySprite;

    void Start()
    {
        state = BattleState.START;

        // Until we can pass values into the battle manager, we will use this to set up the battle.
        // This is a placeholder for the actual battle setup logic.
        // Instead of using this, eventually we will have a transitionary script which will pass the player and fish objects to the battle manager.
        StartCoroutine(SetupBattle(new BasePlayer("Fisherman", 100, 30, 10, 5), new BaseFish("Smallmouth Bass", 20, 20, 10, 5, dummySprite)));
    }

    void Update()
    {

    }


    IEnumerator SetupBattle(BasePlayer p, BaseFish f)
    {
        player = p;
        DemoMethod();
        fish = f;
        UpdateStats();

        FishSpriteRenderer.sprite = fish.Sprite;
        strategy = new AlwaysPullStrategy(fish.ATK);
        EnqueueMessage($"{fish.Name} is on the other end, will you answer?");
        yield return new WaitForSeconds(2f);

        // I'm going to set the state to PLAYERTURN here because I don't want to do it in the PlayerTurn() function,
        // so that if we call it by accident we don't screw up the order of turns.
        state = BattleState.PLAYERTURN;
        PlayerTurn();
    }

    public void DemoMethod()
    {
        player.Inventory.Add(new WaterBottle());
        player.Inventory.Add(new WaterBottle());
        player.Techniques.Add(new SuperReelTechnique());
    }

    // This method is called when the player presses the reel button.
    // It creates a ReelCommand and executes it.
    public void OnReelButton()
    {
        if (state != BattleState.PLAYERTURN) return; // Just
        ICommand reelCommand = new ReelCommand(player.ATK);
        StartCoroutine(ExecutePlayerCommand(reelCommand));
    }

    public void OnHoldSteadyButton()
    {
        if (state != BattleState.PLAYERTURN) return; // In
        ICommand holdSteadyCommand = new HoldSteadyCommand(player.DEF);
        StartCoroutine(ExecutePlayerCommand(holdSteadyCommand));
    }

    public void OnSnapLineButton()
    {
        if (state != BattleState.PLAYERTURN) return; // Case
        state = BattleState.RUN;
        EndBattle();
    }

    public void OnTechniqueButton()
    {
        if (state != BattleState.PLAYERTURN) return;
        BattleUIManager.Instance.ShowTechniqueMenu();
    }

    public void OnItemButton()
    {
        if (state != BattleState.PLAYERTURN) return;
        BattleUIManager.Instance.ShowItemMenu();
    }

    public bool OnUseTechnique(int techniqueIndex)
    {
        if (state != BattleState.PLAYERTURN) return false; // Just in case
        ICommand techniqueCommand = player.GetTechnique(techniqueIndex);
        if (techniqueCommand == null)
        {
            Debug.LogWarning("Invalid technique index.");
            return false;
        }

        // Check if player has enough MP for the technique

        if (player.MP < player.GetTechnique(techniqueIndex).Cost)
        {
            //play error sound
            return false; // I return a boolean so I can pass it back to UI manager and tell it not to close the menu
        }
        else
        {
            StartCoroutine(ExecutePlayerCommand(techniqueCommand));
            return true;
        }

    }

    public void OnUseItem(int itemIndex)
    {
        if (state != BattleState.PLAYERTURN) return;

        if (itemIndex < 0 || itemIndex >= player.Inventory.Count)
        {
            EnqueueMessage("Invalid item selection.");
            return;
        }

        IItem item = player.Inventory[itemIndex];
        player.Inventory.RemoveAt(itemIndex); // Consume the item

        ICommand command = item.Command;
        StartCoroutine(ExecutePlayerCommand(command));
        BattleUIManager.Instance.HideItemMenu();
    }


    // We use a coroutine to execute commands so that we can yield for potential animations or effects.
    IEnumerator ExecutePlayerCommand(ICommand command)
    {

        command.Execute(player, fish, this);
        isShowingInput = false; //Hide input UI after execution
        UpdateStats(); // Update the stats after executing the command
        yield return new WaitForSeconds(1f);

        if (fish.DST <= 0)
        {
            state = BattleState.WIN;
            EndBattle();
        }
        else
        {
            state = BattleState.FISHTURN;
            StartCoroutine(FishTurn());
        }
    }

    void PlayerTurn()
    {
        if (state != BattleState.PLAYERTURN)
        {
            Debug.LogWarning("It's not the player's turn!");
            return;
        }

        EnqueueMessage("Choose an Action:", true);
    }

    // We use a coroutine to execute commands so that we can yield for potential animations or effects.
    IEnumerator FishTurn()
    {
        if (state != BattleState.FISHTURN)
            Debug.LogWarning("It's not the fish's turn!");

        // Choose attack first
        ICommand command = strategy.ChooseCommand(fish, player);

        // Enqueue fish attack message BEFORE executing
        EnqueueMessage($"{fish.Name} lashes out!"); // Or a more descriptive attack message

        // Wait until the queued message is fully displayed and confirmed
        yield return new WaitUntil(() => messageQueue.Count == 0 && !isShowingMessage);

        // Now execute attack AFTER player has acknowledged the message
        command.Execute(fish, player, this);
        UpdateStats();

        yield return new WaitForSeconds(1f); // Optional animation delay

        if (player.EXH > 100)
        {
            state = BattleState.LOSE;
            EndBattle();
        }
        else
        {
            state = BattleState.PLAYERTURN;
            PlayerTurn();
        }
    }

    void EndBattle()
    {
        if (state == BattleState.WIN)
        {
            EnqueueMessage($"You caught the {fish.Name}!");
            // Handle win logic
        }
        else if (state == BattleState.LOSE)
        {
            EnqueueMessage("Your line snapped!");
            // Handle lose logic
        } else if (state == BattleState.RUN)
        {
            EnqueueMessage("You snapped your line!");
            // Handle run logic
        }
    }

    // Ideally, this should be in a UIManager class, whoops.
    public void EnqueueMessage(string message, bool isPlayerTurnMessage = false)
    {
        messageQueue.Enqueue((message, isPlayerTurnMessage));
        if (!isShowingMessage)
            StartCoroutine(ShowMessages());
    }

    IEnumerator ShowMessages()
    {
        isShowingMessage = true;

        while (messageQueue.Count > 0)
        {
            (string nextMessage, bool isPlayerTurnMessage) = messageQueue.Dequeue();
            yield return StartCoroutine(TypeText(nextMessage));

            isShowingInput = isPlayerTurnMessage;

            yield return new WaitUntil(() => BattleUIManager.Instance.confirmAction.triggered);
        }


        isShowingMessage = false;
    }

    IEnumerator TypeText(string message)
    {
        logText.text = "";

        bool skip = false;
        BattleUIManager.Instance.confirmAction.performed += ctx => skip = true;

        for (int i = 0; i < message.Length; i++)
        {
            if (skip)
            {
                logText.text = message;
                break;
            }

            logText.text += message[i];
            yield return new WaitForSeconds(0.02f);
        }




        BattleUIManager.Instance.confirmAction.performed -= ctx => skip = true;
    }


    public void UpdateStats()
    {
        playerStats.text = $"{player.Name}\nEXH: {player.EXH}/{player.MaxEXH}\nMP: {player.MP}/{player.MaxMP}";
        //enemyStats.text = $"Enemy: {enemy.Name}\nHP: {enemy.HP}/{enemy.MaxHP}\nMP: {enemy.MP}/{enemy.MaxMP}";
    }


}
