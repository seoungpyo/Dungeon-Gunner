using System.Collections;
using System.Collections.Generic;
using UnityEditor.Profiling.Memory.Experimental;
using UnityEngine;

[CreateAssetMenu(fileName ="PlayerDetails_",menuName ="Scriptable Object/Player/PlayerDetails")]
public class PlayerDetailsSO : ScriptableObject
{
    #region Header Player Base Details
    [Space(10)]
    [Header("Player Base Details")]
    #endregion Header Player Base Details
    #region Tooltip
    [Tooltip("Player CharacterName")]
    #endregion Tooltip
    public string playerCharacterName;

    #region Tooltip
    [Tooltip("Prefab gameobject for the player")]
    #endregion Tooltip
    public GameObject playerPrefab;

    #region Tooltip
    [Tooltip("Player runtime animation controller")]
    #endregion Tooltip
    public RuntimeAnimatorController runtimeAnimatorController;

    #region Header Health
    [Space(10)]
    [Header("HEALTH")]
    #endregion Header Health
    #region Tooltip
    [Tooltip("player starting health amount")]
    #endregion Tooltip
    public int playerHealthAmount;

    #region Header OTHER
    [Space(10)]
    [Header("OTHER")]
    #endregion Header OTHER
    #region Tooltip
    [Tooltip("Player icon sprite to be used in the minimap")]
    #endregion Tooltip
    public Sprite playerMiniMapIcon;

    #region Tooltip
    [Tooltip("Player hand sprite")]
    #endregion Tooltip
    public Sprite playerHandSprite;

    #region Validattion
#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilitie.ValidateCheckEmptyString(this, nameof(playerCharacterName), playerCharacterName);
        HelperUtilitie.ValidateCheckNullValue(this, nameof(playerPrefab), playerPrefab);
        HelperUtilitie.ValidateCheckPositiveValue(this, nameof(playerHealthAmount), playerHealthAmount, false);
        HelperUtilitie.ValidateCheckNullValue(this, nameof(playerMiniMapIcon), playerMiniMapIcon);
        HelperUtilitie.ValidateCheckNullValue(this, nameof(runtimeAnimatorController), runtimeAnimatorController);
    }
#endif
    #endregion Validattion
}
