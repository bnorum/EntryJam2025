using UnityEngine;

public class FishBattleTest : MonoBehaviour
{
    public Sprite troutSprite;
    public GameObject dummyBackground;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Battle Stated!");
        GameManager.Instance.StartBattle(new BaseFish("Rainbow Trout", 35, 20, 10, 5, troutSprite, new RainbowTroutStrategy()), dummyBackground);
    }
}
