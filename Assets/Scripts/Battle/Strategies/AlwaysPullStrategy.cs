using System.IO;

public class AlwaysPullStrategy : IStrategy
{


    public AlwaysPullStrategy()
    {

    }

    public ICommand ChooseCommand(IFish self, IPlayer player)
    {
        return new PullCommand(self.ATK);
    }
}