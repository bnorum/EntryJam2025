[System.Serializable]

public class SaveData
{
    public float posX;
    public float posY;
    public int mapNumber;
    public int level;
    public int exp;
    public int expToNext;
    public string playerName;
    public int exhaustion;
    public int maxExhaustion;
    public bool bracing;
    public int mp;
    public int maxMP;
    public int attack;
    public int defense;
    public int[] itemIDs; // List of item IDs, assuming items are identified by their IDs
    // will have to get creative about serializing items.

    public SaveData(float posX, float posY, int mapNumber, IPlayer player)
    {
        this.posX = posX;
        this.posY = posY;
        this.mapNumber = mapNumber;
        this.level = player.LVL;
        this.exp = player.EXP;
        this.expToNext = player.EXPtoNext;
        this.playerName = player.Name;
        this.exhaustion = player.EXH;
        this.maxExhaustion = player.MaxEXH;
        this.bracing = player.Bracing;
        this.mp = player.MP;
        this.maxMP = player.MaxMP;
        this.attack = player.ATK;
        this.defense = player.DEF;
        this.itemIDs = new int[ItemData.items.Count];
        for (int i = 0; i < ItemData.items.Count; i++)
        {
            itemIDs[i] = ItemData.items[i].ID; 
        }
    }

}