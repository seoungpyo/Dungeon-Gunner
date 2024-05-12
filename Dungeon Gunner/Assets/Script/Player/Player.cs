using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderKeywordFilter;
using UnityEngine;
using UnityEngine.Rendering;

#region REQUIRE COMPONENTS
[RequireComponent(typeof(Health))]
[RequireComponent(typeof(PlayerControl))]
[RequireComponent(typeof(IdleEvent))]
[RequireComponent(typeof(Idle))]
[RequireComponent(typeof(AimWeaponEvent))]
[RequireComponent(typeof(AimWeapon))]
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
    [HideInInspector] public IdleEvent idleEvent;
    [HideInInspector] public AimWeaponEvent aimWeaponEvent;
    [HideInInspector] public SpriteRenderer spriteRenderer;
    [HideInInspector] public Animator animator;

    private void Awake()
    {
        // load component
        health = GetComponent<Health>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        idleEvent = GetComponent<IdleEvent>();
        aimWeaponEvent = GetComponent<AimWeaponEvent>();
    }

    /// <summary>
    ///  Initalize the player
    /// </summary>
    /// <param name="playerDetails"></param>
    public void Initalize(PlayerDetailsSO playerDetails)
    {
        this.playerDetails = playerDetails;

        // set player starting health
        SetPlayerHealth();
    }

    /// <summary>
    /// Set player health from playerDetails SO
    /// </summary>
    private void SetPlayerHealth()
    {
        health.SetStartingHealth(playerDetails.playerHealthAmount);
    }
}

