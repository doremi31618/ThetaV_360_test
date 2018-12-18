using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter),typeof(MeshRenderer))]
public class Grid : MonoBehaviour {


    public int xSize = 1;
    public int ySize = 1;
    private Vector3[] vertices;
    private Vector2[] uv;
    private Mesh mesh;

    private void Awake()
    {
        StartCoroutine(Generator());
    }
    private IEnumerator Generator()
    {
        GetComponent<MeshFilter>().mesh = mesh = new Mesh();
        mesh.name = "Procedural Grid";
        vertices = new Vector3[(xSize + 1) * (ySize + 1)];
        Vector4[] tangents = new Vector4[vertices.Length];
        uv = new Vector2[vertices.Length];
        for (int i = 0, y = 0; y <= ySize; y++)
        {
            for (int x = 0; x <= xSize; x++, i++)
            {
                vertices[i] = new Vector3(x, y);
                uv[i] = new Vector2((float)x / xSize, (float)y / ySize);
            }
        }
        mesh.vertices = vertices;
        int[] triangles = new int[6 * xSize * ySize];
        //i = index of triangles array
        //x = base point of x 
        for (int i = 0,y = 0,vi = 0; y < ySize;y++, vi++){
            for (int x = 0; x < xSize; i += 6, x++,vi++)
            {
                //first triangle
                triangles[i]     = vi;
                triangles[i + 1] = triangles[i + 4] = vi + xSize + 1;
                triangles[i + 2] = triangles[i + 3] = vi + 1;
                //second triangle
                triangles[i + 5] = vi +(xSize + 1) + 1;

                mesh.triangles = triangles;
                mesh.RecalculateNormals();
               
                yield return new WaitForSeconds(0.1f);
            }
        }

        mesh.uv = uv;
        //mesh.tangents = tangents;
        mesh.RecalculateTangents();
    }
    private void OnDrawGizmos()
    {
        if(vertices == null){
            return;
        }
        Gizmos.color = Color.black;
        for (int i = 0; i < vertices.Length;i++){
            Gizmos.DrawSphere(vertices[i],0.1f);
        }
    }
}
