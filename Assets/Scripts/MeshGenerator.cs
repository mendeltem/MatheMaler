using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


[RequireComponent(typeof(MeshFilter))]
public class MeshGenerator : MonoBehaviour
{
    public Material material;

    public Transform point1;
    public Transform point2;
    public Transform point3;
    public Transform point4;
    public Transform point5;
    public Transform point6;
    public Transform point7;
    public Transform point8;
    public Transform point9;
    public Transform point10;
    

    public TextMeshProUGUI lognews;

    public int index = 0;

    Mesh mesh;
    Vector3[] vertices;
    int[] triangles;

	public BoxCollider boxCollider;
    public MeshRenderer meshRenderer;
    public GameObject quad;

    void Start()
    {
        showVertices = true;

        
    }

    void Update()
    {
        Mesh mesh = GetComponent<MeshFilter>().mesh;

        meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.material = material;


        vertices = new Vector3[]
        {
                point1.position,  
                point2.position,  
                point3.position,
                point4.position,   
                point5.position,  
                point6.position,  
                point7.position,
                point8.position,
                point9.position,
                point10.position,
        };

        if (index == 3){

            triangles = new int[]
            {
                0,2,1,
                0,1,2
            };
        }

        if (index == 4){

            triangles = new int[]
            {
                0,2,1,
                0,1,2,
                0,3,2,
                0,2,3
            };
        }

        if (index == 5){

            triangles = new int[]
            {
                0,2,1,
                0,1,2,
                0,3,2,
                0,2,3,
                0,4,3,
                0,3,4
            };
        }

        if (index == 6){

            triangles = new int[]
            {
                0,2,1,
                0,1,2,
                0,3,2,
                0,2,3,
                0,4,3,
                0,3,4,                
                0,5,4,
                0,4,5
            };
        }

        if (index == 7){

            triangles = new int[]
            {
                0,2,1,
                0,1,2,
                0,3,2,
                0,2,3,
                0,4,3,
                0,3,4,               
                0,5,4,
                0,4,5,
                0,6,5,
                0,5,6
            };
        }

        if (index == 8){

            triangles = new int[]
            {
                0,2,1,
                0,1,2,
                0,3,2,
                0,2,3,
                0,4,3,
                0,3,4,               
                0,5,4,
                0,4,5,
                0,6,5,
                0,5,6,
                0,7,6,
                0,6,7
            };
        }

        if (index == 9){

            triangles = new int[]
            {
                0,2,1,
                0,1,2,
                0,3,2,
                0,2,3,
                0,4,3,
                0,3,4,               
                0,5,4,
                0,4,5,
                0,6,5,
                0,5,6,
                0,7,6,
                0,6,7,
                0,8,7,
                0,7,8
            };
        }

        if (index == 10){

            triangles = new int[]
            {
                0,2,1,
                0,1,2,
                0,3,2,
                0,2,3,
                0,4,3,
                0,3,4,               
                0,5,4,
                0,4,5,
                0,6,5,
                0,5,6,
                0,7,6,
                0,6,7,
                0,8,7,
                0,7,8,
                0,9,8,
                0,8,9
            };
        }
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;

    }
    public bool showVertices
    {
        set
        {
            //Bestimmung der Größe von Start und Endpunkten
            point1.GetComponent<MeshRenderer>().enabled = true;
            point2.GetComponent<MeshRenderer>().enabled = true;
            point3.GetComponent<MeshRenderer>().enabled = true;
            point4.GetComponent<MeshRenderer>().enabled = true;
            point5.GetComponent<MeshRenderer>().enabled = true;
            point6.GetComponent<MeshRenderer>().enabled = true;
            point7.GetComponent<MeshRenderer>().enabled = true;
            point8.GetComponent<MeshRenderer>().enabled = true;
            point9.GetComponent<MeshRenderer>().enabled = true;
            point10.GetComponent<MeshRenderer>().enabled = true;


            point1.transform.localScale  = new Vector3(0.01f, 0.01f, 0.01f);
            point2.transform.localScale  = new Vector3(0.01f, 0.01f, 0.01f);
            point3.transform.localScale  = new Vector3(0.01f, 0.01f, 0.01f);
            point4.transform.localScale  = new Vector3(0.01f, 0.01f, 0.01f);
            point5.transform.localScale  = new Vector3(0.01f, 0.01f, 0.01f);
            point6.transform.localScale  = new Vector3(0.01f, 0.01f, 0.01f);
            point7.transform.localScale  = new Vector3(0.01f, 0.01f, 0.01f);
            point8.transform.localScale  = new Vector3(0.01f, 0.01f, 0.01f);
            point9.transform.localScale  = new Vector3(0.01f, 0.01f, 0.01f);
            point10.transform.localScale  = new Vector3(0.01f, 0.01f, 0.01f);
        }
    }

/*
    void CreatShape ()
    {
        vertices = new Vector3[]
        {
            start.position,  //  0
            middle.position, //  1
            end.position,//  2
            //new Vector3 (0,0,10), //  3
            //new Vector3 (-10,0,5), // 4

            //new Vector3 (-10,0,-5), // 5      
        };

        triangles = new int[]
        {
            0,2,1,
            0,1,2,
            //0,2,3,
            //0,3,2,
            //0,4,3,
            //0,3,4,
            //0,5,4,
            //0,4,5
            //0,4,3
        };

    }

    void update()
    {

        CreatShape ();
        UpdateMesh();

    }



    // Update is called once per frame
    void UpdateMesh()
    {


        //lognews.text =  ""+ (start.position).ToString();
    }
    */
}
