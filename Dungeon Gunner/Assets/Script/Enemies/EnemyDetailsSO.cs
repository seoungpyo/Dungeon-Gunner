using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyDetails_", menuName = "Scriptable Object/Enemy/Enemy Details")]
public class EnemyDetailsSO : ScriptableObject
{
    #region Header BASE ENEMY DETAILS
    [Space(10)]
    [Header("BASE ENEMY DETAILS")]
    #endregion Header BASE ENEMY DETAILS
    public string enemyName;
    public GameObject enemyPrefab;
    public float chaseDistance = 50f;

    #region Validation
#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilitie.ValidateCheckEmptyString(this, nameof(enemyName), enemyName);
        HelperUtilitie.ValidateCheckNullValue(this, nameof(enemyPrefab), enemyPrefab);
        HelperUtilitie.ValidateCheckPositiveValue(this, nameof(chaseDistance), chaseDistance,false);
    }
#endif
    #endregion Validation
}
