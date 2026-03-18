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
        if(Target != null){
        StartCoroutine(GoToTarget(Vector2.Distance(exiting.absPos, Target.absPos), exiting.absPos, Target.absPos));
        }
    }

    private IEnumerator GoToTarget(float Distance, Vector2 Start, Vector2 T)
    {
        if(exiting.CheckIsItemChangeValid(false, ID, -1 * Count)){
        exiting.ChangeItemCount(false, ID, -1 * Count);
        exiting.canSendResources = false;
        Vector2 Offset = Functions.MidPoint(Start, T) + (Random.insideUnitCircle * Vector2.Distance(Start, T) * 0.25f);
        for(int i = 0; i < (int)Distance * 10; i++)
        {
            float t = (float)i / (Distance * 10);
            transform.position = (Vector3)Functions.Bezier(Start, T, Offset, Mathf.Pow(t-1, 5) + 1);
            yield return new WaitForSeconds(0.01f);
        }
        exiting.canSendResources = true;
        if(Target != null){
        Target.ChangeItemCount(true, ID, Count);
        }
        Destroy(gameObject);
        } else
        {
           Destroy(gameObject); 
        }
    }


}
