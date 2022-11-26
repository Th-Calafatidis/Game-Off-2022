// /* --------------------
// -----------------------
// Creation date: 06/11/2022
// Author: Alex
// Description: The battle manager will be responsible for driving the different states of the battle.
// -----------------------
// ------------------- */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BattleManager : MonoBehaviour
{
    public static BattleManager Instance { get; private set; }

    // The current state of the battle.
    private BattleState m_state;

    // Instance of a turn queue in order to track what order of actions.
    private TurnQueue m_turnQueue;

    // Because we want to be able to add actions with different priority based on speed,
    // we store all actions in a buffer that will be added to turn queue when starting enemy turn.
    private List<QueueBufferItem> m_turnQueueBuffer;

    // Track the player for easy access to methods and variables.
    private Player m_player;

    // For displaying updates about the battle, we use the announcement text
    private AnnouncementText m_announcement;

    // Events
    public Action OnBattleStart;
    public Action OnRoundStart;
    public Action OnRoundEnd;
    public Action OnPlayerTurnStart;
    public Action OnPlayerTurnEnd;
    public Action OnHostilesTurnStart;
    public Action OnHostilesTurnEnd;

    private void Awake()
    {
        // Set singleton instance and throw error if it already exists.
        if (Instance == null) Instance = this;
        else Debug.LogError("There is more than one BattleManager in the scene!");

        // Setup what is needed
        m_turnQueue = new TurnQueue();
        m_turnQueue.Initialize();
        m_state = BattleState.Player;
        m_turnQueueBuffer = new List<QueueBufferItem>();
        m_announcement = GameObject.Find("Announcement").GetComponent<AnnouncementText>();
    }

    private void Start()
    {
        // Battle should start after a small delay. This delay is here to ensure that all scripts
        // have finished initalizing.
        //this.Invoke(StartBattle, 0.05f);
        StartCoroutine(StartBattle());
    }

    #region Combat

    /// <summary>
    /// The first step when starting a new battle. Lets enemies move before starting first round.
    /// </summary>
    public IEnumerator StartBattle()
    {
        m_announcement.Announce("Game start", 3);

        // Just to make sure that everything is properly initialized and registered, we wait one frame.
        yield return new WaitForSeconds(3f);

        // Call event to signal that the battle has started.
        OnBattleStart?.Invoke();

        // We find the reference to the player here, because attempting to find the player
        // in awake will not always work, because of script run order.
        m_player = (Player)Grid.Instance.GetUnitsOfType<Player>()[0];

        // Now we allow all enemies to move before the round starts.
        yield return StartCoroutine(HandleEnemyMovement());

        // Finally, start the first round.
        StartRound();
    }

    /// <summary>
    /// At the start of each round, hazards will be updated, then enemies will determine their actions.
    /// Finally, player turn starts.
    /// </summary>
    private void StartRound()
    {
        // Call the event to signal that the round has started.
        OnRoundStart?.Invoke();

        // Update hazards.
        Grid.Instance.GetAllHazards().ForEach(hazard => hazard.UpdateHazard());

        // Determine enemy actions.
        DetermineEnemyActions();

        // Finally we can start the player's turn.
        StartPlayerTurn();
    }

    /// <summary>
    /// Logic for end of round.
    /// TODO: Check for win condition.
    /// TODO: Add maybe some wait here?
    /// </summary>
    private void EndRound()
    {
        // Call event then start new round.
        OnRoundEnd?.Invoke();

        // Here we check for the win condition, which should just be if there are no more enemies.
        // Might add some more logic here later.
        if (GetEnemyCount() == 0)
        {
            // TEMPORARY: Load next scene. this should be changed later
            m_announcement.Announce("Level complete", 3f);
            this.Invoke(Utility.LoadNextScene, 3f);
        }

        else
        {
            StartRound();
        }
    }

    #endregion

    #region Queue

    /// <summary>
    /// Adds an action to the queue. Will be used during enemy turns.
    /// </summary>
    /// <param name="action">The action to add</param>
    public void AddActionToQueue(Unit performer, ICombatAction action, int priority)
    {
        // Create new turn buffer item and add it.
        QueueBufferItem item = new QueueBufferItem(performer, priority, action);
        m_turnQueueBuffer.Add(item);
    }

    /// <summary>
    /// Removes an action from the queue.
    /// </summary>
    /// <param name="action">The action to remove.</param>
    public void RemoveActionFromQueue(ICombatAction action)
    {
        // Find the item with the action and remove it.
        QueueBufferItem item = m_turnQueueBuffer.Find(x => x.Action == action);
        m_turnQueueBuffer.Remove(item);
    }

    #endregion

    #region Turns

    /// <summary>
    /// Starts the player turn.
    /// </summary>
    public void StartPlayerTurn()
    {
        m_announcement.Announce("Player turn", 3);

        // Call event to signal that the player turn has started.
        OnPlayerTurnStart?.Invoke();

        // Start of turn effects.
        m_player.OnTurnStart();

        m_player.RestoreAP();      
        m_state = BattleState.Player;        
    }

    /// <summary>
    /// Should be called once the player is done with turn. Will call any on turn end logic,
    /// then start the hostiles turn.
    /// </summary>
    public void EndPlayerTurn()
    {
        // Make sure we are not already in the enemy turn.
        if (m_state == BattleState.Enemy) return;

        // Call the event to signal that the player turn has ended.
        OnPlayerTurnEnd?.Invoke();

        // Apply effects for end of player turn
        m_player.OnTurnEnd();

        // Start the hostiles turn.
        StartHostilesTurn();
    }

    /// <summary>
    /// We now start the overarching turn for all enemies, which is referred to as hostiles as a group.
    /// </summary>
    public void StartHostilesTurn()
    {
        // Make sure there are any enemies left.
        if (GetEnemyCount() == 0)
            EndRound();

        // Call the event to signal that the hostiles turn has started.
        OnHostilesTurnStart?.Invoke();        

        // All actions in the buffer will be added into the queue based
        // on their priority.
        BufferToQueue();       
        
        // Now all the actions that is in the queue can be executed.
        StartCoroutine(PerformEnemyActionTurns());
    }      

    /// <summary>
    /// Coroutine that will perform all the actions for the enemies.
    /// </summary>
    private IEnumerator PerformEnemyActionTurns()
    {
        m_announcement.Announce("Enemy turn", 3);
        yield return new WaitForSeconds(1);

        // Now it is time to perform all actions that is in the queue.
        // This will be any actions that is not moving, as that will happen
        // later in this coroutine.
        while (m_turnQueue.Count > 0)
        {
            TurnQueueResult next = m_turnQueue.GetNext();

            // Key of next is the sender, so we can get the unit from there. We also
            // have to make sure that the enemy is still alive.
            if (next.Unit == null) continue;
            next.Unit.OnTurnStart();

            // Retrieve the action and execute it.
            ICombatAction action = next.Action;
            yield return action.Execute();

            yield return new WaitForSeconds(1);
        }

        // Since we now have determined if the enemy should move, we can now
        // loop through turn queue again, as units might want to move.
        yield return HandleEnemyMovement();

        // Call event since hostiles turn has ended.
        OnHostilesTurnEnd?.Invoke();

        // When all enemies have moved, end of round is called.
        EndRound();
    }

    /// <summary>
    /// Moves all actions in the buffer into the queue based on their priority
    /// </summary>
    private void BufferToQueue()
    {
        m_turnQueueBuffer.Sort((a, b) => a.Priority.CompareTo(b.Priority));
        foreach (QueueBufferItem item in m_turnQueueBuffer)
        {
            m_turnQueue.AddAction(item.Owner, item.Action);
        }
        m_turnQueueBuffer.Clear();
    }

    private void DetermineEnemyActions()
    {
        // Since all units are already registered in the grid, we can loop through
        // the registry and get all enemies.
        foreach (Enemy enemy in Grid.Instance.GetUnitsOfType<Enemy>())
        {
            enemy.DetermineAction();
        }
    }

    /// <summary>
    /// Gets all desired actions for movement from enemies and perform the actions.
    /// </summary>
    /// <returns></returns>
    private IEnumerator HandleEnemyMovement()
    {
        // Collect list of all enemies alive on grid.
        List<Enemy> enemies = new List<Enemy>();
        foreach (Enemy enemy in Grid.Instance.GetUnitsOfType<Enemy>())
        {
            enemies.Add(enemy);
        }
    
        // This is a very hacky solution to fix the problem of enemies deciding to move into
        // the same position. Essentially, instead of every enemy deciding where to move and then
        // move one by one, each enemy decides where to move and then moves instantly.
        enemies.Sort((a, b) => a.Priority.CompareTo(b.Priority));
        foreach (Enemy enemy in enemies)
        {
            enemy.DetermineMove();
            BufferToQueue();
            while (m_turnQueue.Count > 0)
            {
                ICombatAction action = m_turnQueue.GetNext().Action;
                yield return action.Execute();
                
            }

            // After having moved, we can finally call onturnend
            enemy.OnTurnEnd();
        }
    }

    #endregion

    private int GetEnemyCount()
    {
        return Grid.Instance.GetUnitsOfType<Enemy>().Count;
    }

}
