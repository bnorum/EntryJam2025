using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine.InputSystem;

public class TextManager : MonoBehaviour
{
    public static TextManager instance;

    void Awake()
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

    Queue<(string, bool)> messageQueue = new Queue<(string, bool)>();
    bool isShowingMessage = false;
    public TextMeshProUGUI logText;
    public UnityEngine.UI.Image textBoxImage;

    public InputAction confirmAction;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        confirmAction = InputSystem.actions.FindAction("Submit");

        
    }

    // Update is called once per frame
    void Update()
    {



    }


    public void EnqueueMessage(string message, bool isPlayerTurnMessage = false)
    {
        messageQueue.Enqueue((message, isPlayerTurnMessage));
        if (!isShowingMessage)
            StartCoroutine(ShowMessages());
    }

    IEnumerator ShowMessages()
    {
        isShowingMessage = true;

        while (messageQueue.Count > 0)
        {
            (string nextMessage, bool isPlayerTurnMessage) = messageQueue.Dequeue();
            yield return StartCoroutine(TypeText(nextMessage));



            yield return new WaitUntil(() => confirmAction.triggered);
        }


        isShowingMessage = false;
    }

    IEnumerator TypeText(string message)
    {
        logText.text = "";

        bool skip = false;
        confirmAction.performed += ctx => skip = true;

        for (int i = 0; i < message.Length; i++)
        {
            if (skip)
            {
                logText.text = message;
                break;
            }

            logText.text += message[i];
            yield return new WaitForSeconds(0.02f);
        }




        confirmAction.performed -= ctx => skip = true;
    }
}
