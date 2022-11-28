using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoundCounter : MonoBehaviour
{
    public int CurrentRound { get; private set; }

    [SerializeField] private int m_levelRoundTarget;

    private void Start()
    {
        CurrentRound = 1;

        BattleManager.Instance.OnRoundEnd += CountRound;

    }

    private void CountRound()
    {
        CurrentRound ++;
    }
}
