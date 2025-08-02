using UnityEngine;

public class SuperReelTechnique : ITechnique
{
    public string Name => "Super Reel";
    public string Description => "Reel in the fish with incredible force.";
    public int Cost => 20;

    public void Execute(ICombatant self, ICombatant target, BattleManager battleManager)
    {
        Debug.Log($"Using technique: {Name} on {target.Name}");
        battleManager.EnqueueMessage($"{self.Name} uses {Name}!");
        int reelDistance = 20;
        ((IFish)target).DecreaseDistance(reelDistance);
        battleManager.EnqueueMessage($"{self.Name} reels in the line by {reelDistance} feet.");
        ((IPlayer)self).SpendMP(Cost);
    }

    public string GetDescription(ICombatant self, ICombatant target)
    {
        return $"{self.Name} uses {Name}: {Description}";
    }
}
