using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonMap : SingletonMonobehaviour<DungeonMap>
{
    #region Header GameObject References
    [Space(10)]
    [Header("GameObject References")]
    #endregion Header GameObject References
    [SerializeField] private GameObject minimapUI;
    private Camera dungeonMapCamera;
    private Camera cameraMain;

    private void Start()
    {
        cameraMain = Camera.main;

        Transform playerTransform = GameManager.Instance.GetPlayer().transform;

        CinemachineVirtualCamera cinemachineVirtualCamera = GetComponentInChildren<CinemachineVirtualCamera>();
        cinemachineVirtualCamera.Follow = playerTransform;

        dungeonMapCamera = GetComponentInChildren<Camera>();
        dungeonMapCamera.gameObject.SetActive(false);
    }

    private void Update()
    {
        if(Input.GetMouseButtonDown(0) && GameManager.Instance.gameState == GameState.dungeonOverviewMap)
        {
            GetRoomClicked();
        }
    }

    /// <summary>
    /// Get the room clicked on the map
    /// </summary>
    private void GetRoomClicked()
    {
        Vector3 worldPosition = dungeonMapCamera.ScreenToWorldPoint(Input.mousePosition);
        worldPosition = new Vector3(worldPosition.x, worldPosition.y, 0);

        // check for collisions at cursor position
        Collider2D[] collider2DArray = Physics2D.OverlapCircleAll(new Vector2(worldPosition.x, worldPosition.y), 1f);

        // check if any of the colliders are a room
        foreach(Collider2D collider2D in collider2DArray)
        {
            if(collider2D.GetComponent<InstantiatedRoom>() != null)
            {
                InstantiatedRoom instantiatedRoom = collider2D.GetComponent<InstantiatedRoom>();

                // If clicked room is clear of enemies and previously visited then move player to the room
                if(instantiatedRoom.room.isClearedOfEnemies && instantiatedRoom.room.isPreviouslyVisited)
                {
                    // move player to room
                    StartCoroutine(MovePlayerToRoom(worldPosition, instantiatedRoom.room));
                }
            }
        }
    }

    /// <summary>
    /// Move the player to the selected room
    /// </summary>
    /// <param name="worldPosition"></param>
    /// <param name="room"></param>
    /// <returns></returns>
    private IEnumerator MovePlayerToRoom(Vector3 worldPosition, Room room)
    {
        StaticEventHandler.CallRoomChangedEvent(room);

        yield return StartCoroutine(GameManager.Instance.Fade(0f, 1f, 0f, Color.black));

        // Clear dungeon overview
        ClearDungeonOverViewMap();

        // disable player during the fade
        GameManager.Instance.GetPlayer().playerControl.DisablePlayer();

        // get nearest spawn point in room nearest to player
        Vector3 spawnPosition = HelperUtilitie.GetSpawnPositionNearestToPlayer(worldPosition);

        // move player to new location - spawning them at the closest spawn point
        GameManager.Instance.GetPlayer().transform.position = spawnPosition;

        yield return StartCoroutine(GameManager.Instance.Fade(1f, 0f, 1f, Color.black));

        GameManager.Instance.GetPlayer().playerControl.EnablePlayer();
    }

    /// <summary>
    /// Display dungeon overview map UI
    /// </summary>
    public void DisplayDungeonOverViewMap()
    {
        // set game state
        GameManager.Instance.previousGameState = GameManager.Instance.gameState;
        GameManager.Instance.gameState = GameState.dungeonOverviewMap;

        GameManager.Instance.GetPlayer().playerControl.DisablePlayer();

        // disable main camera and enable dungeon overview camera
        cameraMain.gameObject.SetActive(false);
        dungeonMapCamera.gameObject.SetActive(true);

        ActivateRoomsForDisplay();

        // disable small minimap UI
        minimapUI.SetActive(false);
    }

    /// <summary>
    /// Clear the dungeon overview UI
    /// </summary>
    public void ClearDungeonOverViewMap()
    {
        GameManager.Instance.gameState = GameManager.Instance.previousGameState;
        GameManager.Instance.previousGameState = GameState.dungeonOverviewMap;

        GameManager.Instance.GetPlayer().playerControl.EnablePlayer();

        cameraMain.gameObject.SetActive(true);
        dungeonMapCamera.gameObject.SetActive(false);

        minimapUI.SetActive(true);
    }

    /// <summary>
    /// Ensure all rooms are active so they can be displayed
    /// </summary>
    private void ActivateRoomsForDisplay()
    {
        // Iterate throuh dungeon rooms
        foreach(KeyValuePair<string, Room> keyValuePair in DungeonBuilder.Instance.dungeonBuilderRoomDictionary)
        {
            Room room = keyValuePair.Value;

            room.instantiatedRoom.gameObject.SetActive(true);
        }
    }
}
