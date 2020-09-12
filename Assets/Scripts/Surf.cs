using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


[RequireComponent(typeof(MeshFilter))]
public class Surf : MonoBehaviour
{

    public Transform start;
    public Transform middle;
    public Transform end;


    public TextMeshProUGUI lognews;


    Mesh mesh;
    Vector3[] vertices;
    int[] triangles;

    public int index = 0;

    // Start is called before the first frame update
    void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;




        CreatShape ();
        UpdateMesh();

    }

    void CreatShape ()
    {
        vertices = new Vector3[]
        {
            new Vector3 (0,0,0),  //  0
            new Vector3 (10,0,0), //  1
            new Vector3 (10,0,10),//  2
            //new Vector3 (0,0,10), //  3
            //new Vector3 (-10,0,5), // 4

            //new Vector3 (-10,0,-5), // 5      
        };

        triangles = new int[]
        {
            0,2,1,
            0,1,2,
            0,2,3,
            0,3,2,
            //0,4,3,
            //0,3,4,
            //0,5,4,
            //0,4,5
            //0,4,3
        };

    }

    // Update is called once per frame
    void UpdateMesh()
    {
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;


        lognews.text =  ""+ (start.position).ToString();
    }
}
