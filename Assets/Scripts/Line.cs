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

		public string draw_type;

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

		public TextMeshProUGUI start_text_x;
		public TextMeshProUGUI start_text_y;
		public TextMeshProUGUI start_text_z;

		public TextMeshProUGUI end_text_x;
		public TextMeshProUGUI end_text_y;
		public TextMeshProUGUI end_text_z;


		public TextMeshProUGUI LineVector_X;
		public TextMeshProUGUI LineVector_Y;
		public TextMeshProUGUI LineVector_Z;
		


		public BoxCollider boxCollider;
		public MeshCollider meshCollider;
		
		private bool coloring;

		public float radius;

		
		void Start()
		{
			// Suche nach dem Koordiantensystem
			coord = GameObject.FindWithTag("coord");
			lineRenderer = GetComponent<LineRenderer>();
			boxCollider = quad.GetComponent<BoxCollider>();
			boxCollider.enabled = false;
			meshCollider = quad.GetComponent<MeshCollider>();
			meshCollider.enabled = false;
			quadRenderer = quad.GetComponent<MeshRenderer>();
			showVertices = true;
			quadRenderer.material = material;
			lineRenderer.material = material;
			
		}


        public Vector3 boxScale = Vector3.one;
        
        public MeshRenderer quadRenderer;
        
		//die länge der Linie wird berechnet
        public float linelength => (end.position - start.position).magnitude;
        
        public void Update()
        {
	        
            // http://answers.unity.com/answers/554566/view.html
            var P1 = start.position;
            var P2 = end.position;


            //Ermittlung der lokalen Postition der Punkte 
            end_position_x = (end.position.x - coord.transform.position.x);
            end_position_y = (end.position.y - coord.transform.position.y - 0.07f);
            end_position_z = (end.position.z - coord.transform.position.z- 0.03f);


            start_position_x = (start.position.x - coord.transform.position.x);
            start_position_y = (start.position.y - coord.transform.position.y - 0.07f);
            start_position_z = (start.position.z - coord.transform.position.z- 0.03f);
            
			//erechnung von Linienvektor
            var P21 = P2 - P1;

			var pivot = start.position;

			if (draw_type =="straighline")
			{
				//Zeichne eine Linie mit Quads
            	quad.transform.position   = P1 + P21 / 2.0f;
            	quad.transform.localScale = new Vector3(width, P21.magnitude, width);
            	quad.transform.up         = P21;
				//Die BoxCollider sollten Kürze aber dafür länger sein um die Punkten nicht im Wege zu stehen
                boxCollider.size          =  new Vector3(3f, 0.8f, 3f);

            	if (lineRenderer.enabled)
            	{
					//Zeichne eine Linie mit Linerenderer
					lineRenderer.SetPositions(new[] {start.position, end.position});
					lineRenderer.startWidth  = width;
					lineRenderer.endWidth    = width;

            	}
				//Die Vektor Angaben von den Punkten
				start_text_x.text  = " "+(start_position_x*100).ToString("F0");
				start_text_y.text  = " "+(start_position_z*100).ToString("F0");
				start_text_z.text  = " "+(start_position_y*100).ToString("F0");

				end_text_x.text    = " "+(end_position_x*100).ToString("F0");
				end_text_y.text    = " "+(end_position_z*100).ToString("F0");
				end_text_z.text    = " "+(end_position_y*100).ToString("F0");

				//LineVectorLabel.transform.position = (start.position + end.position) / 2;

			}
			else if (draw_type == "free")
			{
				//zeichne Frei
                lineRenderer.SetVertexCount(numClicks + 1);
                lineRenderer.SetPosition(numClicks, end.position );
                numClicks++;
                lineRenderer.startWidth = width;
                lineRenderer.endWidth = width;
                lineRenderer.material = material;
                lineRenderer.startColor = material.color;
                lineRenderer.endColor = material.color;

				start.GetComponent<MeshRenderer>().enabled = false;
		        end.GetComponent<MeshRenderer>().enabled = true;
				end.GetComponent<MeshRenderer>().material = material;
				//der Endpunkt sollte etwas kleiner sein.
				end.transform.localScale  = new Vector3(0.008f, 0.008f, 0.008f);

			}			
			else if (draw_type == "circle")
			{
				var segments = 360;
				lineRenderer.useWorldSpace = false;
				lineRenderer.startWidth = width;
				lineRenderer.endWidth = width;
				lineRenderer.positionCount = segments + 1;

				var pointCount = segments + 1; // add extra point to make startpoint and endpoint the same to close the circle
				var points = new Vector3[pointCount];


				for (int i = 0; i < pointCount; i++)
				{
					var rad = Mathf.Deg2Rad * (i * 360f / segments);
					points[i] = new Vector3(Mathf.Sin(rad) * radius, 0, Mathf.Cos(rad) * radius);
				}

				lineRenderer.SetPositions(points);

				//Die Vektor Angaben von den Punkten
				start_text_x.text  = " "+(start_position_x*100).ToString("F0");
				start_text_y.text  = " "+(start_position_z*100).ToString("F0");
				start_text_z.text  = " "+(start_position_y*100).ToString("F0");


				end_text_x.text    = " "+(end_position_x*100).ToString("F0");
				end_text_y.text    = " "+(end_position_z*100).ToString("F0");
				end_text_z.text    = " "+(end_position_y*100).ToString("F0");
			}
			else if (draw_type == "points")
			{
				//Zeichne die Punkte

            	//Die Vektor Angaben von den Punkten
				start_text_x.text  = " "+(start_position_x*100).ToString("F0");
				start_text_y.text  = " "+(start_position_z*100).ToString("F0");
				start_text_z.text  = " "+(start_position_y*100).ToString("F0");


				end_text_x.text    = " "+(end_position_x*100).ToString("F0");
				end_text_y.text    = " "+(end_position_z*100).ToString("F0");
				end_text_z.text    = " "+(end_position_y*100).ToString("F0");
            	
			}
				
		}
        
        public bool showVertices
        {
	        set
	        {
				//Bestimmung der Größe von Start und Endpunkten
		        start.GetComponent<MeshRenderer>().enabled = true;
		        end.GetComponent<MeshRenderer>().enabled = true;
		        start.transform.localScale  = new Vector3(0.02f, 0.02f, 0.02f);
		        end.transform.localScale  = new Vector3(0.02f, 0.02f, 0.02f);
	        }
        }




    }
}