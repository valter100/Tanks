using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIitemSwap : MonoBehaviour
{
    //[SerializeField] GameObject bomb;
    //[SerializeField] GameObject grenade;
    //List<GameObject> Icons;
    [SerializeField] GameObject[] Icons;
    int currentIndex;

    // Start is called before the first frame update
    void Start()
    {
        //Icons = GameObject.FindGameObjectsWithTag("Icon");
        currentIndex = 0;
        Icons[currentIndex].SetActive(true);
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateIconOnProjectileSwap(int index)
    {
        Icons[currentIndex].SetActive(false);
        currentIndex = index;
        Icons[currentIndex].SetActive(true);
    }
}
