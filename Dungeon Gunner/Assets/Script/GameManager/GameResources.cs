using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Tilemaps;

public class GameResources : MonoBehaviour
{
    private static GameResources instance;

    public static GameResources Instance
    {
        get
        {
            if(instance == null)
            {
                instance = Resources.Load<GameResources>("GameResources");
            }
            return instance;
        }
    }

    #region Header DUNGEON
    [Space(10)]
    [Header("DUNGEON")]
    #endregion
    #region Tooltip
    [Tooltip("Populate with the dungeon RoomNodeTypeListSO")]
    #endregion
    public RoomNodeTypeListSO roomNodeTypeList;

    #region Header PLAYER SELECTION
    [Space(10)]
    [Header("PLAYER SELECTION")]
    #endregion Header PLAYER SELECTION
    public GameObject playerSelectionPrefab;

    #region Header MUSIC
    [Space(10)]
    [Header("MUSIC")]
    #endregion Header MUSIC
    public AudioMixerGroup musicMasterMixerGroup;
    public AudioMixerSnapshot musicOnFullSnapshot;
    public AudioMixerSnapshot musicLowSnapshot;
    public AudioMixerSnapshot musicOffSnapshot;
    public MusicTrackSO mainMenuMusic;

    #region Header Player
    [Space(10)]
    [Header("PLAYER")]
    #endregion Header Player
    #region Tooltip
    [Tooltip("The current player scriptable object - this is used to reference the current player between scenes")]
    #endregion Tooltip
    public CurrentPlayerSO currentPlayer;
    public List<PlayerDetailsSO> playerDetailList;

    #region Header SOUNDS
    [Space(10)]
    [Header("SOUNDS")]
    #endregion Header SOUNDS
    public AudioMixerGroup soundsMasterMixerGroup;
    public SoundEffectSO doorOpenCloseSoundEffect;
    public SoundEffectSO tableFlip;
    public SoundEffectSO chestOpen;
    public SoundEffectSO healthPickUp;
    public SoundEffectSO weaponPickUp;
    public SoundEffectSO ammoPickUp;

    #region Header MATERIALS
    [Space(10)]
    [Header("MATERIALS")]
    #endregion Header MATERIALS
    #region Tooltip
    [Tooltip("Dimmed Material")]
    #endregion Tooltip
    public Material dimmendMaterial;
    public Material litMaterial;
    public Shader variableLitShader;
    public Shader materializeShader;

    #region Header SPECIAL TILEMAP TILES
    [Space(10)]
    [Header("SPECIAL TILEMAP TILES")]
    #endregion Header SPECIAL TILEMAP TILES
    public TileBase[] enemyUnwalkableCollisionTilesArray;
    public TileBase preferredEnemyPathTile;

    #region Header UI
    [Space(10)]
    [Header("UI")]
    #endregion Header UI
    public GameObject ammoIconPrefab;
    public GameObject heartPrefab;
    public GameObject scorePrefab;

    #region Header CHESTS
    [Space(10)]
    [Header("CHESTS")]
    #endregion Header CHESTS
    public GameObject chestItemPrefab;
    public Sprite heartIcon;
    public Sprite bulletIcon;

    #region Header MINIMAP
    [Space(10)]
    [Header("MINIMAP")]
    #endregion Header MINIMAP
    public GameObject minimapSkullPrefab;

    #region Validation
#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilitie.ValidateCheckNullValue(this, nameof(roomNodeTypeList), roomNodeTypeList);
        HelperUtilitie.ValidateCheckNullValue(this, nameof(playerSelectionPrefab), playerSelectionPrefab);
        HelperUtilitie.ValidateCheckEnumerableValues(this, nameof(playerDetailList), playerDetailList);
        HelperUtilitie.ValidateCheckNullValue(this, nameof(currentPlayer), currentPlayer);
        HelperUtilitie.ValidateCheckNullValue(this, nameof(soundsMasterMixerGroup), soundsMasterMixerGroup);
        HelperUtilitie.ValidateCheckNullValue(this, nameof(doorOpenCloseSoundEffect), doorOpenCloseSoundEffect);
        HelperUtilitie.ValidateCheckNullValue(this, nameof(tableFlip), tableFlip);
        HelperUtilitie.ValidateCheckNullValue(this, nameof(chestOpen), chestOpen);
        HelperUtilitie.ValidateCheckNullValue(this, nameof(healthPickUp), healthPickUp);
        HelperUtilitie.ValidateCheckNullValue(this, nameof(ammoPickUp), ammoPickUp);
        HelperUtilitie.ValidateCheckNullValue(this, nameof(weaponPickUp), weaponPickUp);
        HelperUtilitie.ValidateCheckNullValue(this, nameof(litMaterial), litMaterial);
        HelperUtilitie.ValidateCheckNullValue(this, nameof(dimmendMaterial), dimmendMaterial);
        HelperUtilitie.ValidateCheckNullValue(this, nameof(materializeShader), materializeShader);
        HelperUtilitie.ValidateCheckEnumerableValues(this, nameof(enemyUnwalkableCollisionTilesArray), enemyUnwalkableCollisionTilesArray);
        HelperUtilitie.ValidateCheckNullValue(this, nameof(preferredEnemyPathTile), preferredEnemyPathTile);
        HelperUtilitie.ValidateCheckNullValue(this, nameof(musicMasterMixerGroup), musicMasterMixerGroup);
        HelperUtilitie.ValidateCheckNullValue(this, nameof(musicOnFullSnapshot), musicOnFullSnapshot);
        HelperUtilitie.ValidateCheckNullValue(this, nameof(mainMenuMusic), mainMenuMusic);
        HelperUtilitie.ValidateCheckNullValue(this, nameof(musicLowSnapshot), musicLowSnapshot);
        HelperUtilitie.ValidateCheckNullValue(this, nameof(musicOffSnapshot), musicOffSnapshot);
        HelperUtilitie.ValidateCheckNullValue(this, nameof(variableLitShader), variableLitShader);
        HelperUtilitie.ValidateCheckNullValue(this, nameof(heartPrefab), heartPrefab);
        HelperUtilitie.ValidateCheckNullValue(this, nameof(ammoIconPrefab), ammoIconPrefab);
        HelperUtilitie.ValidateCheckNullValue(this, nameof(scorePrefab), scorePrefab);
        HelperUtilitie.ValidateCheckNullValue(this, nameof(chestItemPrefab), chestItemPrefab);
        HelperUtilitie.ValidateCheckNullValue(this, nameof(heartIcon), heartIcon);
        HelperUtilitie.ValidateCheckNullValue(this, nameof(bulletIcon), bulletIcon);
        HelperUtilitie.ValidateCheckNullValue(this, nameof(minimapSkullPrefab), minimapSkullPrefab);
    }
#endif
    #endregion Validation
}
