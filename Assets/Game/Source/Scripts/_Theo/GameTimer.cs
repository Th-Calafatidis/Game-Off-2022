using System.Collections;
using System.Collections.Generic;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameTimer : MonoBehaviour
{
    // Live Updates
    private TMP_Text m_timerText;
    private TMP_Text m_turnText;

    // Cleared Updates
    private TMP_Text m_stageClearTimer;
    private TMP_Text m_deathClearCounter;
    private TMP_Text m_turnClearCounter;

    // Total Updates
    private TMP_Text m_totalTurnsText;
    private TMP_Text m_totalDeathText;
    private TMP_Text m_totalTimeText;

    //Total Counter
    private float m_totalGameTime;
    private int m_totalDeaths;
    private int m_totalTurns;


    // Level Counters
    private float m_levelTimer;
    private float m_levelTimerCounter;
    private int m_turnCounter = 0;
    private int m_levelDeathCounter = 0;

    private GameObject m_finalScreen;
    private GameObject m_stageCleared;

    private bool m_levelFinished;

    private static GameTimer m_instance;

    private void PersistentSingleton()
    {
        if (m_instance != null)
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
        SceneLoader.Instance.OnSceneRestarted += DeathCounterUpdate;

        SceneLoader.Instance.OnStageCleared += ResetOnCleared;
        SceneLoader.Instance.OnStageCleared += DisplayClearedTimer;
        SceneLoader.Instance.OnStageCleared += DisplayClearedDeaths;
        SceneLoader.Instance.OnStageCleared += DisplayClearedTurns;

        SceneLoader.Instance.OnGameFinished += FinalScreenDisplay;                

        m_timerText = GameObject.Find("TimeCounter").GetComponentInChildren<TMP_Text>();
        m_turnText = GameObject.Find("TurnCounter").GetComponentInChildren<TMP_Text>();

        m_stageClearTimer = GameObject.Find("ClearedTimeText").GetComponentInChildren<TMP_Text>();
        m_deathClearCounter = GameObject.Find("ClearedDeathText").GetComponentInChildren<TMP_Text>();
        m_turnClearCounter = GameObject.Find("ClearedTurnsText").GetComponentInChildren<TMP_Text>();

        m_totalTurnsText = GameObject.Find("TotalTurnsText").GetComponentInChildren<TMP_Text>();
        m_totalDeathText = GameObject.Find("TotalDeathText").GetComponentInChildren<TMP_Text>();
        m_totalTimeText = GameObject.Find("TotalTimeText").GetComponentInChildren<TMP_Text>();

        m_stageCleared = GameObject.Find("StageCleared");
        m_stageCleared.SetActive(false);

        m_finalScreen = GameObject.Find("FinalScreen");
        m_finalScreen.SetActive(false);

        m_levelFinished = false;
    }

    private void Update()
    {

        TimeCounterUpdate();
        DisplayTimer();
        DisplayTurnCount();
    }

    private void ResetCounters()
    {
        Debug.Log("Total Game Time: " + m_totalGameTime);
        Debug.Log("Total Turns: " + m_totalTurns);
        Debug.Log("Total Deaths: " + m_totalDeaths);

        m_totalGameTime += m_levelTimerCounter;

        m_levelTimer = 0f;
        m_levelTimerCounter = 0f;
        m_turnCounter = 0;

        m_levelFinished = false;

        m_timerText = GameObject.Find("TimeCounter").GetComponentInChildren<TMP_Text>();
        m_turnText = GameObject.Find("TurnCounter").GetComponentInChildren<TMP_Text>();
    }

    private void ResetOnCleared()
    {
        m_levelDeathCounter = 0;
        m_turnCounter = 0;
        m_levelTimer = 0;
        m_levelTimerCounter = 0;
    }


    #region Counter Updates

    private void TimeCounterUpdate()
    {
        m_levelTimerCounter += Time.deltaTime;
    }


    private void TurnCounterUpdate()
    {
        m_turnCounter++;
    }

    private void DeathCounterUpdate()
    {
        m_levelDeathCounter++;
    }

    #endregion

    #region Live Displays

    private void DisplayTimer()
    {

        m_timerText.text = TimeSpan.FromSeconds(m_levelTimerCounter).ToString("m\\:ss\\:f");
    }

    private void DisplayTurnCount()
    {
        m_turnText.text = "Turn: " + m_turnCounter;
    }

    #endregion

    #region Cleared Display

    private void DisplayClearedTimer()
    {
        m_levelTimer = m_levelTimerCounter;

        m_totalGameTime += m_levelTimer;

        m_stageClearTimer.text = "TIME: " + TimeSpan.FromSeconds(m_levelTimerCounter).ToString("mm\\:ss\\:f");
    }

    private void DisplayClearedDeaths()
    {
        m_totalDeaths += m_levelDeathCounter;

        m_deathClearCounter.text = "DEATHS: " + m_levelDeathCounter.ToString();
    }

    private void DisplayClearedTurns()
    {
        m_totalTurns += m_turnCounter;

        m_turnClearCounter.text = "TURNS: " + m_turnCounter.ToString();
    }


    #endregion

    #region Total Updates

    private void FinalScreenDisplay()
    {
        m_finalScreen.SetActive(true);
        DisplayTimerTotal();
        DisplayDeathTotal();
        DisplayTurnTotal();
    }

    private void DisplayTimerTotal()
    {
        m_totalTimeText.text = "TIME: " + TimeSpan.FromSeconds(m_totalGameTime).ToString("mm\\:ss\\:f");
    }

    private void DisplayDeathTotal()
    {
        m_totalDeathText.text = "DEATHS: " + m_totalDeaths.ToString();
    }

    private void DisplayTurnTotal()
    {
        m_totalTurnsText.text = "TURNS: " + m_totalTurns.ToString();
    }


    #endregion


}
