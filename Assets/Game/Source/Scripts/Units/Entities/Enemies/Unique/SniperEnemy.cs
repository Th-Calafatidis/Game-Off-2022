// /* --------------------
// -----------------------
// Creation date: 15/11/2022
// Author: Alex
// Description: This enemy will try to snipe the player when in line of sight. This is different from other enemies,
//              as the player
//
// Edited: By Theodore on 24/11/2022
// Added: SFX and Animations, Player Lock Icon mechanic, default ActionLine and other changes.
// -----------------------
// ------------------- */

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
    [SerializeField] private AudioClip m_hitSound;
    [SerializeField] private AudioClip m_deathSound;

    [Header("Particles")]
    [SerializeField] private GameObject m_lockParticles;
    [SerializeField] private GameObject m_shootParticles;
    [SerializeField] private GameObject m_textParticles;
    [SerializeField] private GameObject m_deathParticles;

    public int MaxRange { get { return m_maxRange; } }

    private bool m_actionLocked;
    private GameObject playerLockIcon;

    protected override void Start()
    {
        base.Start();

        SetLine("default");

        playerLockIcon = GetPlayer().LockIcon;

        if(playerLockIcon.activeSelf)
        playerLockIcon.SetActive(false);
    }

    public override void DetermineAction()
    {
        m_actionLocked = false;

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
        ICombatAction targetedShot = new TargetedShotAction(this, m_damage, 
            () => { PlaySound(m_shootSound); transform.LookAt(GetPlayer().transform); Instantiate(m_lockParticles, transform.position, Quaternion.identity); }, 
            () => {Animator.SetBool("isAiming", false); Animator.SetTrigger("isShooting"); 
                Instantiate(m_shootParticles, transform.position, Quaternion.identity); 
                Instantiate(m_textParticles, new Vector3(transform.position.x, 2f, transform.position.y), Quaternion.identity);
            });
        SetAction(targetedShot);

        m_actionLocked = true;

        SetLine("snipe", m_damage);
        Animator.SetBool("isAiming", true);
        PlaySound(m_lockOnSound);
    }

    public override void DetermineMove()
    {
        // Rotate towards the player's position
        transform.LookAt(GetPlayer().transform);
    }

    public override void TakeDamage(int damage)
    {
        base.TakeDamage(damage);

        Animator.SetTrigger("hit");
        PlaySound(m_hitSound);
    }

    public override void OnDeath()
    {
        Instantiate(m_deathParticles, new Vector3(transform.position.x, 1.5f, transform.position.y), Quaternion.identity);

        m_actionLocked = false;
        playerLockIcon.SetActive(false);

        base.OnDeath();

        Animator.SetTrigger("death");
        PlaySound(m_deathSound);

        
    }


    private void Update()
    {
        if (!m_actionLocked)
            return;

        ShowTargetIcon();
    }

    private void ShowTargetIcon()
    {
        if (!m_actionLocked)
        {
            playerLockIcon.SetActive(false);
            return;
        }


        List<Vector2Int> tilesToPlayer = Grid.Instance.BresenhamLine(this.GridPosition.x, this.GridPosition.y,
                                                                     GetPlayer().GridPosition.x, GetPlayer().GridPosition.y);

        for (int i = 1; i < tilesToPlayer.Count; i++)
        {

            // Since a unit counts as occupying a tile, we have to check for that manually first.
            Unit hitUnit = Grid.Instance.GetUnitAt(tilesToPlayer[i]);
            if (hitUnit != null)
            {
                if (hitUnit.CompareTag("Player"))
                {
                    playerLockIcon.SetActive(true);
                }
            }
            else
            {
                playerLockIcon.SetActive(false);
            }
        }

        for (int i = 1; i < tilesToPlayer.Count - 1; i++)
        {
            var node = Grid.Instance.GetNodeAt(tilesToPlayer[i].x, tilesToPlayer[i].y);

            if (node.IsObstructed)
            {
                playerLockIcon.SetActive(false);
            }
        }    
    }
}
