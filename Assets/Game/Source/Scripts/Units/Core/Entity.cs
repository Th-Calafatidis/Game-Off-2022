// /* --------------------
// -----------------------
// Creation date: 04/11/2022
// Author: Alex
// Description: An entity is anything that is considered "alive" in the game. So, this is for player and enemy units.
//              By default, all entities are pushable and damagable, have health and methods for moving around on the grid.
//
//              Note: While all entities will have a health instance, it will not be instanciated by default. This is because
//              entities like the enemies will set up their health parameters a different way than say the player.
// -----------------------
// ------------------- */

using Sirenix.Serialization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Entity : Unit, IPushable
{
    // Learning note:
    // Protected keyword is used for variables that are only accessible by the class itself and any inheriting classes.
    protected Health m_health;

    // All entities will have animation, so therefore we can include it here.
    protected Animator m_animator;

    [Header("Health")]
    [SerializeField] protected int m_maxHealth;
    [SerializeField] protected int m_startingHealth;

    [Header("Movement")]
    // For enemies, this will be the max amount of tiles they can move.
    // For player, the cost for moving per tile.
    [SerializeField] protected int m_movementAmount;
    [SerializeField] protected int m_physicalMovementSpeed;

    /// <summary>
    /// The health component of the entity
    /// </summary>
    public Health Health
    {
        get { return m_health; }
    }

    public int MovementSpeed
    {
        get { return m_movementAmount; }
        set { m_movementAmount = value; }
    }

    public Animator Animator
    {
        get { return m_animator; }
    }

    public override void Awake()
    {
        base.Awake();
        InitializeHealth();

        // Find the animator component in either this object or children.
        m_animator = GetComponentInChildren<Animator>();
    }

    public override void SetShield(int amount)
    {
        m_health.SetShield(amount);
    }

    /// <summary>
    /// Turns the gameobject to face the given direction
    /// </summary>
    /// <param name="direction"></param>
    public void FaceDirection(Direction direction)
    {
        transform.forward = Grid.Instance.DirectionToWorldSpace(direction);
    }

    public abstract void InitializeHealth();
}
