using System.Collections.Generic;
using UnityEngine;

public static class ItemData
{
    public static Dictionary<int, IItem> items = new Dictionary<int, IItem>
    {
        { 0, new WaterBottle()

        },
    };
}
