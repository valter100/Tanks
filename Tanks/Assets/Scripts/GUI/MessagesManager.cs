using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MessagesManager : MonoBehaviour
{
    [SerializeField] private GameObject messagePrefab;

    private static MessagesManager messagesManager;

    private void Start()
    {
        messagesManager = this;
    }

    public static Message AddMessage(string text)
    {
        Message message = Instantiate(messagesManager.messagePrefab, messagesManager.transform).GetComponent<Message>();
        message.name = text;
        return message.SetText(text);
    }

    public static void ClearAllMessages()
    {
        while (messagesManager.transform.childCount > 0)
            Destroy(messagesManager.transform.GetChild(0).gameObject);
    }

}
