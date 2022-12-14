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

public class DestructableObjectEnemy : Enemy, IPushable
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

    [Header("Sound Effects")]
    [SerializeField] private AudioClip m_explosionSound;

    [Header("Particles")]
    [SerializeField] private GameObject m_deathParticles;
    [SerializeField] private GameObject m_textParticles;


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
        m_timerObject.SetActive(false);


        if (m_enemy.DestroyDelay == 0) m_enemy.DestroyDelay = 0.01f;
    }

    protected override void Start()
    {
        base.Start();

        SetLine("default");

        surroundingTiles = Grid.Instance.GetSurroundingTiles(GridPosition, Range);

        List<Vector2Int> highlights = new List<Vector2Int>();

        foreach (Vector2Int tile in surroundingTiles)
        {
            highlights.Add(tile);

            if (!Grid.Instance.IsInBounds(tile) || (Grid.Instance.GetNodeAt(tile.x, tile.y).IsObstructed && !Grid.Instance.GetUnitAt(tile)))
            {
                highlights.Remove(tile);
            }
        }

        m_enemy.CreateHighlight(highlights, Color.red);

        if (m_destroyOnTimer)
        {
            m_timerObject.SetActive(true);
            m_timerObject.GetComponentInChildren<TMP_Text>().text = m_timer.ToString();
            BattleManager.Instance.OnRoundStart += TimerCopuntdown;
        }
                    
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


        Instantiate(m_deathParticles, this.transform.position, Quaternion.identity);
        Instantiate(m_textParticles, new Vector3(transform.position.x, 2f, transform.position.y), Quaternion.identity);

        PlaySound(m_explosionSound);

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
            var unit = Grid.Instance.GetUnitAt(tile);

            var node = Grid.Instance.GetNodeAt(tile.x, tile.y);

            if (node.IsObstructed)
            {
                if (unit != null) node.IsObstructed = false;
            }

            if (!node.IsObstructed)
            {
                EnvironmentHazard.CreateHazard(m_hazardType, m_hazardDuration, tile);
            }
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
