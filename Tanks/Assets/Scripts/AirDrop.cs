using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirDrop : MonoBehaviour
{
    public enum Type
    {
        Health,
        Fule,
        Amo

    }
    public GameObject GroundDetection;
    public GameObject Canopy;
    //public Light DropLight;
    public ParticleSystem Smoke;
    private Rigidbody AirDropRB;
    private bool Landed = false;
    private Type type;
    //private static Random rnd;

    // Start is called before the first frame update
    void Start()
    {
        AirDropRB = transform.GetComponent<Rigidbody>();
        int random = Random.Range(0, 3);
        type = Type.Amo;
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit objectHit;

        if(Physics.Raycast(transform.position, Vector3.down, out objectHit, 1))
        {
            if(objectHit.collider.gameObject.tag != "Tank")
            {
                Landed = true;
            }
        }

        if (Landed)
        {
            DropHasLanded();
            Landed = false;
        }
    }

    public void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Tank")
        {
            collision.gameObject.GetComponent<Tank>().SetHealth(10);
        }
    }

    void DropHasLanded()
    {
        //DropLight.gameObject.SetActive(true);
        Smoke.gameObject.SetActive(true);
        AirDropRB.drag = 0;
        AirDropRB.mass = 5000;
        Destroy(GroundDetection);
        Destroy(Canopy);
    }
}
