using System.Collections;
using System.Collections.Generic;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameTimer : MonoBehaviour
{
    public static GameTimer Instance { get; private set; }

    [SerializeField] private TMP_Text m_timerText;

    private double m_gameTimer;
    private float m_gameTimerCounter;

    private void Awake()
    {
        PersistentSingleton();

        m_gameTimer = 0f;
        m_gameTimerCounter = 0f;
    }

    private void Update()
    {
        m_gameTimer += Time.deltaTime;
    }

    private void DisplayTimer()
    {
         m_gameTimer = m_gameTimerCounter;

         m_gameTimer = Math.Round(m_gameTimer, 2);

        m_timerText.text = "Total Time: " + m_gameTimer;
    }

    private void PersistentSingleton()
    {
        if (Instance == null) Instance = this;
        else Destroy(this);

        DontDestroyOnLoad(this);
    }
}
