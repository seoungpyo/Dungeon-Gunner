using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine.Rendering;
using Unity.Plastic.Newtonsoft.Json.Bson;

public class RoomNodeGraphEditor : EditorWindow
{
    private GUIStyle roomNodeStyle;
    private GUIStyle roomNodeSelectedStyle;
    private static RoomNodeGraphSO currentRoomNodeGraph;
    private static RoomNodeTypeListSO roomNodeTypeList;
    private static RoomNodeSO currentRoomNode;

    private Vector2 graphOffset;
    private Vector2 graphDrag;

    //노드 값 설정
    private const float nodeWidth = 160f;
    private const float nodeHeight = 75f;
    private const int nodePadding = 25;
    private const int nodeBorder = 12;

    //connecting line value
    private const float connectingLineWidth = 3f;
    private const float connectingLineArrowSize = 6f;

    //grid spacing
    private const float gridLarge = 100f;
    private const float gridSmall = 25f;

    [MenuItem("Room Node Graph Editor", menuItem = "Window/Dungeon Editor/Room Node Graph Editor")]
    private static void OpenWindow()
    {
        GetWindow<RoomNodeGraphEditor>("Room Node Graph Editor");
    }

    private void OnEnable()
    {
        //subscrible to the inspector selection chaged event
        Selection.selectionChanged += InspectorSelectionChanged;

        //set node style
        roomNodeStyle = new GUIStyle();
        roomNodeStyle.normal.background = EditorGUIUtility.Load("node1") as Texture2D;
        roomNodeStyle.normal.textColor = Color.white;
        roomNodeStyle.padding = new RectOffset(nodePadding, nodePadding, nodePadding, nodePadding);
        roomNodeStyle.border = new RectOffset(nodeBorder, nodeBorder, nodeBorder, nodeBorder);

        //define selected node style
        roomNodeSelectedStyle = new GUIStyle();
        roomNodeSelectedStyle.normal.background = EditorGUIUtility.Load("node1 on") as Texture2D;
        roomNodeSelectedStyle.normal.textColor = Color.white;
        roomNodeSelectedStyle.padding = new RectOffset(nodePadding, nodePadding, nodePadding, nodePadding);
        roomNodeSelectedStyle.border = new RectOffset(nodeBorder, nodeBorder, nodeBorder, nodeBorder);

        //load room node type
        roomNodeTypeList = GameResources.Instance.roomNodeTypeList;
    }

    private void OnDisable()
    {
        Selection.selectionChanged -= InspectorSelectionChanged;
    }

    /// <summary>
    /// asset double click event in Room Node Graph Editor
    /// </summary>
    [OnOpenAsset(0)] //name space UnityEditor.Callbacks use 
    public static bool OnDoubleClickAsset(int instanceID, int line)
    {
        RoomNodeGraphSO roomNodeGraph = EditorUtility.InstanceIDToObject(instanceID) as RoomNodeGraphSO;

        if (roomNodeGraph != null)
        {
            OpenWindow();

            currentRoomNodeGraph = roomNodeGraph;

            return true;
        }
        return false;
    }

    private void OnGUI()
    {
        //If a scriptable object of type RoomNodeGraphSO has been selected then process
        if(currentRoomNodeGraph != null)
        {
            //draw grid
            DrawBackgroundGrid(gridSmall, 0.2f, Color.gray);
            DrawBackgroundGrid(gridLarge, 0.3f, Color.gray);

            //draw line if being dragged
            DrawDraggedLine();

            //process event
            ProcessEvent(Event.current);

            //draw connection between room node
            DrawRoomConnection();

            //Draw Room Node
            DrawRoomNode();
        }

        if (GUI.changed)
            Repaint();
    }

