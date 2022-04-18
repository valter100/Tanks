using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreezeBullet : Bullet
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<Tank>() != null && other.gameObject.GetComponent<Tank>() == ownTank)
            return;

        Hit(other.gameObject);
    }

    public override void Hit(GameObject other)
    {
        other.GetComponent<Tank>().SetIsSlowed(true);
        base.Hit(other);
    }
}
