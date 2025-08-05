using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BasePlayer : IPlayer
{
    [SerializeField] private int level;
    [SerializeField] private int exp;
    [SerializeField] private int expToNext;
    [SerializeField] private string playerName;
    [SerializeField] private int exhaustion;
    [SerializeField] private int maxExhaustion;
    [SerializeField] private bool bracing;
    [SerializeField] private int mp;
    [SerializeField] private int maxMP;
    [SerializeField] private int attack;
    [SerializeField] private int defense;
    [SerializeField] private List<IItem> inventory;
    [SerializeField] private List<ITechnique> techniques;

    public string Name => playerName;
    public int LVL => level;
    public int EXH => exhaustion;
    public int EXP => exp;
    public int EXPtoNext => expToNext;
    public int MaxEXH => maxExhaustion;
    public bool Bracing => bracing;
    public int MP => mp;
    public int MaxMP => maxMP;
    public int ATK => attack;
    public int DEF => defense;
    public List<IItem> Inventory => inventory;
    public List<ITechnique> Techniques => techniques;


    // Constructor.
    // We create a player like this:
    // Player player = new Player("PlayerName", 100, 20, 10, 5);
    public BasePlayer(int lvl, int exp, int exptonext, string name, int maxExh, int maxMp, int atk, int def)
    {
        this.level = lvl;
        this.exp = exp;
        this.expToNext = exptonext;
        playerName = name;
        maxExhaustion = maxExh;
        exhaustion = 0;
        maxMP = maxMp;
        mp = maxMp;
        attack = atk;
        defense = def;
        bracing = false;
        inventory = new List<IItem>();
        techniques = new List<ITechnique>();
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

    public ITechnique GetTechnique(int index)
    {
        return Techniques[index];
    }

    public IItem GetItem(int index)
    {
        if (index < 0 || index >= inventory.Count) return null;
        return inventory[index];
    }
}
