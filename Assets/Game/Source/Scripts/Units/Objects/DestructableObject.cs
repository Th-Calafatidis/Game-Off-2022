// /* --------------------
// -----------------------
// Creation date: 27/11/2022
// Author: Theodore
// Description: This is a static object enemy that can create hazards, deal AoE damage and explode on a Timer;
//
// -----------------------
// ------------------- */

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;

public class DestructableObject : Enemy, IPushable
{
    public int Range = 1;
    
    [Header("Destroy On Timer")]
    [SerializeField] private bool m_destroyOnTimer;
    [SerializeField] private int m_timer;

    [Header("Damage")]
    [SerializeField] private bool m_dealDamage;
    [SerializeField] private int m_damageAmount;

    [Header("Pushing")]
    [SerializeField] private bool m_pushable;

    [Header("Hazards")]
    [SerializeField] private bool m_createHazard;
    [SerializeField] private Hazard m_hazardType;
    [SerializeField] private int m_hazardDuration;

    private List<Vector2Int> surroundingTiles;

    private Enemy m_enemy;
    private GameObject m_timerObject;


    override public void Awake()
    {
        base.Awake();

        // Initialize health
        m_health = new Health(m_maxHealth);
        m_health.OnHealthZero += OnDeath;

        m_enemy = GetComponent<Enemy>();
        m_timerObject = transform.Find("TimerIcon").gameObject;
    }

    protected override void Start()
    {
        base.Start();

        surroundingTiles = Grid.Instance.GetSurroundingTiles(GridPosition, Range);
        m_enemy.CreateHighlight(surroundingTiles, Color.red);

        if (!m_destroyOnTimer)
        {
            m_timerObject.SetActive(false);
            return;
        }
            

        BattleManager.Instance.OnRoundStart += TimerCopuntdown;
    }

    
    public override void AddStatusEffect(StatusEffect effect)
    {
        // Do nothing, as status effects should not affect this.
    }

    public override void Push(Direction direction, int distance)
    {
        if (!m_pushable) return;
        base.Push(direction, distance);
    }

    public override void OnDeath()
    {

        base.OnDeath();

        if (m_createHazard)
            CreateHazard();

        if (m_dealDamage)
            DealDamage();

        m_enemy.ClearHighlights();

        BattleManager.Instance.OnRoundStart -= TimerCopuntdown;
    }

    private void CreateHazard()
    {
        EnvironmentHazard.CreateHazard(m_hazardType, m_hazardDuration, GridPosition);

        List<Vector2Int> surroundingTiles = Grid.Instance.GetSurroundingTiles(GridPosition, Range);

        foreach (Vector2Int tile in surroundingTiles)
        {
            var node = Grid.Instance.GetNodeAt(tile.x, tile.y);

            if (node.IsObstructed)
                return;

            EnvironmentHazard.CreateHazard(m_hazardType, m_hazardDuration, tile);
        }
    }

    private void DealDamage()
    {
        List<Vector2Int> targetTiles = Grid.Instance.GetSurroundingTiles(this.GridPosition, Range);

        foreach (Vector2Int tile in targetTiles)
        {
            Unit target = Grid.Instance.GetUnitAt(tile);

            if (target != null)
            {
                target.TakeDamage(m_damageAmount);
            }

        }
    }

    private void TimerCopuntdown()
    {
        m_timer--;

        m_timerObject.GetComponentInChildren<TMP_Text>().text = m_timer.ToString();

        if (m_timer <= 0)
        {
            OnDeath();
        }
    }

    public override void DetermineAction()
    {
        
    }

    public override void DetermineMove()
    {
        
    }
}
