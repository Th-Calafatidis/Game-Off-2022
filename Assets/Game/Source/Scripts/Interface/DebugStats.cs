using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DebugStats : MonoBehaviour
{
    public Player m_player;
    public TMP_Text m_ap;
    public TMP_Text m_hp;

    private void Start()
    {
        m_player.Health.OnHealthChanged += OnHealthChange;
        m_player.ActionPoints.OnActionPointChange += OnAPChange;

        OnHealthChange();
        OnAPChange();
    }

    private void OnHealthChange()
    {
        m_hp.text = "HP: " + m_player.Health.CurrentHealth.ToString() + " + " + m_player.Health.Shield.ToString();
    }
    
    private void OnAPChange()
    {
        m_ap.text = "AP: " + m_player.ActionPoints.CurrentActionPoints.ToString() + " + " + m_player.ActionPoints.PooledActionPoints.ToString();
    }
}
