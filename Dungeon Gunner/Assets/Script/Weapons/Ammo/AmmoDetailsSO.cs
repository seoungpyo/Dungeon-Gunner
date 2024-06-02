using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AmmoDetails_", menuName = "Scriptable Object/Weapon/Ammo Details")]
public class AmmoDetailsSO : ScriptableObject
{
    #region Header BASIC AMMO DETAILS
    [Space(10)]
    [Header("BASIC AMMO DETAILS")]
    #endregion Header BASIC AMMO DETAILS
    public string ammoName;
    public bool isPlayerAmmo;

    #region Header AMMO SPRITE, PREFAB & MATERIALS
    [Space(10)]
    [Header("AMMO SPRITE, PREFAB & MATERIALS")]
    #endregion Header AMMO SPRITE, PREFAB & MATERIALS
    public Sprite ammoSprite;
    public GameObject[] ammoPrefabArray;
    public Material ammoMaterial;
    public float ammoChargeTime = 0.1f;
    public Material ammoChargeMaterial;

    #region Header AMMO BASE PARAMETERS
    [Space(10)]
    [Header("AMMO BASE PARAMETERS")]
    #endregion Header AMMO BASE PARAMETERS
    public int ammoDamage = 1;
    public float ammoSpeedMin = 20f;
    public float ammoSpeedMax = 20f;
    public float ammoRange = 20f;
    public float ammoRatationSpeed = 1f;

    #region Header AMMO SPREAD DETAILS
    [Space(10)]
    [Header("AMMO SPREAD DETAILS")]
    #endregion Header AMMO SPREAD DETAILS
    public float ammoSpreadMin = 0f;
    public float ammoSpreadMax = 0f;

    #region Header AMMO SPAWN DETAILS
    [Space(10)]
    [Header("AMMO SPAWN DETAILS")]
    #endregion Header AMMO SPAWN DETAILS
    public int ammoSpawnAmountMin = 1;
    public int ammoSpawnAmountMax = 1;
    public float ammoSpawnIntervalMin = 0f;
    public float ammoSpawnIntervalMax = 0f;

    #region Header AMMO TRAIL DETAILS
    [Space(10)]
    [Header("AMMO TRAIL DETAILS")]
    #endregion Header AMMO TRAIL DETAILS
    public bool isAmmoTrail = false;
    public float ammoTrailTime = 3f;
    public Material ammoTrailMaterial;
    [Range(0f, 1f)] public float ammoTrailStartWidth;
    [Range(0f, 1f)] public float ammoTrailEndWidth;

    #region Validation
#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilitie.ValidateCheckEmptyString(this, nameof(ammoName), ammoName);
        HelperUtilitie.ValidateCheckNullValue(this, nameof(ammoSprite), ammoSprite);
        HelperUtilitie.ValidateCheckEnumerableValues(this, nameof(ammoPrefabArray), ammoPrefabArray);
        HelperUtilitie.ValidateCheckNullValue(this, nameof(ammoMaterial), ammoMaterial);

        if (ammoChargeTime > 0) HelperUtilitie.ValidateCheckNullValue(this, nameof(ammoChargeMaterial), ammoChargeMaterial);

        HelperUtilitie.ValidateCheckPositiveValue(this, nameof(ammoDamage), ammoDamage, false);
        HelperUtilitie.ValidateCheckPositiveRange(this, nameof(ammoSpeedMin), ammoSpeedMin, nameof(ammoSpeedMax), ammoSpeedMax, false);
        HelperUtilitie.ValidateCheckPositiveValue(this, nameof(ammoRange), ammoRange, false);
        HelperUtilitie.ValidateCheckPositiveRange(this, nameof(ammoSpreadMin), ammoSpreadMin, nameof(ammoSpreadMax), ammoSpreadMax, true);
        HelperUtilitie.ValidateCheckPositiveRange(this, nameof(ammoSpawnIntervalMin), ammoSpawnIntervalMin, nameof(ammoSpawnIntervalMax), ammoSpawnIntervalMax, true);
        HelperUtilitie.ValidateCheckPositiveRange(this, nameof(ammoSpawnAmountMin), ammoSpawnAmountMin, nameof(ammoSpawnAmountMax), ammoSpawnAmountMax, false);

        if (isAmmoTrail)
        {
            HelperUtilitie.ValidateCheckPositiveValue(this, nameof(ammoTrailTime), ammoTrailTime, false);
            HelperUtilitie.ValidateCheckNullValue(this, nameof(ammoTrailMaterial), ammoTrailMaterial);
            HelperUtilitie.ValidateCheckPositiveValue(this, nameof(ammoTrailStartWidth), ammoTrailStartWidth,false);
            HelperUtilitie.ValidateCheckPositiveValue(this, nameof(ammoTrailEndWidth), ammoTrailEndWidth,false);
        }
            }
#endif
    #endregion Validation
}
