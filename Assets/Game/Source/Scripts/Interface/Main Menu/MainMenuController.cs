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

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private GameObject m_topMenu;

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
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void OpenCreditsMenu()
    {
        m_topMenu.SetActive(false);
    }

    #endregion
}
