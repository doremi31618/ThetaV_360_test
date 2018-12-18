using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshVertexCountTest : MonoBehaviour {

    private void OnEnable()
    {
        var mesh = GetComponent<MeshFilter>().mesh;
        var meshIndices = mesh.GetIndices(0);
        var meshUVlength = mesh.uv.Length;
        var meshUV = mesh.uv;
        for (int i = 0; i < meshUVlength;i++)
        {
            Debug.Log(meshUV[i]);
        }

    }
}
