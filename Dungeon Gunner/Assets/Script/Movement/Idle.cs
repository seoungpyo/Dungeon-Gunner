using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(IdleEvent))]
public class Idle : MonoBehaviour
{
    private Rigidbody2D rigidBody2D;
    private IdleEvent idleEvent;

    private void Awake()
    {
        // load component
        rigidBody2D = GetComponent<Rigidbody2D>();
        idleEvent = GetComponent<IdleEvent>();
    }

    private void OnEnable()
    {
        // subsrible to idle event
        idleEvent.OnIdle += IdleEvent_OnIdle;
    }

    private void OnDisable()
    {
        // unsubscirble to idle event
        idleEvent.OnIdle -= IdleEvent_OnIdle;
    }

    private void IdleEvent_OnIdle(IdleEvent idleEvent)
    {
        MoveRigidBody();
    }

    private void MoveRigidBody()
    {
        // ensure the rb collision detection is set to continuous
        rigidBody2D.velocity = Vector2.zero;
    }
}
