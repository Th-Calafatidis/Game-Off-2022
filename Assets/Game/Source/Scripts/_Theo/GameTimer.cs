using System.Collections;
using System.Collections.Generic;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameTimer : MonoBehaviour
{
    [SerializeField] private TMP_Text m_timerText;
    [SerializeField] private TMP_Text m_turnText;

    private double m_levelTimer;
    private double m_levelTimerCounter;
    private double m_totalGameTime;
    private int m_turnCounter = 0;

    private bool m_levelFinished;

    private static GameTimer m_instance;

    private void PersistentSingleton()
    {
        if(m_instance != null)
        {
            Destroy(gameObject);
        }
    }

    private void Awake()
    {
        PersistentSingleton();

        m_instance = this;

        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        BattleManager.Instance.OnRoundStart += TurnCounterUpdate;
        SceneLoader.Instance.OnSceneLoaded += ResetCounters;

        m_timerText = GameObject.Find("TimeCounter").GetComponentInChildren<TMP_Text>();
        m_turnText = GameObject.Find("TurnCounter").GetComponentInChildren<TMP_Text>();
    }

    private void Update()
    {
        m_levelTimerCounter += Time.deltaTime;

        
        DisplayTurnCount();
    }

    private void LateUpdate()
    {
        DisplayTimer();
    }

    private void ResetCounters()
    {

        m_totalGameTime += m_levelTimerCounter;

        m_levelTimer = 0f;
        m_levelTimerCounter = 0f;
        m_turnCounter = 0;

        m_levelFinished = false;

        m_timerText = GameObject.Find("TimeCounter").GetComponentInChildren<TMP_Text>();
        m_turnText = GameObject.Find("TurnCounter").GetComponentInChildren<TMP_Text>();
    }


    private void DisplayTimer()
    {
        //m_levelTimerCounter = Math.Round(m_levelTimerCounter, 2);

        m_timerText.text = TimeSpan.FromSeconds(m_levelTimerCounter).ToString("m\\:ss\\:f");
    }


    private void DisplayTimerEndScreen()
    {
        m_levelTimer = m_levelTimerCounter;

        m_levelTimer = Math.Round(m_levelTimer, 2);

        m_timerText.text = "Time: " + m_levelTimer;
    }

    private void DisplayTimerTotal()
    {

    }

    private void DisplayTurnCount()
    {
        m_turnText.text = "Turn: " + m_turnCounter;
    }

    private void TurnCounterUpdate()
    {
        m_turnCounter ++ ;
    }
}
