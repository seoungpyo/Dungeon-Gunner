using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Xml.Serialization;
using UnityEditor.Rendering.Universal;
using UnityEditor.Searcher;
using UnityEngine;
using UnityEngine.Android;

[DisallowMultipleComponent]
public class DungeonBuilder : SingletonMonobehaviour<DungeonBuilder>
{
    public Dictionary<string, Room> dungeonBuilderRoomDictionary = new Dictionary<string, Room>();
    private Dictionary<string, RoomTemplateSO> roomTemplateDictionary = new Dictionary<string, RoomTemplateSO>();
    private List<RoomTemplateSO> roomTemplateList = null;
    private RoomNodeTypeListSO roomNodeTypeList;
    private bool dungeonBuildSuccessful;

    private void OnEnable()
    {
        GameResources.Instance.dimmendMaterial.SetFloat("Alpha_Slider", 0f);
    }

    private void OnDisable()
    {
        GameResources.Instance.dimmendMaterial.SetFloat("Alpha_Slider", 1f);
    }

    protected override void Awake()
    {
        base.Awake();

        // Load the room node type list
        LoadRoomNodeTypeList();

        // set dimmed material to fully visible
        // GameResources.Instance.dimmendMaterial.SetFloat("Alpha_Slider", 1f);
    }

    /// <summary>
    /// load the room node type list
    /// </summary>
    private void LoadRoomNodeTypeList()
    {
        roomNodeTypeList = GameResources.Instance.roomNodeTypeList;
    }

    /// <summary>
    /// gener random dungeon, returns true if dungeon built, false if failed
    /// </summary>
    /// <param name="currentDungeonLevel"></param>
    /// <returns></returns>
    public bool GenerateDungeon(DungeonLevelSO currentDungeonLevel)
    {
        roomTemplateList = currentDungeonLevel.roomTemplateList;

        // load the scriptable object room template into the dictionary
        LoadRoomTemplateIntoDictionary();

        dungeonBuildSuccessful = false;
        int dungeonBuilderAttempts = 0;

        while (!dungeonBuildSuccessful && dungeonBuilderAttempts < Settings.maxDungeonBuildAttempts)
        {
            dungeonBuilderAttempts++;

            //select a random room node graph from the list
            RoomNodeGraphSO roomNodeGraph = SelectRandomRoomNodeGraph(currentDungeonLevel.roomNodeGraphList);

            int dungeonRebuildAttemptsForNodeGraph = 0;
            dungeonBuildSuccessful = false;

            // Loop until dungeon successfully built or more than max attempts for node graph
            while (!dungeonBuildSuccessful && dungeonRebuildAttemptsForNodeGraph <= Settings.maxDungeonRebuildAttemptsForRoomGraph)
            {
                //clear dungeon room gameobject and dugeon room dictionary
                ClearDungeon();
                dungeonRebuildAttemptsForNodeGraph++;

                // Attempt To Build A random dugeon for the selected room node graph
                dungeonBuildSuccessful = AttemptToBuildRandomDungeon(roomNodeGraph);
            }

            if (dungeonBuildSuccessful)
            {
                //instantiate room gameobjects
                InstantiateRoomGameobject();
            }

        }

        return dungeonBuildSuccessful;
    }

    /// <summary>
    /// Load the room templates into the dictionary
    /// </summary>
    private void LoadRoomTemplateIntoDictionary()
    {   
        //Clear room template dictionary
        roomTemplateDictionary.Clear();

        // load room template list into dictionary
        foreach (RoomTemplateSO roomTemplate in roomTemplateList)
        {
            if (!roomTemplateDictionary.ContainsKey(roomTemplate.guid))
            {
                roomTemplateDictionary.Add(roomTemplate.guid, roomTemplate);
            }
            else
            {
                Debug.Log("Duplicate Room Template Key In" + roomTemplateList);
            }
        }
    }

    /// <summary>
    /// select a random room node graph from the list of room node graphs
    /// </summary>
    /// <param name="roomNodeGraphList"></param>
    /// <returns></returns>
    private RoomNodeGraphSO SelectRandomRoomNodeGraph(List<RoomNodeGraphSO> roomNodeGraphList)
    {
        if (roomNodeGraphList.Count > 0)
        {
            return roomNodeGraphList[UnityEngine.Random.Range(0, roomNodeGraphList.Count)];
        }
        else
        {
            Debug.Log("NO room node graph in list");
            return null;
        }
    }

