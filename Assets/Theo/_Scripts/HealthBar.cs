using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    private Health m_health;

    private Transform m_containerTransform;

    [SerializeField] private GameObject m_cell;

    private void Start()
    {
        m_health = transform.parent.gameObject.GetComponent<Entity>().Health;

        m_containerTransform = GetComponentInChildren<HorizontalLayoutGroup>().transform;

        CreateHealthBar();

        m_health.OnHealthChanged += UpdateHealthBar;
    }

    private void UpdateHealthBar()
    {
        // Show damage dealt. Shield can be added here as m_health.MaxHealth + m_health.Shield and then check if
        // m_health.Shield > 0 and deactivate the shield object first.
        for (int i = m_health.MaxHealth; i >= 0; i--)
        {
            if (m_health.CurrentHealth < i)
            {
                m_containerTransform.GetChild(m_health.MaxHealth - i).GetChild(2).gameObject.SetActive(false);
            }
        }
    }

    private void CreateHealthBar()
    {
        for (int i = 0; i < m_health.MaxHealth; i++)
        {
            Instantiate(m_cell, m_containerTransform);
        }
    }
}
