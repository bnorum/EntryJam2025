using UnityEngine;

public class ReelCommand : ICommand
{
    private int reelDistance;

    public ReelCommand(int distance)
    {
        reelDistance = distance;
    }

    public void Execute(ICombatant self, ICombatant player, BattleManager battleManager)
    {
        // Logic for executing the reel command
        Debug.Log($"Executing Reel with distance: {reelDistance}");
    }

    public string GetDescription(ICombatant self, ICombatant player)
    {
        return $"Reels the line by {reelDistance} units.";
    }
}
