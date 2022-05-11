using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tanks
{
    public class MapObject : MonoBehaviour
    {
        //[SerializeField] private bool rotateAroundZ;
        private Rigidbody rb;
        private Rigidbody Rigidbody => rb ??= GetComponent<Rigidbody>();

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.tag == "Tank" || collision.gameObject.tag == "Projectile")
                Destroy(gameObject);

            //if (collision.gameObject.tag == "Map")
            //    AttachToObject();
        }

        //private void OnCollisionExit(Collision collision)
        //{
        //    if (collision.gameObject.tag == "Map")
        //        DetachFromObject();
        //}

        //private void OnCollisionStay(Collision collision)
        //{
        //    if (collision.gameObject.tag == "Map")
        //    {
        //        if (IsInvoking(nameof(DetachFromObject)))
        //            CancelInvoke(nameof(DetachFromObject));
        //        Invoke(nameof(DetachFromObject), 0.1f);
        //    }
                
        //}

        //private void AttachToObject()
        //{
        //    Ray ray = new Ray(transform.position, Vector3.down);
        //    RaycastHit hit;
        //    bool hitATarget = Physics.Raycast(ray, out hit);
        //    if (hitATarget)
        //    {
        //        Quaternion rotation;
        //        if (rotateAroundZ)
        //            rotation = Quaternion.Euler(Mathf.Atan2(hit.normal.y, hit.normal.x) * Mathf.Rad2Deg, 90, 90);
        //        else
        //            rotation = Quaternion.Euler(0, 0, Mathf.Atan2(hit.normal.y, hit.normal.x) * Mathf.Rad2Deg);
        //        Rigidbody.rotation = Quaternion.Euler(rotation.eulerAngles + new Vector3(90, 0));

        //        Rigidbody.constraints = RigidbodyConstraints.FreezeAll;
        //    }
        //}

        //private void DetachFromObject()
        //{
        //    Rigidbody.constraints = RigidbodyConstraints.FreezePositionZ;
        //}
    }
}


