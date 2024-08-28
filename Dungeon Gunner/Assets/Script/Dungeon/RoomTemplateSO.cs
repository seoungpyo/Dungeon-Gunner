using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "Room_", menuName = "Scriptable Object/Dungeon/Room")]
public class RoomTemplateSO : ScriptableObject
{
    [HideInInspector] public string guid;

    #region Header ROOM PREFAB

    [Space(10)]
    [Header("ROOM PREFAB")]

    #endregion Header ROOM PREFAB

    #region Tooltip

    [Tooltip("The gameobject prefab for the room (this will contain all the tilemaps for the room and environment game objects")]

    #endregion Tooltip

    public GameObject prefab;

    [HideInInspector] public GameObject previousPrefab; // this is used to regenerate the guid if the so is copied and the prefab is changed

    #region Header ROOM MUSIC
    [Space(10)]
    [Header("ROOM MUSIC")]
    #endregion Header ROOM MUSIC
    public MusicTrackSO battleMusic;
    public MusicTrackSO ambientMusic;

    #region Header ROOM CONFIGURATION

    [Space(10)]
    [Header("ROOM CONFIGURATION")]

    #endregion Header ROOM CONFIGURATION

    #region Tooltip

    [Tooltip("The room node type SO. The room node types correspond to the room nodes used in the room node graph.  The exceptions being with corridors.  In the room node graph there is just one corridor type 'Corridor'.  For the room templates there are 2 corridor node types - CorridorNS and CorridorEW.")]

    #endregion Tooltip

    public RoomNodeTypeSO roomNodeType;

    #region Tooltip

    [Tooltip("If you imagine a rectangle around the room tilemap that just completely encloses it, the room lower bounds represent the bottom left corner of that rectangle. This should be determined from the tilemap for the room (using the coordinate brush pointer to get the tilemap grid position for that bottom left corner (Note: this is the local tilemap position and NOT world position")]

    #endregion Tooltip

    public Vector2Int lowerBounds;

    #region Tooltip

    [Tooltip("If you imagine a rectangle around the room tilemap that just completely encloses it, the room upper bounds represent the top right corner of that rectangle. This should be determined from the tilemap for the room (using the coordinate brush pointer to get the tilemap grid position for that top right corner (Note: this is the local tilemap position and NOT world position")]

    #endregion Tooltip

    public Vector2Int upperBounds;

    #region Tooltip

    [Tooltip("There should be a maximum of four doorways for a room - one for each compass direction.  These should have a consistent 3 tile opening size, with the middle tile position being the doorway coordinate 'position'")]

    #endregion Tooltip

    [SerializeField] public List<Doorway> doorwayList;

    #region Tooltip

    [Tooltip("Each possible spawn position (used for enemies and chests) for the room in tilemap coordinates should be added to this array")]

    #endregion Tooltip

    public Vector2Int[] spawnPositionArray;

    #region Header ENEMY DETAILS
    [Space(10)]
    [Header("ENEMY DETAILS")]
    #endregion Header ENEMY DETAILS
    public List<SpawnableObjectsByLevel<EnemyDetailsSO>> enemiesByLevelList;

    public List<RoomEnemySpawnParameters> roomEnemySpawnParametersList;

    /// <summary>
    /// Returns the list of Entrances for the room template
    /// </summary>
    public List<Doorway> GetDoorwayList()
    {
        return doorwayList;
    }

    #region Validation

#if UNITY_EDITOR

    // Validate SO fields
    private void OnValidate()
    {
        // Set unique GUID if empty or the prefab changes
        if (guid == "" || previousPrefab != prefab)
        {
            guid = GUID.Generate().ToString();
            previousPrefab = prefab;
            EditorUtility.SetDirty(this);
        }
        HelperUtilitie.ValidateCheckNullValue(this, nameof(prefab), prefab);
        HelperUtilitie.ValidateCheckNullValue(this, nameof(battleMusic), battleMusic);
        HelperUtilitie.ValidateCheckNullValue(this, nameof(ambientMusic), ambientMusic);
        HelperUtilitie.ValidateCheckNullValue(this, nameof(roomNodeType), roomNodeType);

        HelperUtilitie.ValidateCheckEnumerableValues(this, nameof(doorwayList), doorwayList);

        if (enemiesByLevelList.Count > 0 || roomEnemySpawnParametersList.Count > 0)
        {
            HelperUtilitie.ValidateCheckEnumerableValues(this, nameof(enemiesByLevelList), enemiesByLevelList);
            HelperUtilitie.ValidateCheckEnumerableValues(this, nameof(roomEnemySpawnParametersList), roomEnemySpawnParametersList);

            foreach(RoomEnemySpawnParameters roomEnemySpawnParameters in roomEnemySpawnParametersList)
            {
                HelperUtilitie.ValidateCheckNullValue(this, nameof(roomEnemySpawnParameters.dungeonLevel), roomEnemySpawnParameters.dungeonLevel);
                HelperUtilitie.ValidateCheckPositiveRange(this, nameof(roomEnemySpawnParameters.minTotalEnemiesToSpawn),
                    roomEnemySpawnParameters.minTotalEnemiesToSpawn, nameof(roomEnemySpawnParameters.maxTotalEnemiesToSpawn),
                    roomEnemySpawnParameters.maxTotalEnemiesToSpawn, true);

                HelperUtilitie.ValidateCheckPositiveRange(this, nameof(roomEnemySpawnParameters.minSpawnInterval),
                    roomEnemySpawnParameters.minSpawnInterval, nameof(roomEnemySpawnParameters.maxSpawnInterval),
                    roomEnemySpawnParameters.maxSpawnInterval, true);

                HelperUtilitie.ValidateCheckPositiveRange(this, nameof(roomEnemySpawnParameters.minConcurrentEnemies),
                    roomEnemySpawnParameters.minConcurrentEnemies, nameof(roomEnemySpawnParameters.maxConcurrentEnemies),
                    roomEnemySpawnParameters.maxConcurrentEnemies, false);

                bool isEnemyTypesListForDungeonLevel = true;

                foreach(SpawnableObjectsByLevel<EnemyDetailsSO> dungeonObjectByLevel in enemiesByLevelList)
                {
                    if(dungeonObjectByLevel.dungeonLevel == roomEnemySpawnParameters.dungeonLevel && dungeonObjectByLevel.spawnableObjectRatioList.Count > 0)
                    {
                        isEnemyTypesListForDungeonLevel = true;
                    }

                    HelperUtilitie.ValidateCheckNullValue(this, nameof(dungeonObjectByLevel.dungeonLevel), dungeonObjectByLevel.dungeonLevel);

                    foreach(SpawnableObjectRatio<EnemyDetailsSO> dungeonObjectRatio in dungeonObjectByLevel.spawnableObjectRatioList)
                    {
                        HelperUtilitie.ValidateCheckNullValue(this, nameof(dungeonObjectRatio.dungeonObject), dungeonObjectRatio.dungeonObject);
                        HelperUtilitie.ValidateCheckPositiveValue(this, nameof(dungeonObjectRatio.ratio), dungeonObjectRatio.ratio, false);
                    }
                }

                if(isEnemyTypesListForDungeonLevel == false && roomEnemySpawnParameters.dungeonLevel != null)
                {
                    Debug.Log("No enemy types specified in for dungeon level" + roomEnemySpawnParameters.dungeonLevel.levelName + "in gameobject" + this.name.ToString());
                }
            }
        }

        // Check spawn positions populated
        HelperUtilitie.ValidateCheckEnumerableValues(this, nameof(spawnPositionArray), spawnPositionArray);
    }

#endif

    #endregion Validation
}