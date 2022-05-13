using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Drawing;
using System;
using Random = UnityEngine.Random;
//using System.Windows.Media;

public class AirDrop : MonoBehaviour
{
    public enum Type
    {
        Health,
        Fule,
        Amo
    }

    public enum AmoType
    {
        Bullet,
        ExplodingBullet,
        RollBomb,
        Slowing,
        BiggerExplosion,
        DropBomb,
        BounceBomb,
        ShockWave
    }

    [SerializeField] public Color color;
    [SerializeField] protected GameObject Crate;
    public GameObject GroundDetection;
    public GameObject Canopy;
    //public Light DropLight;
    public ParticleSystem Smoke;
    private Rigidbody AirDropRB;
    private bool Landed = false;
    private Type type;
    private AmoType amo;
    private int amoTypes;
    private int amount;

    public Type GetCrateType() => type;

    public AmoType GetAmoType() => amo;

    public int GetAmount() => amount;

    // Start is called before the first frame update
    void Start()
    {
        AirDropRB = transform.GetComponent<Rigidbody>();
        amoTypes = Enum.GetValues(typeof(AmoType)).Length;

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
            switch (type)
            {
                case Type.Health:
                    collision.gameObject.GetComponent<Tank>().SetHealth(amount);
                    break;
                case Type.Fule:
                    collision.gameObject.GetComponent<Tank>().SetFuel(amount);
                    break;
                case Type.Amo:
                    //increase the amo amount based on the "amotype" and "amount"
                    break;
                default:
                    break;
            }
            Crate.SetActive(false);
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

    private void SetColor(Type type)
    {
        switch (type)
        {
            case Type.Health:
                Crate.GetComponent<Renderer>().material.color = Color.red;
                amount = 10;
                break;
            case Type.Fule:
                Crate.GetComponent<Renderer>().material.color = new Color(139f/255f, 69f/255f, 19f/255f, 1f); //Should be brown
                amount = 20;
                break;
            case Type.Amo:
                SetAmoType();
                SetAmoColor(amo);
                amount = 2;
                break;
            default:
                break;
        }
    }

    private void SetAmoColor(AmoType index)
    {
        //Purple 147,112,219
        //blue 0,0,255
        //Green 0, 128, 0,
        //Yellow 255, 255, 0
        int enumLen = Enum.GetValues(typeof(AmoType)).Length;
        float r = 0f + (int)index * 255f / (float)enumLen;
        float g = 0f + (int)index * 255f / (float)enumLen;
        float b = 255f - (int)index * 255f / (float)enumLen;
        Crate.GetComponent<Renderer>().material.color = new Color(r/255f, g/255f, b/255f, 1f);
    }

    private void SetAmoType()
    {
        int random = Random.Range(0, 10 * amoTypes);
        int index = random / 10;
        amo = (AmoType)index;
    }

    public void SetCrateType()
    {
        int random = Random.Range(0, 10 * Enum.GetValues(typeof(Type)).Length);
        int index = random / 10;
        type = (Type)index;
        if (type == Type.Amo)
            SetAmoType();
        SetColor(type);
    }

    
}
