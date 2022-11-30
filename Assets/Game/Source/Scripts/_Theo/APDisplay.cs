// /* --------------------
// -----------------------
// Creation date: 29/11/2022
// Author: Theodore
// Description: Responsible for creating and updating the AP bar.
// -----------------------
// ------------------- */

using UnityEngine;

public class APDisplay : MonoBehaviour
{
    private Player m_player;

    [SerializeField] private GameObject m_cell;

    private void Start()
    {
        m_player = GameObject.Find("Player").GetComponent<Player>();

        CreateActionPointsBar();        

        m_player.ActionPoints.OnActionPointChange += ActionPointsUpdate;

        BattleManager.Instance.OnPlayerTurnStart += ActionPointsUpdate;
    }

    private void ActionPointsUpdate()
    {
        // Set all the empty AP symbols to inactive
        foreach(Transform child in transform)
        {
            child.GetChild(1).gameObject.SetActive(false);
        }

        // Go through the last AP symbol to the first and if current AP is less than that amount set the symbol to the empty
        for (int i = m_player.ActionPoints.MaxActionPoints + m_player.ActionPoints.ActionPointPoolSize; i >= 0; i--)
        {
            if (m_player.ActionPoints.CurrentActionPoints + m_player.ActionPoints.PooledActionPoints < i)
            {
                transform.GetChild(i - 1).GetChild(1).gameObject.SetActive(true);
            }
        }
    }

    private void CreateActionPointsBar()
    {
        for (int i = 0; i < m_player.ActionPoints.MaxActionPoints + m_player.ActionPoints.ActionPointPoolSize; i++)
        {
            Instantiate(m_cell, this.transform);
        }
    }
}
