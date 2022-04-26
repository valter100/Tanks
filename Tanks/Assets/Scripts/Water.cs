using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Water : MonoBehaviour
{
    [SerializeField] ParticleSystem splashEffect;
    private void OnCollisionEnter(Collision collision)
    {
        Instantiate(splashEffect, collision.transform.position, Quaternion.identity, null);
    }
}
