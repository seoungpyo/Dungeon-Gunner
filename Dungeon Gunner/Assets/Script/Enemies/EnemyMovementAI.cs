using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Enemy))]
[DisallowMultipleComponent]
public class EnemyMovementAI : MonoBehaviour
{
    [SerializeField] private MovementDetailsSO movementDetails;
    private Enemy enemy;
    private Stack<Vector3> movementSteps = new Stack<Vector3>();
    private Vector3 playerReferencePosition;
    private Coroutine moveEnemyRoutine;
    private float currentEnemyPathRebuildCooldown;
    private WaitForFixedUpdate waitForFixedUpdate;
    [HideInInspector] public float moveSpeed;
    [HideInInspector] public int updateFrameNumber = 1; // default value
    private bool chasePlayer = false;

    private void Awake()
    {
        enemy = GetComponent<Enemy>();

        moveSpeed = movementDetails.GetMoveSpeed();
    }

    private void Start()
    {
        waitForFixedUpdate = new WaitForFixedUpdate();

        // reset player reference position
        playerReferencePosition = GameManager.Instance.GetPlayer().GetPlayerPosition();
    }

    private void Update()
    {
        MovementEnemy();
    }

    /// <summary>
    /// Use Astar pathfinding to build a path to the player - and then move the enemy to each grid localtion on the path
    /// </summary>
    private void MovementEnemy()
    {
        currentEnemyPathRebuildCooldown -= Time.deltaTime;

        if(!chasePlayer && Vector3.Distance(transform.position, GameManager.Instance.GetPlayer().GetPlayerPosition()) < enemy.enemyDetails.chaseDistance)
        {
            chasePlayer = true;
        }

        if (!chasePlayer) return;

        // only process a star path rebuild on certain frames to spread the load between enemies
        if (Time.frameCount % Settings.targetFrameRateToSpreadPathfindingOver != updateFrameNumber) return;

        if(currentEnemyPathRebuildCooldown <=0f || Vector3.Distance(playerReferencePosition, GameManager.Instance.GetPlayer().GetPlayerPosition()) >
            Settings.playerMoveDistanceToRebuildPath)
        {
            currentEnemyPathRebuildCooldown = Settings.enemyPathRebuildCooldown;

            playerReferencePosition = GameManager.Instance.GetPlayer().GetPlayerPosition();

            CreatePath();
        
            if(movementSteps != null)
            {
                if (moveEnemyRoutine != null)
                {
                    enemy.idleEvent.CallIdleEvent();
                    StopCoroutine(moveEnemyRoutine);
                }

                moveEnemyRoutine = StartCoroutine(MoveEnemyRoutine(movementSteps));
            }
        }
    }

    private IEnumerator MoveEnemyRoutine(Stack<Vector3> movementSteps)
    {
        while(movementSteps.Count > 0)
        {
            Vector3 nextPosition = movementSteps.Pop();

            while (Vector3.Distance(nextPosition, transform.position) > 0.2f)
            {
                enemy.movementToPositionEvent.CallMovementToPostionEvent(nextPosition, transform.position, moveSpeed, (nextPosition -
                    transform.position).normalized);

                yield return waitForFixedUpdate;
            }

            yield return waitForFixedUpdate;
        }

        enemy.idleEvent.CallIdleEvent();
    }

    /// <summary>
    /// Use the AStar static class to create a path for the enemy
    /// </summary>
    private void CreatePath()
    {
        Room currentRoom = GameManager.Instance.GetCurrentRoom();

        Grid grid = currentRoom.instantiatedRoom.grid;

        Vector3Int playerGridPostion = GetNearesNonObstaclePlayerPosition(currentRoom);
        Vector3Int enemyGridPostion = grid.WorldToCell(transform.position);

        movementSteps = Astar.BuildPath(currentRoom, enemyGridPostion, playerGridPostion);

        if(movementSteps != null)
        {
            movementSteps.Pop();
        }
        else
        {
            enemy.idleEvent.CallIdleEvent();
        }
    }

    /// <summary>
    /// Set the frame number that the enemy path will be recalculated on - to avoid performance spikes
    /// </summary>
    /// <param name="updateFrameNumber"></param>
    public void SetUpdateFrameNumber(int updateFrameNumber)
    {
        this.updateFrameNumber = updateFrameNumber;
    }

    /// <summary>
    /// Get the nearset position to the player that isn't on an obstacle
    /// </summary>
    /// <param name="currentRoom"></param>
    /// <returns></returns>
    private Vector3Int GetNearesNonObstaclePlayerPosition(Room currentRoom)
    {
        Vector3 playerPostion = GameManager.Instance.GetPlayer().GetPlayerPosition();

        Vector3Int playerCellPosition = currentRoom.instantiatedRoom.grid.WorldToCell(playerPostion);

        Vector2Int adjustedPlayerCellPosition = new Vector2Int(playerCellPosition.x - currentRoom.templateLowerBounds.x,
            playerCellPosition.y - currentRoom.templateLowerBounds.y);

        int obstacle = currentRoom.instantiatedRoom.aStarMovementPenalty[adjustedPlayerCellPosition.x, adjustedPlayerCellPosition.y];
        
        // if the player isn't on a cell square marked as an obstacle then return that position
        if(obstacle != 0)
        {
            return playerCellPosition;
        }
        // find a surrounding cell that isn't an obstacle - required because with the 'half collision' tiles
        // the player can be on a grid square that is marked as an obstacle
        else
        {
            for(int i = -1; i <=1; i++)
            {
                for(int j = -1; j<=1; j++)
                {
                    if (j == 0 && i == 0) continue;

                    try
                    {
                        obstacle = currentRoom.instantiatedRoom.aStarMovementPenalty[adjustedPlayerCellPosition.x + obstacle + i, adjustedPlayerCellPosition.y + j];
                        if (obstacle != 0) return new Vector3Int(playerCellPosition.x + i, playerCellPosition.y + j, 0);
                    }
                    catch
                    {
                        continue;
                    }
                }
            }

            // no non-obstacle cells surrounding the player so just return the player position
            return playerCellPosition;
        }
    }

    #region Validation
#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilitie.ValidateCheckNullValue(this, nameof(movementDetails), movementDetails);
    }
#endif
    #endregion Validation
}
