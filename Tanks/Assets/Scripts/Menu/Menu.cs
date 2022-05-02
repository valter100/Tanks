using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Menu : MonoBehaviour
{
    public enum Type { Main, Play, Settings, Statistics, HowToPlay }

    static protected MenuManager menuManager;

    protected virtual void Start()
    {
        if (menuManager == null)
            menuManager = GameObject.Find("Canvas").GetComponent<MenuManager>();
    }

    protected virtual void Update()
    {
        
    }

    protected abstract void OnEnable();

    protected abstract void OnDisable();

    protected void Disable()
    {
        gameObject.SetActive(false);
    }
}
