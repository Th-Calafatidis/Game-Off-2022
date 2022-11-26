// /* --------------------
// -----------------------
// Creation date: 16/11/2022
// Author: Alex
// Description: Controls a window to display information about a unit on hover, as well as a line for their
//              intended action.
// -----------------------
// ------------------- */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EnemyDisplay : MonoBehaviour
{
    [SerializeField] private TMP_Text m_actionLineText;
    [SerializeField] private TMP_Text m_healthText;
    [SerializeField] private TMP_Text m_shieldText;

    private Enemy m_enemy;

    private void Awake()
    {
        m_enemy = null;
        Hide();
    }
        
    /// <summary>
    /// Sets the information of the display to that of an enemy.
    /// </summary>
    /// <param name="enemy"></param>
    public void SetEnemy(Enemy enemy)
    {
        // If there is an enemy set already, that means the health change event has already been subscribed to,
        // we unsubscribe here for that.
        if (m_enemy != null)
        {
            m_enemy.Health.OnHealthChanged -= LoadEnemyData;
            m_enemy.Health.OnHealthZero -= Hide;
        }

        m_enemy = enemy;
        m_enemy.Health.OnHealthChanged += LoadEnemyData;
        m_enemy.Health.OnHealthZero += Hide;
    }

    /// <summary>
    /// Shows the display on screen
    /// </summary>
    public void Show()
    {
        LoadEnemyData();
        // Enable all children
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(true);
        }
    }

    /// <summary>
    /// Hides the display from being seen
    /// </summary>
    public void Hide()
    {
        // Disable all children
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(false);
        }
    }

    private void LoadEnemyData()
    {
        if (m_enemy != null)
        {
            if (m_enemy.HasLine())
                m_actionLineText.text = m_enemy.GetActionLine();
            else
                m_actionLineText.text = "";
            
            m_healthText.text = "<color=red>" + m_enemy.Health.CurrentHealth + "/" + m_enemy.Health.MaxHealth;
            m_shieldText.text = "<color=blue>" + m_enemy.Health.Shield.ToString();
        }
    }
}
