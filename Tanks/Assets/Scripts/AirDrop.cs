using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirDrop : MonoBehaviour
{
    public GameObject GroundDetection;
    public GameObject Canopy;
    //public Light DropLight;
    public ParticleSystem Smoke;
    private Rigidbody AirdropRB;
    private bool landed = false;
    // Start is called before the first frame update
    void Start()
    {
        AirdropRB = transform.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit objectHit;

        if(Physics.Raycast(transform.position, Vector3.down, out objectHit, 1))
        {
            if(objectHit.collider.gameObject.name != "Tank1fbx")
            {
                landed = true;
            }
        }

        if (landed)
        {
            DropHasLanded();
            landed = false;
        }
    }

    void DropHasLanded()
    {
        //DropLight.gameObject.SetActive(true);
        Smoke.gameObject.SetActive(true);
        AirdropRB.drag = 0;
        AirdropRB.mass = 5000;
        Destroy(GroundDetection);
        Destroy(Canopy);
    }
}
