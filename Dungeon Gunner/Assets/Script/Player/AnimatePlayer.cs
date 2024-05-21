using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Player))]
[DisallowMultipleComponent]
public class AnimatePlayer : MonoBehaviour
{
    private Player player;

    private void Awake()
    {
        // load component
        player = GetComponent<Player>();

    }

    private void OnEnable()
    {
        // Subscribe to movement by velocity event
        player.movementByVelocityEvent.OnMovementByVelocity += MovementByVelocityEvent_OnMovementByVelocity;

        // Subscribe to movement to postion event
        player.movementToPositionEvent.OnMovementToPosition += MovementToPositionEvent_OnMovementToPosition;

        // Subscribe to idle event
        player.idleEvent.OnIdle += IdleEvent_OnIdle;

        // subscribe to aim event
        player.aimWeaponEvent.OnWeaponAim += AimWeaponEvent_OnWeaponAim;

    }

    private void OnDisable()
    {
        // Unsubscribe to movement by velocity event
        player.movementByVelocityEvent.OnMovementByVelocity -= MovementByVelocityEvent_OnMovementByVelocity;

        // Unsubscribe to movement to postion event
        player.movementToPositionEvent.OnMovementToPosition -= MovementToPositionEvent_OnMovementToPosition;

        // Unsubscrible to idle event
        player.idleEvent.OnIdle -= IdleEvent_OnIdle;

        // Unsubscrible from weapon aim event 
        player.aimWeaponEvent.OnWeaponAim -= AimWeaponEvent_OnWeaponAim;
    }

    /// <summary>
    /// On movement by velocity event handler
    /// </summary>
    /// <param name="movementByVelocityEvent"></param>
    /// <param name="movementByVelocityArgs"></param>
    private void MovementByVelocityEvent_OnMovementByVelocity(MovementByVelocityEvent movementByVelocityEvent,
        MovementByVelocityArgs movementByVelocityArgs)
    {
        InitalizeRollAnimationParameters();
        SetMovementAnimationParameters();
    }

    /// <summary>
    /// On movement to postion event
    /// </summary>
    /// <param name="movementToPositionEvent"></param>
    /// <param name="movementToPositionArgs"></param>
    private void MovementToPositionEvent_OnMovementToPosition(MovementToPositionEvent movementToPositionEvent,
        MovementToPositionArgs movementToPositionArgs)
    {
        InitalizeAimAnimationParameters();
        InitalizeRollAnimationParameters();
        SetMovementAnimationParameters(movementToPositionArgs);
    }

    /// <summary>
    /// On idle event handler
    /// </summary>
    /// <param name="idleEvent"></param>
    private void IdleEvent_OnIdle(IdleEvent idleEvent)
    {
        InitalizeRollAnimationParameters();
        SetIdleAnimationParameters();
    }

    /// <summary>
    /// On weapon Aim event handler
    /// </summary>
    /// <param name="aimWeaponEvent"></param>
    /// <param name="aimWeaponEventArgs"></param>
    private void AimWeaponEvent_OnWeaponAim(AimWeaponEvent aimWeaponEvent, AimWeaponEventArgs aimWeaponEventArgs)
    {
        InitalizeAimAnimationParameters();
        InitalizeRollAnimationParameters();
        SetAimWeaponAnimationParameters(aimWeaponEventArgs.aimDirection);
    }

    /// <summary>
    /// Initalize aim animation parameters
    /// </summary>
    private void InitalizeAimAnimationParameters()
    {
        player.animator.SetBool(Settings.aimUp, false);
        player.animator.SetBool(Settings.aimUpRight, false);
        player.animator.SetBool(Settings.aimUpLeft, false);
        player.animator.SetBool(Settings.aimLeft, false);
        player.animator.SetBool(Settings.aimRight, false);
        player.animator.SetBool(Settings.aimDown, false);
    }

    private void InitalizeRollAnimationParameters()
    {
        player.animator.SetBool(Settings.rollDown, false);
        player.animator.SetBool(Settings.rollUp, false);
        player.animator.SetBool(Settings.rollRight, false);
        player.animator.SetBool(Settings.rollLeft, false);
    }

    /// <summary>
    /// Set aim animation parameters
    /// </summary>
    /// <param name="aimDirection"></param>
    private void SetAimWeaponAnimationParameters(AimDirection aimDirection)
    {
        // set aim direction
        switch (aimDirection)
        {
            case AimDirection.Up:
                player.animator.SetBool(Settings.aimUp, true);
                break;

            case AimDirection.UpRight:
                player.animator.SetBool(Settings.aimUpRight, true);
                break;

            case AimDirection.UpLeft:
                player.animator.SetBool(Settings.aimUpLeft, true);
                break;

            case AimDirection.Right:
                player.animator.SetBool(Settings.aimRight, true);
                break;

            case AimDirection.Left:
                player.animator.SetBool(Settings.aimLeft, true);
                break;

            case AimDirection.Down:
                player.animator.SetBool(Settings.aimDown, true);
                break;

        }
    }

    /// <summary>
    /// set movement animation parameters
    /// </summary>
    private void SetMovementAnimationParameters()
    {
        player.animator.SetBool(Settings.isMoving, true);
        player.animator.SetBool(Settings.isIdle, false);
    }

    /// <summary>
    /// Set movement to postion animation parameters
    /// </summary>
    /// <param name="movementToPositionArgs"></param>
    private void SetMovementAnimationParameters(MovementToPositionArgs movementToPositionArgs)
    {
        // Animation roll
        if (movementToPositionArgs.isRolling)
        {
            if(movementToPositionArgs.moveDirection.x > 0f)
            {
                player.animator.SetBool(Settings.rollRight,true);
            }
            else if (movementToPositionArgs.moveDirection.x < 0f)
            {
                player.animator.SetBool(Settings.rollLeft, true);
            }
            else if (movementToPositionArgs.moveDirection.y > 0f)
            {
                player.animator.SetBool(Settings.rollUp, true);
            }
            else if (movementToPositionArgs.moveDirection.y < 0f)
            {
                player.animator.SetBool(Settings.rollDown, true);
            }

        }
    }

    /// <summary>
    /// set movement animation parameters
    /// </summary>
    private void SetIdleAnimationParameters()
    {
        player.animator.SetBool(Settings.isMoving, false);
        player.animator.SetBool(Settings.isIdle, true);
    }
}
