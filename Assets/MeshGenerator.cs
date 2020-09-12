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

    public TextMeshProUGUI lognews;

    public int index = 0;

    Mesh mesh;
    Vector3[] vertices;
    int[] triangles;

    public MeshRenderer meshRenderer;

    void Update()
    {
        Mesh mesh = GetComponent<MeshFilter>().mesh;

        meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.material = material;



        if (index == 3){

            vertices = new Vector3[]
            {
                point1.position,  //  0
                point2.position,  //  1
                point3.position   //  2  
            };
            triangles = new int[]
            {
                0,2,1,
                0,1,2
            };

            mesh.Clear();
            mesh.vertices = vertices;
            mesh.triangles = triangles;


        }

        if (index == 4){

            vertices = new Vector3[]
            {
                point1.position,  
                point2.position,  
                point3.position,
                point4.position    
            };
            triangles = new int[]
            {
                0,2,1,
                0,1,2,
                0,3,2,
                0,2,3
            };


            mesh.Clear();
            mesh.vertices = vertices;
            mesh.triangles = triangles;

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
