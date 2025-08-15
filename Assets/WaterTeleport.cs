using UnityEngine;

public class WaterTeleport : MonoBehaviour
{
    public int dockIndex = 0;
    public Vector3 outLocation;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            GameManager.Instance.overworldPlayerPosition.position = outLocation;
        }
    }
}
