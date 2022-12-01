using ProjectUtilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TotalCounterData : PersistentSingleton<TotalCounterData>
{

    public float TotalTime { get; set; }

    public int TotalDeaths { get; set; }

    public int LevelDeaths { get; set; }

    public int TotalTurns { get; set; }
    
}
