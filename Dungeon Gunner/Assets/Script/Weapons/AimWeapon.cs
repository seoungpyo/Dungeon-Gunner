using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

[RequireComponent(typeof(AimWeaponEvent))]
[DisallowMultipleComponent]
public class AimWeapon : MonoBehaviour
{
    #region Tooltip
    [Tooltip("Populate with the Transform from the child WeaponRotationPoint gameobject")]
    #endregion Tooltip
    [SerializeField] private Transform weaponRotationPointTransform;

    private AimWeaponEvent aimWeaponEvent;

    private void Awake()
    {
        // load component
        aimWeaponEvent = GetComponent<AimWeaponEvent>();
    }

    private void OnEnable()
    {
        // subscribe to aim weapon event
        aimWeaponEvent.OnWeaponAim += AimWeaponEvent_OnWeaponAim;
    }

    private void OnDisable()
    {
        // Unsubscribe from aim weapon event
        aimWeaponEvent.OnWeaponAim -= AimWeaponEvent_OnWeaponAim;
    }

    /// <summary>
    /// Aim weapon event handler
    /// </summary>
    /// <param name="aimWeaponEvent"></param>
    /// <param name="aimWeaponEventArgs"></param>
    private void AimWeaponEvent_OnWeaponAim(AimWeaponEvent aimWeaponEvent, AimWeaponEventArgs aimWeaponEventArgs)
    {
        Aim(aimWeaponEventArgs.aimDirection, aimWeaponEventArgs.aimAngle);
    }

    /// <summary>
    /// Aim the weapon
    /// </summary>
    /// <param name="aimDirection"></param>
    /// <param name="aimAngle"></param>
    private void Aim(AimDirection aimDirection, float aimAngle)
    {
        // Set angle of the weapon transform
        weaponRotationPointTransform.eulerAngles = new Vector3(0f, 0f, aimAngle);

        // filp weapon transform based on player direction
        switch (aimDirection)
        {
            case AimDirection.Left:
            case AimDirection.UpLeft:
                weaponRotationPointTransform.localScale = new Vector3(1f, -1f, 0f);
                break;

            case AimDirection.Right:
            case AimDirection.UpRight:
            case AimDirection.Down:
            case AimDirection.Up:
                weaponRotationPointTransform.localScale = new Vector3(1f, 1f, 0);
                break;
        }
    }

    #region Validation
#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilitie.ValidateCheckNullValue(this, nameof(weaponRotationPointTransform), weaponRotationPointTransform);
    }
#endif
    #endregion Validation
}
