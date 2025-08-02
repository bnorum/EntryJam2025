using UnityEngine;

public class WaterBottleCommand : ICommand
{
    private IItem item;

    public WaterBottleCommand(IItem item)
    {
        this.item = item;
    }

    public void Execute(ICombatant self, ICombatant target, BattleManager battleManager)
    {
        Debug.Log($"Using item: {item.Name} on {target.Name}");
        // Logic to apply the item's effect
        battleManager.EnqueueMessage($"You drink the water bottle.");
        ((IPlayer)self).Relieve(10);
    }

    public string GetDescription(ICombatant self, ICombatant target)
    {
        return $"{self.Name} uses {item.Name}: {item.Description}";
    }
}

