using System;
using UnityEngine;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;


namespace Picasso
{
    //[RequireComponent(typeof(LineRenderer))]
    public class Line : MonoBehaviour
    {
        public Transform start;

        public Transform end;
        
		public Transform pivot;

		public int draw_type;

        public float width;

        public Material material;

        public LineRenderer lineRenderer;

        public GameObject quad;

		public int numClicks = 0;

		public Vector3 dir => end.position - start.position;

		public int positionCount = 0;

		public GameObject coord;
		
		public GameObject player;

		public float start_position_x;
		public float start_position_y;
		public float start_position_z;

		public float end_position_x;
		public float end_position_y;
		public float end_position_z;

		public TextMeshProUGUI start_text;
		public TextMeshProUGUI end_text;
		
		private bool coloring;
		
		void Start()
		{
			coord = GameObject.FindWithTag("coord");
			
			//player=coord.transform.parent.gameObject;
			
			lineRenderer = GetComponent<LineRenderer>();
			boxCollider = quad.GetComponent<BoxCollider>();

			//meshCollider = quad.GetComponent<BoxCollider>();

			//end.SphereCollider.enabled = false;
            
			boxCollider.enabled = false;
			quadRenderer = quad.GetComponent<MeshRenderer>();

			showVertices = true;
			quadRenderer.material = material;
			lineRenderer.material = material;
			
			
		}

		//GameObject newText = new GameObject(text.Replace(" ", "-"), typeof(RectTransform));
		
        public BoxCollider boxCollider;

		

        public Vector3 boxScale = Vector3.one;
        
        public MeshRenderer quadRenderer;
        
        public float linelength => (end.position - start.position).magnitude;
        
        public void Update()
        {
	        
            // http://answers.unity.com/answers/554566/view.html
            var P1 = start.position;
            var P2 = end.position;
            
            //position_x = end.position.x - player.transform.position.x - coord.transform.position.x;
            end_position_x = (end.position.x - coord.transform.position.x) * 100f;
            end_position_y = (end.position.y - coord.transform.position.y - 0.07f) * 100f;
            end_position_z = (end.position.z - coord.transform.position.z- 0.03f) * 100f;


            start_position_x = (start.position.x - coord.transform.position.x) * 100f;
            start_position_y = (start.position.y - coord.transform.position.y - 0.07f) * 100f;
            start_position_z = (start.position.z - coord.transform.position.z- 0.03f) * 100f;
            
            var P21 = P2 - P1;

			var pivot = start.position;

			
			//var newTextComp = newText.AddComponent<Text>();

			if (draw_type ==1)
			{
            	quad.transform.position   = P1 + P21 / 2.0f;
            	quad.transform.localScale = new Vector3(width, P21.magnitude, width);
            	quad.transform.up         = P21;
                boxCollider.size          =  new Vector3(3f, 0.8f, 3f);

            	if (lineRenderer.enabled)
            	{

                lineRenderer.SetPositions(new[] {start.position, end.position});

                lineRenderer.startWidth  = width;
                lineRenderer.endWidth    = width;
                //lineRenderer.material    = material;
                //lineRenderer.startColor  = material.color;
                //lineRenderer.endColor    = material.color;
            	}

			}
			else if (draw_type == 2)
			{
                lineRenderer.SetVertexCount(numClicks + 1);
                lineRenderer.SetPosition(numClicks, end.position );
                numClicks++;
                lineRenderer.startWidth = width;
                lineRenderer.endWidth = width;
                lineRenderer.material = material;
                lineRenderer.startColor = material.color;
                lineRenderer.endColor = material.color;
				
				start.GetComponent<MeshRenderer>().enabled = false;
		        end.GetComponent<MeshRenderer>().enabled = false;
			}			
			else if (draw_type == 3)
			{
            	quad.transform.position   = P1 + P21 / 2.0f;
            	quad.transform.localScale = new Vector3(width, P21.magnitude, width);
            	quad.transform.up         = P21;
                boxCollider.size          =  new Vector3(3f, 0.8f, 3f);

            	if (lineRenderer.enabled)
            	{

				Vector3 dir = end.position - start.position;

				Vector3 extended_vector =  dir * 1000f;

				lineRenderer.SetPositions(new[] {start.position, end.position + extended_vector});

                lineRenderer.startWidth = width;
                lineRenderer.endWidth = width;
                lineRenderer.material = material;
                lineRenderer.startColor = material.color;
                lineRenderer.endColor = material.color;
            	}
			}
			else if (draw_type == 4)
			{
            	
            	
			}


			start_text.text  = "( X: "+start_position_x.ToString("F0") +" , Y: "+
									start_position_z.ToString("F0") +" , Z:"+
									start_position_y.ToString("F0")+" )";
			end_text.text     = "( X: "+end_position_x.ToString("F0") +" , Y: "+
									end_position_z.ToString("F0") +" , Z:"+
									end_position_y.ToString("F0")+" )";									


		}
        
        public bool showVertices
        {
	        set
	        {
		        start.GetComponent<MeshRenderer>().enabled = true;
		        end.GetComponent<MeshRenderer>().enabled = true;
		        start.transform.localScale  = new Vector3(0.03f, 0.03f, 0.03f);
		        end.transform.localScale  = new Vector3(0.03f, 0.03f, 0.03f);
	        }
        }


    }
}