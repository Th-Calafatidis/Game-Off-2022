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

    // Level Counters
    private float m_levelTimer;
    private float m_levelTimerCounter;
    private int m_turnCounter = 0;
    private int m_levelDeathCounter ;

    private GameObject m_finalScreen;
    private GameObject m_stageCleared;

    private void Start()
    {

        BattleManager.Instance.OnRoundStart += TurnCounterUpdate;

        SceneLoader.Instance.OnSceneLoaded += ResetCounters;
        SceneLoader.Instance.OnSceneLoaded += ResetOnCleared;

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
    }

    private void Update()
    {
        if (!m_stageCleared.activeSelf)
        {
            TimeCounterUpdate();
        }
        
        DisplayTimer();
        DisplayTurnCount();
    }

    private void ResetCounters()
    {
        
        m_levelTimer = 0f;
        m_levelTimerCounter = 0f;
        m_turnCounter = 0;
    }

    private void ResetOnCleared()
    {
        
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

        TotalCounterData.Instance.TotalTime += m_levelTimer;

        m_stageClearTimer.text = "TIME: " + TimeSpan.FromSeconds(m_levelTimerCounter).ToString("mm\\:ss\\:f");
    }

    private void DisplayClearedDeaths()
    {

        m_deathClearCounter.text = "DEATHS: " + TotalCounterData.Instance.LevelDeaths.ToString();

    }

    private void DisplayClearedTurns()
    {
        TotalCounterData.Instance.TotalTurns += m_turnCounter;

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
        m_totalTimeText.text = "TIME: " + TimeSpan.FromSeconds(TotalCounterData.Instance.TotalTime).ToString("mm\\:ss\\:f");
    }

    private void DisplayDeathTotal()
    {
        m_totalDeathText.text = "DEATHS: " + TotalCounterData.Instance.TotalDeaths.ToString();
    }

    private void DisplayTurnTotal()
    {
        m_totalTurnsText.text = "TURNS: " + TotalCounterData.Instance.TotalTurns.ToString();
    }


    #endregion


}
