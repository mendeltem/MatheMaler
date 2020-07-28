using System;
using UnityEngine;

namespace Picasso
{

    public class RayHit : MonoBehaviour
    {
        public Transform hit;

        public Material oiriginalMaterial;

        public Material hitMaterial;

        private MeshRenderer renderer;

        public bool changeMaterial;
        

        private void Awake()
        {
            renderer = GetComponent<MeshRenderer>();
        }

        void Start()
        {
            oiriginalMaterial = renderer.material;
        }

        void OnTriggerEnter(Collider other)
        {
            hit = other.transform;
            //Debug.Log("hit " + other.gameObject.name);
        }

        private void OnTriggerExit(Collider other)
        {
            hit = null;
        }

        private void OnTriggerStay(Collider other)
        {
            hit = other.transform;
        }

        private void Update()
        {
            if(!changeMaterial) return;
            
            if (hit != null && hit.gameObject != null)
            {
                renderer.material = hitMaterial;
            }
            else
            {
                renderer.material = oiriginalMaterial;
            }
        }
    }
}