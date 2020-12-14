using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineRender : MonoBehaviour
{
    private Transform targetTR;

    [SerializeField]
    private GameObject lineBase;
    private Queue<GameObject> linePool;

    private void Start()
    {
        Vector3[] vector = new Vector3[] { new Vector3(-1, 1, 0), new Vector3(1, 0, 0), new Vector3(1, 0, 0), new Vector3(-1, -1, 0) };

        int[] triangles = new int[] { 0, 1, 2, 0, 2, 3 };
        Mesh mesh = new Mesh();

        mesh.vertices = vector;
        mesh.triangles = triangles;
        GetComponent<MeshFilter>().mesh = mesh;
    }

    private void CreateLine()
    {
        Instantiate(lineBase);
    }
}

public class Line
{

}
