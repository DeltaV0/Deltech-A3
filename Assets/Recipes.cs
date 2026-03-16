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
    }

    public class Recipe
    {
        public Dictionary<int, int> In;
        public Dictionary<int, int> Out;

        public int ID;

        float Time;
        public Recipe(int Id, Dictionary<int, int> Input, Dictionary<int, int> Output, float t)
        {
            In = Input;
            Out = Output;
            Time = t;
            ID = Id;
            me.recipes.Add(ID, this);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
