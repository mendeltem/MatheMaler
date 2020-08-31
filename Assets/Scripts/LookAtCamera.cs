using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Picasso
{
    public class LookAtCamera : MonoBehaviour
    {
        public GameObject camera;

        
        void Update()
        {
            // Rotate the camera every frame so it keeps looking at the target
            transform.LookAt(camera.transform);
            transform.rotation = Quaternion.LookRotation(transform.position - camera.transform.position);
            
        }
    }
}