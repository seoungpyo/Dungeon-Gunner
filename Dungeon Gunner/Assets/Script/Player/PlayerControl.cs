using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Player))]
[DisallowMultipleComponent]
public class PlayerControl : MonoBehaviour
{
    #region Tooltip
    [Tooltip("MovementDetailsSO scriptable object containing movement details such as speed")]
    #endregion Tooltip

    [SerializeField] private MovementDetailsSO movementDetails;

    private Player player;
    private bool leftMouseDownPreviousFrame = false;
    private int currentWeaponIndex = 1;
    private float moveSpeed;
    private Coroutine playerRollCoroutine;
    private WaitForFixedUpdate waitForFixedUpdate;
    private bool isPlayerRolling = false;
    private float playerRollCooldownTimer = 0f;

    private void Awake()
    {
        // load component
        player = GetComponent<Player>();

        moveSpeed = movementDetails.GetMoveSpeed();
    }

    private void Start()
    {
        // Create waitforfixed update for use in coroutine
        waitForFixedUpdate = new WaitForFixedUpdate();

        // set starting weapon
        SetStartingWeapon();

        // set player animation speed;
        SetPlayerAnimationSpeed();
    }

    /// <summary>
    ///  Set the player starting weapon
    /// </summary>
    private void SetStartingWeapon()
    {
        int index = 1;

        foreach(Weapon weapon in player.weaponList)
        {
            if(weapon.weaponDetails == player.playerDetails.startingWeapon)
            {
                SetWeaponByIndex(index);
                break;
            }
            index++;
        }
    }

    private void SetPlayerAnimationSpeed()
    {
        // set animator speed to match movement speed
        player.animator.speed = moveSpeed / Settings.baseSpeedForPlayerAnimatons;
    }

    private void Update()
    {
        if (isPlayerRolling) return;

        // process the player movement input
        MovementInput();

        // process the player weapon input
        WeaponInput();

        // Player roll cooldown timer
        PlayerRollCooldownTimer();
    }

    // player movement input
    private void MovementInput()
    {
        // get movement input
        float horizontalMovement = Input.GetAxisRaw("Horizontal");
        float verticalMovement = Input.GetAxisRaw("Vertical");
        bool rightMouseButtonDown = Input.GetKeyDown(KeyCode.Space);

        // create a direction vector based on the input
        Vector2 direction = new Vector2(horizontalMovement, verticalMovement);

        if(horizontalMovement != 0f && verticalMovement != 0f)
        {
            //direction *= 0.7f;
            direction = direction.normalized;
        }

        // if there is movement either move or roll
        if (direction != Vector2.zero)
        {
            if (!rightMouseButtonDown)
            {
                // trigger movement event
                player.movementByVelocityEvent.CallMovementByVElocityEvent(direction, moveSpeed);
            }
            // else player roll if not cooling down
            else if(playerRollCooldownTimer <= 0f)
            {
                PlayerRoll((Vector3)direction);
            }
        }
        // else trigger idle event
        else
        {
            player.idleEvent.CallIdleEvent();
        }
    }

    /// <summary>
    /// Player roll
    /// </summary>
    /// <param name="direction"></param>
    private void PlayerRoll(Vector3 direction)
    {
        playerRollCoroutine = StartCoroutine(PlayerRollRoutine(direction));
    }

    /// <summary>
    /// player Roll Coroutine
    /// </summary>
    /// <param name="direction"></param>
    /// <returns></returns>
    private IEnumerator PlayerRollRoutine(Vector3 direction)
    {
        // minDistance used to decide when to exit corroutine loop
        float minDistance = 0.2f;
        isPlayerRolling = true;

        Vector3 targetPostion = player.transform.position + (Vector3)direction * movementDetails.rollDistance;

        while(Vector3.Distance(player.transform.position, targetPostion) > minDistance)
        {
            player.movementToPositionEvent.CallMovementToPostionEvent(targetPostion, player.transform.position, movementDetails.rollSpeed,
                direction, isPlayerRolling);

            yield return waitForFixedUpdate;
        }

        isPlayerRolling = false;

        playerRollCooldownTimer = movementDetails.rollCooldownTime;

        player.transform.position = targetPostion;
    }

    private void PlayerRollCooldownTimer()
    {
        if (playerRollCooldownTimer >= 0f)
        {
            playerRollCooldownTimer -= Time.deltaTime;
        }
    }

    // weapon input
    private void WeaponInput()
    {
        Vector3 weaponDirection;
        float weaponAngleDegrees, playerAngleDegrees;
        AimDirection playerAimDirection;

        // aim weapon input
        AimWeaponInput(out weaponDirection, out weaponAngleDegrees, out playerAngleDegrees, out playerAimDirection);

        // fire weapon input
        FireWeaponInput(weaponDirection, weaponAngleDegrees, playerAngleDegrees, playerAimDirection);

        SwitchWeaponInput();

        ReloadWeaponInput();
    }

