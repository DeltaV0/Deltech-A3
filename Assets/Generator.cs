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
        public Dictionary<Vector2Int, Blocks.Block> buildings = new Dictionary<Vector2Int, Blocks.Block>();
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

        public Blocks.Block GetBlockAt(Vector2Int position, bool building = false, int size = 1)
        {
            Blocks.Block sel = null;
            if(!building){
            sel = FindMultiblock(position, 1, 1);
            if(sel == null)
            {
               sel = FindMultiblock(position, 2, 2); 
            }
            } else
            {
                sel = FindMultiblock(position, 1, 1);
                if(size == 1)
                {
                   if(sel == null)
                    {
                    sel = FindMultiblock(position, 2, 2); 
                    } 
                } else
                {
                    if(sel == null)
                    {
                        sel = FindMultiblock(position, 3, 2); 
                    }
                }
                
            }
            return sel;
        }

        public Blocks.Block FindMultiblock(Vector2Int position, int size, int lookingFor)
        {
            for(int x = size - 1; x >= -1 * size + 1; x--)
            {
                for(int y = size - 1; y >= -1 * size + 1; y--)
                    {
                        if(buildings.ContainsKey(position + new Vector2Int(x, y)) && buildings[position + new Vector2Int(x, y)].size.x == lookingFor)
                    {
                        Debug.Log("yea" + size);
                        return buildings[position + new Vector2Int(x, y)];
                    }
                    }
            }
            return null;
        }

        public void Build(Vector2Int position, BlockInfo i)
        {
            
            if(GetBlockAt(position, true, i.size) == null){
                Blocks.Block newBlock = new Blocks.Block(position - pos, pos, i.ID, new Vector2Int(i.size, i.size));
                Debug.Log(i.size);
                            buildings.Add(position, newBlock);
                            
                            }
            
            
        }

        public void Break(Vector2Int position){
            if(buildings.ContainsKey(position)){
                buildings[position].destroy();
            }
        }

        public void RunBuildingLogic()
        {   
            //spaghetti!!! fix this!!!
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
                            for(int q = 0; q < block.ItemsOut.Count; q++)
                            {
                                block.txt.text += block.ItemsOut.ElementAt(q).Key + ", " + block.ItemsOut.ElementAt(q).Value + "\n";
                            }
                           
                        }
                        
                        if(block.ItemsIn.Count > 0){
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
                    } else if(block.Type == 2)
                    {
                        block.txt.text = "";
                        if (block.ItemsOut.Count > 0)
                        {
                           for(int q = 0; q< block.ItemsOut.Count; q++)
                            {
                                block.txt.text += block.ItemsOut.ElementAt(q).Key + ", " + block.ItemsOut.ElementAt(q).Value + "\n";
                            }
                        }
                        
                        if(block.ItemsIn.Count > 0){
                            
                            block.ChangeItemCount(false, block.ItemsIn.ElementAt(0).Key, block.ItemsIn.ElementAt(0).Value);
                            block.ItemsIn.Remove(block.ItemsIn.ElementAt(0).Key);
                        }
                        block.TrySendResources();
                    } else if(block.Type == 3)
                    {
                        block.txt.text = "";
                        if (block.ItemsOut.Count > 0)
                        {
                           for(int q = 0; q < block.ItemsOut.Count; q++)
                            {
                                block.txt.text += block.ItemsOut.ElementAt(q).Key + ", " + block.ItemsOut.ElementAt(q).Value + "\n";
                            }
                        }

                        block.txt.text += "-------\n";
                        
                        if(block.ItemsIn.Count > 0){
                            for(int q = 0; q < block.ItemsIn.Count; q++)
                            {
                                block.txt.text += block.ItemsIn.ElementAt(q).Key + ", " + block.ItemsIn.ElementAt(q).Value + "\n";
                            }
                        }
                        block.Craft();
                        if (block.busy)
                        {
                            block.txt.text += "Crafting!!!";
                        }
                        block.TrySendResources();
                    }
            }
            }
        }
    }

    Dictionary<Vector2Int, Chunk> chunks = new Dictionary<Vector2Int, Chunk>();
    public static Generator me;

    [System.Serializable]
    public class BlockInfo
    {
        public int ID;
        public GameObject prefab;

        public int size;
    }
    public BlockInfo[] info;
    public Dictionary<int, BlockInfo> Info = new Dictionary<int, BlockInfo>();
    

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
        for(int i = 0; i < info.Length; i++)
        {
            Info.Add(info[i].ID, info[i]);
        }
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
        GetChunkAt(new Vector2Int(0, 0)).buildings.Add(new Vector2Int(0, 0), new Blocks.Block(new Vector2Int(0, 0) - GetChunkAt(new Vector2Int(0, 0)).pos, GetChunkAt(new Vector2Int(0, 0)).pos, 2, new Vector2Int(1, 1)));
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
        for(int i = 1; i < 10; i++)
        {
            n += PN(p.x * i * 0.5f, p.y * i * 0.5f) / (i * 0.5f);
        }
        n *= 0.2f;
        n = Mathf.Pow(n, 8);
        n += PN(p.x * 0.1f, p.y * 0.1f) * 0.25f;
        n += PN(p.x * 0.2f, p.y * 0.2f) * 0.1f;

        n = Mathf.Round(n * 32) / 32;
        
       return n; 
    }

    public int GetResourceNoiseAtPos(Vector2 p, int i)
    {
        //float n = 0;
        if(i == 1)
        {
            return (Mathf.Pow(PN(p.x * 1 * 5f, p.y * 1 * 5f), 4) + Random.Range(-0.2f, 0.2f)) > 0.5f ? 1 : 0;
        }
        if(i == 2)
        {
            return (Mathf.Pow(PN(p.x * 0.5f * 5f, p.y * 2f * 5f), 4) + Random.Range(-0.2f, 0.2f)) > 0.7f ? 1 : 0;
        } 
        if(i == 3)
        {
            return (Mathf.Pow(PN(p.x * 2f * 5f, p.y * 0.25f * 3f), 3) + Random.Range(-0.3f, 0.3f)) > 0.8f ? 1 : 0;
        }
        return 0;
    }

    private float PN(float x, float y)
    {
        return Mathf.PerlinNoise(x * 0.05f + NoiseOffset.x, y * 0.05f + NoiseOffset.y);
    }
}
