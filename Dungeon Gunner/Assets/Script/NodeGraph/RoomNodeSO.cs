using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEditor;
using UnityEngine;

public class RoomNodeSO : ScriptableObject
{
    [HideInInspector] public string id;
    [HideInInspector] public List<string> parentRoomNodeIDList = new List<string>();
    [HideInInspector] public List<string> childRoomNodeIDList = new List<string>();
    [HideInInspector] public RoomNodeGraphSO roomNodeGraph;

    public RoomNodeTypeSO roomNodeType;

    [HideInInspector] public RoomNodeTypeListSO roomNodeTypeList;
    [HideInInspector] public bool isLeftClickDragging = false;
    [HideInInspector] public bool isSelected = false;

    #region Editor Code

#if UNITY_EDITOR

    [HideInInspector] public Rect rect;

    /// <summary>
    /// initalise node
    /// </summary>
    /// <param name="rect"></param>
    /// <param name="nodeGraph"></param>
    /// <param name="roomNodeType"></param>
    public void Initialise(Rect rect, RoomNodeGraphSO nodeGraph, RoomNodeTypeSO roomNodeType)
    {
        this.rect = rect;
        this.id = Guid.NewGuid().ToString();
        this.name = "RoomNode";
        this.roomNodeGraph = nodeGraph;
        this.roomNodeType = roomNodeType;

        //Load room node type list
        roomNodeTypeList = GameResources.Instance.roomNodeTypeList;
    }

    public void Draw(GUIStyle nodeStlye)
    {
        //Draw node box using begin area
        GUILayout.BeginArea(rect, nodeStlye);

        //start region to Detect Popup Selection Change
        EditorGUI.BeginChangeCheck();

        //if the room node has a parent or is of type entrace then display a label else display a popup
        if(parentRoomNodeIDList.Count>0 || roomNodeType.isEntrance)
        {
            //dispaly a label that can't be change
            EditorGUILayout.LabelField(roomNodeType.roomNodeTypeName);
        }
        else
        {
            //display a popup using the roomnodetype name values that can be selected from (default to the currently set roomnNodeType)
            int selected = roomNodeTypeList.list.FindIndex(x => x == roomNodeType);

            int selection = EditorGUILayout.Popup("", selected, GetRoomNodeTypeToDisplay());
            roomNodeType = roomNodeTypeList.list[selection];

            //if the room type selection has changed making child connections potentially invalid
            if (roomNodeTypeList.list[selected].isCorridor && !roomNodeTypeList.list[selection].isCorridor || !roomNodeTypeList.list[selected].isCorridor
                && roomNodeTypeList.list[selection].isCorridor || !roomNodeTypeList.list[selected].isBossRoom && roomNodeTypeList.list[selection].isBossRoom)
            {

                if (childRoomNodeIDList.Count > 0)
                {
                    for (int i = childRoomNodeIDList.Count - 1; i >= 0; i--)
                    {
                        //Get child room node
                        RoomNodeSO childRoomNode = roomNodeGraph.GetRoomNode(childRoomNodeIDList[i]);

                        //if the child room node is not null
                        if (childRoomNode != null)
                        {
                            //remove childID from parent room node
                            RemoveChildRoomNodeIDFromRoomNode(childRoomNode.id);

                            //remove parentID from child room node
                            childRoomNode.RemoveChildRoomNodeIDFromRoomNode(id);
                        }
                    }
                }
            }
        }        

        if (EditorGUI.EndChangeCheck())
            EditorUtility.SetDirty(this);

        GUILayout.EndArea();
    }

    /// <summary>
    /// populate a string array with the room node tpyes to display that can be selected
    /// </summary>
    /// <returns></returns>
    public string[] GetRoomNodeTypeToDisplay()
    {
        string[] roomArray = new string[roomNodeTypeList.list.Count];

        for(int i =0; i<roomNodeTypeList.list.Count; i++)
        {
            if (roomNodeTypeList.list[i].displayInNodeGraphEditor)
            {
                roomArray[i] = roomNodeTypeList.list[i].roomNodeTypeName;
            }
        }

        return roomArray;
    }

    /// <summary>
    /// Process events for the node
    /// </summary>
    /// <param name="currentEvent"></param>
    public void ProcessEvent(Event currentEvent)
    {
        switch (currentEvent.type)
        {
            //process mouse down event
            case EventType.MouseDown:
                ProcessMouseDownEvent(currentEvent);
                break;
            //process mouse up event
            case EventType.MouseUp:
                ProcessMouseUpEvent(currentEvent);
                break;
            //process mouse drag event
            case EventType.MouseDrag:
                ProcessMouseDragEvent(currentEvent);
                break;

            default:
                break;
        }
    }

    /// <summary>
    /// process mouse down events
    /// </summary>
    /// <param name="currentEvent"></param>
    private void ProcessMouseDownEvent(Event currentEvent)
    {
        //mouse left click
        if(currentEvent.button == 0)
        {
            ProcessLeftClickDownEvent();
        }
        //mouse right click 
        else if(currentEvent.button == 1)
        {
            ProcessRightClickDownEvent(currentEvent);
        }

    }

    /// <summary>
    /// process left click down event
    /// </summary>
    private void ProcessLeftClickDownEvent()
    {
        Selection.activeObject = this;

        //toggle node selection
        if(isSelected == true)
        {
            isSelected = false;
        }
        else
        {
            isSelected = true;
        }
    }

