using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighlightManager : MonoBehaviour
{
    private List<Enemy> m_enemyRegistry;

    private bool m_highlightsShown = false;

    private void Update()
    {
        ShowThreatAreas();
    }

    private void RegisterEnemies()
    {
        foreach (Unit unit in Grid.Instance.UnitRegistry.Keys)
        {
            Enemy enemy = unit.GetComponent<Enemy>();

            if (enemy != null)
            {
                m_enemyRegistry.Add(enemy);
            }
        }
    }

    private void ShowThreatAreas()
    {

        if (Input.GetKey(KeyCode.Tab))
        {
            if (!m_highlightsShown)
            {
                if(m_enemyRegistry.Count != 0)
                {
                    m_enemyRegistry.Clear();
                }

                RegisterEnemies();

                foreach (Enemy enemy in m_enemyRegistry)
                {
                    enemy.ShowHighlights();
                }

                m_highlightsShown = true;
            }
        }

        if (Input.GetKeyUp(KeyCode.Tab))
        {
            foreach (Enemy enemy in m_enemyRegistry)
            {
                enemy.HideHighlights();
            }
        }

        m_highlightsShown = false;
    }
}
