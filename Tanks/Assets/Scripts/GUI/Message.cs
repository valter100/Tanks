using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Message : MonoBehaviour
{
    [SerializeField] private RectTransform rect;
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private Animator animator;
    [SerializeField] private float duration;
    [SerializeField] private Vector3 velocity;

    private Vector3? worldPosition;
    private Vector3 offset;

    private void Start()
    {
        transform.localPosition += new Vector3(0.0f, 0.0f, 10.0f);
    }

    private void Update()
    {
        if (worldPosition == null)
        {
            transform.localPosition += velocity * Time.deltaTime;
        }

        else
        {
            offset += velocity * Time.deltaTime;

            Vector3 screenPoint = Camera.main.WorldToScreenPoint(worldPosition.Value);
            Vector3 viewpointPoint = Camera.main.ScreenToViewportPoint(screenPoint + offset);

            rect.anchorMax = rect.anchorMin = viewpointPoint;
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
        transform.localPosition = Vector3.zero;
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

    /// <summary>
    /// (Optional) Sets the speed of the message.
    /// </summary>
    public Message SetSpeed(float speed)
    {
        velocity *= speed;
        return this;
    }

}
