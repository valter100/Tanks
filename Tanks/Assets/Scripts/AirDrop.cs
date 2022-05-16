using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Drawing;
using System;
using Random = UnityEngine.Random;
using System.Linq;
//using System.Windows.Media;

public class AirDrop : MonoBehaviour
{
    public enum Type
    {
        Health,
        Fuel,
        Item
    }

    [SerializeField] public Color color;
    [SerializeField] protected GameObject Crate;

    public GameObject GroundDetection;
    public GameObject Canopy;
    //public Light DropLight;
    public ParticleSystem smokeParticles;
    private new Rigidbody rigidbody;
    private bool landed;
    private Type type;
    private Prefab usablePrefab;
    private int amount;

    public Type GetCrateType() => type;

    public Prefab GetUsablePrefab() => usablePrefab;

    public int GetAmount() => amount;

    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        SetType();
    }

    void Update()
    {
        RaycastHit objectHit;

        if (Physics.Raycast(transform.position, Vector3.down, out objectHit, 1))
        {
            if (objectHit.collider.gameObject.tag != "Tank")
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

    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Tank")
        {
            Tank tank = collision.gameObject.GetComponent<Tank>();

            switch (type)
            {
                case Type.Health:
                    tank.AddHealth(amount);
                    break;

                case Type.Fuel:
                    tank.AddFuel(amount);
                    break;

                case Type.Item:
                    tank.GetPlayer().Inventory.AddItem(Prefabs.Usables[usablePrefab].GetComponent<Usable>(), amount, true);
                    break;
            }

            Crate.SetActive(false);
        }

        else if (collision.gameObject.tag == "Water")
        {
            Crate.SetActive(false);
        }
    }

    void DropHasLanded()
    {
        //DropLight.gameObject.SetActive(true);
        smokeParticles.gameObject.SetActive(true);
        rigidbody.drag = 0;
        rigidbody.mass = 5000;
        Destroy(GroundDetection);
        Destroy(Canopy);
    }

    private void SetColor(Type type)
    {
        Color color;

        switch (type)
        {
            case Type.Health:
                color = Color.green;
                amount = 10;
                break;

            case Type.Fuel:
                color = new Color(0f, 190f/255f, 1f);
                amount = 75;
                break;

            case Type.Item:
                color = GetUsableColor();
                amount = 2;
                break;

            default:
                color = Color.white;
                amount = 0;
                break;
        }

        Crate.GetComponent<Renderer>().material.color = color;
    }

    private Color GetUsableColor()
    {
        // Purple 147, 112, 219
        // Blue     0,   0, 255
        // Green    0, 128,   0
        // Yellow 255, 255,   0

        int index = Prefabs.Usables.Keys.ToList().FindIndex(prefab => prefab == usablePrefab);

        float r =   0f + index * 255f / Prefabs.Usables.Count;
        float g =   0f + index * 255f / Prefabs.Usables.Count;
        float b = 255f - index * 255f / Prefabs.Usables.Count;
        return new Color(r/255f, g/255f, b/255f, 1f);
    }

    private void SetUsablePrefab()
    {
        int index = Random.Range(0, Prefabs.Usables.Count);
        usablePrefab = Prefabs.Usables.Keys.ElementAt(index);
    }

    public void SetType()
    {
        type = (Type)Random.Range(0, Enum.GetValues(typeof(Type)).Length);

        if (type == Type.Item)
            SetUsablePrefab();

        SetColor(type);
    }

}
