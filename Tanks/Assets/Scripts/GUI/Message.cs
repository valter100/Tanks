using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Message : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private Animator animator;
    [SerializeField] private float duration;
    [SerializeField] private Vector3 velocity;

    private Vector3? worldPosition;
    private Vector3 offset;


    void Start()
    {

    }

    void Update()
    {
        if (worldPosition == null)
        {
            transform.position += velocity * Time.deltaTime;
        }

        else
        {
            offset += velocity * Time.deltaTime;
            transform.position = Camera.main.WorldToScreenPoint(worldPosition.Value) + offset;
        }

        duration -= Time.deltaTime;

        if (duration <= 0.3f)
            animator.SetTrigger("Disappearing");

        if (duration <= 0.0f)
            Destroy(gameObject);
    }

    /// <summary>
    /// (Mandatory) Sets the text of the message.
    /// </summary>
    public Message SetText(string text)
    {
        this.text.text = text;
        return this;
    }

    /// <summary>
    /// (Optional) Sets the world position of the message.
    /// </summary>
    public Message SetWorldPosition(Vector3 worldPosition)
    {
        this.worldPosition = worldPosition;
        return this;
    }

    /// <summary>
    /// (Optional) Sets the color of the message text.
    /// </summary>
    public Message SetColor(Color color)
    {
        text.color = color;
        return this;
    }

    /// <summary>
    /// (Optional) Sets the duration of the message.
    /// </summary>
    public Message SetDuration(float duration)
    {
        this.duration = duration;
        return this;
    }

}
