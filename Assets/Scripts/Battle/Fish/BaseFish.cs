using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BaseFish : IFish
{
    [SerializeField] private string fishName;
    [SerializeField] private int distance;
    [SerializeField] private int maxDistance;
    [SerializeField] private int mp;
    [SerializeField] private int maxMP;
    [SerializeField] private int attack;
    [SerializeField] private int defense;
    [SerializeField] private Sprite sprite;

    public string Name => fishName;
    public int DST => distance;
    public int MaxDST => maxDistance;
    public int MP => mp;
    public int MaxMP => maxMP;
    public int ATK => attack;
    public int DEF => defense;
    public Sprite Sprite => sprite;


    // Constructor.
    // We create a fish like this:
    // BaseFish fish = new BaseFish("FishName", 100, 20, 10, 5);
    public BaseFish(string name, int maxDist, int maxMp, int atk, int def, Sprite spr)
    {
        fishName = name;
        maxDistance = maxDist;
        distance = maxDist;
        maxMP = maxMp;
        sprite = spr;
        mp = maxMp;
        attack = atk;
        defense = def;
    }

    public void IncreaseDistance(int amount)
    {
        distance = Mathf.Min(maxDistance, distance + amount);
    }

    public void DecreaseDistance(int amount)
    {
        distance = Mathf.Max(0, distance - amount);
    }

    public void SpendMP(int amount)
    {
        mp = Mathf.Max(0, mp - amount);
    }
    public void RestoreMP(int amount)
    {
        mp = Mathf.Min(maxMP, mp + amount);
    }




}
