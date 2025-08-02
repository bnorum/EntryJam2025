using UnityEngine;

public class ReelCommand : ICommand
{
    private int reelDistance;

    public ReelCommand(int distance)
    {
        reelDistance = distance;
    }

    public void Execute(ICombatant self, ICombatant fish, BattleManager battleManager)
    {
        Debug.Log($"Executing Reel with distance: {reelDistance}");
        ((IFish)fish).DecreaseDistance(reelDistance);
        battleManager.EnqueueMessage($"{self.Name} reels in the line by {reelDistance} feet.");
    }

    public string GetDescription(ICombatant self, ICombatant fish)
    {
        return $"Reels the line by {reelDistance} units.";
    }
}
