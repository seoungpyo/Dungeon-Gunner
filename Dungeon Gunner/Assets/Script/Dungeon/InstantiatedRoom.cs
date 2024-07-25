using System.Collections;
using System.Collections.Generic;
using System.Drawing.Printing;
using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(BoxCollider2D))]
public class InstantiatedRoom : MonoBehaviour
{
    [HideInInspector] public Room room;
    [HideInInspector] public Grid grid;
    [HideInInspector] public Tilemap groundTilemap;
    [HideInInspector] public Tilemap decoration1Tilemap;
    [HideInInspector] public Tilemap decoration2Tilemap;
    [HideInInspector] public Tilemap frontTilemap;
    [HideInInspector] public Tilemap collisionTilemap;
    [HideInInspector] public Tilemap minimapTilemap;
    [HideInInspector] public int[,] aStarMovementPenalty;
    [HideInInspector] public Bounds roomColliderBounds;

    private BoxCollider2D boxCollider2D;

    private void Awake()
    {
        boxCollider2D = GetComponent<BoxCollider2D>();

        // save room collider bounds
        roomColliderBounds = boxCollider2D.bounds;
    }

    // Trigger room changed event when player enters a room
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //if the player triggerd the collider
        if(collision.tag == Settings.playerTag && room != GameManager.Instance.GetCurrentRoom())
        {
            // set room as visited
            this.room.isPreviouslyVisited = true;

            StaticEventHandler.CallRoomChangedEvent(room);
        }
        
    }

    /// <summary>
    /// Initalise The Instantiated Room
    /// </summary>
    /// <param name="roomGameobject"></param>
    public void Initialise(GameObject roomGameobject)
    {
        PopulateTilemapMemberVariables(roomGameobject);

        BlockOffUnusedDoorways();

        AddObstaclesAndPreferredPaths();

        AddDoorsToRoom();

        DisableCollisionTilemapRenderer();
    }

    /// <summary>
    /// Populate the tilemap and grid member variables.
    /// </summary>
    /// <param name="roomGameobject"></param>
    private void PopulateTilemapMemberVariables(GameObject roomGameobject)
    {
        // get the grid component.
        grid = roomGameobject.GetComponentInChildren<Grid>();

        // get tilemap in children.
        Tilemap[] tilemaps = roomGameobject.GetComponentsInChildren<Tilemap>();

        foreach(Tilemap tilemap in tilemaps)
        {
            if(tilemap.gameObject.tag == "groundTilemap")
            {
                groundTilemap = tilemap;
            }
            else if (tilemap.gameObject.tag == "decoration1Tilemap")
            {
                decoration1Tilemap = tilemap;
            }
            else if (tilemap.gameObject.tag == "decoration2Tilemap")
            {
                decoration2Tilemap = tilemap;
            }
            else if (tilemap.gameObject.tag == "frontTilemap")
            {
                frontTilemap = tilemap;
            }
            else if (tilemap.gameObject.tag == "collisionTilemap")
            {
                collisionTilemap = tilemap;
            }
            else if (tilemap.gameObject.tag == "minimapTilemap")
            {
                minimapTilemap = tilemap;
            }
        }

    }

    /// <summary>
    /// block off unused doorway in the room
    /// </summary>
    private void BlockOffUnusedDoorways()
    {
        // loop through all doorways
        foreach(Doorway doorway in room.doorWayList)
        {
            if (doorway.isConnected)
            {
                continue;
            }

            if(collisionTilemap != null)
            {
                BlockADoorwayOnTilemapLayer(collisionTilemap, doorway);
            }

            if (minimapTilemap != null)
            {
                BlockADoorwayOnTilemapLayer(minimapTilemap, doorway);
            }

            if (groundTilemap != null)
            {
                BlockADoorwayOnTilemapLayer(groundTilemap, doorway);
            }

            if (frontTilemap != null)
            {
                BlockADoorwayOnTilemapLayer(frontTilemap, doorway);
            }

            if (decoration1Tilemap != null)
            {
                BlockADoorwayOnTilemapLayer(decoration1Tilemap, doorway);
            }

            if (decoration2Tilemap != null)
            {
                BlockADoorwayOnTilemapLayer(decoration2Tilemap, doorway);
            }
        }
    }

    /// <summary>
    /// Block a doorway on a tilemap layer
    /// </summary>
    /// <param name="tilemap"></param>
    /// <param name="doorway"></param>
    private void BlockADoorwayOnTilemapLayer(Tilemap tilemap, Doorway doorway)
    {
        switch (doorway.orientation)
        {
            case Orientation.north:
            case Orientation.south:
                BlockDoorwayHorizontally(tilemap, doorway);
                break;

            case Orientation.east:
            case Orientation.west:
                BlockDoorwayVertically(tilemap, doorway);
                break;

            case Orientation.none:
                break;

            default:
                break;
        }
    }

    /// <summary>
    /// Block doorway horizontally - for North and South doorways
    /// </summary>
    /// <param name="tilemap"></param>
    /// <param name="doorway"></param>
    private void BlockDoorwayHorizontally(Tilemap tilemap, Doorway doorway)
    {
        Vector2Int startPostion = doorway.doorwayStartCopyPosition;

        // loop through all tile to copy
        for (int xPos = 0; xPos < doorway.doorwayCopyTileWidth; xPos++)
        {
            for(int yPos =0; yPos<doorway.doorwayCopyTileHeight; yPos++)
            {
                // get rotation of tile being copied
                Matrix4x4 transformMatrix = tilemap.GetTransformMatrix(new Vector3Int(startPostion.x + xPos, startPostion.y + yPos, 0));

                // copy tile
                tilemap.SetTile(new Vector3Int(startPostion.x + 1 + xPos, startPostion.y - yPos, 0), tilemap.GetTile(new Vector3Int(startPostion.x + xPos,
                    startPostion.y - yPos, 0)));

                // set rotation of tile copied
                tilemap.SetTransformMatrix(new Vector3Int(startPostion.x + 1 + xPos, startPostion.y - yPos, 0), transformMatrix);
            }
        }
    }

    /// <summary>
    /// Block doorway vertically - for East and West doorways
    /// </summary>
    /// <param name="tilemap"></param>
    /// <param name="doorway"></param>
    private void BlockDoorwayVertically(Tilemap tilemap, Doorway doorway)
    {
        Vector2Int startPostion = doorway.doorwayStartCopyPosition;

        // loop through all tiles to copy
        for(int yPos = 0; yPos< doorway.doorwayCopyTileHeight; yPos++)
        {
            for(int xPos = 0; xPos< doorway.doorwayCopyTileWidth; xPos++)
            {
                // get rotation of tile being copied
                Matrix4x4 transformMatrix = tilemap.GetTransformMatrix(new Vector3Int(startPostion.x + xPos, startPostion.y - yPos, 0));
                
                // copy tile
                tilemap.SetTile(new Vector3Int(startPostion.x + xPos, startPostion.y - 1 - yPos, 0), tilemap.GetTile(new Vector3Int(startPostion.x + xPos,
                    startPostion.y - yPos, 0)));

                // set rotaition of tile copied
                tilemap.SetTransformMatrix(new Vector3Int(startPostion.x + xPos, startPostion.y - 1 - yPos), transformMatrix);
            }
        }
    }

    /// <summary>
    /// Add opening doors if this is not a corridor room
    /// </summary>
    private void AddDoorsToRoom()
    {
        // if the room is a corridor then return
        if (room.roomNodeType.isCorridorEw || room.roomNodeType.isCorridorNs) return;
    
        // Instantiate door prefabs at doorway position
        foreach(Doorway doorway in room.doorWayList)
        {
            // if the doorway prefabs isnt null and the doorway is connected
            if(doorway.doorPrefab != null && doorway.isConnected)
            {
                float tileDistance = Settings.tileSizePixels / Settings.pixelsPerUnit;

                GameObject door = null;

                if(doorway.orientation == Orientation.north)
                {
                    // create  door with parent as the room
                    door = Instantiate(doorway.doorPrefab, gameObject.transform);
                    door.transform.localPosition = new Vector3(doorway.position.x + tileDistance / 2f, doorway.position.y + tileDistance, 0f);
                }
                else if(doorway.orientation == Orientation.south)
                {
                    // create door with parent as the room
                    door = Instantiate(doorway.doorPrefab, gameObject.transform);
                    door.transform.localPosition = new Vector3(doorway.position.x + tileDistance / 2f, doorway.position.y, 0f); 
                }
                else if (doorway.orientation == Orientation.east)
                {
                    // create door with parent as the room
                    door = Instantiate(doorway.doorPrefab, gameObject.transform);
                    door.transform.localPosition = new Vector3(doorway.position.x + tileDistance, doorway.position.y + tileDistance * 1.25f, 0f);
                }
                else if (doorway.orientation == Orientation.west)
                {
                    // create door with parent as the room
                    door = Instantiate(doorway.doorPrefab, gameObject.transform);
                    door.transform.localPosition = new Vector3(doorway.position.x, doorway.position.y + tileDistance * 1.25f, 0f);
                }

                Door doorComponent = door.GetComponent<Door>();

                // set if door is part of a boos room
                if (room.roomNodeType.isBossRoom)
                {
                    doorComponent.isBossRoomDoor = true;

                    // lock the door to prevent acces to the room
                    doorComponent.LockDoor();
                }
            }

        }
    }

    /// <summary>
    /// Update obstacles used by Astar pathfinding
    /// </summary>
    private void AddObstaclesAndPreferredPaths()
    {
        aStarMovementPenalty = new int[room.templateUpperBounds.x - room.templateLowerBounds.x + 1, 
            room.templateUpperBounds.y - room.templateLowerBounds.y + 1];

        for(int x = 0; x<(room.templateUpperBounds.x-room.templateLowerBounds.x +1); x++)
        {
            for (int y = 0; y < (room.templateUpperBounds.y - room.templateLowerBounds.y + 1); y++)
            {
                aStarMovementPenalty[x, y] = Settings.defaultAStarMovementPenalty;

                TileBase tile = collisionTilemap.GetTile(new Vector3Int(x + room.templateLowerBounds.x, y + room.templateLowerBounds.y, 0));

                foreach(TileBase collisionTile in GameResources.Instance.enemyUnwalkableCollisionTilesArray)
                {
                    if(tile == collisionTile)
                    {
                        aStarMovementPenalty[x, y] = 0;
                        break;
                    }
                }

                if(tile == GameResources.Instance.preferredEnemyPathTile)
                {
                    aStarMovementPenalty[x, y] = Settings.preferredPathAStarMovementPenalty;
                }
            }
        }
    }

    /// <summary>
    /// Disable collision Tilemap renderer
    /// </summary>
    private void DisableCollisionTilemapRenderer()
    {
        // disable collision tilemap renderer
        collisionTilemap.gameObject.GetComponent<TilemapRenderer>().enabled = false;
    }

    /// <summary>
    /// Disable the room trigger collider that is used to trigger when the player enters a room
    /// </summary>
    public void DisableRoomCollider()
    {
        boxCollider2D.enabled = false;
    }
    
    /// <summary>
    /// Lock the room door
    /// </summary>
    public void LockDoors()
    {
        Door[] doorArray = GetComponentsInChildren<Door>();

        foreach (Door door in doorArray)
        {
            door.LockDoor();
        }

        DisableRoomCollider();
    }

}
