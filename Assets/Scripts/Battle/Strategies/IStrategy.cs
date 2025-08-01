
// This is the interface for the strategy pattern in the battle system.
// It allows for different strategies to be implemented for enemies choosing actions in battle.


public interface IStrategy
{
    ICommand ChooseCommand(IFish self, IPlayer player);
}
