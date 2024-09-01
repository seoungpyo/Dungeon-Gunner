using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[DisallowMultipleComponent]
public class ActiveRooms : MonoBehaviour
{
    #region Header POPULATE WITH MINIMAP CAMERA
    [Space(10)]
    [Header("POPULATE WITH MINIMAP CAMERA")]
    #endregion Header POPULATE WITH MINIMAP CAMERA
    [SerializeField] private Camera miniMapCamera;

    private Camera cameraMain;

    private void Start()
    {
        cameraMain = Camera.main;

        InvokeRepeating("EnableRooms", 0.5f, 0.75f);
    }

    private void EnableRooms()
    {
        // if currently showing the dungeon map UI don't process
        if (GameManager.Instance.gameState == GameState.dungeonOverviewMap) return;

        HelperUtilitie.CameraWorldPositionBounds(out Vector2Int miniMapCameraWorldPositionLowerBounds, out Vector2Int miniMapCameraWorldPositionUpperBounds, miniMapCamera);

        HelperUtilitie.CameraWorldPositionBounds(out Vector2Int mainCameraWorldPositionLowerBounds, out Vector2Int mainCameraWorldPositionUpperBounds, cameraMain);

        foreach (KeyValuePair<string,Room>keyValuePair in DungeonBuilder.Instance.dungeonBuilderRoomDictionary)
        {
            Room room = keyValuePair.Value;

            if ((room.lowerBounds.x <= miniMapCameraWorldPositionUpperBounds.x && room.lowerBounds.y <= miniMapCameraWorldPositionUpperBounds.y) &&
                (room.upperBounds.x >= miniMapCameraWorldPositionLowerBounds.x && room.upperBounds.y >= miniMapCameraWorldPositionLowerBounds.y))
            {
                room.instantiatedRoom.gameObject.SetActive(true); 

                if ((room.lowerBounds.x <= mainCameraWorldPositionUpperBounds.x && room.lowerBounds.y <= mainCameraWorldPositionUpperBounds.y) &&
                    (room.upperBounds.x >= mainCameraWorldPositionLowerBounds.x && room.upperBounds.y >= mainCameraWorldPositionLowerBounds.y) && room.isLit)
                {
                    room.instantiatedRoom.ActivateEnvironmentGameObjects();
                }
                else
                {
                    room.instantiatedRoom.DeactivateEnvironmentGameObjects();
                }
            }
            else
            {
                room.instantiatedRoom.gameObject.SetActive(false);
            }
        }
    }

    #region Validation
#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilitie.ValidateCheckNullValue(this, nameof(miniMapCamera), miniMapCamera);
    }
#endif
    #endregion Validation
}
