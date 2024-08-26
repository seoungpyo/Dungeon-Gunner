using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderKeywordFilter;
using UnityEngine;
using UnityEngine.Rendering;

#region REQUIRE COMPONENTS
[RequireComponent(typeof(HealthEvent))]
[RequireComponent(typeof(Destroyed))]
[RequireComponent(typeof(DestroyedEvent))]
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
[RequireComponent(typeof(FireWeaponEvent))]
[RequireComponent(typeof(FireWeapon))]
[RequireComponent(typeof(WeaponFiredEvent))]
[RequireComponent(typeof(ReloadWeaponEvent))]
[RequireComponent(typeof(ReloadWeapon))]
[RequireComponent(typeof(WeaponReloadedEvent))]
[RequireComponent(typeof(SetActiveWeaponEvent))]
[RequireComponent(typeof(SetActiveWeapon))]
[RequireComponent(typeof(SortingGroup))] // Layer renderer squence controll component.
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(AnimatePlayer))]
[RequireComponent(typeof(BoxCollider2D))] // define collider for interacting with the dungeon map.
[RequireComponent(typeof(PolygonCollider2D))] // define collider for interacting with enemy attacks.
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(ReceiveContactDamage))]
[RequireComponent(typeof(DealContactDamage))]
[DisallowMultipleComponent] // exit component only one.
#endregion REQUIRE COMPONENTS

public class Player : MonoBehaviour
{
    [HideInInspector] public PlayerDetailsSO playerDetails;
    [HideInInspector] public DestroyedEvent destroyedEvent;
    [HideInInspector] public HealthEvent healthEvent;
    [HideInInspector] public Health health;
    [HideInInspector] public MovementByVelocityEvent movementByVelocityEvent;
    [HideInInspector] public MovementToPositionEvent movementToPositionEvent;
    [HideInInspector] public IdleEvent idleEvent;
    [HideInInspector] public AimWeaponEvent aimWeaponEvent;
    [HideInInspector] public FireWeaponEvent fireWeaponEvent;
    [HideInInspector] public ReloadWeaponEvent reloadWeaponEvent;
    [HideInInspector] public SetActiveWeaponEvent setActiveWeaponEvent;
    [HideInInspector] public SetActiveWeapon setActiveWeapon;
    [HideInInspector] public WeaponFiredEvent weaponFiredEvent;
    [HideInInspector] public WeaponReloadedEvent weaponReloadedEvent;
    [HideInInspector] public SpriteRenderer spriteRenderer;
    [HideInInspector] public Animator animator;
    [HideInInspector] public PlayerControl playerControl;

    public List<Weapon> weaponList = new List<Weapon>();

    private void Awake()
    {
        // load component
        healthEvent = GetComponent<HealthEvent>();
        health = GetComponent<Health>();
        destroyedEvent = GetComponent<DestroyedEvent>();
        playerControl = GetComponent<PlayerControl>();
        movementByVelocityEvent = GetComponent<MovementByVelocityEvent>();
        movementToPositionEvent = GetComponent<MovementToPositionEvent>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        fireWeaponEvent = GetComponent<FireWeaponEvent>();
        idleEvent = GetComponent<IdleEvent>();
        aimWeaponEvent = GetComponent<AimWeaponEvent>();
        weaponFiredEvent = GetComponent<WeaponFiredEvent>();
        weaponReloadedEvent = GetComponent<WeaponReloadedEvent>();
        reloadWeaponEvent = GetComponent<ReloadWeaponEvent>();
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

    private void OnEnable()
    {
        healthEvent.OnHealthChanged += HealthEvent_OnHealthChanged;
    }

    private void OnDisable()
    {
        healthEvent.OnHealthChanged -= HealthEvent_OnHealthChanged;
    }

    private void HealthEvent_OnHealthChanged(HealthEvent healthEvent, HealthEventArgs healthEventArgs)
    {
        Debug.Log("HealthAmount = " + healthEventArgs.healthAmount);

        if(healthEventArgs.healthAmount <= 0f)
        {
            destroyedEvent.CallDestroyedEvent(true, 0);
        }
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
    /// Return the player position
    /// </summary>
    /// <returns></returns>
    public Vector3 GetPlayerPosition()
    {
        return transform.position;
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

    /// <summary>
    /// Returns true if the weapon is held by the player - otherwise returns false
    /// </summary>
    /// <param name="weaponDetails"></param>
    /// <returns></returns>
    public bool IsWeaponHeldByPlayer(WeaponDetailsSO weaponDetails)
    {
        foreach(Weapon weapon in weaponList)
        {
            if (weapon.weaponDetails == weaponDetails) return true;
        }

        return false;
    }
}