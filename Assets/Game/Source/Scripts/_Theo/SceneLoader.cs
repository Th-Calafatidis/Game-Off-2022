using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader Instance { get; private set; }

    private AnnouncementText m_announcement;
    private Player m_player;
    private AudioSource m_audioSource;

    private bool m_levelFinished;

    public Action OnSceneLoaded;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        
        m_announcement = GameObject.Find("Announcement").GetComponent<AnnouncementText>();
        m_player = GameObject.Find("Player").GetComponent<Player>();
        m_audioSource = Camera.main.GetComponent<AudioSource>();

        m_levelFinished = false;

        AbilityButtonManager.Instance.OnQuitButtonPress += LoadMainMenu;

    }

    private void Update()
    {

        if (BattleManager.Instance.GetEnemyCount() == 0 && !m_levelFinished)
        {
            OnSceneLoaded?.Invoke();

            m_levelFinished = true;
            m_announcement.Announce("Level Complete", 4f);
            m_audioSource.Stop();
            m_audioSource.clip = null;
            m_audioSource.PlayOneShot(AudioManager.Instance.WinSound);
            this.Invoke(Utility.LoadNextScene, 4f);

        }

        else if (m_player.Health.CurrentHealth <= 0 && !m_levelFinished)
        {
            OnSceneLoaded?.Invoke();

            m_levelFinished = true;
            m_announcement.Announce("You Died", 4f);
            m_audioSource.Stop();
            m_audioSource.clip = null;
            m_audioSource.PlayOneShot(AudioManager.Instance.LossSound);
            this.Invoke(Utility.RestartScene, 4f);

        }
    }

    private void LoadMainMenu()
    {
        SceneManager.LoadScene(0);
    }
}
