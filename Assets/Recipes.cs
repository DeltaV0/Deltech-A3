using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class Recipes : MonoBehaviour
{
    public static Recipes me;
    public Dictionary<int, Recipe> recipes = new Dictionary<int, Recipe>(); 
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
      me = this;  
      AddRecipe(new int[]{0}, new int[]{1}, new int[]{5}, new int[]{1}, 1, 2f);
    }
    public void AddRecipe(int[] iI, int[] iC, int[] oI, int[] oC, int ID, float time)
    {
        Recipe r = new Recipe(ID, time);
        for(int i = 0; i < iI.Length; i++)
        {
            r.In.Add(iI[i], iC[i]);
        }
        for(int i = 0; i < oI.Length; i++)
        {
            r.Out.Add(oI[i], oC[i]);
        }
    }
    [System.Serializable]
    public class Recipe
    {
        public Dictionary<int, int> In;
        public Dictionary<int, int> Out;

        public int ID;

        public float Time;
        public Recipe(int Id, Dictionary<int, int> Input, Dictionary<int, int> Output, float t)
        {
            In = Input;
            Out = Output;
            Time = t;
            ID = Id;
            me.recipes.Add(ID, this);
        }
        public Recipe(int Id, float t)
        {
            In = new Dictionary<int, int>();
            Out = new Dictionary<int, int>();
            ID = Id;
            Time = t;
            me.recipes.Add(ID, this);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
