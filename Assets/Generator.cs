using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class Generator : MonoBehaviour
{
    public Vector2Int NoiseOffset;
    [System.Serializable]
    public class Chunk
    {
        public Vector2Int pos;
        Dictionary<Vector2Int, Blocks.Terrain> terrain = new Dictionary<Vector2Int, Blocks.Terrain>();
        Dictionary<Vector2Int, Blocks.Block> buildings = new Dictionary<Vector2Int, Blocks.Block>();
        public Chunk(Vector2Int position)
        {
            terrain.Clear();
            //Debug.Log(position);
            for(int i = 0; i < 16; i++)
            {
                for(int q = 0; q < 16; q++)
                {
                    terrain.Add(new Vector2Int(i, q), new Blocks.Terrain(new Vector2Int(i, q), position));
                }
            }
            pos = position;
        }

        public Blocks.Block GetBlockAt(Vector2Int position)
        {
            if(buildings.ContainsKey(position)){
                return buildings[position];
            } else
            {
                return null;
            }
        }

        public void Build(Vector2Int position, int i)
        {
            if(!buildings.ContainsKey(position)){
                buildings.Add(position, new Blocks.Block(position - pos, pos, i));
            }
        }

        public void RunBuildingLogic()
        {   
            if(buildings.Count > 0){
            for(int i = 0; i < buildings.Count; i++)
            {
                Blocks.Block block = buildings.ElementAt(i).Value;
                if(block.Type == 0)
                    {
                        if(!block.busy){
                            block.MiningBlockLogic(terrain[block.relPos]);
                        }
                        
                        block.txt.text = "";
                        if (block.ItemsOut.Count > 0)
                        {
                           block.txt.text += terrain[block.relPos].Resource + ", " + block.ItemsOut[terrain[block.relPos].Resource] ;
                        }
                        
                        if(block.ItemsIn.Count > 0){
                            block.txt.text += "\n" + block.ItemsIn.ElementAt(0).Key + ", " + block.ItemsIn.ElementAt(0).Value;
                            block.ChangeItemCount(false, block.ItemsIn.ElementAt(0).Key, block.ItemsIn.ElementAt(0).Value);
                            block.ItemsIn.Remove(block.ItemsIn.ElementAt(0).Key);
                        }
                        block.TrySendResources();
                    } else if(block.Type == 1)
                    {
                        if(block.ItemsIn.Count > 0){
                            //block.txt.text += "\n" + block.ItemsIn.ElementAt(0).Key + ", " + block.ItemsIn.ElementAt(0).Value;
                            block.ChangeItemCount(false, block.ItemsIn.ElementAt(0).Key, block.ItemsIn.ElementAt(0).Value);
                            block.ItemsIn.Remove(block.ItemsIn.ElementAt(0).Key);
                        }
                        block.TrySendResources();
                    }
            }
            }
        }
    }

    Dictionary<Vector2Int, Chunk> chunks = new Dictionary<Vector2Int, Chunk>();
    public static Generator me;
    public GameObject[] objPf;

    public Vector2Int cPos;

    public void GenerateChunk(Vector2Int Position)
    {

        if(GetChunkAt(Position) == null)
        {
            Vector2Int RealPos = Vector2Int.zero;

        Vector2 p = new Vector2(Position.x, Position.y);

        RealPos.x = Mathf.FloorToInt(p.x / 16f) * 16;
        RealPos.y = Mathf.FloorToInt(p.y / 16f) * 16;

            Chunk newChunk = new Chunk(RealPos);
            chunks.Add(RealPos, newChunk);
        }
    }

    public Blocks.Block GetBlockAt(Vector2Int Position)
    {
        return GetChunkAt(Position).GetBlockAt(Position);
    }

    public Chunk GetChunkAt(Vector2Int Position)
    {
        Vector2Int RealPos = Vector2Int.zero;

        Vector2 p = new Vector2(Position.x, Position.y);

        RealPos.x = Mathf.FloorToInt(p.x / 16f) * 16;
        RealPos.y = Mathf.FloorToInt(p.y / 16f) * 16;
        if(chunks.ContainsKey(RealPos)){
            return chunks[RealPos];
        } else
        {
            return null;
        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        me = this;
        NoiseOffset = Vector2Int.FloorToInt(Random.insideUnitCircle * (int.MaxValue / 1024));
        Debug.Log(NoiseOffset);
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        cPos = new Vector2Int((int)CamMover.me.transform.position.x, (int)CamMover.me.transform.position.y);
        

        for(int cx = -2; cx < 3; cx++)
        {
            for(int cy = -2; cy < 3; cy++)
            {
                if(GetChunkAt(new Vector2Int(cPos.x + (16 * cx), cPos.y + (16 * cy))) == null)
                {
                    GenerateChunk(new Vector2Int(cPos.x + (16 * cx), cPos.y + (16 * cy)));
                }
            }
        }

        for(int i = 0; i < chunks.Count; i++)
            {
                chunks.ElementAt(i).Value.RunBuildingLogic();
                
            }
    }

    public float GetNoiseAtPos(Vector2 p)
    {
        float n = 0;
        n = Mathf.Pow(PN(p.x, p.y) + PN(p.x * 2, p.y * 2) * 0.5f + PN(p.x * 4, p.y * 4) * 0.25f - 0.1f, 5)  - Mathf.Pow(PN(p.x, p.y) * 1.25f, 6) * 0.3f + PN(p.x, p.y);
        n *= 0.5f;
       return n; 
    }

    public float GetResourceNoiseAtPos(Vector2 p)
    {
        float n = 0;
        n = Mathf.Pow(PN(p.x, p.y) + PN(p.x * 0.5f, p.y * 0.5f) * 0.5f + PN(p.x * 0.25f, p.y * 0.25f) * 0.25f - 0.1f, 4);
       return n; 
    }

    private float PN(float x, float y)
    {
        return Mathf.PerlinNoise(x * 0.05f + NoiseOffset.x, y * 0.05f + NoiseOffset.y);
    }
}
