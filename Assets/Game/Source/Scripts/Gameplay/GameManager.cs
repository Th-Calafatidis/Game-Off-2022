using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Levels
    private int m_level;


    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("Multiple instances of GameManager detected.");
            return;
        }
        DontDestroyOnLoad(this);
    }
}
