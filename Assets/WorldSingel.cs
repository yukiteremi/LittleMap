using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldSingel 
{
   public static WorldSingel ins=new WorldSingel();

    public Obj_MainPlayer player=null;
    public Dictionary<string, Obj> CharatorPool = new Dictionary<string, Obj>();
}
