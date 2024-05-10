using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RoomNodeTypeListSO", menuName = "Scriptable Object/Dungeon/Room Node Type List")]
public class RoomNodeTypeListSO : ScriptableObject
{
    #region Header Room NODE LIST;
    [Space(10)]
    [Header("ROOM NODE TYPE LIST")]
    #endregion
    #region Tooltip
    [Tooltip("This list should be populated with all the RoomNodeTypeSO for the game - it is used insteda of an eunm")]
    #endregion
    public List<RoomNodeTypeSO> list;

    #region Validation
#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilitie.ValidateCheckEnumerableValues(this, nameof(list), list);
    }
#endif
    #endregion
}
