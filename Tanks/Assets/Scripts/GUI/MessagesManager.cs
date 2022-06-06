using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MessagesManager : MonoBehaviour
{
    [SerializeField] private GameObject messagePrefab;

    private static MessagesManager messagesManager;

    private static List<Message> delayedMessages;
    private static List<int> delayedMessagesFrames;

    private void Start()
    {
        messagesManager = this;
        delayedMessages = new List<Message>();
        delayedMessagesFrames = new List<int>();
    }

    private void Update()
    {
        for (int i = delayedMessages.Count - 1; i >= 0; --i)
        {
            if (--delayedMessagesFrames[i] == 0)
            {
                Message message = delayedMessages[i];
                message.GetComponent<Animator>().enabled = true;
                message.GetComponent<TextMeshProUGUI>().enabled = true;

                delayedMessages.RemoveAt(i);
                delayedMessagesFrames.RemoveAt(i);
            }
        }
    }

    public static Message AddMessage(string text)
    {
        Message message = Instantiate(messagesManager.messagePrefab, messagesManager.transform).GetComponent<Message>();
        message.name = text;
        return message.SetText(text);
    }

    public static Message AddDelayedMessage(int frames, string text)
    {
        Message message = Instantiate(messagesManager.messagePrefab, messagesManager.transform).GetComponent<Message>();
        message.name = text;

        message.GetComponent<Animator>().enabled = false;
        message.GetComponent<TextMeshProUGUI>().enabled = false;

        delayedMessages.Add(message);
        delayedMessagesFrames.Add(frames);

        return message.SetText(text);
    }

    public static void ClearAllMessages()
    {
        foreach (Transform child in messagesManager.transform)
            Destroy(child.gameObject);
    }

}
