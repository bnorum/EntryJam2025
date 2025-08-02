using System.Collections.Generic;
using UnityEngine;


// This is the interface for the player character. We use an interface incase we want to vary player stats in some way, or have multiple characters acting.

public interface IPlayer : ICombatant
{
    int EXH { get; } // Exhaustion. A unique player value.
    int MaxEXH { get; } // Max Exhaustion. A unique player value.
    bool Bracing { get; } // Defending, but for fishing.

    List<IItem> Inventory { get; }
    List<ITechnique> Techniques { get; }


    void Exhaust(int amount); // Increase Exhaustion
    void Relieve(int amount); // Decrease Exhaustion

    void Brace(); // Defend
    void Unbrace(); // Stop Defending

    ITechnique GetTechnique(int index); // Get a technique from the player's list
    IItem GetItem(int index); // Get an item from the player's inventory
}
