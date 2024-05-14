using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class MovementByVelocityEvent : MonoBehaviour
{
    public event Action<MovementByVelocityEvent, MovementByVelocityArgs> OnMovementByVelocity;

    public void CallMovementByVElocityEvent(Vector2 moveDirection, float moveSpeed)
    {
        OnMovementByVelocity?.Invoke(this, new MovementByVelocityArgs()
        {
            moveDirection = moveDirection,
            moveSpeed = moveSpeed
        });
    }
}

public class MovementByVelocityArgs : EventArgs
{
    public Vector2 moveDirection;
    public float moveSpeed;
}
