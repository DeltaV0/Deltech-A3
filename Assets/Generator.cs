using UnityEngine;
using System.Collections.Generic;

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
    }

    public float GetNoiseAtPos(Vector2 p)
    {
        float n = 0;
        n = Mathf.Pow(PN(p.x, p.y) + PN(p.x * 2, p.y * 2) * 0.5f + PN(p.x * 4, p.y * 4) * 0.25f - 0.1f, 5)  - Mathf.Pow(PN(p.x, p.y) * 1.25f, 6) * 0.3f;
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