    private void DrawBackgroundGrid(float gridSize, float gridOpacity, Color gridColor)
    {
        int verticalLineCount = Mathf.CeilToInt((position.width + gridSize) / gridSize);
        int horizonLineCount = Mathf.CeilToInt((position.height + gridSize) / gridSize);

        Handles.color = new Color(gridColor.r, gridColor.g, gridColor.b, gridOpacity);
        graphOffset += graphDrag * 0.5f;

        Vector3 gridOffset = new Vector3(graphOffset.x % gridSize, graphOffset.y % gridSize, 0);
        
        for(int i =0; i < verticalLineCount; i++)
        {
            Handles.DrawLine(new Vector3(gridSize * i, -gridSize, 0) + gridOffset, new Vector3(gridSize * i, position.height + gridSize, 0f) + gridOffset);
        }

        for (int i = 0; i < horizonLineCount; i++)
        {
            Handles.DrawLine(new Vector3(-gridSize, gridSize * i, 0) + gridOffset, new Vector3(position.width + gridSize,gridSize * i, 0f) + gridOffset);
        }

        Handles.color = Color.white;
    }

    private void DrawDraggedLine()
    {
        if(currentRoomNodeGraph.linePosition != Vector2.zero)
        {
            //Draw line from node to line position
            Handles.DrawBezier(currentRoomNodeGraph.roomNodeToDrawLineFrom.rect.center, currentRoomNodeGraph.linePosition,
                currentRoomNodeGraph.roomNodeToDrawLineFrom.rect.center, currentRoomNodeGraph.linePosition, Color.white, null, connectingLineWidth);
        }
    }

    /// <summary>
    /// process room node graph event
    /// </summary>
    private void ProcessEvent(Event currentEvent)
    {
        graphDrag = Vector2.zero;

        //get room node that mouse is over if it's null or not currently being dragged
        if (currentRoomNode == null || currentRoomNode.isLeftClickDragging == false)
        {
            currentRoomNode = IsMouseOverRoomNode(currentEvent);
        }

        //if mouse isn't over room node or we are currently dragging a line from the room node than process graph event
        if(currentRoomNode == null || currentRoomNodeGraph.roomNodeToDrawLineFrom != null)
        {
            ProcessRoomNodeGraphEvent(currentEvent);
        }

        //else process room node event
        else
        {
            //process room node event
            currentRoomNode.ProcessEvent(currentEvent);
        }
    }

    /// <summary>
    /// check to see to mouse is over a room node - if so then return the room node else return null
    /// </summary>
    /// <param name="currentEvent"></param>
    /// <returns></returns>
    private RoomNodeSO IsMouseOverRoomNode(Event currentEvent)
    {
        for(int i = currentRoomNodeGraph.roomNodeList.Count -1; i>=0; i--)
        {
            //Rect.Contain method verifies that param are include in the Rect location, if exist,true ,else,return false 
            if (currentRoomNodeGraph.roomNodeList[i].rect.Contains(currentEvent.mousePosition))
            {
                return currentRoomNodeGraph.roomNodeList[i];
            }
        }

        return null;
    }

    private void ProcessRoomNodeGraphEvent(Event currentEvent)
    {
        switch (currentEvent.type)
        {
            //process mouse down event
            case EventType.MouseDown:
                ProcessMouseDownEvent(currentEvent);
                break;
    
            //process mouse drag event
            case EventType.MouseDrag:
                ProcessMouseDragEvent(currentEvent);
                break;

            //process mouse up event
            case EventType.MouseUp:
                ProcessMouseUpEvent(currentEvent);
                break;



            default:
                break;
        }
    }

    /// <summary>
    /// process mouse down event on the room node graph (not over a node)
    /// </summary>
    /// <param name="currentEvent"></param>
    private void ProcessMouseDownEvent(Event currentEvent)
    {
        //process right click mouse down on graph event(show context menu)
        if(currentEvent.button == 1)
        {
            ShowContextMenu(currentEvent.mousePosition);
        }
        // process left mouse down on graph event
        else if(currentEvent.button == 0)
        {
            ClearLineDrag();
            ClearAllSelectedRoomNode();
        }
    }

