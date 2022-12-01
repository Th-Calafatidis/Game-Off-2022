// /* --------------------
// -----------------------
// Creation date: 15/11/2022
// Author: Alex
// Description: This is what controls the flow and different states of the main menu.
// -----------------------
// ------------------- */

using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private GameObject m_topMenu;
    [SerializeField] private GameObject m_backButton;
    [SerializeField] private GameObject m_playButton;
    [SerializeField] private GameObject m_optionsButton;
    [SerializeField] private GameObject m_exitButton;
    [SerializeField] private GameObject m_helpPanel;
    [SerializeField] private GameObject m_creditsPanel;

    public static MainMenuController Instance { get; private set; }

    public Action OnHighlighted;
    public Action OnPressed;

    private AudioSource m_audioSource;

    #region Top Menu

    /// <summary>
    /// Loads the next scene in the load order, which should be the actual game scene.
    /// </summary>
    public void StartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);       
    }

    public void OpenSettingsMenu()
    {
        m_topMenu.SetActive(false);
        m_helpPanel.SetActive(true);
        m_backButton.SetActive(true);
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void OpenCreditsMenu()
    {
        m_topMenu.SetActive(false);
        m_creditsPanel.SetActive(true);
        m_backButton.SetActive(true);
    }

    public void Back()
    {
        m_topMenu.SetActive(true);
        m_helpPanel.SetActive(false);
        m_creditsPanel.SetActive(false);
        m_backButton.SetActive(false);
    }

    #endregion
}
