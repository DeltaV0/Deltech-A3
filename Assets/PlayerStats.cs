using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public Blocks.Block selectedBlock;

    public int action;
    public static PlayerStats me;

    public bool mouseOverUI;

    public Blocks.Block bStart;
    //public Blocks.Block bEnd;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        me = this;
    }


    public void SelectBlock(Blocks.Block b, Vector2Int p)
    {
        if(b != null && b.absPos != null)
        {
            selectedBlock = b;
        }
        if(action == 1)
        {
            Generator.me.GetChunkAt(p).Build(p);
        } else if(action == 5)
        {
            if(b != null && b.gobj != null && b.absPos != null){
            if(bStart == null || bStart.gobj == null)
            {
                bStart = b;
            } else
            {
                bStart.ConnectionsOutPos.Add(b.absPos);
               // b.ConnectionsIn.Add(bStart);
                bStart = null;
            }
            } else {
                bStart = null;
            }
        }
        
    }
    public void SetAction(int i)
    {
        action = i;
    }

    public void SetOverUI(bool a)
    {
        mouseOverUI = a;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
