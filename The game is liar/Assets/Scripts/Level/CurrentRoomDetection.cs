﻿using UnityEngine.Events;
using UnityEngine;
using ProceduralLevelGenerator.Unity.Generators.Common.Rooms;

public class CurrentRoomDetection : MonoBehaviour
{
    public UnityEvent updateCurrentRoom;
    public RoomInstanceVariable currentRoom;
    public BoundsIntVariable currentBounds;
    RoomInstance room;

    void Start()
    {
        RoomInstance baseRoom = GetComponentInParent<RoomInfo>().RoomInstance;
        Vector2Int facingDir = baseRoom.Doors[0].FacingDirection;
        int i = 0;
        if      (transform.name == "Left"  && facingDir == Vector2Int.right)
        {
            i = 1;
        }
        else if (transform.name == "Right" && facingDir == Vector2Int.left)
        {
            i = 1;
        }
        else if (transform.name == "Up"    && facingDir == Vector2Int.down)
        {
            i = 1;
        }
        else if (transform.name == "Down"  && facingDir == Vector2Int.up)
        {
            i = 1;
        }
        room = baseRoom.Doors[i].ConnectedRoomInstance;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (currentRoom.value != room)
            {
                currentRoom.value = room;
                currentBounds.value = RoomManager.GetRoomBounds(room);
                updateCurrentRoom?.Invoke(); 
            }
        }
    }
}
