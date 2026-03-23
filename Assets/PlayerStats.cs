using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class PlayerStats : MonoBehaviour
{
    public Blocks.Block selectedBlock;

    public int action;
    public static PlayerStats me;

    public bool mouseOverUI;

    public Blocks.Block bStart;

    public int selection;

    public int MaxSelection;

    public TMP_Text t;

    [System.Serializable]
    public class Selectable
    {
        public string name;
        public Color color;
    }

    public Selectable[] coloursBuild;
    public Selectable[] coloursTransmit;
    public Selectable[] coloursMisc;

    public Dictionary<int, int> Items = new Dictionary<int, int>(); 
    public Blocks.Block CoreBlock;
    //public Blocks.Block bEnd;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        me = this;
        selection = 0;
    }

    public bool CheckIfCanChangeItems(int i, int c)
    {
        if (Items.ContainsKey(i))
                {
                    if(c >= 0)
                    {
                        return true;
                    } else if(c < 0 && Items[i] - c >= 0)
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

    public void ChangeValue(int i, int c)
    {
        if(CheckIfCanChangeItems(i, c))
        {
            if (Items.ContainsKey(i))
                    {
                        Items[i] += c;
                    } else
                    {
                        Items.Add(i, c);
                    }
                    if(Items[i] <= 0)
                    {
                        Items.Remove(i);
                    }
        }
    }


    public void SelectBlock(Blocks.Block b, Vector2Int p)
    {
        if(b != null && b.absPos != null)
        {
            selectedBlock = b;
        }
        if(action == 1)
        {
            Generator.me.GetChunkAt(p).Build(p, Generator.me.Info[selection]);

        } else if(action == 5)
        {
            if(b != null && b.gobj != null && b.absPos != null){
            if(bStart == null || bStart.gobj == null)
            {
                bStart = b;
            } else
            {
                if(bStart.Type == 1 || b.Type == 1)
                    {
                        if(!bStart.ConnectionsOutPos.Contains(b.absPos)){
                        bStart.ConnectionsOutPos.Add(b.absPos);
                        } else
                        {
                            bStart.ConnectionsOutPos.Remove(b.absPos);
                        }
                        
               // b.ConnectionsIn.Add(bStart);
                        bStart = null;
                    } else
                    {
                        bStart = null;
                    }
                
            }
            } else {
                bStart = null;
            }
        } else if(action == -1)
        {
            Generator.me.GetChunkAt(p).Break(p);
        } else if(action == -5)
        {
            Blocks.Terrain t = Generator.me.GetChunkAt(p).terrain[p - Generator.me.GetChunkAt(p).pos];
            if(t != null && t.Resource != 0){
            CoreBlock.ChangeItemCount(false, t.Resource, 1);
            }
            
        }
        
    }
    public void SetAction(int i)
    {
        selection = 0;
        MaxSelection = 0;
        if(i == 1)
        {
            MaxSelection = coloursBuild.Length - 1;
        }
        if(i == 5)
        {
            MaxSelection = coloursTransmit.Length - 1;
        }
        action = i;
    }

    public void SetOverUI(bool a)
    {
        mouseOverUI = a;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        selection += (int)Mathf.Round(Input.mouseScrollDelta.y);
            if(selection > MaxSelection){
                selection = 0;
            } else if(selection < 0)
            {
                selection = MaxSelection;
            }
        if(action == 1){
            t.text = coloursBuild[selection].name;
            } else if(action == 5)
            {
               t.text = coloursTransmit[selection].name; 
            } else
            {
                if(action == -1){
                t.text = coloursMisc[0].name;
                } else if(action == -5)
            {
                t.text = coloursMisc[1].name;
            }else if(action == 0)
            {
                if(selectedBlock != null)
                {
                    selectedBlock.circuit += (int)Mathf.Round(Input.mouseScrollDelta.y);
                }
                t.text = coloursMisc[2].name;
            }
            }

    }
}
