using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class QuitManager : MonoBehaviour
{
    public static QuitManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public InputAction QuitAction;

    public float QuitHoldTime = 3.5f; // Time in seconds to hold the quit button
    private float quitTimer = 0f;

    public TextMeshProUGUI quitText;
    public CanvasGroup quitCanvasGroup;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        QuitAction = InputSystem.actions.FindAction("Quit");
    }

    // Update is called once per frame
    void Update()
    {
        if (QuitAction == null)
            return;

        if (QuitAction.ReadValue<float>() > 0)
        {
            quitTimer += Time.deltaTime;
            if (quitTimer >= QuitHoldTime)
            {
                if (SceneManager.GetActiveScene().name != "Title")
                {
                    SaveManager.Save(new SaveData(
                    GameManager.Instance.overworldPlayerPosition.position.x,
                    GameManager.Instance.overworldPlayerPosition.position.y,
                    GameManager.Instance.currentMap,
                    GameManager.Instance.player
                    ));
                }
                Application.Quit();

            }
            quitText.text = $"Quitting" + new string('.', (int)(quitTimer * 2) % 4);
            quitCanvasGroup.alpha = Mathf.Clamp01(quitTimer / QuitHoldTime);
        }
        else
        {
            quitTimer = 0f;
            quitText.text = "Hold to Quit";
            quitCanvasGroup.alpha = 0f;
        }
    }
}