    /// <summary>
    /// show context menu
    /// </summary>
    /// <param name="mouseposition"></param>
    private void ShowContextMenu(Vector2 mouseposition)
    {
        GenericMenu menu = new GenericMenu();
        menu.AddItem(new GUIContent("Create Room Node"), false, CreateRoomNode, mouseposition);
        menu.AddSeparator("");
        menu.AddItem(new GUIContent("Select All Room Nodes"), false, SelectAllRoomNodes);
        menu.AddSeparator("");
        menu.AddItem(new GUIContent("Delete Selected Room Node Links"), false, DeleteSelectedRoomNodeLinks);
        menu.AddItem(new GUIContent("Delete Selected Room Nodes"), false, DeleteSelectedRoomNodes);
        menu.ShowAsContext();
    }

    #region 검토

    /// <summary>
    /// delete the links between the selected room node
    /// </summary>
    private void DeleteSelectedRoomNodeLinks()
    {
        //iterate through all room node
        foreach(RoomNodeSO roomNode in currentRoomNodeGraph.roomNodeList)
        {
            if(roomNode.isSelected && roomNode.childRoomNodeIDList.Count > 0)
            {
                for(int i = roomNode.childRoomNodeIDList.Count -1; i>=0; i--)
                {
                    //Get child room node
                    RoomNodeSO childRoomNode = currentRoomNodeGraph.GetRoomNode(roomNode.childRoomNodeIDList[i]);

                    //if the child room node is selected
                    if(childRoomNode != null && childRoomNode.isSelected)
                    {
                        //remove childID from parent room node
                        roomNode.RemoveChildRoomNodeIDFromRoomNode(childRoomNode.id);

                        //remove parentID from child room node
                        childRoomNode.RemoveChildRoomNodeIDFromRoomNode(roomNode.id);
                    }
                }
            }
        }

        //Clear all selected room node
        ClearAllSelectedRoomNode();
    }

    /// <summary>
    /// create room node at mouse position
    /// </summary>
    /// <param name="mousepostionObject"></param>
    private void CreateRoomNode(object mousepostionObject)
    {
        //if current node graph empty then add entrace room node first
        if(currentRoomNodeGraph.roomNodeList.Count == 0)
        {
            CreateRoomNode(new Vector2(200f, 200f), roomNodeTypeList.list.Find(x => x.isEntrance));
        }

        CreateRoomNode(mousepostionObject, roomNodeTypeList.list.Find(x => x.isNone));
    }

    /// <summary>
    /// crate a room node at the mouse position - overloaded to also pass in RoomNodeType
    /// </summary>
    /// <param name="mousepostionObject"></param>
    /// <param name="roomNodeType"></param>
    private void CreateRoomNode(object mousepostionObject, RoomNodeTypeSO roomNodeType)
    {
        Vector2 mousePosition = (Vector2)mousepostionObject;

        //create room node scriptable object asset
        RoomNodeSO roomNode = ScriptableObject.CreateInstance<RoomNodeSO>();

        //add room node to current room node graph room node list
        currentRoomNodeGraph.roomNodeList.Add(roomNode);

        //set room node values
        roomNode.Initialise(new Rect(mousePosition, new Vector2(nodeWidth, nodeHeight)), currentRoomNodeGraph, roomNodeType);

        //add room node to room node graph scriptable object asset database;
        AssetDatabase.AddObjectToAsset(roomNode, currentRoomNodeGraph);

        AssetDatabase.SaveAssets();

        //refresh graph node dictionary
        currentRoomNodeGraph.OnValidate();
    }
    #endregion

