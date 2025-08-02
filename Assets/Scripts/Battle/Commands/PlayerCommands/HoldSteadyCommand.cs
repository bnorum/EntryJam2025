using UnityEngine;

public class HoldSteadyCommand : ICommand
{
    private int defenseAmount;

    public HoldSteadyCommand(int defenseAmount)
    {
        this.defenseAmount = defenseAmount;
    }

    public void Execute(ICombatant user, ICombatant target, BattleManager battle)
    {
        ((IPlayer)user).Brace(); // Risky business. But never let a fish brace.
        battle.EnqueueMessage($"{user.Name} holds steady.");
    }

    public string GetDescription(ICombatant user, ICombatant target)
    {
        return $"{user.Name} will hold steady.";
    }
}
