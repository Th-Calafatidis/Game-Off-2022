using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

public class AbilityButtonManager : MonoBehaviour
{
    //private static AbilityButtonManager m_instance;
    public static AbilityButtonManager Instance { get; private set; }

    [SerializeField] private Button m_moveButton;
    [SerializeField] private Button m_meleeButton;
    [SerializeField] private Button m_mindBlastButton;
    [SerializeField] private Button m_blinkButton;
    [SerializeField] private Button m_endTurnButton;

    public Action OnMoveButtonPress;
    public Action OnMeleeButtonPress;
    public Action OnMindBlastButtonPress;
    public Action OnBlinkButtonPress;
    public Action OnEndTurnButtonPress;

    private void Awake()
    {
        Instance = this;

        m_moveButton.onClick.AddListener(() => OnMoveButtonPress?.Invoke());
        m_meleeButton.onClick.AddListener(() => OnMeleeButtonPress?.Invoke());
        m_mindBlastButton.onClick.AddListener(() => OnMindBlastButtonPress?.Invoke());
        m_blinkButton.onClick.AddListener(() => OnBlinkButtonPress?.Invoke());
        m_endTurnButton.onClick.AddListener(() => OnEndTurnButtonPress?.Invoke());
    }
}
