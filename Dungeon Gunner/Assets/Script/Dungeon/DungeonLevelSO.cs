using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEditor.Searcher;
using UnityEngine;

[CreateAssetMenu(fileName ="DungeonLevel_", menuName ="Scriptable Object/Dungeon/Dungeon Level")]
public class DungeonLevelSO : ScriptableObject
{
    #region Header BASIC LEVEL DETATLS
    [Space(10)]
    [Header("BASIC LELVEL DETATLS")]
    #endregion Header BASIC LEVEL DETATLS
    #region Tooltip
    [Tooltip("The name for the level")]
    #endregion Tooltip

    public string levelName;

    #region Header ROOM TEMPLATES FOR LEVEL
    [Space(10)]
    [Header("ROOM TAMPLATES FOR LEVEL")]
    #endregion Header ROOM TEMPLATES FOR LEVEL
    #region Tooltip
    [Tooltip("Populate the list with the room templates that you want to be part of the level. You need to ensure that room templates are include for" +
        "all room node types that are specified in the Room Node Graphs for the level.")]
    #endregion Tooltip

    public List<RoomTemplateSO> roomTemplateList;

    #region Header ROOM NODE GRAPHS FOR LEVEL
    [Space(10)]
    [Header("ROOM NODE GRAPHS FOR LEVEL")]
    #endregion Header ROOM NODE GRAPHS FOR LEVEL
    #region Tooltip
    [Tooltip("Populate this list with the room node graphs which should be randomly selected from for the level")]
    #endregion Tooltip

    public List<RoomNodeGraphSO> roomNodeGraphList;

    #region Validation
#if UNITY_EDITOR

    // validate scriptable object details enetered
    private void OnValidate()
    {
        HelperUtilitie.ValidateCheckEmptyString(this, nameof(levelName), levelName);
        if (HelperUtilitie.VaildateCheckEnumerableValues(this, nameof(roomTemplateList), roomTemplateList))
            return;
        if (HelperUtilitie.VaildateCheckEnumerableValues(this, nameof(roomNodeGraphList), roomNodeGraphList))
            return;

        // Check to make sure that room templates are specified for all the node types in the specified node graphs


        // First check that north/south corridor, east/west corridor and entrance types have been specified
        bool isEwCorridor = false;
        bool isNsCorridor = false;
        bool isEntrance = false;

        //Loop through all room templates to check that this node type has been specified
        foreach(RoomTemplateSO roomTemplateSO in roomTemplateList)
        {
            if (roomTemplateSO == null)
                return;

            if (roomTemplateSO.roomNodeType.isCorridorEw)
                isEwCorridor = true;

            if (roomTemplateSO.roomNodeType.isCorridorNs)
                isNsCorridor = true;

            if (roomTemplateSO.roomNodeType.isEntrance)
                isEntrance = true;
        }


        if(isEwCorridor == false)
        {
            Debug.Log("In" + this.name.ToString() + " : No E/W Corridor Room Type Specified");
        }

        if (isNsCorridor == false)
        {
            Debug.Log("In" + this.name.ToString() + " : No N/S Corridor Room Type Specified");
        }

        if (isEntrance == false)
        {
            Debug.Log("In" + this.name.ToString() + " : No Entrance Room Type Specified");
        }

        // Loop through all node graphs
        foreach(RoomNodeGraphSO roomNodeGraph in roomNodeGraphList)
        {
            if (roomNodeGraph == null)
                return;

            foreach(RoomNodeSO roomNodeSO in roomNodeGraph.roomNodeList)
            {
                if (roomNodeSO == null)
                    continue;

                // Check that a room template has been specified for each roomNode type

                // Corridors and entrance alread checked 
                if (roomNodeSO.roomNodeType.isEntrance || roomNodeSO.roomNodeType.isCorridorEw || roomNodeSO.roomNodeType.isCorridorNs ||
                    roomNodeSO.roomNodeType.isCorridor || roomNodeSO.roomNodeType.isNone)
                    continue;

                bool isRoomNodeTypeFound = false;

                // Loop through all room templates to check  that this node type has been specified
                foreach(RoomTemplateSO roomTemplateSO in roomTemplateList)
                {
                    if (roomTemplateSO == null)
                        continue;

                    if(roomTemplateSO.roomNodeType == roomNodeSO.roomNodeType)
                    {
                        isRoomNodeTypeFound = true;
                        break;
                    }
                }

                if (!isRoomNodeTypeFound)
                {
                    Debug.Log("In" + this.name.ToString() + " : No room template " + roomNodeSO.roomNodeType.name.ToString() + " found for node graph"
                        + roomNodeGraph.name.ToString());
                }
            }

        }
    }
#endif
    #endregion Validation
}