    private void AimWeaponInput(out Vector3 weaponDirection, out float weaponAngleDegrees, out float playerAngleDegrees, out AimDirection playerAimDirection)
    {
        // get mouse world position
        Vector3 mouseWorldPostion = HelperUtilitie.GetMouseWorldPosition();

        // calculate direction vector of cursor from weapon shoot position
        weaponDirection = (mouseWorldPostion - player.setActiveWeapon.GetShootPosition());

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

    private void FireWeaponInput(Vector3 weaponDirection, float weaponAngleDegrees, float playerAngleDegrees, AimDirection playerAimDireciton)
    {
        // fire when left mouse button is  clicked
        if (Input.GetMouseButton(0))
        {
            // trigger fire weapon event 
            player.fireWeaponEvent.CallFireWeaponEvent(true, leftMouseDownPreviousFrame, playerAimDireciton, playerAngleDegrees, weaponAngleDegrees, weaponDirection);
            leftMouseDownPreviousFrame = true;
        }
        else
        {
            leftMouseDownPreviousFrame = false;
        }
    }

    private void SwitchWeaponInput()
    {
        // switch weapon if mouse scroll wheel selected
        if(Input.mouseScrollDelta.y < 0f)
        {
            PreviousWeapon();
        }

        if (Input.mouseScrollDelta.y > 0f)
        {
            NextWeapon();
        }

        if (Input.GetKeyDown(KeyCode.Alpha1)) { SetWeaponByIndex(1); }
        if (Input.GetKeyDown(KeyCode.Alpha2)) { SetWeaponByIndex(2); }
        if (Input.GetKeyDown(KeyCode.Alpha3)) { SetWeaponByIndex(3); }
        if (Input.GetKeyDown(KeyCode.Alpha4)) { SetWeaponByIndex(4); }
        if (Input.GetKeyDown(KeyCode.Alpha5)) { SetWeaponByIndex(5); }
        if (Input.GetKeyDown(KeyCode.Alpha6)) { SetWeaponByIndex(6); }
        if (Input.GetKeyDown(KeyCode.Alpha7)) { SetWeaponByIndex(7); }
        if (Input.GetKeyDown(KeyCode.Alpha8)) { SetWeaponByIndex(8); }
        if (Input.GetKeyDown(KeyCode.Alpha9)) { SetWeaponByIndex(9); }
        if (Input.GetKeyDown(KeyCode.Alpha0)) { SetWeaponByIndex(10); }
        if (Input.GetKeyDown(KeyCode.Minus)) { SetCurrentWeaponToFirstInTheList(); }

    }

    private void PreviousWeapon()
    {
        currentWeaponIndex--;

        if (currentWeaponIndex < 1)
        {
            currentWeaponIndex = player.weaponList.Count;
        }

        SetWeaponByIndex(currentWeaponIndex);
    }

    private void NextWeapon()
    {
        currentWeaponIndex++;

        if(currentWeaponIndex > player.weaponList.Count)
        {
            currentWeaponIndex = 1;
        }

        SetWeaponByIndex(currentWeaponIndex);
    }

    /// <summary>
    /// Set the current weapon to be first in the player weapon list
    /// </summary>
    private void SetCurrentWeaponToFirstInTheList()
    {
        List<Weapon> tempWeaponList = new List<Weapon>();

        // add the current weapon to first in the temp list
        Weapon currentWeapon = player.weaponList[currentWeaponIndex - 1];
        currentWeapon.weaponListPosition = 1;
        tempWeaponList.Add(currentWeapon);

        // loop through existing weapon list and add - skipping current weapon
        int index = 2;

        foreach(Weapon weapon in player.weaponList)
        {
            if (weapon == currentWeapon) continue;

            tempWeaponList.Add(weapon);
            weapon.weaponListPosition = index;
            index++;
        }

        player.weaponList = tempWeaponList;

        currentWeaponIndex = 1;
        SetWeaponByIndex(currentWeaponIndex);
    }

    private void SetWeaponByIndex(int weaponIndex)
    {
        if(weaponIndex - 1 < player.weaponList.Count)
        {
            currentWeaponIndex = weaponIndex;
            player.setActiveWeaponEvent.CallSetActiveWeaponEvent(player.weaponList[weaponIndex - 1]);
        }
    }

    private void ReloadWeaponInput()
    {
        Weapon currentWeapon = player.setActiveWeapon.GetCurrentWeapon();

        // if current weapon is reloading return
        if (currentWeapon.isWeaponReloading) return;

        // remaining ammo is less than clip capacity then return and not infinite ammo then return
        if (currentWeapon.weaponRemainingAmmo < currentWeapon.weaponDetails.weaponClipAmmoCapacity && !currentWeapon.weaponDetails.hasInfiniteAmmo) return;

        // if ammo in clip equals clip capacity then return
        if (currentWeapon.weaponClipRemainingAmmo == currentWeapon.weaponDetails.weaponClipAmmoCapacity) return;

        if (Input.GetKeyDown(KeyCode.R))
        {
            player.reloadWeaponEvent.CallOnReloadWeapon(player.setActiveWeapon.GetCurrentWeapon(), 0);
        }

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //if collided with  something stop player roll coroutine
        StopPlayerRollRoutine();
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        //if collided with  something stop player roll coroutine
        StopPlayerRollRoutine();
    }

    private void StopPlayerRollRoutine()
    {
        if(playerRollCoroutine != null)
        {
            StopCoroutine(playerRollCoroutine);

            isPlayerRolling = false;
        }
    }

    #region Validation
#if UNITY_EDITOR

    private void OnValidate()
    {
        HelperUtilitie.ValidateCheckNullValue(this, nameof(movementDetails), movementDetails);    
    }

#endif
    #endregion Validation
}
