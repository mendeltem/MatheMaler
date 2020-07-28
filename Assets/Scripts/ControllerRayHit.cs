using UnityEngine;

namespace Picasso
{
    public class ControllerRayHit : MonoBehaviour
    {

        public GameObject Dummy;
        
        public GameObject hit
        {
            get => whiteboard.lineHitObject;
            set => whiteboard.lineHitObject = value;
        }
        
        public Material oiriginalMaterial;

        public Material hitMaterial;

        private MeshRenderer renderer;

        public bool changeMaterial;

        public Whiteboard whiteboard;

		public GameObject HitBox;

        private void Awake()
        {
            renderer = GetComponent<MeshRenderer>();
            HitBox = GameObject.FindWithTag("HitBox");
        }

        void Start()
        {
            oiriginalMaterial = renderer.material;
        }

        void OnTriggerEnter(Collider other)
        {
            var o = other.gameObject;
            hit = o;
            //Debug.Log("hit object " + o.transform.parent.gameObject.gameObject.tag);
            
        }

        private void OnTriggerExit(Collider other)
        {
            hit = Dummy;
        }

        private void OnTriggerStay(Collider other)
        {
            hit = other.gameObject;
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