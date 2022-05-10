using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tanks
{
    public class MapObject : MonoBehaviour
    {
        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.tag == "Tank" || collision.gameObject.tag == "Projectile")
                Destroy(gameObject);
        }
    }
}


