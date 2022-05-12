using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator : MonoBehaviour
{
    [SerializeField] Vector3 rotation;
    [SerializeField] float speed;
    Vector3 totalRotation;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //transform.Rotate(rotation * speed * Time.deltaTime);
        totalRotation += rotation * speed * Time.deltaTime;
        totalRotation.x = 24;
        transform.rotation = Quaternion.Euler(totalRotation);
    }
}
