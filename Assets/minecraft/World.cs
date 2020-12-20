using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour {
    public int seed;
    public BiomeAttributes biome;
    public Transform player;
    public Vector3 spawnPosition;
    public Material material;
    public BlockType[] blocktypes;

    Chunk[,] chunks = new Chunk[VoxelData.WorldSizeInChunks, VoxelData.WorldSizeInChunks];
    List<ChunkCoord> activeChunks = new List<ChunkCoord>();
    ChunkCoord playerLastChunkCoord;
    ChunkCoord playerChunkCoord;

    private void Start() {
        Random.InitState(seed);
        spawnPosition = new Vector3((VoxelData.WorldSizeInChunks * VoxelData.ChunkWidth) / 2f, VoxelData.ChunkHeight + 2f, (VoxelData.WorldSizeInChunks * VoxelData.ChunkWidth) / 2f);
        GenerateWorld();
        playerLastChunkCoord = GetChunkCoordFromVector3(player.position);
    }

    private void Update() {
        playerChunkCoord=GetChunkCoordFromVector3(player.position);
        // if (!playerChunkCoord.Equals(playerLastChunkCoord))
        //     CheckViewDistance();
    }

    ChunkCoord GetChunkCoordFromVector3 (Vector3 pos) {

        int x = Mathf.FloorToInt(pos.x / VoxelData.ChunkWidth);
        int z = Mathf.FloorToInt(pos.z / VoxelData.ChunkWidth);
        return new ChunkCoord(x, z);

    }

    private void GenerateWorld () {

        for (int x = VoxelData.WorldSizeInChunks / 2 - VoxelData.ViewDistanceInChunks; x < VoxelData.WorldSizeInChunks / 2 + VoxelData.ViewDistanceInChunks; x++) {
            for (int z = VoxelData.WorldSizeInChunks / 2 - VoxelData.ViewDistanceInChunks; z < VoxelData.WorldSizeInChunks / 2 + VoxelData.ViewDistanceInChunks; z++) {
                CreateChunk(new ChunkCoord(x, z));
            }
        }
        // player.position=spawnPosition;
    }

    private void CheckViewDistance () {

        ChunkCoord coord=GetChunkCoordFromVector3(player.position);

        List<ChunkCoord> previouslyActiveChunks = new List<ChunkCoord>(activeChunks);

        for (int x = coord.x - VoxelData.ViewDistanceInChunks / 2; x < coord.x + VoxelData.ViewDistanceInChunks / 2; x++) {
            for (int z = coord.z - VoxelData.ViewDistanceInChunks / 2; z < coord.z + VoxelData.ViewDistanceInChunks / 2; z++) {
                if (IsChunkInWorld(x, z)) {
                    ChunkCoord thisChunk = new ChunkCoord(x, z);
                    if (chunks[x, z] == null)
                        CreateChunk(thisChunk);
                    else if (!chunks[x, z].isActive) {
                        chunks[x, z].isActive = true;
                        activeChunks.Add(thisChunk);
                    }
                    for (int i = 0; i < previouslyActiveChunks.Count; i++) {
                        if (previouslyActiveChunks[i].x == x && previouslyActiveChunks[i].z == z)
                            previouslyActiveChunks.RemoveAt(i);
                    }
                }
            }
        }
        foreach (ChunkCoord c in previouslyActiveChunks)
            chunks[c.x, c.z].isActive = false;
    }

    public bool CheckForVoxel(float _x,float _y,float _z)
    {
        int xCheck=Mathf.FloorToInt(_x);
        int yCheck=Mathf.FloorToInt(_y);
        int zCheck=Mathf.FloorToInt(_z);

        int xChunk=xCheck/VoxelData.ChunkWidth;
        int zChunk=zCheck/VoxelData.ChunkWidth;

        xCheck-=(xChunk*VoxelData.ChunkWidth);
        zCheck-=(zChunk*VoxelData.ChunkWidth);
        
        return blocktypes[chunks[xChunk,zChunk].voxelMap[xCheck,yCheck,zCheck]].isSolid;
    }

    bool IsChunkInWorld(int x, int z) {
        if (x > 0 && x < VoxelData.WorldSizeInChunks - 1 && z > 0 && z < VoxelData.WorldSizeInChunks - 1)
            return true;
        else
            return false;
    }

    private void CreateChunk (ChunkCoord coord) {
        chunks[coord.x, coord.z] = new Chunk(new ChunkCoord(coord.x, coord.z), this);
        activeChunks.Add(new ChunkCoord(coord.x, coord.z));
    }

    bool IsVoxelInWorld (Vector3 pos) {

        if (pos.x >= 0 && pos.x < VoxelData.WorldSizeInBlocks && pos.y >= 0 && pos.y < VoxelData.ChunkHeight && pos.z >= 0 && pos.z < VoxelData.WorldSizeInBlocks)
            return true;
        else
            return false;

    }

    public byte GetVoxel (Vector3 pos) {
        int yPos=Mathf.FloorToInt(pos.y);

        if (!IsVoxelInWorld(pos))
            return 0;

        if(yPos==0){
            return 1;
        }

        int terrainHeight=Mathf.FloorToInt(biome.terrainHeight*Noise.Get2DPerlin(new Vector2(pos.x,pos.z),0,biome.terrainScale))+biome.solidGroundHeight;
        byte voxelValue=0;

        if(yPos==terrainHeight){
            voxelValue=3;
        }else if(yPos>terrainHeight){
            return 0;
        }else if(yPos<terrainHeight&&yPos>terrainHeight-4){
            voxelValue=5;
        }
        else{
            voxelValue=2;
        }

        if(voxelValue==2){
            foreach(Lode lode in biome.lodes){
                if(yPos>lode.minHeight&&yPos<lode.maxHeight){
                    if(Noise.Get3DPerlin(pos,lode.noiseOffset,lode.scale,lode.threshold)){
                        voxelValue=lode.blockID;
                    }
                }
            }
        }else{
            return voxelValue;
        }
        return voxelValue;
    }
}

public class ChunkCoord {
    public int x;
    public int z;
    public ChunkCoord (int _x, int _z) {
        x = _x;
        z = _z;
    }
    public bool Equals(ChunkCoord other) {
        if (other == null)
            return false;
        else if (other.x == x && other.z == z)
            return true;
        else
            return false;
    }
}

[System.Serializable]
public class BlockType {
    public string blockName;
    public bool isSolid;

    [Header("Texture Values")]
    public int backFaceTexture;
    public int frontFaceTexture;
    public int topFaceTexture;
    public int bottomFaceTexture;
    public int leftFaceTexture;
    public int rightFaceTexture;

    // Back, Front, Top, Bottom, Left, Right

    public int GetTextureID (int faceIndex) {
        switch (faceIndex) {
            case 0:
                return backFaceTexture;
            case 1:
                return frontFaceTexture;
            case 2:
                return topFaceTexture;
            case 3:
                return bottomFaceTexture;
            case 4:
                return leftFaceTexture;
            case 5:
                return rightFaceTexture;
            default:
                Debug.Log("Error in GetTextureID; invalid face index");
                return 0;
        }
    }
}