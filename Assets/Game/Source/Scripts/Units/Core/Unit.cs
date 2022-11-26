// /* --------------------
// -----------------------
// Creation date: 04/11/2022
// Author: Alex
// Description: A unit in the context of this system is anything physical on the map that is not static.
//              For an example, a wall is not going to move or be interacted with, so it is considered static and not a unit.
//              A unit can be a player, an enemy, something placed down, destructible objects etc.
//
//              This is the base for any unit in the game. It contains the basic functionality for movement.
// -----------------------
// ------------------- */

using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Unit : MonoBehaviour, IDamagable
{
    private List<StatusEffect> m_statusEffects;

    public virtual void Awake()
    {
        // Initialize status effect list
        m_statusEffects = new List<StatusEffect>();
    }

    public virtual void OnEnable()
    {
        
    }

    private void Start()
    {
        // We need to register this unit into the grid registry once it is created.
        // It is not done in awake, as we need to make sure grid is already initialized.
        Grid.Instance.RegisterUnit(this);
    }

    /// <summary>
    /// Gets the current position of the unit on the grid
    /// </summary>
    public Vector2Int GridPosition
    {
        get { return Grid.Instance.GetGridPosition(transform.position); }
    }

    public virtual void TakeDamage(int damage) { }

    /// <summary>
    /// Empty method meant for overriding by inheriting classes for adding shields.
    /// </summary>
    /// <param name="amount"></param>
    public virtual void SetShield(int amount) { }

    /// <summary>
    /// Removes the unit from the scene. Should be called instead of Destroy().
    /// </summary>
    public void RemoveUnit()
    {
        Grid.Instance.UnregisterUnit(this);     
        Grid.Instance.DelayedBake();
        Destroy(gameObject);
    }

    /// <summary>
    /// Sets the position of the entity directly on the grid.
    /// </summary>
    /// <param name="x">The x position on grid.</param>
    /// <param name="y">The y position on grid.</param>
    public virtual void SetPosition(int x, int y)
    {
        Vector3 worldPos = Grid.Instance.GetWorldPosition(x, y);
        transform.position = worldPos;

        // After completion, we have to rebake the grid.
        Grid.Instance.UpdateUnit(this);
    }

    /// <summary>
    /// Pushes the unit in a direction, if possible.
    /// </summary>
    public virtual void Push(Direction direction, int distance)
    {
        IPushable pushable = GetComponent<IPushable>();
        if (pushable != null)
        {
            StartCoroutine(PushCoroutine(direction, distance));
        }
    }

    private IEnumerator PushCoroutine(Direction direction, int distance)
    {
        // First we need to determine if all tiles along the push direction are empty.
        // We count all tiles available from current location to where we will end up.
        // If there are less tiles available, we move to the furthest tile not blocked,
        // and deal damage based on the difference between the two.
        int possibleDistance = 0;
        Vector2Int checkPosition = Grid.Instance.GetGridPosition(transform.position);
        Vector2Int finalPosition = checkPosition;
        for (int i = 0; i < distance; i++)
        {
            // Is the next tile unit would move to free?
            Vector2Int next = Grid.Instance.PositionWithDirection(checkPosition, direction);
            if (Grid.Instance.IsTileFree(next))
            {
                possibleDistance += 1;
                finalPosition = next;

                // We move the check distance even further
                checkPosition = next;
            }
        }

        // We now have to get the path to the final position.
        Vector2Int startPosition = Grid.Instance.GetGridPosition(transform.position);
        List<Vector2Int> path = Grid.Instance.GetPath(startPosition, finalPosition);

        // Then we move the unit along the path.
        // TODO: Make the speed value not hardcoded. Not sure where to put it though...
        yield return MoveTo(path, 12);

        // Deal damage to the unit based on untraveled distance.
        // TODO: Pass multiplier. Or other effects.
        int damage = distance - possibleDistance;
        if (damage > 0)
        {
            GetComponent<IDamagable>().TakeDamage(damage);
        }

        OnPushed();
    }

    /// <summary>
    /// Called when the unit has been pushed. Used for when an enemy has an attack planned,
    /// but needs to move it when pushed.
    /// </summary>
    public virtual void OnFinishedMoving() { }

    public virtual void OnPushed() { }

    public IEnumerator MoveTo(List<Vector2Int> targetPosition, float speed)
    {
        while (targetPosition.Count > 0)
        {
            // Unit might die while moving. Check for that and stop if true
            if (gameObject == null)
            {
                yield break;
            }

            // Get the world position of the first target position.
            Vector3 worldPos = Grid.Instance.GetWorldPosition(targetPosition[0].x, targetPosition[0].y);

            // Set the transform's forward direction to the movement direction, so unit always faces
            // the direction it is moving. This is for keeping models facing the right direction.
            Vector3 direction = worldPos - transform.position;
            direction.y = 0;
            transform.forward = direction;

            // Get the distance between the entity and the target position.
            float distance = Vector3.Distance(transform.position, worldPos);

            // While the distance is greater than 0.1f, move towards the target position.
            while (distance != 0)
            {
                transform.position = Vector3.MoveTowards(transform.position, worldPos, speed * Time.deltaTime);
                distance = Vector3.Distance(transform.position, worldPos);
                yield return null;
            }

            // Remove the first position from the list.
            targetPosition.RemoveAt(0);

            // Now check if the unit is on a tile that has an effect.
            CheckForHazard();
        }

        // After completion, we have to update the position in the grid registry.
        // This will automatically rebake the navmesh.
        Grid.Instance.UpdateUnit(this);
        OnFinishedMoving();
    }

    /// <summary>
    /// Here we will check if the current position has a hazard on it. If so, we need to
    /// apply the status effect of that hazard onto the unit. It is done here, because we
    /// want to check after moving to each tile.
    /// </summary>
    private void CheckForHazard()
    {
        EnvironmentHazard hazard = Grid.Instance.GetHazardAt(GridPosition);
        if (hazard != null)
        {
            hazard.ApplyHazard(this);
        }
    }

    #region Status effects

    public virtual void AddStatusEffect(StatusEffect effect)
    {
        // Check if we already have the status effect. If so, remove the current one,
        // and apply a new one.
        if (HasEffect(effect))
        {
            RemoveStatusEffect(effect);
        }

        m_statusEffects.Add(effect);
        effect.OnEffectAdd(this);
    }

    public void RemoveStatusEffect(StatusEffect effect)
    {
        // Remove all status effects from list with same type
        m_statusEffects.RemoveAll(x => x.GetType() == effect.GetType());
        effect.OnEffectRemoved(this);
    }

    public bool HasEffect(StatusEffect effect)
    {
        foreach (StatusEffect e in m_statusEffects)
        {
            if (e.GetType() == effect.GetType())
                return true;
        }

        return false;
    }

    public StatusEffect GetStatusEffect<T>()
    {
        foreach (StatusEffect effect in m_statusEffects)
        {
            if (effect is T)
            {
                return effect;
            }
        }

        return null;
    }

    /// <summary>
    /// Calls the OnStartTurn method in all status effects on this unit.
    /// </summary>
    private void TurnStartEffects()
    {
        // Because the list of effects might be modified by removing the effect while in the loop,
        // we create a copy of the list and iterate through that.
        List<StatusEffect> effects = new List<StatusEffect>(m_statusEffects);

        foreach (StatusEffect effect in effects)
        {
            effect.OnStartTurn(this);
        }
    }

    /// <summary>
    /// Calls the OnEndTurn method in all status effects on this unit.
    /// </summary>
    private void TurnEndEffects()
    {
        // Because the list of effects might be modified by removing the effect while in the loop,
        // we create a copy of the list and iterate through that.
        List<StatusEffect> effects = new List<StatusEffect>(m_statusEffects);

        foreach (StatusEffect effect in effects)
        {
            effect.OnEndTurn(this);
        }
    }

    #endregion

    #region Turns

    /// <summary>
    /// Called when the turn of the unit starts.
    /// </summary>
    public virtual void OnTurnStart()
    {
        TurnStartEffects();
    }

    /// <summary>
    /// Called when the turn of the unit ends.
    /// </summary>
    public virtual void OnTurnEnd()
    {
        TurnEndEffects();
        CheckForHazard();
    }

    #endregion
}
