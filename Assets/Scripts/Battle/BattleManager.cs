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
    private Queue<string> messageQueue = new Queue<string>();
    public bool isShowingMessage = false;
    public bool isShowingInput = false;
    public TMP_Text logText;

    public TMP_Text playerStats;



    // Needs to be expanded if we decide to have a party system.
    public enum BattleState { START, PLAYERTURN, FISHTURN, WIN, LOSE }
    public BattleState state;

    //DUMMY SPRITE FOR NOW :))))
    public Sprite dummySprite;

    void Start()
    {
        state = BattleState.START;

        // Until we can pass values into the battle manager, we will use this to set up the battle.
        // This is a placeholder for the actual battle setup logic.
        StartCoroutine(SetupBattle(new BasePlayer("Fisherman", 100, 30, 10, 5), new BaseFish("Smallmouth Bass", 20, 20, 10, 5, dummySprite)));
    }

    void Update()
    {
        Debug.Log(fish.DST);
    }


    IEnumerator SetupBattle(BasePlayer player, BaseFish fish)
    {
        this.player = player;
        this.fish = fish;

        FishSpriteRenderer.sprite = fish.Sprite;
        strategy = new AlwaysPullStrategy(fish.ATK);
        EnqueueMessage($"{fish.Name} is on the other end, will you answer?");
        yield return new WaitForSeconds(2f);

        // I'm going to set the state to PLAYERTURN here because I don't want to do it in the PlayerTurn() function,
        // so that if we call it by accident we don't screw up the order of turns.
        state = BattleState.PLAYERTURN;
        PlayerTurn();
    }

    // This method is called when the player presses the reel button.
    // It creates a ReelCommand and executes it.
    public void OnReelButton()
    {
        if (state != BattleState.PLAYERTURN) return; // Just
        ICommand reelCommand = new ReelCommand(5);
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
        ICommand snapLineCommand = new SnapLineCommand(10);
        StartCoroutine(ExecutePlayerCommand(snapLineCommand));
    }


    // We use a coroutine to execute commands so that we can yield for potential animations or effects.
    IEnumerator ExecutePlayerCommand(ICommand command)
    {
        isShowingInput = false;
        command.Execute(player, fish, this);
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

        EnqueueMessage("Choose an Action:");
        //TODO: Overhaul this!!!!!!!
    }

    // We use a coroutine to execute commands so that we can yield for potential animations or effects.
    IEnumerator FishTurn()
    {
        if (state != BattleState.FISHTURN) Debug.LogWarning("It's not the fish's turn!");

        ICommand command = strategy.ChooseCommand(fish, player); // Unlike the player, the fish doesn't have buttons to press, so we use the strategy to choose a command.
        command.Execute(fish, player, this);
        UpdateStats();

        yield return new WaitForSeconds(1f); // Simulate some delay for the command execution.

        if (player.EXH > 100)
        {
            state = BattleState.LOSE;
            EndBattle();
        }
        else
        {
            state = BattleState.PLAYERTURN; // Switch back to player's turn.
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
        }
    }

    // Ideally, this should be in a UIManager class, whoops.
    public void EnqueueMessage(string message)
    {
        messageQueue.Enqueue(message);
        if (!isShowingMessage)
            StartCoroutine(ShowMessages());
    }

    IEnumerator ShowMessages()
    {
        isShowingMessage = true;

        while (messageQueue.Count > 0)
        {
            string nextMessage = messageQueue.Dequeue();
            yield return StartCoroutine(TypeText(nextMessage));

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

        if (logText.text == "Choose an Action:") isShowingInput = true; //TODO: Fix this later


        BattleUIManager.Instance.confirmAction.performed -= ctx => skip = true;
    }


    public void UpdateStats()
    {
        playerStats.text = $"{player.Name}\nEXH: {player.EXH}/{player.MaxEXH}\nMP: {player.MP}/{player.MaxMP}";
        //enemyStats.text = $"Enemy: {enemy.Name}\nHP: {enemy.HP}/{enemy.MaxHP}\nMP: {enemy.MP}/{enemy.MaxMP}";
    }


}