    /// <summary>
    /// attempt to randomly build the dungeon for the specified room node graph. returns true if a
    /// succesful random layout was generated, else returns false if a problem was encoutered and
    /// another attempt is required.
    /// </summary>
    /// <param name="roomNodeGraph"></param>
    /// <returns></returns>
    private bool AttemptToBuildRandomDungeon(RoomNodeGraphSO roomNodeGraph)
    {
        // create open room node queue
        Queue<RoomNodeSO> openRoomNodeQueue = new Queue<RoomNodeSO>();

        // add entrance node to room node queue from room node graph
        RoomNodeSO entranceNode = roomNodeGraph.GetRoomNode(roomNodeTypeList.list.Find(x => x.isEntrance));

        if (entranceNode != null)
        {
            openRoomNodeQueue.Enqueue(entranceNode);
        }
        else
        {
            Debug.Log("No Entrance Node");
            return false; // Dungeon Not Built
        }

        //start with no room overlaps
        bool nodeRoomOverlaps = true;

        //process open room nodes queue
        nodeRoomOverlaps = ProcessRoomInOpenRoomNodeQueue(roomNodeGraph, openRoomNodeQueue, nodeRoomOverlaps);

        // if all the room node have been processed and there hasn't been a room overlap then return true
        if(openRoomNodeQueue.Count == 0 && nodeRoomOverlaps)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// process room in the open room node queue, returning true if there are no room overlaps
    /// </summary>
    /// <param name="roomNodeGraph"></param>
    /// <param name="openRoomNodeQueue"></param>
    /// <param name="noRoomOverlaps"></param>
    /// <returns></returns>
    private bool ProcessRoomInOpenRoomNodeQueue(RoomNodeGraphSO roomNodeGraph, Queue<RoomNodeSO> openRoomNodeQueue, bool noRoomOverlaps)
    {
        //while room node in open room node queue & no room overlaps detected
        while(openRoomNodeQueue.Count >0 && noRoomOverlaps == true)
        {
            //Get next room node from open room node queue.
            RoomNodeSO roomNode = openRoomNodeQueue.Dequeue();

            //add child room node to queue from room node graph (with links to this parent room)
            foreach( RoomNodeSO childRoomNode in roomNodeGraph.GetChildRoomNode(roomNode))
            {
                openRoomNodeQueue.Enqueue(childRoomNode);
            }

            //if the room is the entrance mark as positioned and add to room dictionary
            if (roomNode.roomNodeType.isEntrance)
            {
                RoomTemplateSO roomTemplate = GetRandomRoomTemplate(roomNode.roomNodeType);

                Room room = CreateRoomFromRoomTemplate(roomTemplate, roomNode);

                room.isPositioned = true;

                // add room to room dictionary
                dungeonBuilderRoomDictionary.Add(room.id, room);
            }

            // else if the room type isn't an entrance
            else
            {
                // Else get parent room for node
                Room parentRoom = dungeonBuilderRoomDictionary[roomNode.parentRoomNodeIDList[0]];

                //set if room can be placed without overlaps
                noRoomOverlaps = CanPlaceRoomWithNoOverlaps(roomNode, parentRoom);
            }
        }

        return noRoomOverlaps;
    }

    /// <summary>
    /// Attemp to place the room node in the dungeon - if room can be placed return the room,else return null
    /// </summary>
    /// <param name="roomNode"></param>
    /// <param name="parentRoom"></param>
    /// <returns></returns>
    private bool CanPlaceRoomWithNoOverlaps(RoomNodeSO roomNode, Room parentRoom)
    {
        //initialise and assume ovelap until proven otherwise;
        bool roomOverlaps = true;

        //do while room overlaps - try to place against all available doorways of the parent until
        //the room is successfully placed without overlap.
        while (roomOverlaps)
        {
            //select random unconnected available doorway for parent
            List<Doorway> unconnectedAvailableParentDoorways = GetUnconnectedAvailableDoorways(parentRoom.doorWayList).ToList();

            if(unconnectedAvailableParentDoorways.Count == 0)
            {
                //if no more doorways to try then overlap failure.
                return false; // room overlaps
            }

            Doorway doorwayParent = unconnectedAvailableParentDoorways[UnityEngine.Random.Range(0, unconnectedAvailableParentDoorways.Count)];

            //get a random room template for room node that is consistent with the parent door orientation 
            RoomTemplateSO roomTemplate = GetRandomTemplateForRoomConsistentWithParent(roomNode, doorwayParent);

            // create room
            Room room = CreateRoomFromRoomTemplate(roomTemplate, roomNode);

            //place the room - return true if the room doesn't overlap
            if (PlaceTheRoom(parentRoom,doorwayParent, room))
            {
                // if room doesn't overlap then set to false to exit while loop
                roomOverlaps = false;

                // mark room as positioned
                room.isPositioned = true;

                // add room to dictionary
                dungeonBuilderRoomDictionary.Add(room.id, room);
            }
            else
            {
                roomOverlaps = true; 
            }
        }
        return true; // no room overlaps
    }

    /// <summary>
    /// get unconnected doorways
    /// </summary>
    /// <param name="roomDoorList"></param>
    /// <returns></returns>
    private IEnumerable<Doorway> GetUnconnectedAvailableDoorways(List<Doorway> roomDoorList)
    {
        foreach(Doorway doorway in roomDoorList)
        {
            if(!doorway.isConnected && !doorway.isUnavailable)
            {
                yield return doorway;
            }
        }
    }

    /// <summary>
    /// get random room template for room node taking into account the parent doorway orientation
    /// </summary>
    /// <param name="roomNode"></param>
    /// <param name="doorwayParent"></param>
    /// <returns></returns>
    private RoomTemplateSO GetRandomTemplateForRoomConsistentWithParent(RoomNodeSO roomNode, Doorway doorwayParent)
    {
        RoomTemplateSO roomtemplate = null;

        //if room node is a corridor then select random correct Corridor room template based on
        // parent doorway orientation
        if (roomNode.roomNodeType.isCorridor)
        {
            switch (doorwayParent.orientation)
            {
                case Orientation.north:
                case Orientation.south:
                    roomtemplate = GetRandomRoomTemplate(roomNodeTypeList.list.Find(x => x.isCorridorNs));
                    break;

                case Orientation.east:
                case Orientation.west:
                    roomtemplate = GetRandomRoomTemplate(roomNodeTypeList.list.Find(x => x.isCorridorEw));
                    break;

                case Orientation.none:
                    break;

                default:
                    break;

            }
        }
        //else select random room template
        else
        {
            roomtemplate = GetRandomRoomTemplate(roomNode.roomNodeType);
        }

        return roomtemplate;

    }

    /// <summary>
    /// place the room - returns true if the room doesn't overlap. flase otherwise
    /// </summary>
    /// <param name="parentRoom"></param>
    /// <param name="doorwayParent"></param>
    /// <param name="room"></param>
    /// <returns></returns>
    private bool PlaceTheRoom(Room parentRoom, Doorway doorwayParent, Room room)
    {
        //get current room doorway postision
        Doorway doorway = GetOppositeDoorway(doorwayParent, room.doorWayList);

        // return if no doorway in room opposite to parent doorway
        if(doorway == null)
        {
            //just mark the parent doorway as unavailable so we don't try and connect it again
            doorwayParent.isUnavailable = true;
            return false;
        }

        //calculate 'world' grid parent doorway postion
        Vector2Int parentDoorwayPostion = parentRoom.lowerBounds + doorwayParent.position - parentRoom.templateLowerBounds;

        Vector2Int adjustment = Vector2Int.zero;

        //calculate adjustment postion offset based on room doorway postion that we are trying to connect
        //(e.g. if this doorway is west then we need to add (1,0) to the east parent doorway)

        switch (doorway.orientation)
        {
            case Orientation.east:
                adjustment = new Vector2Int(-1, 0);
;                break;
            
            case Orientation.south:
                adjustment = new Vector2Int(0, 1);
                break;
            
            case Orientation.north:
                adjustment = new Vector2Int(0, -1);
                break;
            
            case Orientation.west:
                adjustment = new Vector2Int(1, 0);
                break;
           
            case Orientation.none:
                break;

            default:
                break;
        }

        //calculate  room lower bounds and upper bounds based on positioning to align with parent doorway
        room.lowerBounds = parentDoorwayPostion + adjustment + room.templateLowerBounds - doorway.position;
        room.upperBounds = room.lowerBounds + room.templateUpperBounds - room.templateLowerBounds;

        Room overlappingRoom = CheckForRoomOverlaps(room);

        if(overlappingRoom == null)
        {
            //mark doorways as connected & unavailable
            doorwayParent.isConnected = true;
            doorwayParent.isUnavailable = true;

            doorway.isConnected = true;
            doorway.isUnavailable = true;

            //return true to show rooms have been connected with no overlap
            return true;
        }
        else
        {
            //just mark the parent doorway as unavailable so we don't try and connect it again
            doorwayParent.isUnavailable = true;

            return false;
        }
        
    }

    /// <summary>
    /// Get the doorway form the doorway list that has the opposite orientation to doorway
    /// </summary>
    /// <param name="doorwayParent"></param>
    /// <param name="doorwayList"></param>
    /// <returns></returns>
    private Doorway GetOppositeDoorway(Doorway doorwayParent, List<Doorway> doorwayList)
    {
        foreach(Doorway doorwayToCheck in doorwayList)
        {
            if(doorwayParent.orientation == Orientation.east && doorwayToCheck.orientation == Orientation.west)
            {
                return doorwayToCheck;
            }
            else if(doorwayParent.orientation == Orientation.north && doorwayToCheck.orientation == Orientation.south)
            {
                return doorwayToCheck;
            }
            else if(doorwayParent.orientation == Orientation.west && doorwayToCheck.orientation == Orientation.east)
            {
                return doorwayToCheck;  
            }
            else if(doorwayParent.orientation == Orientation.south && doorwayToCheck.orientation == Orientation.north)
            {
                return doorwayToCheck;
            }
        }

        return null;
    }

    /// <summary>
    /// check for room that overlap the upper and lower bounds parameters, and if there are overlapping rooms then return room else return null
    /// </summary>
    /// <param name="room"></param>
    /// <returns></returns>
    private Room CheckForRoomOverlaps(Room roomToTest)
    {
        //iterate through all rooms
        foreach(KeyValuePair<string,Room> keyValuePair in dungeonBuilderRoomDictionary)
        {
            Room room = keyValuePair.Value;

            // skip if same room as room to test or room hasn'tbeen positioned
            if(room.id == roomToTest.id || !room.isPositioned) continue;

            //if room overlaps
            if(IsOverLappingRoom(roomToTest, room))
            {
                return room;
            }
        }

        //return
        return null;
    }

    /// <summary>
    /// check if 2 rooms overlap each other - return true if they overlap or false if they don't overlap
    /// </summary>
    /// <param name="room1"></param>
    /// <param name="room2"></param>
    /// <returns></returns>
    private bool IsOverLappingRoom(Room room1, Room room2)
    {
        bool isOverlappingX = IsOverLappingInterval(room1.lowerBounds.x, room1.upperBounds.x, room2.lowerBounds.x, room2.upperBounds.x);
        
        bool isOverlappingY = IsOverLappingInterval(room1.lowerBounds.y, room1.upperBounds.y, room2.lowerBounds.y, room2.upperBounds.y);

        // if (isOverlappingX && isOverlappingY) return true;
        // else return false;
        return isOverlappingX && isOverlappingY;
    }

    /// <summary>
    /// check if interval 1 overlaps interval 2 - this method is used by the isOverlappingRoom method
    /// </summary>
    /// <param name="imin1"></param>
    /// <param name="max1"></param>
    /// <param name="imin2"></param>
    /// <param name="max2"></param>
    /// <returns></returns>
    private bool IsOverLappingInterval(int imin1, int imax1, int imin2, int imax2)
    {
        if(Mathf.Max(imin1,imin2) <= Mathf.Min(imax1, imax2))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// Get a random room template from the roomtemplatelist that matches the roomType and return it
    /// --> return null if no matching room template found.
    /// </summary>
    /// <param name="roomNodeType"></param>
    /// <returns></returns>
    private RoomTemplateSO GetRandomRoomTemplate(RoomNodeTypeSO roomNodeType)
    {
        List<RoomTemplateSO> matchingRoomTemplateList = new List<RoomTemplateSO>();

        //loop through room template list
        foreach (RoomTemplateSO roomTemplate in roomTemplateList)
        {
            // add matching room templates
            if (roomTemplate.roomNodeType == roomNodeType)
            {
                matchingRoomTemplateList.Add(roomTemplate);
            }
        }

        // return null if list is zero
        if (matchingRoomTemplateList.Count == 0) return null;

        // select random room template from list and return
        return matchingRoomTemplateList[UnityEngine.Random.Range(0, matchingRoomTemplateList.Count)];
    }

    private Room CreateRoomFromRoomTemplate(RoomTemplateSO roomTemplate, RoomNodeSO roomNode)
    {
        //initalise room from template
        Room room = new Room();

        room.templateID = roomTemplate.guid;
        room.id = roomNode.id;
        room.prefab = roomTemplate.prefab;
        room.roomNodeType = roomTemplate.roomNodeType;
        room.lowerBounds = roomTemplate.lowerBounds;
        room.upperBounds = roomTemplate.upperBounds;
        room.spawnPositionArray = roomTemplate.spawnPositionArray;
        room.templateLowerBounds = roomTemplate.lowerBounds;
        room.templateUpperBounds = roomTemplate.upperBounds;
        room.childRoomIDList = CopyStringList(roomNode.childRoomNodeIDList);
        room.doorWayList = CopyDoorwayList(roomTemplate.doorwayList);

        //set parent ID for room
        if (roomNode.parentRoomNodeIDList.Count == 0) // isEntrance
        {
            room.parentRoomID = "";
            room.isPreviouslyVisited = true;

            // set entrance in game manager
            GameManager.Instance.SetCurrentRoom(room);
        }
        else
        {
            room.parentRoomID = roomNode.parentRoomNodeIDList[0];
        }

        return room;
    }

    /// <summary>
    /// Create deep copy of string list
    /// </summary>
    /// <param name="oldStringList"></param>
    /// <returns></returns>
    private List<string> CopyStringList(List<string> oldStringList)
    {
        List<string> newStringList = new List<string>();

        foreach (string stringValue in oldStringList)
        {
            newStringList.Add(stringValue);
        }

        return newStringList;
    }

    /// <summary>
    /// instantiate the dungeon room gameobjects from the preafabs
    /// </summary>
    private void InstantiateRoomGameobject()
    {
        // iterate through all dungeon rooms.
        foreach(KeyValuePair<string, Room> keyValuePair in dungeonBuilderRoomDictionary)
        {
            Room room = keyValuePair.Value;

            // Calculate room position (remember the room instantiatation position needs to be adjusted by the room template lower bounds)
            Vector3 roomPosition = new Vector3(room.lowerBounds.x - room.templateLowerBounds.x, room.lowerBounds.y - room.templateLowerBounds.y, 0f);

            // Instantiate room
            GameObject roomGameobject = Instantiate(room.prefab, roomPosition, Quaternion.identity, transform);

            // Get instantiated room component from instantiated prefab.
            InstantiatedRoom instantiatedRoom = roomGameobject.GetComponentInChildren<InstantiatedRoom>();

            instantiatedRoom.room = room;

            // Initalise The Instantiated Room
            instantiatedRoom.Initialise(roomGameobject);

            // Save gameobject reference.
            room.instantiatedRoom = instantiatedRoom;
        }

    }

    /// <summary>
    /// get a room template by room template id, returns null if id doesn't exist
    /// </summary>
    /// <param name="roomTemplateID"></param>
    /// <returns></returns>
    public RoomTemplateSO GetRoomTemplate(string roomTemplateID)
    {
        if(roomTemplateDictionary.TryGetValue(roomTemplateID, out RoomTemplateSO roomTemplate))
        {
            return roomTemplate;
        }
        else
        {
            return null;
        }
    }

    /// <summary>
    /// get room by roomID, if no room exists with that id return null
    /// </summary>
    /// <param name="roomID"></param>
    /// <returns></returns>
    public Room GetRoomByRoomID(string roomID)
    {
        if(dungeonBuilderRoomDictionary.TryGetValue(roomID, out Room room))
        {
            return room;
        }
        else
        {
            return null;
        }
    }

    /// <summary>
    /// Create deep copy of doorway list
    /// </summary>
    /// <param name="oldDoorwayList"></param>
    /// <returns></returns>
    private List<Doorway> CopyDoorwayList(List<Doorway> oldDoorwayList)
    {
        List<Doorway> newDoorwayList = new List<Doorway>();

        foreach(Doorway doorway in oldDoorwayList)
        {
            Doorway newDoorway = new Doorway();

            newDoorway.position = doorway.position;
            newDoorway.orientation = doorway.orientation;
            newDoorway.doorPrefab = doorway.doorPrefab;
            newDoorway.isConnected = doorway.isConnected;
            newDoorway.isUnavailable = doorway.isUnavailable;
            newDoorway.doorwayStartCopyPosition = doorway.doorwayStartCopyPosition;
            newDoorway.doorwayCopyTileHeight = doorway.doorwayCopyTileHeight;
            newDoorway.doorwayCopyTileWidth = doorway.doorwayCopyTileWidth;

            newDoorwayList.Add(newDoorway);
        }

        return newDoorwayList;
    }
  
    /// <summary>
    /// Clear dungoen room gameObject and dugeon room dictionary
    /// </summary>
    private void ClearDungeon()
    {
        //destroy instantiated room gameObject and clear dungeon manager room dictionary
        if (dungeonBuilderRoomDictionary.Count > 0)
        {
            foreach(KeyValuePair<string,Room>keyvaluePair in dungeonBuilderRoomDictionary)
            {
                Room room = keyvaluePair.Value;

                if(room.instantiatedRoom != null)
                {
                    Destroy(room.instantiatedRoom.gameObject);
                }
            }
        }

        dungeonBuilderRoomDictionary.Clear();
    }
}
