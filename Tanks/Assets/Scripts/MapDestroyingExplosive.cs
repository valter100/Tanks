using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tanks
{
    public class MapDestroyingExplosive : MonoBehaviour
    {
        [SerializeField] bool debugRadius;
        [SerializeField] bool explode;
        [SerializeField] float explosionRadius;
        void Update()
        {
            if (explode)
            {
                explode = false;
                Explode();
            }
        }

        public void Explode()
        {
            GameObject[] gameObjects = GameObject.FindGameObjectsWithTag("Map");
            foreach (GameObject gameObject in gameObjects)
                gameObject.GetComponent<TerrainDestruction>().DestroyTerrain(transform.position, explosionRadius);
        }

        public void Explode(float newRadius)
        {
            GameObject[] gameObjects = GameObject.FindGameObjectsWithTag("Map");
            foreach (GameObject gameObject in gameObjects)
                gameObject.GetComponent<TerrainDestruction>().DestroyTerrain(transform.position, newRadius);
        }

        private void OnDrawGizmos()
        {
            if (debugRadius)
                Gizmos.DrawWireSphere(transform.position, explosionRadius);
        }
    }
}

