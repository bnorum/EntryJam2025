using UnityEngine;

public interface ICombatant
{
    string Name { get; }

    int MP { get; } // Placeholder Magic Variable
    int MaxMP { get; } // Placeholder Max Magic Variable
    int ATK { get; } // Placeholder Attack Variable
    int DEF { get; } // Placeholder Defense Variable

    void SpendMP(int amount); // Use MP
    void RestoreMP(int amount); // duh
}
