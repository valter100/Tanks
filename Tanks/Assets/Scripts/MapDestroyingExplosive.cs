using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tanks
{
    public class MapDestroyingExplosive : MonoBehaviour
    {
        [SerializeField] bool explode;
        [SerializeField] float explosionRadius;

        TerrainDestruction[] terrainDestroyers;

        private void Start()
        {
            
        }

        void Update()
        {
            if (explode)
            {
                explode = false;
                Explode();
            }
        }

        private void Explode()
        {
            GameObject[] gameObjects = GameObject.FindGameObjectsWithTag("Map");
            foreach (GameObject gameObject in gameObjects)
                gameObject.GetComponent<TerrainDestruction>().DestroyTerrain(transform.position, explosionRadius);
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(transform.position, explosionRadius);
        }
    }
}

