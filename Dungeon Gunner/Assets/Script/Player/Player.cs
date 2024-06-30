using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderKeywordFilter;
using UnityEngine;
using UnityEngine.Rendering;

#region REQUIRE COMPONENTS
[RequireComponent(typeof(Health))]
[RequireComponent(typeof(PlayerControl))]
[RequireComponent(typeof(MovementByVelocityEvent))]
[RequireComponent(typeof(MovementByVelocity))]
[RequireComponent(typeof(MovementToPositionEvent))]
[RequireComponent(typeof(MovementToPosition))]
[RequireComponent(typeof(IdleEvent))]
[RequireComponent(typeof(Idle))]
[RequireComponent(typeof(AimWeaponEvent))]
[RequireComponent(typeof(AimWeapon))]
[RequireComponent(typeof(SetActiveWeaponEvent))]
[RequireComponent(typeof(SetActiveWeapon))]
[RequireComponent(typeof(SortingGroup))] // Layer renderer squence controll component.
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(AnimatePlayer))]
[RequireComponent(typeof(BoxCollider2D))] // define collider for interacting with the dungeon map.
[RequireComponent(typeof(PolygonCollider2D))] // define collider for interacting with enemy attacks.
[RequireComponent(typeof(Rigidbody2D))]
[DisallowMultipleComponent] // exit component only one.
#endregion REQUIRE COMPONENTS

public class Player : MonoBehaviour
{
    [HideInInspector] public PlayerDetailsSO playerDetails;
    [HideInInspector] public Health health;
    [HideInInspector] public MovementByVelocityEvent movementByVelocityEvent;
    [HideInInspector] public MovementToPositionEvent movementToPositionEvent;
    [HideInInspector] public IdleEvent idleEvent;
    [HideInInspector] public AimWeaponEvent aimWeaponEvent;
    [HideInInspector] public SetActiveWeaponEvent setActiveWeaponEvent;
    [HideInInspector] public SetActiveWeapon setActiveWeapon;
    [HideInInspector] public SpriteRenderer spriteRenderer;
    [HideInInspector] public Animator animator;

    public List<Weapon> weaponList = new List<Weapon>();

    private void Awake()
    {
        // load component
        health = GetComponent<Health>();
        movementByVelocityEvent = GetComponent<MovementByVelocityEvent>();
        movementToPositionEvent = GetComponent<MovementToPositionEvent>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        idleEvent = GetComponent<IdleEvent>();
        aimWeaponEvent = GetComponent<AimWeaponEvent>();
        setActiveWeaponEvent = GetComponent<SetActiveWeaponEvent>();
        setActiveWeapon = GetComponent<SetActiveWeapon>();
    }

    /// <summary>
    ///  Initalize the player
    /// </summary>
    /// <param name="playerDetails"></param>
    public void Initalize(PlayerDetailsSO playerDetails)
    {
        this.playerDetails = playerDetails;

        // Craete player starting weapons
        CreatePlayerStartingWeapon();

        // set player starting health
        SetPlayerHealth();
    }

    /// <summary>
    /// Set the player starting weapon
    /// </summary>
    private void CreatePlayerStartingWeapon()
    {
        // clear list
        weaponList.Clear();

        // populate weapon list from starting weapons
        foreach(WeaponDetailsSO weaponDetails in playerDetails.startingWeaponList)
        {
            // add weapon to player
            AddWeaponToPlayer(weaponDetails);
        }
    }

    /// <summary>
    /// Set player health from playerDetails SO
    /// </summary>
    private void SetPlayerHealth()
    {
        health.SetStartingHealth(playerDetails.playerHealthAmount);
    }

    /// <summary>
    /// Add a weapon to the player weapon dictionary
    /// </summary>
    /// <param name="weaponDetails"></param>
    /// <returns></returns>
    public Weapon AddWeaponToPlayer(WeaponDetailsSO weaponDetails)
    {
        Weapon weapon = new Weapon()
        {
            weaponDetails = weaponDetails,
            weaponReloadTimer = 0f,
            weaponClipRemainingAmmo = weaponDetails.weaponClipAmmoCapacity,
            weaponRemainingAmmo = weaponDetails.weaponAmmoCapacity,
            isWeaponReloading = false
        };

        // add the weapon to the list
        weaponList.Add(weapon);

        // set weapon position in list
        weapon.weaponListPosition = weaponList.Count;

        // set the added weapon as active
        setActiveWeaponEvent.CallSetActiveWeaponEvent(weapon);

        return weapon;
    }
}

