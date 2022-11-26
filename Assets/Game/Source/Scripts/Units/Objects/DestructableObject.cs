// /* --------------------
// -----------------------
// Creation date: 14/11/2022
// Author: Alex
// Description: This is an object that can be pushed and damaged, as well as being destroyed. Has the option to create hazards.
// -----------------------
// ------------------- */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructableObject : Unit, IPushable
{
    [Header("Health")]
    [SerializeField] private bool m_damagable;
    [SerializeField] private int m_maxHealth;
    private Health m_health;

    [Header("Pushing")]
    [SerializeField] private bool m_pushable;

    [Header("Hazards")]
    [SerializeField] private bool m_createHazard;
    [SerializeField] private Hazard m_hazardType;
    [SerializeField] private int m_hazardDuration;

    override public void Awake()
    {
        base.Awake();

        // Initialize health
        m_health = new Health(m_maxHealth);
        m_health.OnHealthZero += OnDeath;
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

    private void OnDeath()
    {
        if (m_createHazard)
            CreateHazard();
        RemoveUnit();
    }

    private void CreateHazard()
    {
        EnvironmentHazard.CreateHazard(m_hazardType, m_hazardDuration, GridPosition);

        // Theo: Added this code here to create a hazard in all adjacent tiles too.
        List<Vector2Int> surroundingTiles = Grid.Instance.GetSurroundingTiles(GridPosition);

        foreach (Vector2Int tile in surroundingTiles)
        {
            EnvironmentHazard.CreateHazard(m_hazardType, m_hazardDuration, tile);
        }

        
    }
}
