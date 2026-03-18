using UnityEngine;
using System.Collections.Generic;
using TMPro;
using System.Collections;
using System.Linq;

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
            noiseR[0] = Generator.me.GetResourceNoiseAtPos((Vector2)APos, 1);
            noiseR[1] = Generator.me.GetResourceNoiseAtPos((Vector2)APos, 2);

            GameObject tile = Instantiate(Generator.me.objPf[0], APos, Quaternion.identity);
            gobj = tile;
            spr = tile.GetComponent<SpriteRenderer>();


            if(noise < 0.25f + UnityEngine.Random.Range(-0.1f, 0.1f)){
                
                if(noiseR[1] > 0.5f)
                {
                    Resource = 2;
                } else if(noiseR[0] > 0.5f){
                    Resource = 1;
                } else
                {
                  Resource = 0;  
                }
                
            } else {
                Resource = 0;
            }
            if(Resource == 0){
                spr.color = Color.HSVToRGB(0.4f + UnityEngine.Random.Range(-0.01f, 0.01f), 0.65f + noise * 0.2f, 0.1f + noise * 0.9f);
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
        public List<Vector2Int> ConnectionsOutPos = new List<Vector2Int>();
        //[SerializeReference]
        //public List<Blocks.Block> ConnectionsIn = new List<Block>();

        public int circuit;

        public bool canSendResources;
        public int sendingIndex;

        public bool busy;

        public Vector2Int size;

        public Block(Vector2Int pos, Vector2Int cpos, int t)
        {
            size.x = 1;
            size.y = 1;

            busy = false;
            canSendResources = true;
            Type = t;
            relPos = pos;
            absPos = pos + cpos;
            Vector3 APos = new Vector3((float)(absPos.x), (float)(absPos.y), 0);
            circuit = 0;
            GameObject tile = Instantiate(Generator.me.objPf[t + 1], APos, Quaternion.identity);
            gobj = tile;
            spr = tile.GetComponent<SpriteRenderer>();
            if(gobj.transform.childCount > 0){
            txt = gobj.transform.GetChild(0).GetComponent<TMP_Text>();
            }
        }

        public void destroy()
        {
            Destroy(gobj);
            Generator.me.GetChunkAt(absPos).buildings.Remove(absPos);
        }
        public bool CheckIsItemChangeValid(bool IO, int i, int c)
        {
            if (IO)
            {
                if (ItemsIn.ContainsKey(i))
                {
                    if(c >= 0)
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
                    if(c >= 0)
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

        public void ChangeSendingIndex(){
            sendingIndex++;
            if(sendingIndex >= ConnectionsOutPos.Count){
                sendingIndex = 0;
            }
        }

        public void TrySendResources()
        {
            if(ConnectionsOutPos.Count > 0 && canSendResources && ItemsOut.Count > 0){
            GameObject q = Instantiate(Blocks.me.Transmission, Vector3.zero, Quaternion.identity);
            q.GetComponent<ItemTransmission>().Go(this, Generator.me.GetBlockAt(ConnectionsOutPos[sendingIndex]), ItemsOut.ElementAt(0).Key, 1);
            ChangeSendingIndex();
            }
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
                    if(ItemsIn[i] <= 0)
                    {
                        ItemsIn.Remove(i);
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
                    if(ItemsOut[i] <= 0)
                    {
                        ItemsOut.Remove(i);
                    }
                }
            }
            
        }
        public void MiningBlockLogic(Terrain t)
        {
            me.StartCoroutine(Mine(t));

        }

        public bool CheckIfCanPerformRecipe(Recipes.Recipe r)
        {
            for(int i = 0; i < r.In.Count; i++)
            {
                if(!CheckIsItemChangeValid(true, r.In.ElementAt(i).Key, -1 * r.In.ElementAt(i).Value))
                {
                    return false;
                }
            }
            return true;
        }

        public void Craft()
        {
            Recipes.Recipe r = Recipes.me.recipes[1];
            if (CheckIfCanPerformRecipe(r) && !busy)
            {
                for(int i = 0; i < r.Out.Count; i++)
                {
                    ChangeItemCount(true, r.Out.ElementAt(i).Key, -1 * r.Out.ElementAt(i).Value);
                }
                me.StartCoroutine(craft(r));
            }
        }
        private IEnumerator craft(Recipes.Recipe r)
        {
            busy = true;
            yield return new WaitForSeconds(r.Time);
            busy = false;
            for(int i = 0; i < r.Out.Count; i++)
            {
                ChangeItemCount(false, r.Out.ElementAt(i).Key, r.Out.ElementAt(i).Value);
            }
            
        }
        private IEnumerator Mine(Terrain t)
        {
            busy = true;
            yield return new WaitForSeconds(1f);
            busy = false;
            ChangeItemCount(false, t.Resource, 1);
        }
    }
}