    /// <summary>
    /// delete slected room node
    /// </summary>
    private void DeleteSelectedRoomNodes()
    {
        Queue<RoomNodeSO> roomNodeDeletionQueue = new Queue<RoomNodeSO>();
        
        //loop through all node
        foreach(RoomNodeSO roomNode in currentRoomNodeGraph.roomNodeList)
        {
            if(roomNode.isSelected && !roomNode.roomNodeType.isEntrance)
            {
                roomNodeDeletionQueue.Enqueue(roomNode);

                //iterate through child room node ID
                foreach(string childRoomNodeID in roomNode.childRoomNodeIDList)
                {
                    //Retrievs child room node
                    RoomNodeSO childRoomNode = currentRoomNodeGraph.GetRoomNode(childRoomNodeID);
                    
                    if(childRoomNode != null)
                    {
                        childRoomNode.RemoveParentRoomNodeIDFromRoomNode(roomNode.id);
                    }
                }

                //iterate through parent room node id
                foreach(string parentRoomNodeID in roomNode.parentRoomNodeIDList)
                {
                    //rstrieve parent node
                    RoomNodeSO parentRoomNode = currentRoomNodeGraph.GetRoomNode(parentRoomNodeID);
                }
            }
        }

        //delete queued room node
        while(roomNodeDeletionQueue.Count > 0)
        {
            //get room node from queue
            RoomNodeSO roomNodeToDelete = roomNodeDeletionQueue.Dequeue();

            //remove mode form dictonary
            currentRoomNodeGraph.roomNodeDictonary.Remove(roomNodeToDelete.id);

            //remove node from list
            currentRoomNodeGraph.roomNodeList.Remove(roomNodeToDelete);

            //remove node from asset database
            DestroyImmediate(roomNodeToDelete, true);

            AssetDatabase.SaveAssets();
        }
    }

    /// <summary>
    /// clear selected from all room node
    /// </summary>
    private void ClearAllSelectedRoomNode()
    {
        foreach(RoomNodeSO roomNode in currentRoomNodeGraph.roomNodeList)
        {
            if (roomNode.isSelected)
            {
                roomNode.isSelected = false;

                GUI.changed = true;
            }
        }
    }

    /// <summary>
    /// select all room node
    /// </summary>
    private void SelectAllRoomNodes()
    {
        foreach(RoomNodeSO roomNode in currentRoomNodeGraph.roomNodeList)
        {
            roomNode.isSelected = true;
        }
        GUI.changed = true;
    }

    /// <summary>
    /// process mouse up event
    /// </summary>
    /// <param name="currentEvent"></param>
    private void ProcessMouseUpEvent(Event currentEvent)
    {
        //if releasing the right mouse button and currently dragging a line
        if(currentEvent.button==1 && currentRoomNodeGraph.roomNodeToDrawLineFrom != null)
        {
            //click if over a room node
            RoomNodeSO roomNode = IsMouseOverRoomNode(currentEvent);
            
            if(roomNode != null)
            {
                //if so set it as a child of the parent room node if it can be added
                if (currentRoomNodeGraph.roomNodeToDrawLineFrom.AddChildRoomNodeIDToRoomNode(roomNode.id))
                {
                    //set parent ID in child room node
                    roomNode.AddParentRoomNodeIDToRoomNode(currentRoomNodeGraph.roomNodeToDrawLineFrom.id);
                }
            }

            ClearLineDrag();
        }
    }

    /// <summary>
    /// process drag event
    /// </summary>
    private void ProcessMouseDragEvent(Event currentEvent)
    {
        //process right click drag event - draw line
        if (currentEvent.button == 1)
        {
            ProcessRightDragEvent(currentEvent);
        }
        //process left click drag event - drag node graph
        else if (currentEvent.button == 0)
        {
            ProcessLeftMouseDragEvent(currentEvent.delta);
        }
    }

    /// <summary>
    /// process right mouse drag event - draw line
    /// </summary>
    /// <param name="currentEvent"></param>
    private void ProcessRightDragEvent(Event currentEvent)
    {
        if(currentRoomNodeGraph.roomNodeToDrawLineFrom != null)
        {
            DragConnectingLine(currentEvent.delta);
            GUI.changed = true;
        }
    }

