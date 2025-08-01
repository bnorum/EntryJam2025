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
        if (!((IPlayer)player).Bracing)
        {
            ((IPlayer)player).Exhaust(Mathf.Max(1, exhaustionDamage / 2 - ((IPlayer)player).DEF / 2));
            ((IFish)self).IncreaseDistance(exhaustionDamage / 4); // Assuming pulling the line also affects the fish's distance. Which it does.
            battleManager.EnqueueMessage($"{self.Name} tugs and exhausts you for {Mathf.Max(1, exhaustionDamage / 2 - ((IPlayer)player).DEF / 2)} points.");
            ((IPlayer)player).Unbrace();
        }
        else
        {
            ((IPlayer)player).Exhaust(Mathf.Max(1, exhaustionDamage - ((IPlayer)player).DEF / 2));
            ((IFish)self).IncreaseDistance(exhaustionDamage / 4); // Ensures distance is not negative.
            battleManager.EnqueueMessage($"{self.Name} tugs and exhausts you for {Mathf.Max(1, exhaustionDamage / 4 - ((IPlayer)player).DEF / 2)} points.");
        }


        Debug.Log($"Executing Pull with exhaustion damage: {exhaustionDamage}");
    }

    public string GetDescription(ICombatant self, ICombatant player)
    {
        return $"Pulls the line with exhaustion damage: {exhaustionDamage}.";
    }
}
