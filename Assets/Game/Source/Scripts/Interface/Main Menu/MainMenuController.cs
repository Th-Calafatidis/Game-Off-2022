// /* --------------------
// -----------------------
// Creation date: 15/11/2022
// Author: Alex
// Description: This is what controls the flow and different states of the main menu.
// -----------------------
// ------------------- */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private GameObject m_topMenu;
    [SerializeField] private GameObject m_backButton;

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
        m_backButton.SetActive(true);
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void OpenCreditsMenu()
    {
        m_topMenu.SetActive(false);
        m_backButton.SetActive(true);
    }

    public void Back()
    {
        m_topMenu.SetActive(true);
        m_backButton.SetActive(false);
    }

    #endregion
}
