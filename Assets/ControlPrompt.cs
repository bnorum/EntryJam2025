using UnityEngine;

public class ControlPrompt : MonoBehaviour
{

    public SpriteRenderer promptImage;
    public Sprite upSprite;

    public bool ShowPrompt;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        promptImage.gameObject.SetActive(ShowPrompt);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Dock"))
        {
            ShowPrompt = true;
            promptImage.sprite = upSprite;
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Dock"))
        {
            ShowPrompt = false;
        }
    }
}
