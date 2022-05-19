using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tanks
{
    public class MapObject : MonoBehaviour
    {
        [Range(0, 90)]
        [SerializeField] float degreeLimit;
        private Rigidbody rb;
        private Rigidbody Rigidbody => rb ??= GetComponent<Rigidbody>();

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.tag == "Tank" || collision.gameObject.tag == "Projectile")
                Destroy(gameObject);

            if (collision.gameObject.tag == "Map")
                AttachToObject();
        }

        private void OnCollisionExit(Collision collision)
        {
            if (collision.gameObject.tag == "Map")
                DetachFromObject();
        }

        private void OnCollisionStay(Collision collision)
        {
            if (collision.gameObject.tag == "Map")
            {
                if (IsInvoking(nameof(DetachFromObject)))
                    CancelInvoke(nameof(DetachFromObject));
                Invoke(nameof(DetachFromObject), 0.1f);
            }

        }
        private void AttachToObject()
        {
            Ray ray = new Ray(transform.position, Vector3.down);
            RaycastHit hit;
            bool hitATarget = Physics.Raycast(ray, out hit);
            if (hitATarget)
            {
                float zRotation = Mathf.Clamp(Mathf.Atan2(hit.normal.y, hit.normal.x) * Mathf.Rad2Deg, 90 - degreeLimit, 90 + degreeLimit);
                Rigidbody.rotation = Quaternion.Euler(0, 0, zRotation);
                Rigidbody.constraints = RigidbodyConstraints.FreezeAll;
            }
        }

        private void DetachFromObject()
        {
            Rigidbody.constraints = RigidbodyConstraints.FreezePositionZ;
        }
    }
}


