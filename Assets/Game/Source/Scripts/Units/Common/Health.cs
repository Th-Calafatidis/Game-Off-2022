// /* --------------------
// -----------------------
// Creation date: 04/11/2022
// Author: Alex
// Description: Simple health class with callbacks on various events.
//              We separate this from the entity class because we might want to have units that are not damagable.
// -----------------------
// ------------------- */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health
{
    #region Public Properties

    /// <summary>
    /// Shield is bonus health that will get added and removed.
    /// </summary>
    public int Shield { get; private set; }

    /// <summary>
    /// The combination of the health and shield.
    /// </summary>
    public int EffectiveHealth
    {
        get { return CurrentHealth + Shield; }
    }

    /// <summary>
    /// The current amount of health left.
    /// </summary>
    public int CurrentHealth { get; private set; }

    /// <summary>
    /// Maximum amount of health possible.
    /// </summary>
    public int MaxHealth { get; private set; }

    // We define various events for the health class.
    public Action OnHealthChanged; // TODO: Maybe pass amount health changed by? So we can react to stuff like big hits.
    public Action OnHealthZero;
    public Action OnDamaged;
    public Action OnHealed;

    #endregion

    #region Constructors

    /// <summary>
    /// Create a new health instance with only max health.
    /// Current health will be set to max.
    /// </summary>
    /// <param name="maxHealth">The max health possible.</param>
    public Health(int maxHealth)
    {
        MaxHealth = maxHealth;
        CurrentHealth = MaxHealth;
    }

    /// <summary>
    /// Creates a new health instance with custom starting health.
    /// </summary>
    /// <param name="maxHealth">Max health possible.</param>
    /// <param name="currentHealth">Current health on creation.</param>
    public Health(int maxHealth, int currentHealth)
    {
        MaxHealth = maxHealth;
        CurrentHealth = currentHealth;
    }

    #endregion

    #region Health

    /// <summary>
    /// Removes health.
    /// </summary>
    /// <param name="amount">The amount to remove.</param>
    public void Damage(int amount)
    {
        ModifyHealth(-amount);
        OnDamaged?.Invoke();
    }

    /// <summary>
    /// Adds health.
    /// </summary>
    /// <param name="amount">The amount to add</param>
    public void Heal(int amount)
    {
        ModifyHealth(amount);
        OnHealed?.Invoke();
    }

    /// <summary>
    /// Instantly sets the health value to 0.
    /// </summary>
    public void Kill()
    {
        ModifyHealth(-EffectiveHealth);
    }

    /// <summary>
    /// Sets the health back to max health.
    /// </summary>
    public void Restore()
    {
        ModifyHealth(MaxHealth);
    }

    #endregion

    #region Shield

    /// <summary>
    /// Sets the shield value directly
    /// </summary>
    /// <param name="amount"></param>
    public void SetShield(int amount)
    {
        Shield = amount;
        OnHealthChanged?.Invoke();
    }

    /// <summary>
    /// Adds shield to the health.
    /// </summary>
    /// <param name="amount">Shielding amount</param>
    public void AddShield(int amount)
    {
        Shield += amount;
        OnHealthChanged?.Invoke();
    }

    /// <summary>
    /// Removes shield from the health.
    /// </summary>
    /// <param name="amount">Amount to remove</param>
    public void RemoveShield(int amount)
    {
        Shield -= amount;
        OnHealthChanged?.Invoke();
    }

    /// <summary>
    /// Removes shield completely.
    /// </summary>
    public void ClearShield()
    {
        Shield = 0;
        OnHealthChanged?.Invoke();
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Updates the health value and calls events.
    /// </summary>
    /// <param name="change">Amount to change by</param>
    private void ModifyHealth(int change)
    {
        int originalHealth = EffectiveHealth;

        // If the change is negative, shields will be prioritized before health.
        // If the change is positive, we only want to increase health and not shield.
        if (change < 0)
        {
            // If the change is negative, we want to remove from the shield first.
            // If the shield is not enough to cover the change, we want to remove the rest from health.
            if (Shield > 0)
            {
                Shield = Mathf.Max(0, Shield + change);
                change = Mathf.Min(0, change + Shield);
            }

            // If the change is still negative, we want to remove from health.
            if (change < 0)
            {
                CurrentHealth = Mathf.Max(0, CurrentHealth + change);
            }
        }
        else
        {
            // If the change is positive, we only want to add to health.
            CurrentHealth = Mathf.Min(MaxHealth, CurrentHealth + change);
        }

        // Now we can compare if the value has changed and call event
        if (EffectiveHealth != originalHealth)
        {
            OnHealthChanged?.Invoke();
        }

        // Call event in case we want to do something when the health reaches 0.
        if (CurrentHealth == 0) OnHealthZero?.Invoke();
    }

    #endregion
}
