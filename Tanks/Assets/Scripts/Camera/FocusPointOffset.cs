using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FocusPointOffset : MonoBehaviour
{
    [SerializeField] Vector3 focusPointOffset;

    public Vector3 offset => focusPointOffset;
}
