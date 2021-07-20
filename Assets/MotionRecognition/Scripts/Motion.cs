using System;
using UnityEngine;
using System.Collections.Generic;

[Serializable]
public class Motion
{
    public string name;
    public List<Vector3> points;
    public Motion(
        string name = "", 
        List<Vector3> points = null 
        )
    {
        this.name = name; 
        this.points = points;
    }
}

public class NullMotion : Motion{
    public NullMotion(){
        name = "NULL";
        points = new List<Vector3>();
    }
}
