// /* --------------------
// -----------------------
// Creation date: 14/11/2022
// Author: Alex
// Description: This is an object that can be pushed and damaged, as well as being destroyed. Has the option to create hazards.
// 
// Edited: Theodore on 27/11/2022
// Added: DestroyOnTimer, DealDamage, TimerCountdown, CreateHazards, configured the script to function with the new features.
// -----------------------
// ------------------- */

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DestructableObject : Unit, IPushable
{
    [Header("Destroy On Timer")]
    [SerializeField] private bool m_destroyOnTimer;
    [SerializeField] private int m_timer;

    [Header("Health")]
    [SerializeField] private bool m_damagable;
    [SerializeField] private int m_maxHealth;
    private Health m_health;

    [Header("Damage")]
    [SerializeField] private bool m_dealDamage;
    [SerializeField] private int m_damageAmount;
    [SerializeField] private int m_damageRadius;

    [Header("Pushing")]
    [SerializeField] private bool m_pushable;

    [Header("Hazards")]
    [SerializeField] private bool m_createHazard;
    [SerializeField] private Hazard m_hazardType;
    [SerializeField] private int m_hazardRadius = 1;
    [SerializeField] private int m_hazardDuration;

    override public void Awake()
    {
        base.Awake();

        // Initialize health
        m_health = new Health(m_maxHealth);
        m_health.OnHealthZero += OnDeath;        
    }

    private void Start()
    {
        if (!m_destroyOnTimer)
            return;

        BattleManager.Instance.OnRoundStart += TimerCopuntdown;
    }


    public override void TakeDamage(int damage)
    {
        if (!m_damagable) return;
        m_health.Damage(damage);
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

    public void OnDeath()
    {
        if (m_createHazard)
            CreateHazard();

        if (m_dealDamage)
            DealDamage();

            RemoveUnit();
    }

    private void CreateHazard()
    {
        EnvironmentHazard.CreateHazard(m_hazardType, m_hazardDuration, GridPosition);

        List<Vector2Int> surroundingTiles = Grid.Instance.GetSurroundingTiles(GridPosition, m_hazardRadius);

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
        List<Vector2Int> targetTiles = Grid.Instance.GetSurroundingTiles(this.GridPosition, m_damageRadius);

        foreach(Vector2Int tile in targetTiles)
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

        if(m_timer <= 0)
        {
            OnDeath();
        }
    }
}
