using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build;
using UnityEngine;

[CreateAssetMenu(fileName = "MovementDetails_", menuName = "Scriptable Object/Movement/MovementDetails")]
public class MovementDetailsSO : ScriptableObject
{

    #region Header Movement Details
    [Space(10)]
    [Header("MOVEMENT DETAILS")]
    #endregion Header Movement Details

    #region Tooltip
    [Tooltip("The minimum move speed. The getmMoveSpeed method calculates a random value between the minimum and maximum")]
    #endregion Tooltip
    public float minMoveSpeed = 8f;
    #region Tooltip
    [Tooltip("The maximum move speed. The getMoveSpeed method calculates a random value between the minimum and maximum")]
    #endregion Tooltip
    public float maxMoveSpeed = 8f;
    #region Tooltip
    [Tooltip("If there is a roll movement - this is the roll speed")]
    #endregion Tooltip
    public float rollSpeed; // for player   
    #region Tooltip
    [Tooltip("If there is a roll movement - this is the roll distance")]
    #endregion Tooltip
    public float rollDistance; // for player
    #region Tooltip
    [Tooltip("If there is roll movement - this is the cooldown time in seconds between roll action")]
    #endregion Tooltip
    public float rollCooldownTime; // for player

    /// <summary>
    /// Get a random movement speed between the minimum and maximum values 
    /// </summary>
    /// <returns></returns>
    public float GetMoveSpeed()
    {
        if(minMoveSpeed == maxMoveSpeed)
        {
            return minMoveSpeed;
        }
        else
        {
            return Random.Range(minMoveSpeed, maxMoveSpeed);
        }
    }

    #region Validation
#if UNITY_EDITOR

    private void OnValidate()
    {
        HelperUtilitie.ValidateCheckPositiveRange(this, nameof(minMoveSpeed), minMoveSpeed, nameof(maxMoveSpeed), maxMoveSpeed, false);

        if (rollDistance != 0 || rollCooldownTime != 0 || rollSpeed != 0)
        {
            HelperUtilitie.ValidateCheckPositiveValue(this, nameof(rollDistance), rollDistance,false);
            HelperUtilitie.ValidateCheckPositiveValue(this, nameof(rollSpeed), rollSpeed, false);
            HelperUtilitie.ValidateCheckPositiveValue(this, nameof(rollCooldownTime), rollCooldownTime, false);
        }
    }

#endif
    #endregion Validation
}
