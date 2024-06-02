using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using UnityEngine;
using static UnityEngine.Rendering.HDROutputUtils;

[CreateAssetMenu (fileName ="WeaponDetails_",menuName ="Scriptable Object/Weapon/Weapon Details")]
public class WeaponDetailsSO : ScriptableObject
{
    #region Header WEAPON BAES DETAILS
    [Space(10)]
    [Header("WEAPON BASE DETAILS")]
    #endregion Header WEAPON BAES DETAILS
    public string weaponName;
    public Sprite weaponSprite; // sprite should have the 'generate phsics shape' option selecet;

    #region Header WEAPON CONFIGURATION
    [Space(10)]
    [Header("WEAPON CONFIGURATION")]
    #endregion Header WEAPON CONFIGURATION
    public Vector3 weaponShootPosition;
    public AmmoDetailsSO weaponCurrentAmmo;

    #region Header WEAPON OPERATING VALUES
    [Space(10)]
    [Header("WEAPON OPERATING VALUES")]
    #endregion Header WEAPON OPERATING VALUES
    public bool hasInfiniteAmmo = false;
    public bool hasInfiniteClipCapacity = false;
    public int weaponClipAmmoCapacity = 6;
    public int weaponAmmoCapacity = 100;
    public float weaponFireRate = 0.2f;
    public float weaponPrechargeTime = 0f;
    public float weaponReloadTime = 0f;

    #region Validation
#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilitie.ValidateCheckEmptyString(this, nameof(weaponName), weaponName);
        HelperUtilitie.ValidateCheckNullValue(this, nameof(weaponCurrentAmmo), weaponCurrentAmmo);
        HelperUtilitie.ValidateCheckPositiveValue(this, nameof(weaponFireRate), weaponFireRate, false);
        HelperUtilitie.ValidateCheckPositiveValue(this, nameof(weaponPrechargeTime), weaponPrechargeTime, true);

        if (!hasInfiniteAmmo)
        {
            HelperUtilitie.ValidateCheckPositiveValue(this, nameof(weaponAmmoCapacity), weaponAmmoCapacity, false);
        }

        if (!hasInfiniteClipCapacity)
        {
            HelperUtilitie.ValidateCheckPositiveValue(this, nameof(weaponClipAmmoCapacity), weaponClipAmmoCapacity, false);
        }

    }
#endif
    #endregion Validation

}
