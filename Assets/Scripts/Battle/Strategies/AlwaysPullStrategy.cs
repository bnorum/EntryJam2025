public class AlwaysPullStrategy : IStrategy
{
    private int pullDistance;

    public AlwaysPullStrategy(int distance = 5)
    {
        pullDistance = distance;
    }

    public ICommand ChooseCommand(IFish self, IPlayer player)
    {
        return new PullCommand(pullDistance);
    }
}