using UnityEngine;
using System.Collections;

public class ItemTransmission : MonoBehaviour
{
    public Blocks.Block exiting;
    public Blocks.Block Target;

    public int ID;
    public int Count;
    
    public void Go(Blocks.Block s, Blocks.Block e, int id, int C)
    {
        ID = id;
        Count = C;
        exiting = s;
        Target = e;
        StartCoroutine(GoToTarget(Vector2.Distance(exiting.absPos, Target.absPos), exiting.absPos, Target.absPos));
    }

    private IEnumerator GoToTarget(float Distance, Vector2 Start, Vector2 T)
    {
        if(exiting.CheckIsItemChangeValid(false, ID, -1 * Count)){
        exiting.ChangeItemCount(false, ID, -1 * Count);
        exiting.canSendResources = false;
        for(int i = 0; i < (int)Distance * 10; i++)
        {
            float t = (float)i / (Distance * 10);
            transform.position = (Vector3)Vector2.Lerp(Start, T, Mathf.Pow(t-1, 5) + 1);
            yield return new WaitForSeconds(0.01f);
        }
        exiting.canSendResources = true;
        Target.ChangeItemCount(true, ID, Count);
        Destroy(gameObject);
        } else
        {
           Destroy(gameObject); 
        }
    }
}
