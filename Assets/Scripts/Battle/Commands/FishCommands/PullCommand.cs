using UnityEngine;

public class PullCommand : ICommand
{
    private int exhaustionDamage;

    public PullCommand(int damage)
    {
        exhaustionDamage = damage;
    }

    public void Execute(ICombatant self, ICombatant player, BattleManager battleManager)
    {
        // Logic for executing the pull command
        Debug.Log($"Executing Pull with exhaustion damage: {exhaustionDamage}");
    }

    public string GetDescription(ICombatant self, ICombatant player)
    {
        return $"Pulls the line with exhaustion damage: {exhaustionDamage}.";
    }
}
