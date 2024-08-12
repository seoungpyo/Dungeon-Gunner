using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

#region REQUIRE COMPONETS
[RequireComponent(typeof(SortingGroup))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CircleCollider2D))]
[RequireComponent(typeof(PolygonCollider2D))]
[RequireComponent(typeof(EnemyMovementAI))]
[RequireComponent(typeof(MovementToPosition))]
[RequireComponent(typeof(MovementToPositionEvent))]
[RequireComponent(typeof(Idle))]
[RequireComponent(typeof(IdleEvent))]
[RequireComponent(typeof(AnimateEnemy))]
[RequireComponent(typeof(MaterializeEffect))]
[RequireComponent(typeof(EnemyWeaponAI))]
[RequireComponent(typeof(AimWeaponEvent))]
[RequireComponent(typeof(AimWeapon))]
[RequireComponent(typeof(FireWeaponEvent))]
[RequireComponent(typeof(FireWeapon))]
[RequireComponent(typeof(SetActiveWeaponEvent))]
[RequireComponent(typeof(SetActiveWeapon))]
[RequireComponent(typeof(WeaponFiredEvent))]
[RequireComponent(typeof(ReloadWeaponEvent))]
[RequireComponent(typeof(ReloadWeapon))]
[RequireComponent(typeof(WeaponReloadedEvent))]
[RequireComponent(typeof(Health))]
[RequireComponent(typeof(HealthEvent))]
[RequireComponent(typeof(DestroyedEvent))]
[RequireComponent(typeof(Destroyed))]
[RequireComponent(typeof(DealContactDamage))]
#endregion REQUIRE COMPONETS

[DisallowMultipleComponent]
public class Enemy : MonoBehaviour
{
    [HideInInspector] public SpriteRenderer spriteRenderer;
    [HideInInspector] public SpriteRenderer[] spriteRendererArray;
    [HideInInspector] public Animator animator;
    [HideInInspector] public MovementToPositionEvent movementToPositionEvent;
    [HideInInspector] public IdleEvent idleEvent;
    [HideInInspector] public EnemyDetailsSO enemyDetails;
    [HideInInspector] public AimWeaponEvent aimWeaponEvent;
    [HideInInspector] public FireWeaponEvent fireWeaponEvent;

    private CircleCollider2D circleCollider2D;
    private PolygonCollider2D polygonCollider2D;
    private EnemyMovementAI enemyMovementAI;
    private MaterializeEffect materializeEffect;
    private FireWeapon fireWeapon;
    private SetActiveWeaponEvent setActiveWeaponEvent;
    private HealthEvent healthEvent;
    private Health health;

    private void Awake()
    {
        // load component
        healthEvent = GetComponent<HealthEvent>();
        health = GetComponent<Health>();
        aimWeaponEvent = GetComponent<AimWeaponEvent>();
        fireWeaponEvent = GetComponent<FireWeaponEvent>();
        fireWeapon = GetComponent<FireWeapon>();
        setActiveWeaponEvent = GetComponent<SetActiveWeaponEvent>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRendererArray = GetComponentsInChildren<SpriteRenderer>();
        circleCollider2D = GetComponent<CircleCollider2D>();
        polygonCollider2D = GetComponent<PolygonCollider2D>();
        animator = GetComponent<Animator>();
        movementToPositionEvent = GetComponent<MovementToPositionEvent>();
        idleEvent = GetComponent<IdleEvent>();
        enemyMovementAI = GetComponent<EnemyMovementAI>();
        materializeEffect = GetComponent<MaterializeEffect>();
    }

    private void OnEnable()
    {
        healthEvent.OnHealthChanged += HealthEvent_OnHealthLost;
    }

    private void OnDisable()
    {
        healthEvent.OnHealthChanged -= HealthEvent_OnHealthLost;
    }

    private void HealthEvent_OnHealthLost(HealthEvent healthEvent, HealthEventArgs healthEventArgs)
    {
        if(healthEventArgs.healthAmount <= 0)
        {
            EnemyDestroyed();
        }
    }

    private void EnemyDestroyed()
    {
        DestroyedEvent destroyedEvent = GetComponent<DestroyedEvent>();
        destroyedEvent.CallDestroyedEvent(false, health.GetStartingHealth());
    }

    public void EnemyInitialization(EnemyDetailsSO enemyDetails, int enemySpawnNumber, DungeonLevelSO dungeonLevel)
    {
        this.enemyDetails = enemyDetails;

        SetEnemyMovementUpdateFrame(enemySpawnNumber);

        SetEnemyStartingHealth(dungeonLevel);

        SetEnemyStartingWeapon();

        SetEnemyAnimationSpeed();

        StartCoroutine(MaterializeEnemy());
    }

    private void SetEnemyStartingHealth(DungeonLevelSO dungeonLevel)
    {
        foreach(EnemyHealthDetails enemyHealthDetails in enemyDetails.enemyHealthDetailsArray)
        {
            if(enemyHealthDetails.dungeonLevel == dungeonLevel)
            {
                health.SetStartingHealth(enemyHealthDetails.enemyHealthAmount);
                return;
            }
        }
        health.SetStartingHealth(Settings.defaultEnemyHealth);
    }

    /// <summary>
    /// Set enemy movement update frame
    /// </summary>
    /// <param name="enemySpawnNumber"></param>
    private void SetEnemyMovementUpdateFrame(int enemySpawnNumber)
    {
        enemyMovementAI.SetUpdateFrameNumber(enemySpawnNumber % Settings.targetFrameRateToSpreadPathfindingOver);
    }

    /// <summary>
    /// Set enemy animator speed to match movement speed
    /// </summary>
    private void SetEnemyAnimationSpeed()
    {
        animator.speed = enemyMovementAI.moveSpeed / Settings.baseSpeedForEnemyAnimation;
    }

    private void SetEnemyStartingWeapon()
    {
        if(enemyDetails.enemyWeapon != null)
        {
            Weapon weapon = new Weapon() { weaponDetails = enemyDetails.enemyWeapon, weaponReloadTimer = 0f, weaponClipRemainingAmmo = enemyDetails.enemyWeapon.weaponClipAmmoCapacity, 
                weaponRemainingAmmo= enemyDetails.enemyWeapon.weaponAmmoCapacity, isWeaponReloading = false };

            setActiveWeaponEvent.CallSetActiveWeaponEvent(weapon);
        }
    }

    private IEnumerator MaterializeEnemy()
    {
        EnemyEnable(false);

        yield return StartCoroutine(materializeEffect.MaterializeRoutine(enemyDetails.enemyMaterializeShader, enemyDetails.enemyMaterializeColor, enemyDetails.enemyMaterializeTime,
            spriteRendererArray, enemyDetails.enemyStandardMaterial));

        EnemyEnable(true);
    }

    private void EnemyEnable(bool isEnalbed)
    {
        // colliders
        circleCollider2D.enabled = isEnalbed;
        polygonCollider2D.enabled = isEnalbed;

        enemyMovementAI.enabled = isEnalbed;

        fireWeapon.enabled = isEnalbed;
    }
}
