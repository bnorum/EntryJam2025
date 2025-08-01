using UnityEngine;

// Battles implement the "Command" design pattern.
// For more info, see here: https://gameprogrammingpatterns.com/command.html

public interface ICommand
{
    void Execute(ICombatant user, ICombatant target, BattleManager battleManager);
    string GetDescription(ICombatant user, ICombatant target);
}
