using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RoomNodeTpye", menuName ="Scriptable Object/Dungeon/Room Node Type")]
public class RoomNodeTypeSO : ScriptableObject
{
    public string roomNodeTypeName;

    [Header("Only flag the RoomNodeTypes that should be visible in the editor")]
    public bool displayInNodeGraphEditor = true;
    [Header("One Type Should Be A Cooridor")]
    public bool isCorridor;
    [Header("One Type Should Be A CorridorNs")]
    public bool isCorridorNs;
    [Header("One Type Should Be A CorridorEW")]
    public bool isCorridorEw;
    [Header("One Type Should Be A Entrance")]
    public bool isEntrance;
    [Header("One Type Should Be A Boss Room")]
    public bool isBossRoom;
    [Header("One type Should Be None(Unassiged)")]
    public bool isNone;

#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilitie.ValidateCheckEmptyString(this, nameof(roomNodeTypeName), roomNodeTypeName);
    }
#endif

}
