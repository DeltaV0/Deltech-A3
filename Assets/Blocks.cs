using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class Blocks : MonoBehaviour
{
    public GameObject Transmission;

    public static Blocks me;

    void Start()
    {
        me = this;
    }
    
    [System.Serializable]
    public class Terrain
    
    {
        public float noise;
        public float[] noiseR;
        public SpriteRenderer spr;
        public GameObject gobj;
        public Vector2Int absPos;
        public Vector2Int relPos;

        public int Resource;
        public Terrain(Vector2Int pos, Vector2Int cpos)
        {
            relPos = pos;
            absPos = pos + cpos;
            Vector3 APos = new Vector3((float)(absPos.x), (float)(absPos.y), 0);

            noise = Generator.me.GetNoiseAtPos((Vector2)APos);
            noiseR = new float[2];
            noiseR[0] = Generator.me.GetResourceNoiseAtPos((Vector2)APos * 1.25f) - 0.1f;
            noiseR[1] = Generator.me.GetResourceNoiseAtPos((Vector2)APos * 2f);

            GameObject tile = Instantiate(Generator.me.objPf[0], APos, Quaternion.identity);
            gobj = tile;
            spr = tile.GetComponent<SpriteRenderer>();


            if(noise > 0.75f + UnityEngine.Random.Range(-0.1f, 0.1f)){
                
                if(noiseR[1] > 2.25f + UnityEngine.Random.Range(-0.1f, 0.1f))
                {
                    Resource = 2;
                } else if(noiseR[0] > 0.6f + UnityEngine.Random.Range(-0.1f, 0.1f) && noiseR[0] < 0.9f + UnityEngine.Random.Range(-0.1f, 0.1f)){
                Resource = 1;
                } else
                {
                  Resource = 0;  
                }
                
            } else {
                Resource = 0;
            }
            if(Resource == 0){
                spr.color = new Color(noise * 0.5f, noise * 0.5f, noise * 0.5f, 1f);
            } else if(Resource == 1)
            {
                spr.color = new Color(noiseR[0] * 0.5f + UnityEngine.Random.Range(0, 0.1f), noiseR[0] * 0.5f, noiseR[0] * 0.5f, 1f);

            } else if(Resource == 2)
            {
                spr.color = new Color(noiseR[1] * 0.5f + UnityEngine.Random.Range(0, 0.1f), noiseR[1] * 0.2f, noiseR[1] * 0.9f, 1f);

            }
        }
    }
    
    [System.Serializable]
    public class Block
    {
        public SpriteRenderer spr;
        public GameObject gobj;
        public Vector2Int absPos;
        public Vector2Int relPos;

        public TMP_Text txt;

        public int Type;
        //int 1 is item ID, int 2 is item Count (ex: 2, 5 would be item wiht ID 2 has a count of 5)
        public Dictionary<int, int> ItemsIn = new Dictionary<int, int>();
        public Dictionary<int, int> ItemsOut = new Dictionary<int, int>();
        [SerializeReference]
        public List<Blocks.Block> ConnectionsOut = new List<Block>();
        //[SerializeReference]
        //public List<Blocks.Block> ConnectionsIn = new List<Block>();

        public int circuit;

        public Block(Vector2Int pos, Vector2Int cpos)
        {
            Type = 0;
            relPos = pos;
            absPos = pos + cpos;
            Vector3 APos = new Vector3((float)(absPos.x), (float)(absPos.y), 0);
            circuit = 0;
            GameObject tile = Instantiate(Generator.me.objPf[1], APos, Quaternion.identity);
            gobj = tile;
            spr = tile.GetComponent<SpriteRenderer>();
            txt = gobj.transform.GetChild(0).GetComponent<TMP_Text>();
        }
        public bool CheckIsItemChangeValid(bool IO, int i, int c)
        {
            if (IO)
            {
                if (ItemsIn.ContainsKey(i))
                {
                    if(c > 0)
                    {
                        return true;
                    } else if(c < 0 && ItemsIn[i] - c >= 0)
                    {
                        return true;

                    } else
                    {
                        return false;
                    }
                } else
                {

                    return c >= 0;
                }
            } else
            {
                if (ItemsOut.ContainsKey(i))
                {
                    if(c > 0)
                    {
                        return true;
                    } else if(c < 0 && ItemsOut[i] - c >= 0)
                    {
                        return true;
                        
                    } else
                    {
                        return false;
                    }
                } else
                {

                    return c >= 0;
                }
            }
        }

        public void TrySendResources()
        {
            GameObject q = Instantiate(Blocks.me.Transmission, Vector3.zero, Quaternion.identity);
            q.GetComponent<ItemTransmission>().Go(this, ConnectionsOut[0], 0, 1);
        }

        public void ChangeItemCount(bool IO, int i, int c)
        {
            if(CheckIsItemChangeValid(IO, i, c)){
            //IO true in IN, false is OUT
            if (IO)
            {
                    if (ItemsIn.ContainsKey(i))
                    {
                        ItemsIn[i] += c;
                    } else
                    {
                        ItemsIn.Add(i, c);
                    }
                
            } else
                {
                    if (ItemsOut.ContainsKey(i))
                    {
                        ItemsOut[i] += c;
                    } else
                    {
                        ItemsOut.Add(i, c);
                    }
                }
            }
            
        }
    }
}
