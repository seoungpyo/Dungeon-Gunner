using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Settings
{
    #region UNITS
    public const float pixelsPerUnit = 16f;
    public const float tileSizePixels = 16f;
    #endregion UNITS

    #region DUNGEON BUILD SETTINGS
    public const int maxDungeonRebuildAttemptsForRoomGraph = 1000;
    public const int maxDungeonBuildAttempts = 10;
    #endregion DUNGEON BUILD SETTINGS

    #region Room SETTINGS
    public const float fadeInTime = 0.5f; // time to fade in the room
    public const int maxChildCorridors = 3;
    //Max number of child corridors leading from a room - maximum should be 3 although this is not recommended
    //since it can cause the dungeon building to fail since the room are more likely to not fit together
    public const float doorUnlockDelay = 1f;
    #endregion Room SETTINGS

    #region ANIMATION PARAMETERS
    // animator parameters - player
    public static int aimUp = Animator.StringToHash("aimUp");
    public static int aimDown = Animator.StringToHash("aimDown");
    public static int aimUpRight = Animator.StringToHash("aimUpRight");
    public static int aimUpLeft = Animator.StringToHash("aimUpLeft");
    public static int aimRight = Animator.StringToHash("aimRight");
    public static int aimLeft = Animator.StringToHash("aimLeft");
    public static int isIdle = Animator.StringToHash("isIdle");
    public static int isMoving = Animator.StringToHash("isMoving");
    public static int rollUp = Animator.StringToHash("rollUp");
    public static int rollLeft = Animator.StringToHash("rollLeft");
    public static int rollRight = Animator.StringToHash("rollRight");
    public static int rollDown = Animator.StringToHash("rollDown");

    public static float baseSpeedForPlayerAnimatons = 8f;
    public static float baseSpeedForEnemyAnimation = 3f;

    // animator parameters - door
    public static int open = Animator.StringToHash("open");
    #endregion ANIMATION PARAMETERS

    #region GAMEOBJECT TAGS
    public const string playerTag = "Player";
    public const string playerWeapon = "playerWeapon";
    #endregion GAMEOBJECT TAGS

    #region FIRING CONTROL
    public const float useAimAngleDistance = 3.5f;
    // if the target distance is less than this then the aim angle will be used (calculated from player)
    // else the weapon aim angle will be used (calculated from the weapon shoot position).
    #endregion FIRING CONTROL

    #region ASTAR PATHFINDING PARAMETERS
    public const int defaultAStarMovementPenalty = 40;
    public const int preferredPathAStarMovementPenalty = 1;
    public const int targetFrameRateToSpreadPathfindingOver = 60;
    public const float playerMoveDistanceToRebuildPath = 3f;
    public const float enemyPathRebuildCooldown = 2f;
    #endregion ASTAR PATHFINDING PARAMETERS

    #region ENEMY PARAMETERS
    public const int defaultEnemyHealth = 20;
    #endregion ENEMY PARAMETERS

    #region UI PARAMETERS
    public const float uiHeartSpacing = 16f;
    public const float uiAmmoIconSpacing = 4f;
    #endregion UI PARAMETERS
}
