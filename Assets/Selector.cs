using UnityEngine;

public class Selector : MonoBehaviour
{
    public bool stationary;
    public Vector2 TPos;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && stationary && !PlayerStats.me.mouseOverUI)
            {
                Vector2Int clickedPos = new Vector2Int(Mathf.FloorToInt(Camera.main.ScreenToWorldPoint(Input.mousePosition).x + 0.5f), Mathf.FloorToInt(Camera.main.ScreenToWorldPoint(Input.mousePosition).y + 0.5f));
                TPos = (Vector2)clickedPos;
                PlayerStats.me.SelectBlock(Generator.me.GetBlockAt(Vector2Int.RoundToInt(TPos)), Vector2Int.RoundToInt(TPos));
            }
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        if (!stationary)
        {
            Vector2Int clickedPos = new Vector2Int(Mathf.FloorToInt(Camera.main.ScreenToWorldPoint(Input.mousePosition).x + 0.5f), Mathf.FloorToInt(Camera.main.ScreenToWorldPoint(Input.mousePosition).y + 0.5f));
            TPos = (Vector2)clickedPos;
        }

        transform.position = Vector3.Lerp(transform.position, (Vector3)TPos, 0.25f);
    }
}
