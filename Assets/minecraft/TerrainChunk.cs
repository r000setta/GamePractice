using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainChunk : MonoBehaviour
{
    public const int chunkWidth=16;
    public const int chunkHeight=64;

    public BlockType[,,] blocks=new BlockType[chunkWidth+2,chunkHeight,chunkWidth+2];

    void Start()
    {

    }

    public void BuildMesh()
    {
        Mesh mesh=new Mesh();
        List<Vector3> verts=new List<Vector3>();
        List<int> tris=new List<int>();

        for(int x=1;x<chunkWidth+1;++x){
            for(int z=1;z<chunkWidth+1;++z){
                for(int y=0;y<chunkHeight;++y){
                    if(blocks[x,y,z]!=BlockType.Air)
                    {
                        Vector3 blockPos=new Vector3(x-1,y,z-1);
                        int numFaces=0;
                        if(y<chunkHeight-1&&blocks[x,y+1,z]==BlockType.Air)
                        {
                            verts.Add(blockPos+new Vector3(0,1,0));
                            verts.Add(blockPos+new Vector3(0,1,1));
                            verts.Add(blockPos+new Vector3(1,1,1));
                            verts.Add(blockPos+new Vector3(1,1,0));
                            numFaces++;
                        }
                        if(y>0&&blocks[x,y-1,z]==BlockType.Air){
                            verts.Add(blockPos + new Vector3(0, 0, 0));
                            verts.Add(blockPos + new Vector3(1, 0, 0));
                            verts.Add(blockPos + new Vector3(1, 0, 1));
                            verts.Add(blockPos + new Vector3(0, 0, 1));
                            numFaces++;
                        }
                        if(blocks[x, y, z - 1] == BlockType.Air)
                        {
                            verts.Add(blockPos + new Vector3(0, 0, 0));
                            verts.Add(blockPos + new Vector3(0, 1, 0));
                            verts.Add(blockPos + new Vector3(1, 1, 0));
                            verts.Add(blockPos + new Vector3(1, 0, 0));
                            numFaces++;
                        }
                        if(blocks[x + 1, y, z] == BlockType.Air)
                        {
                            verts.Add(blockPos + new Vector3(1, 0, 0));
                            verts.Add(blockPos + new Vector3(1, 1, 0));
                            verts.Add(blockPos + new Vector3(1, 1, 1));
                            verts.Add(blockPos + new Vector3(1, 0, 1));
                            numFaces++;
                        }
                        if(blocks[x, y, z + 1] == BlockType.Air)
                        {
                            verts.Add(blockPos + new Vector3(1, 0, 1));
                            verts.Add(blockPos + new Vector3(1, 1, 1));
                            verts.Add(blockPos + new Vector3(0, 1, 1));
                            verts.Add(blockPos + new Vector3(0, 0, 1));
                            numFaces++;
                        }

                        //left
                        if(blocks[x - 1, y, z] == BlockType.Air)
                        {
                            verts.Add(blockPos + new Vector3(0, 0, 1));
                            verts.Add(blockPos + new Vector3(0, 1, 1));
                            verts.Add(blockPos + new Vector3(0, 1, 0));
                            verts.Add(blockPos + new Vector3(0, 0, 0));
                            numFaces++;
                        }
                        int tl=verts.Count-4*numFaces;
                        for(int i = 0; i < numFaces; i++)
                        {
                            tris.AddRange(new int[] { tl + i * 4, tl + i * 4 + 1, tl + i * 4 + 2, tl + i * 4, tl + i * 4 + 2, tl + i * 4 + 3 });
                        }
                    }
                }
                mesh.vertices=verts.ToArray();
                mesh.triangles=tris.ToArray();
                mesh.RecalculateNormals();
                GetComponent<MeshFilter>().mesh=mesh;
                GetComponent<MeshCollider>().sharedMesh=mesh;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
