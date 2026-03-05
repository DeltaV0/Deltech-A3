using UnityEngine;
using System.Collections.Generic;

public class PowerCalculator : MonoBehaviour
{
    Dictionary<int, PowerGrid> grids = new Dictionary<int, PowerGrid>();

    public class PowerGrid
    {
        public int netgain;
        
        public int powerGenRemainingThisTick;

        public int ID;
        public List<Blocks.Block> Wires = new List<Blocks.Block>();

        public PowerGrid(int id){
            ID = id;
        }

        public void ShuffleWireOrder(){
            Blocks.Block t;

            for(int i = 0; i < Wires.Count; i++){
                t = Wires[i];
                int q = UnityEngine.Random.Range(0, Wires.Count);
                Wires[i] = Wires[q];
                Wires[q] = t;
            }
        }
    }
}
