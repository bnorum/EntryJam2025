using UnityEngine;


//RIGHT NOW, THIS IS JUST REEL COMMAND.

//TODO: MAKE THIS A "RUN" COMMAND.

public class SnapLineCommand : ICommand
{
    private int reelDistance;

    public SnapLineCommand(int distance)
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
