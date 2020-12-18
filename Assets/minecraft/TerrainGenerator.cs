using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGenerator : MonoBehaviour
{
    public GameObject terrainChunk;
    public Transform player;
    int chunkDist=5;

    FastNoise noise=new FastNoise();

    public static Dictionary<ChunkPos,TerrainChunk> chunks=new Dictionary<ChunkPos, TerrainChunk>();

    List<ChunkPos> toGenerate=new List<ChunkPos>();

    void Start()
    {
        LoadChunks(true);
    }

    // Update is called once per frame
    void Update()
    {
        LoadChunks();
    }

    void BuildChunk(int xPos,int zPos)
    {
        GameObject chunkGO=Instantiate(terrainChunk,new Vector3(xPos,0,zPos),Quaternion.identity);
        TerrainChunk chunk=chunkGO.GetComponent<TerrainChunk>();
        for(int x=0;x<TerrainChunk.chunkWidth+2;++x){
            for(int z=0;z<TerrainChunk.chunkWidth+2;++z){
                for(int y=0;y<TerrainChunk.chunkHeight;++y){
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
        
        if(curChunk.x!=curChunkPosX||curChunk.z!=curChunkPosZ)
        {
            curChunk.x=curChunkPosX;
            curChunk.z=curChunkPosZ;

            for(int i=curChunkPosX-16*chunkDist;i<=curChunkPosX+16*chunkDist;i+=16){
                for(int j=curChunkPosZ-16*chunkDist;j<=curChunkPosZ+16*chunkDist;j+=16){
                    ChunkPos cp=new ChunkPos(i,j);
                    if(!chunks.ContainsKey(cp)&&!toGenerate.Contains(cp))
                    {
                        if(instant){
                            BuildChunk(i,j);
                        }else{
                            toGenerate.Add(cp);
                        }
                    }
                }
            }
            List<ChunkPos> toDestroy=new List<ChunkPos>();
            foreach(KeyValuePair<ChunkPos,TerrainChunk> c in chunks)
            {
                ChunkPos cp=c.Key;
                if(Mathf.Abs(curChunkPosX-cp.x)>16*(chunkDist+3)||
                    Mathf.Abs(curChunkPosZ-cp.z)>16*(chunkDist+3))
                    {
                        toDestroy.Add(c.Key);
                    }
            }
            foreach(ChunkPos cp in toGenerate)
            {
                if(Mathf.Abs(curChunkPosX - cp.x) > 16 * (chunkDist + 1) ||
                    Mathf.Abs(curChunkPosZ - cp.z) > 16 * (chunkDist + 1))
                    toGenerate.Remove(cp);
            }
            foreach(ChunkPos cp in toDestroy)
            {
                chunks[cp].gameObject.SetActive(false);
                chunks.Remove(cp);
            }

            StartCoroutine(DelayBuildChunks());
        }
    }

    IEnumerator DelayBuildChunks()
    {
        while(toGenerate.Count>0)
        {
            BuildChunk(toGenerate[0].x,toGenerate[0].z);
            toGenerate.RemoveAt(0);
            yield return new WaitForSeconds(.2f);
        }
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