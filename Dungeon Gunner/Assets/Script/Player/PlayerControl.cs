using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerControl : MonoBehaviour
{
    #region Tooltip
    [Tooltip("The player WeaponShootPosition gameobject in the hieracrchy")]
    #endregion Tooltip
    [SerializeField] private Transform weaponShootPosition;

    private Player player;

    private void Awake()
    {
        // load component
        player = GetComponent<Player>();
    }

    private void Update()
    {
        // process the player movement input
        MovementInput();

        // process the player weapon input
        WeaponInput();
    }

    // player movement input
    private void MovementInput()
    {
        player.idleEvent.CallIdleEvent();
    }

    // weapon input
    private void WeaponInput()
    {
        Vector3 weaponDirection;
        float weaponAngleDegrees, playerAngleDegrees;
        AimDirection playerAimDirection;

        // aim weapon input
        AimWeaponInput(out weaponDirection, out weaponAngleDegrees, out playerAngleDegrees, out playerAimDirection);
    }

    private void AimWeaponInput(out Vector3 weaponDirection, out float weaponAngleDegrees, out float playerAngleDegrees, out AimDirection playerAimDirection)
    {
        // get mouse world position
        Vector3 mouseWorldPostion = HelperUtilitie.GetMouseWorldPosition();

        // calculate direction vector of cursor from weapon shoot position
        weaponDirection = (mouseWorldPostion - weaponShootPosition.position);

        // calculate direction vector of mouse cursor from player transform position
        Vector3 playerDirection = (mouseWorldPostion - transform.position);

        // get weapon to cursor angle
        weaponAngleDegrees = HelperUtilitie.GetAngleFromVector(weaponDirection);

        // get player to cursor angle
        playerAngleDegrees = HelperUtilitie.GetAngleFromVector(playerDirection);

        // set player aim direction
        playerAimDirection = HelperUtilitie.GetAimDirection(playerAngleDegrees);

        // trigger weapon aim event
        player.aimWeaponEvent.CallAimWeaponEvent(playerAimDirection, playerAngleDegrees, weaponAngleDegrees, weaponDirection);
    }
}