    /// <summary>
    /// process left mouse drag event - drag room node graph
    /// </summary>
    /// <param name="delta"></param>
    private void ProcessLeftMouseDragEvent(Vector2 dragDelta)
    {
        graphDrag = dragDelta;

        for (int i = 0; i < currentRoomNodeGraph.roomNodeList.Count; i++)
        {
            currentRoomNodeGraph.roomNodeList[i].DragNode(dragDelta);
        }

        GUI.changed = true;
    }

    /// <summary>
    /// Draw connection in the graph window between room node
    /// </summary>
    private void DrawRoomConnection()
    {
        foreach(RoomNodeSO roomNode in currentRoomNodeGraph.roomNodeList)
        {
            //Loop through all room node
            if (roomNode.childRoomNodeIDList.Count > 0)
            {
                //Loop through child room node
                foreach(string childRoomNodeID in roomNode.childRoomNodeIDList)
                {
                    if (currentRoomNodeGraph.roomNodeDictonary.ContainsKey(childRoomNodeID))
                    {
                        DrawConnectionLine(roomNode, currentRoomNodeGraph.roomNodeDictonary[childRoomNodeID]);
                        
                        GUI.changed = true;
                    }
                }
            }
        }
    }

    /// <summary>
    /// Draw connection line between the parent room node and child room node
    /// </summary>
    /// <param name="parentRoomNode"></param>
    /// <param name="childRoomNode"></param>
    private void DrawConnectionLine(RoomNodeSO parentRoomNode, RoomNodeSO childRoomNode)
    {
        //get line start and end position
        Vector2 startPosition = parentRoomNode.rect.center;
        Vector2 endPosition = childRoomNode.rect.center;

        //calculate midway point
        Vector2 midPosition = (endPosition + startPosition) / 2f;

        //vector from start to end position of line
        Vector2 direction = endPosition - startPosition;

        //calculate nomalised perpendicular positions from the mid point
        Vector2 arrowTailPoint1 = midPosition - new Vector2(-direction.y, direction.x).normalized * connectingLineArrowSize;
        Vector2 arrowTailPoint2 = midPosition + new Vector2(-direction.y, direction.x).normalized * connectingLineArrowSize;

        //calculate mid point offset position for arrow head
        Vector2 arrowHeadPoint = midPosition + direction.normalized * connectingLineArrowSize;

        //draw arrow
        Handles.DrawBezier(arrowHeadPoint, arrowTailPoint1, arrowHeadPoint, arrowTailPoint1, Color.white, null, connectingLineWidth);
        Handles.DrawBezier(arrowHeadPoint, arrowTailPoint2, arrowHeadPoint, arrowTailPoint2, Color.white, null, connectingLineWidth);
        
        //draw line
        Handles.DrawBezier(startPosition, endPosition, startPosition, endPosition, Color.white, null, connectingLineWidth);

        GUI.changed = true;
    }

    /// <summary>
    /// drag connecting line from room node
    /// </summary>
    /// <param name="delta"></param>
    private void DragConnectingLine(Vector2 delta)
    {
        currentRoomNodeGraph.linePosition += delta;
    }

    private void ClearLineDrag()
    {
        currentRoomNodeGraph.roomNodeToDrawLineFrom = null;
        currentRoomNodeGraph.linePosition = Vector2.zero;
        GUI.changed = true;
    }

    /// <summary>
    /// Draw room node in the graph window
    /// </summary>
    private void DrawRoomNode()
    {
        //loop through all room nodes and draw them
        foreach(RoomNodeSO roomNode in currentRoomNodeGraph.roomNodeList)
        {
            if (roomNode.isSelected)
            {
                roomNode.Draw(roomNodeSelectedStyle);
            }
            else
            {
                roomNode.Draw(roomNodeStyle);
            }
        }

        GUI.changed = true;
    }

    /// <summary>
    /// selection changed in the inspector
    /// </summary>
    private void InspectorSelectionChanged()
    {
        RoomNodeGraphSO roomNodeGraph = Selection.activeObject as RoomNodeGraphSO;

        if(roomNodeGraph != null)
        {
            currentRoomNodeGraph = roomNodeGraph;
            GUI.changed = true;
        }
    }
}
