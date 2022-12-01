using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TotalCounterData : MonoBehaviour
{
    public static TotalCounterData Instance { get; set; }

    public float TotalTime { get; set; }

    public int TotalDeaths { get; set; }

    public int TotalTurns { get; set; }

    private void PersistentSingleton()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }
    }

    private void Awake()
    {
        PersistentSingleton();

        Instance = this;

        DontDestroyOnLoad(gameObject);
    }
}
