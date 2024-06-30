using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SetActiveWeaponEvent))]
[DisallowMultipleComponent]
public class SetActiveWeapon : MonoBehaviour
{
    [SerializeField] private SpriteRenderer weaponSpriteRenderer;
    [SerializeField] private PolygonCollider2D weaponPolygonCollider2D;
    [SerializeField] private Transform weaponShootPositionTransform;
    [SerializeField] private Transform weaponEffectPositionTransform;

    private SetActiveWeaponEvent setActiveWeaponEvent;
    private Weapon currentWeapon;

    private void Awake()
    {
        setActiveWeaponEvent = GetComponent<SetActiveWeaponEvent>();
    }

    private void OnEnable()
    {
        setActiveWeaponEvent.OnSetActiveWeapon += SetActiveWeapon_OnSetActiveWeapon; 
    }

    private void OnDisable()
    {
        setActiveWeaponEvent.OnSetActiveWeapon -= SetActiveWeapon_OnSetActiveWeapon;
    }

    private void SetActiveWeapon_OnSetActiveWeapon(SetActiveWeaponEvent setActiveWeaponEvent, SetActiveWeaponEventArgs setActiveWeaponEventArgs)
    {
        SetWeapon(setActiveWeaponEventArgs.weapon);
    }

    private void SetWeapon(Weapon weapon)
    {
        currentWeapon = weapon;
        
        // set current weapon sprite
        weaponSpriteRenderer.sprite = currentWeapon.weaponDetails.weaponSprite;
    
        // if the weapon has a polygon collder and a sprite then set it to the weapon sprite physics shape
        if(weaponPolygonCollider2D != null & weaponSpriteRenderer.sprite != null)
        {
            // get sprite physics shape - this returns the sprite physics shape points as a list of vector2s
            List<Vector2> spritePysicsShapePointList = new List<Vector2>();
            weaponSpriteRenderer.sprite.GetPhysicsShape(0, spritePysicsShapePointList);

            // set polygon collider on weapon to pick up phsics shap for sprite - set collider points to sprite physics shape points
            weaponPolygonCollider2D.points = spritePysicsShapePointList.ToArray();
        }

        //set weapon shoot position
        weaponShootPositionTransform.localPosition = currentWeapon.weaponDetails.weaponShootPosition;
    }

    public AmmoDetailsSO GetCurrentAmmo()
    {
        return currentWeapon.weaponDetails.weaponCurrentAmmo;
    }

    public Weapon GetCurrentWeapon()
    {
        return currentWeapon;
    }

    public Vector3 GetShootPosition()
    {
        return weaponShootPositionTransform.position;
    }

    public Vector3 GetShootEffectPostion()
    {
        return weaponEffectPositionTransform.position;
    }

    public void RemoveCurrentWeapon()
    {
        currentWeapon = null;
    }

    #region Validation
    private void OnValidate()
    {
        HelperUtilitie.ValidateCheckNullValue(this, nameof(weaponSpriteRenderer), weaponSpriteRenderer);
        HelperUtilitie.ValidateCheckNullValue(this, nameof(weaponPolygonCollider2D), weaponPolygonCollider2D);
        HelperUtilitie.ValidateCheckNullValue(this, nameof(weaponShootPositionTransform), weaponShootPositionTransform);
        HelperUtilitie.ValidateCheckNullValue(this, nameof(weaponEffectPositionTransform), weaponEffectPositionTransform);
    }
    #endregion Validation
}
