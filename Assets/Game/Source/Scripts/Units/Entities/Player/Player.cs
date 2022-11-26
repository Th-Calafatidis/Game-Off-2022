// /* --------------------
// -----------------------
// Creation date: 04/11/2022
// Author: Alex
// Description: This is the main class for the player entity. It is responsible for setting up the player's health and other
//              things.
// -----------------------
// ------------------- */

using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Player : Entity, IDamagable
{
    [Header("Speed")]
    [SerializeField] private float m_moveSpeed; // how fast player should move visually

    [Header("Action Points")]
    [SerializeField] private int m_maxActionPoints = 5;
    [SerializeField] private int m_startingActionPoints = 5;
    [SerializeField] private int m_actionPointPoolSize = 3;

    [Header("Melee")]
    [SerializeField] private int m_meleeCost;
    [SerializeField] private int m_meleeRange;
    [SerializeField] private int m_meleeDamage;
    [SerializeField] private AudioClip m_meleeAudio;

    [Header("Mind blast")]
    [SerializeField] private int m_mindBlastCost;
    [SerializeField] private int m_mindBlastPushForce;
    [SerializeField] private float m_mindBlastWindupDelay;
    [SerializeField] private AudioClip m_mindBlastAudio;

    [Header("Blink")]
    [SerializeField] private int m_blinkCost;
    [SerializeField] private int m_blinkDistance;
    [SerializeField] private int m_blinkDamage;
    [SerializeField] private float m_blinkWindupDelay;
    [SerializeField] private AudioClip m_blinkAudio;

    // AP
    private ActionPoints m_actionPoints;

    // Preview
    private PlayerPositionPreview m_preview;

    // Sound
    private AudioSource m_audioSource;

    public ActionPoints ActionPoints
    {
        get { return m_actionPoints; }
    }

    /// <summary>
    /// The cost of moving one tile. 
    /// </summary>
    public int MoveCost
    {
        get { return m_movementAmount; }
    }

    public override void Awake()
    {
        base.Awake();
        InitializeActionPoints();
        m_preview = GetComponentInChildren<PlayerPositionPreview>();


        m_health.OnHealthZero += OnDeath;
        m_audioSource = GetComponent<AudioSource>();
    }

    public override void TakeDamage(int damage)
    {
        m_health.Damage(damage);
    }

    public void RestoreAP()
    {
        m_actionPoints.RestoreActionPoints();
    }

    private void OnDeath()
    {
        // Temporary solution to when player dies. Reloads the current scene.
        this.Invoke(Utility.RestartScene, 3f);
    }

    #region Debugging

    [Button("Slow")]
    public void Slow()
    {
        AddStatusEffect(new SlowStatusEffect(3));
    }

    #endregion

    #region Actions

    /// <summary>
    /// Enables move selection mode, where the player will be able to select a tile to move to.
    /// </summary>
    public void StartMoveSelection()
    {
        if (m_actionPoints.TotalActionPoints == 0)
        {
            return;
        }

        StartCoroutine(MoveSelection());
    }

    /// <summary>
    /// Draws a line from the player to the hovered tile for as long as the player hasn't clicked yet,
    /// or have not pressed escape.
    /// </summary>
    /// <returns></returns>
    private IEnumerator MoveSelection()
    {
        LineRenderer lineRenderer = CreateLineRenderer();

        Vector2Int playerPosition = Grid.Instance.GetGridPosition(transform.position);
        List<Vector2Int> currentPath = new List<Vector2Int>();
        bool isValid = false;
        int distance = 0;

        while (!Input.GetKeyDown(KeyCode.Mouse0) && !Input.GetKeyDown(KeyCode.Escape))
        {
            // First get the grid position that mouse is currently looking at.
            Vector2Int mousePosition = GetCursorGridPosition();

            // Perform pathfinding towards the mouse position.
            currentPath = Grid.Instance.GetPath(playerPosition, mousePosition);

            // A path was found. Make sure that it is within the player's movement range.
            // Each item in path list equals one tile, so using the size is the same as distance
            if (currentPath != null && (currentPath.Count * MoveCost) <= m_actionPoints.TotalActionPoints)
            {
                lineRenderer.positionCount = currentPath.Count + 1;
                lineRenderer.SetPosition(0, Grid.Instance.GetWorldPosition(playerPosition.x, playerPosition.y));
                // Go through each point and set the line renderer between them
                for (int i = 0; i < currentPath.Count; i++)
                {
                    Vector3 point = Grid.Instance.GetWorldPosition(currentPath[i].x, currentPath[i].y);
                    lineRenderer.SetPosition(i + 1, point);
                }

                // Since this is valid, we make the line renderer green
                lineRenderer.startColor = Color.green;
                lineRenderer.endColor = Color.green;
                isValid = true;

                distance = currentPath.Count;

                // Show the preview
                if (currentPath.Count > 0)
                {
                    Vector3 previewPos = Grid.Instance.GetWorldPosition(currentPath[currentPath.Count - 1].x, currentPath[currentPath.Count - 1].y);
                    m_preview.Show(previewPos);
                }
            }

            // No valid path found.
            else
            {
                lineRenderer.startColor = Color.red;
                lineRenderer.endColor = Color.red;
                isValid = false;
                m_preview.Hide();
            }

            yield return 0;
        }

        // If the player clicked and not pressed escape, and the path is valid, move the player.
        if (isValid && !Input.GetKeyDown(KeyCode.Escape))
        {
            // Set animator to running
            Animator.SetBool("moving", true);

            // Clean up linerenderer after
            Destroy(lineRenderer.gameObject);
            m_preview.Hide();

            ICombatAction moveAction = new MoveAction(this, currentPath, m_moveSpeed);
            yield return moveAction.Execute();

            // Subtract the cost of the move from the action points.
            m_actionPoints.SpendActionPoints(distance * MoveCost);

            // Stop running animation
            Animator.SetBool("moving", false);
        }
        
        
        else
        {
            // Clean up linerenderer after
            Destroy(lineRenderer.gameObject);
            m_preview.Hide();
        }
    }

    /// <summary>
    /// Creates a new linerenderer for the move selection.
    /// </summary>
    /// <returns></returns>
    private LineRenderer CreateLineRenderer()
    {
        LineRenderer lineRenderer = new GameObject("LineRenderer").AddComponent<LineRenderer>();
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = Color.white;
        lineRenderer.endColor = Color.white;
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
        lineRenderer.positionCount = 0;

        return lineRenderer;
    }

    public void StartMeleeSelection()
    {
        if (m_actionPoints.TotalActionPoints < m_meleeCost)
        {
            Debug.Log("Not enough action points to perform melee attack.");
            return;
        }

        StartCoroutine(MeleeSelection());
    }

    private IEnumerator MeleeSelection()
    {
        // By default, have no target. We will use null to check
        // if valid target is chosen when the player clicks.
        IDamagable target = null;
        Vector2Int targetPosition = Vector2Int.zero;

        while(!Input.GetKeyDown(KeyCode.Escape))
        {
            // Raycast from cursor to check if whatever cursor is over has IDamagable interface
            RaycastHit hit;
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100))
            {
                // Check if it has IDamagable interface
                IDamagable potentialTarget = hit.collider.GetComponent<IDamagable>();
                if (potentialTarget != null)
                {
                    // We need to make sure that target is within 1 tile.
                    Vector2Int playerPos = Grid.Instance.GetGridPosition(transform.position);
                    Vector2Int targetPos = Grid.Instance.GetGridPosition(hit.point);
                    int distance = Grid.Instance.GetManhattenDistance(playerPos, targetPos);

                    if (distance <= m_meleeRange)
                    {
                        // A valid target.
                        target = potentialTarget;
                        targetPosition = targetPos;
                    }
                }
            }

            // TODO: add red/green indicator on target based on validity
            if (Input.GetKeyDown(KeyCode.Mouse0))
                break;

            yield return null;
        }

        // If we have a valid target, we will attack it.
        if (target != null)
        {
            ICombatAction melee = new SingleDamageAction(targetPosition, m_meleeDamage);
            StartCoroutine(melee.Execute());

            // Play attack animation and face towards enemy
            Direction dir = Grid.Instance.GetDirectionTo(targetPosition, GridPosition);
            FaceDirection(dir);
            Animator.SetTrigger("melee");
            m_audioSource.PlayOneShot(m_meleeAudio);
            

            // Subtract the cost of the move from the action points.
            m_actionPoints.SpendActionPoints(m_meleeCost);
        }
    }

    /// <summary>
    /// Pushes adjecent enemies away.
    /// </summary>
    public void StartMindblast()
    {
        // Make sure we have enough action points to perform this action.
        if (m_actionPoints.TotalActionPoints < m_mindBlastCost)
        {
            Debug.Log("Not enough action points to perform mindblast.");
            return;
        }

        StartCoroutine(PerformMindblast());
    }

    private IEnumerator PerformMindblast()
    {
        m_audioSource.PlayOneShot(m_mindBlastAudio);
        Animator.SetTrigger("mindBlast");
        yield return new WaitForSeconds(m_mindBlastWindupDelay);

        // Check for units above, below, right and left of player
        Vector2Int playerPos = Grid.Instance.GetGridPosition(transform.position);
        Vector2Int[] adjacentPositions = new Vector2Int[4];
        adjacentPositions[0] = Grid.Instance.PositionWithDirection(playerPos, Direction.Right);
        adjacentPositions[1] = Grid.Instance.PositionWithDirection(playerPos, Direction.Down);
        adjacentPositions[2] = Grid.Instance.PositionWithDirection(playerPos, Direction.Left);
        adjacentPositions[3] = Grid.Instance.PositionWithDirection(playerPos, Direction.Up);

        // Check if there is a unit in any of those positions
        for (int i = 0; i < adjacentPositions.Length; i++)
        {
            // Check if there is a unit in those positions
            Unit unit = Grid.Instance.GetUnitAt(adjacentPositions[i]);
            if (unit != null)
            {
                // We find direction and push. Checking how far they should get pushed and if they can
                // get pushed is handled by the unit itself.
                Vector2Int unitPosition = Grid.Instance.GetGridPosition(unit.transform.position);
                Direction pushDirection = Grid.Instance.GetDirectionTo(unitPosition, playerPos);
                unit.Push(pushDirection, m_mindBlastPushForce);
            }
        }

        // Subtract the cost of the move from the action points.
        m_actionPoints.SpendActionPoints(m_mindBlastCost);
    }

    /// <summary>
    /// Performs a blink attack in the given direction
    /// </summary>
    public void StartBlinkAttack()
    {
        // Do we have enough action points to perform this action?
        if (m_actionPoints.TotalActionPoints < m_blinkCost)
        {
            Debug.Log("Not enough action points to perform blink attack.");
            return;
        }

        // For every direction, we need to check if there is anything blocking the path
        // in all tiles in that direction. If there is, we have to blink in front of the
        // blocking object.
        Dictionary<Direction, Vector2Int> blinkPositions = new Dictionary<Direction, Vector2Int>();
        Vector2Int playerPos = Grid.Instance.GetGridPosition(transform.position);
        foreach (Direction dir in Enum.GetValues(typeof(Direction)))
        {
            // Traverse through all tiles in the given direction
            Vector2Int currentPos = playerPos;
            for (int i = 0; i < m_blinkDistance; i++)
            {
                // Is the next tile free?
                Vector2Int nextPos = Grid.Instance.PositionWithDirection(currentPos, dir);
                if (Grid.Instance.IsTileFree(nextPos)) currentPos = nextPos;

                // Stop loop if hit something blocking.
                else break;
            }

            if (currentPos != playerPos)
            {
                // We found a valid position to blink to.
                blinkPositions.Add(dir, currentPos);
            }
        }

        if (blinkPositions.Count > 0)
            StartCoroutine(BlinkSelection(blinkPositions));
    }

    /// <summary>
    /// Creates green highlights at the blink positions and waits until the user
    /// either presses escape or clicks on one of the valid blink tiles.
    /// </summary>
    /// <param name="blinkPositions"></param>
    /// <returns></returns>
    private IEnumerator BlinkSelection(Dictionary<Direction, Vector2Int> blinkPositions)
    {
        Debug.Log(blinkPositions.Values.Count);

        // Start by creating green highlights at all valid blink positions.
        List<GameObject> highlights = new List<GameObject>();        
        foreach (Vector2Int pos in blinkPositions.Values)
        {
            // Cache the highlight for easy removal later
            GameObject highlight = Grid.Instance.HighlightTile(pos, Color.green);
            highlights.Add(highlight);
            highlight.SetActive(true);
        }

        // While loop will only listen for escape input. The only other escape from while loop
        // is clicking on a valid tile, which will break loop.
        Vector2Int chosenPosition = Vector2Int.zero;
        while (!Input.GetKeyDown(KeyCode.Escape))
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                // Is the clicked tile one of the blink positions?
                Vector2Int cursorPos = GetCursorGridPosition();
                if (blinkPositions.ContainsValue(cursorPos))
                {
                    // We found a valid blink position. Break out of while loop.
                    chosenPosition = cursorPos;
                    break;
                }
            }

            yield return 0;
        }

        // Clean up all highlights..
        foreach (GameObject highlight in highlights)
        {
            Destroy(highlight);
        }

        // As long as we didn't manually cancel the action, we will perform the blink.
        if (!Input.GetKeyDown(KeyCode.Escape))
        {
            // Rotate transform forward towards blink direction and start blink animation.
            Direction dir = Grid.Instance.GetDirectionTo(chosenPosition, GridPosition);
            FaceDirection(dir);

            // Play audio
            m_audioSource.PlayOneShot(m_blinkAudio);

            Animator.SetTrigger("blink");
            yield return new WaitForSeconds(Animator.GetCurrentAnimatorClipInfo(0).Length);
            //yield return new WaitForSeconds(m_blinkWindupDelay);

            // Create an instant move action and execute it here.
            ICombatAction blink = new InstantMoveAction(this, chosenPosition);
            StartCoroutine(blink.Execute());

            // Subtract the cost of the move from the action points.
            m_actionPoints.SpendActionPoints(m_blinkCost);

            // For the final part of the attack, if there is an enemy the tile in front of us,
            // they will be dealt damage.
            Direction chosenDirection = blinkPositions.FirstOrDefault(x => x.Value == chosenPosition).Key;
            Vector2Int damagePosition = Grid.Instance.PositionWithDirection(chosenPosition, chosenDirection);
            ICombatAction damage = new SingleDamageAction(damagePosition, m_blinkDamage);
            StartCoroutine(damage.Execute());
        }
    }

    #endregion

    /// <summary>
    /// Gets the current grid position that cursor is at.
    /// </summary>
    /// <returns></returns>
    private Vector2Int GetCursorGridPosition()
    {
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100))
        {
            return Grid.Instance.GetGridPosition(hit.point);
        }

        return Vector2Int.zero;
    }

    /// <summary>
    /// For the player, health will be initialized through variables set in the editor.
    /// </summary>
    public override void InitializeHealth()
    {
        m_health = new Health(m_maxHealth, m_startingHealth);
    }

    private void InitializeActionPoints()
    {
        m_actionPoints = new ActionPoints(m_maxActionPoints, m_startingActionPoints, m_actionPointPoolSize);
    }
}
