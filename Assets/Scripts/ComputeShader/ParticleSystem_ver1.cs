using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using System.Xml.Schema;
using UnityEngine.Assertions;

/// <summary>
/// 寫到合成mesh的部分
/// </summary>
struct ParticleBuffer
{
    int id;
    bool active;
    Vector3 _position;
    Vector3 _velocity;
    Vector3 _rotation;
    Vector3 _angVelocity;
    float scale;
    float time;
    float lifeTime;

}
/// <summary>
/// compute shader with gpu instancing script
/// </summary>
[RequireComponent(typeof(MeshFilter),typeof(MeshRenderer))]
public class ParticleSystem_ver1 : MonoBehaviour {
    const int MAX_VERTEX_NUM = 65534;
    public ComputeShader computeshader;
    public Shader shader;
    public GameObject prefab;
    private List<GameObject> pool = new List<GameObject>();
    public int MaxNumber;

    [Header("Particle velocity")]public Vector3 Velocity;
    [Header("Particle angle velocity")]public Vector3 AngVelocity;
    [Header("Particle position range")]public Vector3 Range;

    int UpdateKernel;
    int EmitKernel;
    int InitKernel;
    private ComputeBuffer buffer;


    Mesh combinedMesh_;
    [Header("Particle Mesh")] public Mesh mesh;
    List<Material> materials = new List<Material>();
    int particleNum;
    int meshNum;

    Mesh CreateCombineMesh(Mesh mesh,int num)
    {
        Assert.IsTrue(mesh.vertexCount * num <= MAX_VERTEX_NUM);

        //取得單個particle形狀的資訊
        var meshIndices = mesh.GetIndices(0);
        var IndexNum = meshIndices.Length;

        //新增"建立mesh所需的屬性"
        var vertices = new List<Vector3>();
        var indices = new int[num * IndexNum];
        var normals = new List<Vector3>();
        var tangents = new List<Vector4>();
        var uv0 = new List<Vector2>();
        var uv1 = new List<Vector2>();

        //賦值給上面新增的屬性
        for (int id = 0; id < num;id++)
        {
            vertices.AddRange(mesh.vertices);
            normals.AddRange(mesh.normals);
            tangents.AddRange(mesh.tangents);
            uv0.AddRange(mesh.uv);

            for (int n = 0; n < IndexNum;n++)
            {
                indices[id * IndexNum + n] = id * mesh.vertexCount + meshIndices[n];
            }

            for (int n = 0; n < mesh.uv.Length;n++)
            {
                uv1.Add(new Vector3(id, id));//?????
            }
        }

        var CombinedMesh = new Mesh();
        CombinedMesh.SetVertices(vertices);
        CombinedMesh.SetNormals(normals);
        CombinedMesh.SetTangents(tangents);
        CombinedMesh.SetIndices(indices, MeshTopology.Triangles, 0);
        CombinedMesh.RecalculateNormals();
        CombinedMesh.SetUVs(0, uv0);
        CombinedMesh.SetUVs(1, uv1);
        CombinedMesh.RecalculateBounds();
        CombinedMesh.bounds.SetMinMax(Vector3.one * -100, Vector3.one * 100);

        return CombinedMesh;
    }
    private void OnEnable()
    {
        particleNum = MAX_VERTEX_NUM / mesh.vertexCount;
        meshNum = (int)Mathf.Ceil((float)MaxNumber / particleNum);

        for (int i = 0; i < meshNum;i++)
        {
            var material = new Material(shader);
            material.SetInt("_IdOffset", particleNum* i);
            materials.Add(material);
        }

        combinedMesh_ = CreateCombineMesh(mesh, particleNum);
        //新增一個適當大小的computebuffer
        buffer = new ComputeBuffer(MaxNumber,Marshal.SizeOf(typeof(ParticleBuffer)));

        //新增要傳遞的struct陣列
        ParticleBuffer[] particles = new ParticleBuffer[MaxNumber];
       
        InitKernel = computeshader.FindKernel("Init");
        EmitKernel = computeshader.FindKernel("Emit");
        UpdateKernel = computeshader.FindKernel("Update");

        //這裡有三個參數分別是
        //1.kernel index : 要設置的Kernel名稱
        //2.name : compute shader裡實體化結構的名字
        //(ex : RWStructure<Particle> _particle,"_particle"即是要填入的內容)
        //p.s輸入時必須是字串型別
        //3.buffer : 要傳送的buffer
        computeshader.SetBuffer(InitKernel, "_particle", buffer);//傳遞資料到compute shader  
        computeshader.SetVector("_Velocity", Velocity);
        computeshader.SetVector("_AngVelocity",AngVelocity * Mathf.Deg2Rad);
        computeshader.SetVector("_Range", Range);
        computeshader.Dispatch(InitKernel, MaxNumber / 8, 1, 1);
    
    }
    private void OnDisable()
    {
        buffer.Release();
    }

    // Update is called once per frame
    void Update () {
        computeshader.SetVector("_Velocity", Velocity);
        computeshader.SetVector("_AngVelocity", AngVelocity * Mathf.Deg2Rad);
        computeshader.SetVector("_Range", Range);

        computeshader.SetBuffer(EmitKernel, "_particle", buffer);
        computeshader.Dispatch(EmitKernel, MaxNumber/8,1,1);

        computeshader.SetFloat("_DeltaTime", Time.deltaTime);
        computeshader.SetBuffer(UpdateKernel, "_particle", buffer);
        computeshader.Dispatch(UpdateKernel, MaxNumber / 8, 1, 1);

        for (int i = 0; i < meshNum;i++)
        {
            var material = materials[i];
            material.SetInt("_idOffset", particleNum * i);
            material.SetBuffer("_particle", buffer);
            Graphics.DrawMesh(combinedMesh_, transform.position, transform.rotation, material, 0);
        }
	}
}
