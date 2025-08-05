using System.IO;

public class RainbowTroutStrategy : IStrategy
{


    public RainbowTroutStrategy()
    {

    }

    public ICommand ChooseCommand(IFish self, IPlayer player)
    {
        return new PullCommand(self.ATK);
    }
}