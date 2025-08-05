using UnityEngine;

public class WaterBottle : IItem
{
    public int ID => 0; 
    public string Name => "Water Bottle";
    public string Description => "A refreshing bottle of water that relieves ten exhaustion.";
    public ICommand Command { get; private set; }

    public WaterBottle()
    {
        Command = new WaterBottleCommand(this);
    }
}
