using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class FollowCursor : MonoBehaviour
{
    [SerializeField] Image image;
    Vector3 offset;

    private void Start()
    {
        image = GetComponent<Image>();
        offset = new Vector2(-image.sprite.bounds.size.x/4, -image.sprite.bounds.size.y/4);
    }

    void Update()
    {
        transform.position = Input.mousePosition + offset;
    }
}
