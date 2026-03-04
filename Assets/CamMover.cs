using UnityEngine;

public class CamMover : MonoBehaviour
{
    public static CamMover me;

    public Vector3 campos;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        me = this;
        campos = new Vector3(0, 0, -10);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float moveX = Input.GetAxisRaw ("Horizontal");
		float moveY = Input.GetAxisRaw ("Vertical");

        Vector2 moveDir = new Vector2 (Mathf.RoundToInt(moveX), Mathf.RoundToInt(moveY)).normalized;

        campos += (Vector3)moveDir * 0.5f;
        transform.position = Vector3.Lerp(transform.position, new Vector3(Mathf.RoundToInt(campos.x) + 0.5f, Mathf.RoundToInt(campos.y), Mathf.RoundToInt(campos.z)), 0.25f);
    }
}
