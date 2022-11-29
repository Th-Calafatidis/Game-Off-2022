// /* --------------------
// -----------------------
// Creation date: 05/11/2022
// Author: Alex
// Description: The base for any enemy in the game. For simplicty sake since we don't have time for very complicated
//              enemy setup, each enemy will have it's own class that inherits from this one.
// Edited: Theodore on 27/11/2022
// Changes: Changed Highligts list into a public getter to be able to access those via UIManager script.
// -----------------------
// ------------------- */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : Entity, IPushable
{
    [Header("Generic")]
    [SerializeField] private string m_displayName;
    [SerializeField] private int m_actionPriority;
    [SerializeField] private float m_destroyDelay;

    [Header("Lines")]
    [SerializeField] private List<ActionLine> m_lines;
    
    public List<GameObject> Highlights { get; private set; }

    // Track the intended action, for automatic removal upon death
    private ICombatAction m_intendedAction;

    // Current line based on action
    private ActionLine m_currentLine;

    private EnemyDisplay m_display;
    protected AudioSource m_audioSource;

    public ICombatAction IntendedAction
    {
        get { return m_intendedAction; }
    }

    public int Priority
    {
        get { return m_actionPriority; }
    }

    public override void InitializeHealth()
    {
        m_health = new Health(m_maxHealth, m_startingHealth);
        m_health.OnHealthZero += OnDeath;
    }

    public override void TakeDamage(int damage)
    {
        // If the enemy has the protected status effect, redirect damage to the protector instead.
        ProtectedStatusEffect protect = (ProtectedStatusEffect)GetStatusEffect<ProtectedStatusEffect>();
        if (protect != null)
        {
            // Make sure protector isn't dead.
            if (protect.Protector == null)
            {
                RemoveStatusEffect(protect);
            }

            else
            {
                protect.Protector.TakeDamage(damage);
                return;
            }
        }

        m_health.Damage(damage);
    }

    /// <summary>
    /// The method that will get called when deciding on what action should be taken by the enemy.
    /// Should be overridden by each enemy type to decide what actions to use.
    /// </summary>
    public abstract void DetermineAction();
    public abstract void DetermineMove();

    public void OnMouseEnter()
    {
        if (m_display == null)
        {
            m_display = GameObject.Find("Enemy Display").GetComponent<EnemyDisplay>();
        }
        m_display.SetEnemy(this);
        m_display.Show();

        if (Highlights != null)
            ShowHighlights();
    }

    private void OnMouseExit()
    {
        if (Highlights != null)
            HideHighlights();

        // It is impossible to trigger this event without first calling OnMouseEnter, so we
        // don't need to check if m_display is null.
        m_display.Hide();
    }


    /// <summary>
    /// Sends the action that the enemy will perform to the queue to be executed.
    /// </summary>
    /// <param name="action"></param>
    public void SetAction(ICombatAction action)
    {
        // Add it to the queue for when it's enemies turn to attack.
        BattleManager.Instance.AddActionToQueue(this, action, m_actionPriority);
        m_intendedAction = action;
    }

    /// <summary>
    /// Removes the action from the queue.
    /// </summary>
    /// <param name="action"></param>
    public void RemoveAction(ICombatAction action)
    {
        BattleManager.Instance.RemoveActionFromQueue(action);
        ClearHighlights();
    }

    /// <summary>
    /// Removes the current intended action
    /// </summary>
    public void ClearAction()
    {
        BattleManager.Instance.RemoveActionFromQueue(m_intendedAction);
        ClearHighlights();
    }

    /// <summary>
    /// Method that will be called when the enemy dies. When overriding, it is important
    /// that the base is called AFTER custom logic, unless you want to override removal process.
    /// </summary>
    public virtual void OnDeath()
    {
        // Note that we do not need to clear the highlights here when the enemy dies,
        // because it is handled when removing action.
        ClearAction();

        StartCoroutine(DestroyUnit());
    }

    private IEnumerator DestroyUnit()
    {
        float time = 0f;

        while (time < m_destroyDelay)
        {
            time += Time.deltaTime;

            yield return 0;
        }

        RemoveUnit();
    }

    #region Player

    /// <summary>
    /// Gets the direction from this enemy to the player.
    /// </summary>
    /// <returns>Direction to player</returns>
    public Direction GetDirectionToPlayer()
    {
        Vector2Int currentPosition = Grid.Instance.GetGridPosition(transform.position);
        return Grid.Instance.GetDirectionTo(currentPosition, GetPlayer().GridPosition);
    }

    /// <summary>
    /// Gets a reference to the player on the grid.
    /// </summary>
    /// <returns></returns>
    public Player GetPlayer()
    {
        // There will always just be one player on the board,
        // so we can reliably use index 0.
        return (Player)Grid.Instance.GetUnitsOfType<Player>()[0];
    }

    public bool LineOfSightToPlayer()
    {
        // First we attempt to see the player with Bresenham algorithm.
        Vector2Int player = GetPlayer().GridPosition;
        List<Vector2Int> points = Grid.Instance.BresenhamLine(GridPosition.x, GridPosition.y, player.x, player.y);
        foreach (Vector2Int point in points)
        {
            if (!Grid.Instance.IsTileFree(point))
            {
                if (point != player && point != GridPosition)
                    return false;
            }
        }
      
        return true;
    }


    #endregion

    #region Highlights

    /// <summary>
    /// Creates highlights based on the type of action is performed
    /// </summary>
    /// <param name="action"></param>
    /// <returns></returns>
    public void CreateHighlight(List<Vector2Int> positions, Color color)
    {
        if (Highlights == null) Highlights = new List<GameObject>();

        foreach (Vector2Int position in positions)
        {
            GameObject highlight = Grid.Instance.HighlightTile(position, color);
            highlight.SetActive(false);
            Highlights.Add(highlight);
        }
    }

    /// <summary>
    /// Creates highlights based on the type of action is performed
    /// </summary>
    /// <param name="action"></param>
    /// <returns></returns>
    public void CreateHighlight(Vector2Int position, Color color)
    {
        if (Highlights == null) Highlights = new List<GameObject>();
        GameObject highlight = Grid.Instance.HighlightTile(position, color);
        Highlights.Add(highlight);
    }

    /// <summary>
    /// Destroys all highlight objects
    /// </summary>
    public void ClearHighlights()
    {
        if (Highlights == null) return;

        foreach (GameObject highlight in Highlights)
        {
            Destroy(highlight);
        }
        Highlights = null;
    }

    /// <summary>
    /// Disables all the highlight objects
    /// </summary>
    public void HideHighlights()
    {
        foreach (GameObject highlight in Highlights)
        {
            highlight.SetActive(false);
        }
    }

    /// <summary>
    /// Enables all the highlight objects
    /// </summary>
    public void ShowHighlights()
    {
        foreach (GameObject highlight in Highlights)
        {
            highlight.SetActive(true);
        }
    }

    #endregion

    #region Lines

    /// <summary>
    /// Picks a random line with the corresponding tag at replaces the variable in the line
    /// with the number.
    /// </summary>
    /// <param name="tag"></param>
    /// <param name="number"></param>
    public void SetLine(string tag, int number)
    {
        m_currentLine = GetLineWithTag(tag);
        m_currentLine.SetVariable("num", number);
    }

    /// <summary>
    /// Picks a random line with the corresponding tag.
    /// </summary>
    /// <param name="tag"></param>
    public void SetLine(string tag)
    {
        m_currentLine = GetLineWithTag(tag);
    }

    /// <summary>
    /// Returns the current line in a format for text mesh pro.
    /// </summary>
    /// <returns></returns>
    public string GetActionLine()
    {
        return TextMeshProConvert(m_currentLine.Line);
    }

    public bool HasLine()
    {
        return m_currentLine != null;
    }

    /// <summary>
    /// Converts the input string's color tags to be compatible with rich text function of text mesh pro.
    /// </summary>
    /// <param name="raw">The raw string data.</param>
    /// <returns></returns>
    private string TextMeshProConvert(string raw)
    {
        // Look through the raw string and extract all words that are surrounded by the characters []
        // and replace them with the appropriate color tag.
        string converted = raw;
        string[] words = raw.Split(' ');
        foreach (string word in words)
        {
            if (word.StartsWith("[") && word.EndsWith("]"))
            {
                // Check if it is a start or stop by seeing if there is a slash in the word
                if (word.Contains("/"))
                {
                    converted = converted.Replace(word, "</color>");
                }
                else
                {
                    string color = word.Substring(1, word.Length - 2);
                    converted = converted.Replace(word, $"<color={color}>");
                }
            }
        }

        return converted;
    }

    /// <summary>
    /// Returns a random action line from the list of lines with the matching
    /// tag/
    /// </summary>
    /// <param name="tag">The tag to search for.</param>
    /// <returns></returns>
    private ActionLine GetLineWithTag(string tag)
    {
        // Pick a random ActionLine from list with the matching tag
        List<ActionLine> lines = m_lines.FindAll(x => x.Tag == tag);
        if (lines.Count == 0)
            return null;

        return lines[Random.Range(0, lines.Count)];
    }

    #endregion

    #region Audio

    public void PlaySound(AudioClip clip)
    {
        if (m_audioSource == null) GetAudioSource();
        m_audioSource.PlayOneShot(clip);
    }

    private void GetAudioSource()
    {
        m_audioSource = GetComponent<AudioSource>();
        if (m_audioSource == null)
        {
            Debug.LogError("No audio source found on " + gameObject.name);
        }
    }

    #endregion
}
