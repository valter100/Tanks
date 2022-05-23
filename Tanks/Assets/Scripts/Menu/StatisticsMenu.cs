using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatisticsMenu : Menu
{
    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        if (playerController.Trigger_Back())
            OnClick_Back();
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
