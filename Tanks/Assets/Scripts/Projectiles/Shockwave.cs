using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tanks
{

    public class Shockwave : Projectile
    {
        [Header("Shockwave specific")]
        [SerializeField] float pushbackRadius;
        [SerializeField] float pushbackForce;

        protected override void OnCollision(Collider other)
        {
            base.OnCollision(other);
            Detonate(other);
        }

        protected override void Detonate(Collider other)
        {
            base.Detonate(other);
            PushbackExplosion();
        }

        public void PushbackExplosion()
        {
            Instantiate(detonationParticles, transform.position, Quaternion.Euler(-90, 0, 0));

            Collider[] tankColliders = Physics.OverlapSphere(transform.position, pushbackRadius);

            foreach (Collider collider in tankColliders)
            {
                Tank tank = collider.GetComponent<Tank>();

                if (tank)
                    tank.GetComponent<Rigidbody>().AddExplosionForce(pushbackForce, transform.position, pushbackRadius);
            }
        }
    }
}