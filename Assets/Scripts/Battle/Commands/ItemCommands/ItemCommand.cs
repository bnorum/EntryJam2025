using UnityEngine;

public class ItemCommand : ICommand
{
    private IItem item;

    public ItemCommand(IItem item)
    {
        this.item = item;
    }

    public void Execute(ICombatant self, ICombatant target, BattleManager battleManager)
    {
        Debug.Log($"Using item: {item.Name} on {target.Name}");
        // Logic to apply the item's effect
        battleManager.EnqueueMessage($"{self.Name} uses {item.Name} on {target.Name}.");
    }

    public string GetDescription(ICombatant self, ICombatant target)
    {
        return $"{self.Name} uses {item.Name}: {item.Description}";
    }
}

