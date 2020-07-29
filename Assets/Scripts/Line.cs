using System;
using UnityEngine;

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

		public float position_x;
		public float position_y;
		public float position_z;
		
		void Start()
		{
			coord = GameObject.FindWithTag("coord");
			
			//player=coord.transform.parent.gameObject;
			
			lineRenderer = GetComponent<LineRenderer>();
			boxCollider = quad.GetComponent<BoxCollider>();

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
            position_x = (end.position.x - coord.transform.position.x) * 100f;
            position_y = (end.position.y - coord.transform.position.y -0.07f) * 100f;
            position_z = (end.position.z - coord.transform.position.z) * 100f;
            
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

                lineRenderer.startWidth = width;
                lineRenderer.endWidth = width;
                lineRenderer.material = material;
                lineRenderer.startColor = material.color;
                lineRenderer.endColor = material.color;
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
			}
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