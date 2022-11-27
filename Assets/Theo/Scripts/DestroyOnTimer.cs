using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyOnTimer : MonoBehaviour
{
    [SerializeField] private int m_timer;

    private void Awake()
    {
        BattleManager.Instance.OnRoundStart += TimerCopuntdown;
    }

    private void TriggerAction()
    {
        if(m_timer <= 0)
        {
            // Trigger Action
        }
    }

    private void TimerCopuntdown()
    {
        m_timer -- ;
    }
}