    /// <summary>
    /// process mouse right click 
    /// </summary>
    /// <param name="currentEvent"></param>
    private void ProcessRightClickDownEvent(Event currentEvent)
    {
        roomNodeGraph.SetNodeToDrawConnectionLineFrom(this, currentEvent.mousePosition);
    }

    /// <summary>
    /// Process mouse up event
    /// </summary>
    /// <param name="currentEvent"></param>
    private void ProcessMouseUpEvent(Event currentEvent)
    {
        if(currentEvent.button == 0)
        {
            ProcessLeftClickUpEvent();
        }
    }

    /// <summary>
    /// Process mouse drag event
    /// </summary>
    private void ProcessLeftClickUpEvent()
    {
        if (isLeftClickDragging)
        {
            isLeftClickDragging = false;
        }
    }

    private void ProcessMouseDragEvent(Event currentEvent)
    {
        //process left drag event
        if(currentEvent.button == 0)
        {
            ProcessLeftMouseDragEvent(currentEvent);
        }
    }

    /// <summary>
    /// Process left drag evnet
    /// </summary>
    private void ProcessLeftMouseDragEvent(Event currentEvent)
    {
        isLeftClickDragging = true;

        DragNode(currentEvent.delta);
        GUI.changed = true;
    }

    /// <summary>
    /// Drag Node
    /// </summary>
    /// <param name="delta"></param>
    public void DragNode(Vector2 delta) 
    {
        rect.position += delta;
        EditorUtility.SetDirty(this);
    }

    /// <summary>
    /// add childID to the node(return true if the node has been added, false otherwise)
    /// </summary>
    /// <param name="childID"></param>
    /// <returns></returns>
    public bool AddChildRoomNodeIDToRoomNode(string childID)
    {
        if (IsChildRoomValid(childID))
        {
            childRoomNodeIDList.Add(childID);
            return true;
        }

        return false;
    }

    /// <summary>
    /// check the child room node can be validly added to the parent node - return true if it can otherwise return false
    /// </summary>
    /// <param name="childID"></param>
    /// <returns></returns>
    public bool IsChildRoomValid(string childID)
    {
        bool isConnectedBossNodeAlready = false;
        // Check if there is there already a connected boss room in the node graph
        foreach (RoomNodeSO roomNode in roomNodeGraph.roomNodeList)
        {
            if (roomNode.roomNodeType.isBossRoom && roomNode.parentRoomNodeIDList.Count > 0)
                isConnectedBossNodeAlready = true;
        }

        // if the child node has a type of boss room and there is already a connected boss room node then return false
        if (roomNodeGraph.GetRoomNode(childID).roomNodeType.isBossRoom && isConnectedBossNodeAlready)
            return false;

        // If the child node has a type of none then return false
        if (roomNodeGraph.GetRoomNode(childID).roomNodeType.isNone)
            return false;

        // If the node already has a child with this child ID return false
        if (childRoomNodeIDList.Contains(childID))
            return false;

        // If this node ID and the child ID are the same return false
        if (id == childID)
            return false;

        // If this childID is already in the parentID list return false
        if (parentRoomNodeIDList.Contains(childID))
            return false;

        // If the child node already has a parent return false
        if (roomNodeGraph.GetRoomNode(childID).parentRoomNodeIDList.Count > 0)
            return false;

        // If child is a corridor and this node is a corridor return false
        if (roomNodeGraph.GetRoomNode(childID).roomNodeType.isCorridor && roomNodeType.isCorridor)
            return false;

        // If child is not a corridor and this node is not a corridor return false
        if (!roomNodeGraph.GetRoomNode(childID).roomNodeType.isCorridor && !roomNodeType.isCorridor)
            return false;

        // If adding a corridor check that this node has < the maximum permitted child corridors
        if (roomNodeGraph.GetRoomNode(childID).roomNodeType.isCorridor && childRoomNodeIDList.Count >= Settings.maxChildCorridors)
            return false;

        // if the child room is an entrance return false - the entrance must always be the top level parent node
        if (roomNodeGraph.GetRoomNode(childID).roomNodeType.isEntrance)
            return false;

        // If adding a room to a corridor check that this corridor node doesn't already have a room added
        if (!roomNodeGraph.GetRoomNode(childID).roomNodeType.isCorridor && childRoomNodeIDList.Count > 0)
            return false;

        return true;
    }

    /// <summary>
    /// add parentID to the node (return true if the node has been added, fales otherwise)
    /// </summary>
    /// <param name="parentID"></param>
    /// <returns></returns>
    public bool AddParentRoomNodeIDToRoomNode(string parentID)
    {
        parentRoomNodeIDList.Add(parentID);
        return true;
    }

    /// <summary>
    /// Remove childID from the node(return true if the node has been removed, false otherwise)
    /// </summary>
    /// <param name="childID"></param>
    /// <returns></returns>
    public bool RemoveChildRoomNodeIDFromRoomNode(string childID)
    {
        //if the node contain the child ID then remove it
        if (childRoomNodeIDList.Contains(childID))
        {
            childRoomNodeIDList.Remove(childID);
            return true;

        }
        return false;
    }

    /// <summary>
    /// revmoe parent ID from the node(return true if the node has been remove, false otherwise)
    /// </summary>
    /// <param name="parentID"></param>
    /// <returns></returns>
    public bool RemoveParentRoomNodeIDFromRoomNode(string parentID)
    {
        //if the node contain the parent ID then remove it
        if (parentRoomNodeIDList.Contains(parentID))
        {
            parentRoomNodeIDList.Remove(parentID);
            return true;
        }
        return false;
    }
#endif

#endregion
}
