// /* --------------------
// -----------------------
// Creation date: 15/11/2022
// Author: Alex
// Description: This enemy will try to snipe the player when in line of sight. This is different from other enemies,
//              as the player
// -----------------------
// ------------------- */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SniperEnemy : Enemy
{
    [Header("Snipe")]
    [SerializeField] private int m_recharge;
    [SerializeField] private int m_maxRange;
    [SerializeField] private int m_damage;

    [Header("Audio")]
    [SerializeField] private AudioClip m_lockOnSound;
    [SerializeField] private AudioClip m_shootSound;

    public int MaxRange { get { return m_maxRange; } }

    public override void DetermineAction()
    {
        // Ignore unless within range
        if (Grid.Instance.GetDistanceBetweenUnits(this, GetPlayer()) > m_maxRange)
        {
            return;
        }

        // Make sure there is line of sight to the player.
        if (!LineOfSightToPlayer())
        {
            SetLine("no los");
            return;
        }

        // Create the action
        ICombatAction targetedShot = new TargetedShotAction(this, m_damage, () => { PlaySound(m_shootSound); });
        SetAction(targetedShot);
        SetLine("snipe", m_damage);
        Animator.SetBool("isAiming", true);
        PlaySound(m_lockOnSound);
    }

    public override void DetermineMove()
    {

    }

    public override void TakeDamage(int damage)
    {
        base.TakeDamage(damage);

        Animator.SetTrigger("isHit");
    }

    public override void OnDeath()
    {
        base.OnDeath();

        Animator.SetTrigger("isDead");
    }
}
