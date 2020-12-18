using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk : MonoBehaviour
{
    public MeshRenderer meshRenderer;
    public MeshFilter meshFilter;

    int vertexIndex=0;
    List<Vector3> vertices=new List<Vector3>();
    List<int> triangles=new List<int>();
    List<Vector2> uvs=new List<Vector2>();

    bool[,,] voxelMap = new bool[VoxelData.chunkWidth, VoxelData.chunkHeight, VoxelData.chunkWidth];

    void Start()
    {
        PopulateVoxelMap();
        CreateMeshData();
        CreateMesh();
    }

    void PopulateVoxelMap()
    {
        for (int y = 0; y < VoxelData.chunkHeight; ++y)
        {
            for (int x = 0; x < VoxelData.chunkWidth; ++x)
            {
                for (int z = 0; z < VoxelData.chunkWidth; ++z)
                {
                    voxelMap[x, y, z] = true;
                }
            }
        }
    }

    void CreateMeshData()
    {
        for (int y = 0; y < VoxelData.chunkHeight; ++y)
        {
            for (int x = 0; x < VoxelData.chunkWidth; ++x)
            {
                for (int z = 0; z < VoxelData.chunkWidth; ++z)
                {
                    AddVoxelDataToChunk(new Vector3(x, y, z));
                }
            }
        }
    }

    void AddVoxelDataToChunk(Vector3 pos){
        for(int p=0;p<6;++p){
            if (!checkVoxel(pos + VoxelData.faceChecks[p]))
            {
                vertices.Add(pos + VoxelData.voxelVerts[VoxelData.voxelTris[p, 0]]);
                vertices.Add(pos + VoxelData.voxelVerts[VoxelData.voxelTris[p, 1]]);
                vertices.Add(pos + VoxelData.voxelVerts[VoxelData.voxelTris[p, 2]]);
                vertices.Add(pos + VoxelData.voxelVerts[VoxelData.voxelTris[p, 3]]);

                uvs.Add(VoxelData.voxelUvs[0]);
                uvs.Add(VoxelData.voxelUvs[1]);
                uvs.Add(VoxelData.voxelUvs[2]);
                uvs.Add(VoxelData.voxelUvs[3]);

                triangles.Add(vertexIndex);
                triangles.Add(vertexIndex+1);  
                triangles.Add(vertexIndex+2);
                triangles.Add(vertexIndex+2);
                triangles.Add(vertexIndex+1);
                triangles.Add(vertexIndex+3);

                vertexIndex+=4;
            }
        }
    }

    bool checkVoxel(Vector3 pos)
    {
        int x = Mathf.FloorToInt(pos.x);
        int y = Mathf.FloorToInt(pos.y);
        int z = Mathf.FloorToInt(pos.z);

        if (x < 0 || x > VoxelData.chunkWidth - 1 || y < 0 || y > VoxelData.chunkHeight - 1 || z < 0 || z > VoxelData.chunkWidth - 1)
            return false;

        return voxelMap[x, y, z];
    }

    void CreateMesh()
    {
        Mesh mesh=new Mesh();
        mesh.vertices=vertices.ToArray();
        mesh.triangles=triangles.ToArray();
        mesh.uv=uvs.ToArray();
        mesh.RecalculateNormals();

        meshFilter.mesh=mesh;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
