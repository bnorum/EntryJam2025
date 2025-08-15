using UnityEngine;
using UnityEngine.InputSystem;

public class Dock : MonoBehaviour
{
    [SerializeField] public int dockIndex;
    [SerializeField] public string dockName;

    InputAction talkAction;

    public bool canOpen = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        talkAction = InputSystem.actions.FindAction("Talk");
    }



    // Update is called once per frame
    void Update()
    {
        if (canOpen && talkAction.triggered)
        {
            TravelManager.Instance.OpenTravelCanvas();
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            canOpen = true;
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            canOpen = false;
        }
    }
}
