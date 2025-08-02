using UnityEngine;

public class WaterBottle : IItem
{
    public string Name => "Water Bottle";
    public string Description => "A refreshing bottle of water that relieves thirst.";
    public ICommand Command { get; private set; }

    public WaterBottle()
    {
        Command = new WaterBottleCommand(this);
    }
}
