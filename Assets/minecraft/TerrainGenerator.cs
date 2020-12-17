using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGenerator : MonoBehaviour
{
    public GameObject terrainChunk;
    public Transform player;

    FastNoise noise=new FastNoise();

    public static Dictionary<ChunkPos,TerrainChunk> chunks=new Dictionary<ChunkPos, TerrainChunk>();

    void Start()
    {
        LoadChunks();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void BuildChunk(int xPos,int zPos)
    {
        GameObject chunkGO=Instantiate(terrainChunk,new Vector3(xPos,0,zPos),Quaternion.identity);
        TerrainChunk chunk=chunkGO.GetComponent<TerrainChunk>();
        for(int x=0;x<TerrainChunk.chunkWidth+2;++x){
            for(int z=0;z<TerrainChunk.chunkWidth+2;++z){
                for(int y=0;y<TerrainChunk.chunkHeight;++y){
                    // chunk.blocks[x,y,z]=GetBlockType(xPos+x-1,y,zPos+z-1);
                    if(Mathf.PerlinNoise((xPos + x-1) * .1f, (zPos + z-1) * .1f) * 10 + y < TerrainChunk.chunkHeight * .5f){
                        chunk.blocks[x,y,z]=BlockType.Dirt;
                    }else{
                        chunk.blocks[x,y,z]=BlockType.Air;
                    }
                }
            }
        }
        chunk.BuildMesh();

        chunks.Add(new ChunkPos(xPos,zPos),chunk);
    }

    ChunkPos curChunk=new ChunkPos(-1,-1);

    BlockType GetBlockType(int x,int y,int z)
    {
        return BlockType.Dirt;
    }

    void LoadChunks(bool instant=false)
    {
        int curChunkPosX=Mathf.FloorToInt(player.position.x/16)*16;
        int curChunkPosZ=Mathf.FloorToInt(player.position.z/16)*16;
        
        if(curChun)
    }
}


public struct ChunkPos
{
    public int x,z;
    public ChunkPos(int x,int z)
    {
        this.x=x;
        this.z=z;
    }
}