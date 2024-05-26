using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    #region Header Player
    [Space(10)]
    [Header("PLAYER")]
    #endregion Header Player
    #region Tooltip
    [Tooltip("The current player scriptable object - this is used to reference the current player between scenes")]
    #endregion Tooltip
    public CurrentPlayerSO currentPlayer;

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

    #region Validation
#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilitie.ValidateCheckNullValue(this, nameof(roomNodeTypeList), roomNodeTypeList);
        HelperUtilitie.ValidateCheckNullValue(this, nameof(currentPlayer), currentPlayer);
        HelperUtilitie.ValidateCheckNullValue(this, nameof(litMaterial), litMaterial);
        HelperUtilitie.ValidateCheckNullValue(this, nameof(dimmendMaterial), dimmendMaterial);
        HelperUtilitie.ValidateCheckNullValue(this, nameof(variableLitShader), variableLitShader);
    }
#endif
    #endregion Validation
}
