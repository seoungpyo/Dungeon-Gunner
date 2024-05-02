using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class GameManager : SingletoneMonobehaviour<GameManager>
{
    #region Header Dungeon Level
    [Space(10)]
    [Header("Dungeon Level")]
    #endregion Header Dungeon Level

    #region Tooltip
    [Tooltip("Populate with the dungeon level scriptable objecets")]
    #endregion Tooltip

    [SerializeField] private List<DungeonLevelSO> dungeonLevelList;

    #region Tooltip
    [Tooltip("Populate with the starting dungeon level for testing, first level = 0")]
    #endregion Tooltip

    [SerializeField] private int currentDungeonLevelListIndex = 0;

    [HideInInspector] public GameState gameState;

    private void Start()
    {
        gameState = GameState.gameStarted;

    }

    private void Update()
    {
        HandleGameState();

        //Dungeon create test code later will delete
        #region Dungeon create test code 
        if (Input.GetKeyDown(KeyCode.R))
        {
            gameState = GameState.gameStarted;
        }
        #endregion Dungeon create test code 
    }

    /// <summary>
    /// Handle game state
    /// </summary>
    private void HandleGameState()
    {
        //Handle game state
        switch (gameState)
        {
            case GameState.gameStarted:
                // play first level
                PlayDungeonLevel(currentDungeonLevelListIndex);

                gameState = GameState.playingLevel;
                break;
        }
    }

    private void PlayDungeonLevel(int currentDungeonLevelListIndex)
    {

    }

    #region Validation
#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilitie.VaildateCheckEnumerableValues(this, nameof(dungeonLevelList), dungeonLevelList);
    }
#endif
    #endregion Validation
}
