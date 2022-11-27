using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoundCounter : MonoBehaviour
{
    public int RoundsPassed { get; private set; }

    private void Awake()
    {
        BattleManager.Instance.OnRoundEnd += CountRound;
    }

    private void CountRound()
    {
        RoundsPassed ++ ;
    }
}
