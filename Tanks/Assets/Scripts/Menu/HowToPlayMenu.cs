using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HowToPlayMenu : Menu
{
    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {

    }

    protected override void OnEnable()
    {

    }

    protected override void OnDisable()
    {

    }

    public void OnClick_Back()
    {
        menuManager.SwapMenu(this, Type.Main);
    }
}
