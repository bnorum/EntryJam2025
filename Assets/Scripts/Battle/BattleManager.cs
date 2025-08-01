using System;
using System.Collections;
using UnityEngine;

public class BattleManager : MonoBehaviour
{

    public static BattleManager Instance;
    // This object is a singleton. This is not the most efficient way of setting up the system,
    // but because we are running on such a low profile, it shouldn't be an issue.

    // Don't put more than one of these in a scene. It will blow up your computer.

    public IPlayer player;
    public IFish fish;
    public IStrategy strategy;

    public SpriteRenderer FishSpriteRenderer;

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

    // Needs to be expanded if we decide to have a party system.
    public enum BattleState { START, PLAYERTURN, ENEMYTURN, WIN, LOSE }
    public BattleState state;

    void Start()
    {
        state = BattleState.START;
        StartCoroutine(SetupBattle());
    }

    void Update()
    {

    }

    // Until we can pass values into the battle manager, we will use this to set up the battle.
    // This is a placeholder for the actual battle setup logic.
    IEnumerator SetupBattle()
    {
        player = new BasePlayer("Fisherman", 100, 30, 10, 5);
        fish = new BaseFish("Smallmouth Bass", 20, 20, 10, 5);

        FishSpriteRenderer.sprite = fish.Sprite;
        strategy = new AlwaysPullStrategy(fish.ATK);

        yield return null;

        // I'm going to set the state to PLAYERTURN here because I don't want to do it in the PlayerTurn() function,
        // so that if we call it by accident we don't screw up the order of turns.
        state = BattleState.PLAYERTURN;
        PlayerTurn();
    }

    // This method is called when the player presses the reel button.
    // It creates a ReelCommand and executes it.
    public void OnReelButton()
    {
        ICommand reelCommand = new ReelCommand(5);
        StartCoroutine(ExecutePlayerCommand(reelCommand));
    }


    // We use a coroutine to execute commands so that we can yield for potential animations or effects.
    IEnumerator ExecutePlayerCommand(ICommand command)
    {
        command.Execute(player, fish, this);
        yield return null;
    }

    void PlayerTurn()
    {
        if (state != BattleState.PLAYERTURN)
        {
            Debug.LogWarning("It's not the player's turn!");
            return;
        }
        // Player Turn Logic
    }

    // We use a coroutine to execute commands so that we can yield for potential animations or effects.
    IEnumerator FishTurn()
    {
        if (state != BattleState.ENEMYTURN) Debug.LogWarning("It's not the enemy's turn!");

        ICommand command = strategy.ChooseCommand(fish, player); // Unlike the player, the fish doesn't have buttons to press, so we use the strategy to choose a command.
        command.Execute(fish, player, this);

        yield return new WaitForSeconds(1f); // Simulate some delay for the command execution.

        if (player.EXH <= 0)
        {
            state = BattleState.LOSE;
            Debug.Log("You lost the battle!");
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
            Debug.Log("You won the battle!");
            // Handle win logic
        }
        else if (state == BattleState.LOSE)
        {
            Debug.Log("You lost the battle!");
            // Handle lose logic
        }
    }

    //As of 8/1 at 3:37, this is unfinished. We need a UI manager to handle the UI updates, and a way of displaying dialogue.

}
