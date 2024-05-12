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
        // Subscrible to idle event
        player.idleEvent.OnIdle += IdleEvent_OnIdle;

        // subscrible to aim event
        player.aimWeaponEvent.OnWeaponAim += AimWeaponEvent_OnWeaponAim;

    }

    private void OnDisable()
    {
        // unsubscrible to idle event
        player.idleEvent.OnIdle -= IdleEvent_OnIdle;

        // unsubscrible from weapon aim event 
        player.aimWeaponEvent.OnWeaponAim -= AimWeaponEvent_OnWeaponAim;
    }

    /// <summary>
    /// On idle event handler
    /// </summary>
    /// <param name="idleEvent"></param>
    private void IdleEvent_OnIdle(IdleEvent idleEvent)
    {
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
    private void SetIdleAnimationParameters()
    {
        player.animator.SetBool(Settings.isMoving, false);
        player.animator.SetBool(Settings.isIdle, true);
    }
}
