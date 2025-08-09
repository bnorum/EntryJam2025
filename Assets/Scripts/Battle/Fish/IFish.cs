using UnityEngine;


// This is the interface for the fish character.

public interface IFish : ICombatant
{
    int DST { get; } // Distance, a Unique fish value.
    int MaxDST { get; } // Max Distance, a Unique fish value.
    Sprite Sprite { get; } // The fish sprite.
    IStrategy Strategy { get; } // The fish's strategy.
    int XP { get; }

    void IncreaseDistance(int amount); // Increase Distance
    void DecreaseDistance(int amount); // Decrease Distance
}
