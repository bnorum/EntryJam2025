using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BasePlayer : IPlayer
{
    [SerializeField] private string playerName;
    [SerializeField] private int exhaustion;
    [SerializeField] private int maxExhaustion;
    [SerializeField] private bool bracing;
    [SerializeField] private int mp;
    [SerializeField] private int maxMP;
    [SerializeField] private int attack;
    [SerializeField] private int defense;
    [SerializeField] private List<Item> inventory;

    public string Name => playerName;
    public int EXH => exhaustion;
    public int MaxEXH => maxExhaustion;
    public bool Bracing => bracing;
    public int MP => mp;
    public int MaxMP => maxMP;
    public int ATK => attack;
    public int DEF => defense;
    public List<Item> Inventory => inventory;


    // Constructor.
    // We create a player like this:
    // Player player = new Player("PlayerName", 100, 20, 10, 5);
    public BasePlayer(string name, int maxExh, int maxMp, int atk, int def)
    {
        playerName = name;
        maxExhaustion = maxExh;
        exhaustion = 0;
        maxMP = maxMp;
        mp = maxMp;
        attack = atk;
        defense = def;
        bracing = false;
        inventory = new List<Item>();
    }



    public void Exhaust(int amount)
    {
        exhaustion = Mathf.Min(maxExhaustion, exhaustion + amount);
    }

    public void Relieve(int amount)
    {
        exhaustion = Mathf.Max(0, exhaustion - amount);
    }

    public void SpendMP(int amount)
    {
        mp = Mathf.Max(0, mp - amount);
    }
    public void RestoreMP(int amount)
    {
        mp = Mathf.Min(maxMP, mp + amount);
    }

    public void Brace()
    {
        bracing = true;
    }

    public void Unbrace()
    {
        bracing = false;
    }

    public void UseItem(int index)
    {
        if (index < 0 || index >= inventory.Count) return;

        Item item = inventory[index];
        // Implement item usage logic here
    }
}
