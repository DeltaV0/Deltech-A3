using UnityEngine;

public class Blocks : MonoBehaviour
{
    
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

            GameObject tile = Instantiate(Generator.me.objPf, APos, Quaternion.identity);
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
                spr.color = new Color(noise * 0.75f + 0.25f, noise * 0.5f + 0.25f, noise * 0.5f + 0.25f, 1f);
            } else if(Resource == 1)
            {
                spr.color = new Color(noiseR[0] * 0.5f + UnityEngine.Random.Range(0, 0.1f), noiseR[0] * 0.5f, noiseR[0] * 0.5f, 1f);

            } else if(Resource == 2)
            {
                spr.color = new Color(noiseR[1] * 0.5f + UnityEngine.Random.Range(0, 0.1f), noiseR[1] * 0.2f, noiseR[1] * 0.9f, 1f);

            }
        }
    }
}
