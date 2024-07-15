using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class Ammo : MonoBehaviour, IFireable
{
    [SerializeField] private TrailRenderer trailRenderer;

    private float ammoRange = 0f; // the range of each ammo
    private float ammoSpeed;
    private Vector3 fireDirectionVector;
    private float fireDirectionAngle;
    private SpriteRenderer spriteRenderer;
    private AmmoDetailsSO ammoDetails;
    private float ammoChargeTimer;
    private bool isAmmoMaterialSet = false;
    private bool overrideAmmoMovement;

    private void Awake()
    {
        // cache sprite renderer
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        // Ammo charge effect
        if(ammoChargeTimer > 0f)
        {
            ammoChargeTimer -= Time.deltaTime;
            return;
        }
        else if (!isAmmoMaterialSet)
        {
            SetAmmoMaterial(ammoDetails.ammoMaterial);
            isAmmoMaterialSet = true;
        }

        //Calculate distance vector to move ammo
        Vector3 distanceVector = fireDirectionVector * ammoSpeed * Time.deltaTime;

        transform.position += distanceVector;

        // disable after max range reached
        ammoRange -= distanceVector.magnitude;

        if (ammoRange < 0f)
        {
            DisableAmmo();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        AmmoHitEffect();

        DisableAmmo();
    }

    /// <summary>
    /// Initalise the ammo being fired - using the ammodetails, the aimangle, weaponAngle, and
    /// weaponAimDirectionVector. If this ammo is part of a pattern the ammo movement can be
    /// overriden by setting overrideAmmoMovement to true
    /// </summary>
    /// <param name="ammoDetails"></param>
    /// <param name="aimAngle"></param>
    /// <param name="weaponAimAngle"></param>
    /// <param name="ammoSpeed"></param>
    /// <param name="weaponAimDirectionVector"></param>
    /// <param name="overrideAmmoMovement"></param>
    public void InitialiseAmmo(AmmoDetailsSO ammoDetails, float aimAngle, float weaponAimAngle, float ammoSpeed, Vector3 weaponAimDirectionVector,
        bool overrideAmmoMovement = false)
    {
        #region Ammo

        this.ammoDetails = ammoDetails;

        // set fire direction
        SetFireDirection(ammoDetails, aimAngle, weaponAimAngle, weaponAimDirectionVector);

        // set ammo sprite
        spriteRenderer.sprite = ammoDetails.ammoSprite;
        
        // set inital ammo material depending on whether there is an ammo charge period
        if( ammoDetails.ammoChargeTime > 0f)
        {
            // set ammo charge timer
            ammoChargeTimer = ammoDetails.ammoChargeTime;
            SetAmmoMaterial(ammoDetails.ammoChargeMaterial);
            isAmmoMaterialSet = false;
        }
        else
        {
            ammoChargeTimer = 0f;
            SetAmmoMaterial(ammoDetails.ammoMaterial);
            isAmmoMaterialSet = true;
        }

        // set ammo range
        ammoRange = ammoDetails.ammoRange;

        // set ammo speed
        this.ammoSpeed = ammoSpeed;

        // override ammo movement
        this.overrideAmmoMovement = overrideAmmoMovement;

        // Active ammo gameobject
        gameObject.SetActive(true);

        #endregion Ammo 

        #region Trail
        if (ammoDetails.isAmmoTrail)
        {
            trailRenderer.gameObject.SetActive(true);
            trailRenderer.emitting = true;
            trailRenderer.material = ammoDetails.ammoTrailMaterial;
            trailRenderer.startWidth = ammoDetails.ammoTrailStartWidth;
            trailRenderer.endWidth = ammoDetails.ammoTrailEndWidth;
            trailRenderer.time = ammoDetails.ammoTrailTime;
        }
        else
        {
            trailRenderer.emitting = false;
            trailRenderer.gameObject.SetActive(false);
        }

        #endregion Trail
    }

    /// <summary>
    /// Set ammo fire direction and angle based on the input angle and direction adjusted by the random spred
    /// </summary>
    /// <param name="ammoDetails"></param>
    /// <param name="aimAngle"></param>
    /// <param name="weaponAimAngle"></param>
    /// <param name="weaponAimDirectionVector"></param>
    private void SetFireDirection(AmmoDetailsSO ammoDetails, float aimAngle, float weaponAimAngle, Vector3 weaponAimDirectionVector)
    {
        // calculate random spread angle between min and max
        float randomSpread = Random.Range(ammoDetails.ammoSpreadMin, ammoDetails.ammoSpreadMax);

        // Get a random spread toggle of 1 or -1
        int spreadToggle = Random.Range(0, 2) * 2 - 1;

        if(weaponAimDirectionVector.magnitude < Settings.useAimAngleDistance)
        {
            fireDirectionAngle = aimAngle;
        }
        else
        {
            fireDirectionAngle = weaponAimAngle;
        }
        
        // Adjust ammo fire angle angle by random spread
        fireDirectionAngle += spreadToggle * randomSpread;

        // Set ammo rotatipon
        transform.eulerAngles = new Vector3(0f, 0f, fireDirectionAngle);

        // set ammo fire direction
        fireDirectionVector = HelperUtilitie.GetDirectionVectorFromAngle(fireDirectionAngle);
    }

    /// <summary>
    /// Disable the ammo - thus returning it to the object pool
    /// </summary>
    private void DisableAmmo()
    {
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Display the ammo hit effect
    /// </summary>
    private void AmmoHitEffect()
    {
        if(ammoDetails.ammoHitEffect != null && ammoDetails.ammoHitEffect.ammoHitEffectPrefab != null)
        {
            // get ammo hit effect gameobject from the pool
            AmmoHitEffect ammoHitEffect = (AmmoHitEffect)PoolManager.Instance.ReuseComponent(ammoDetails.ammoHitEffect.ammoHitEffectPrefab,
                transform.position, Quaternion.identity);

            // set hit effect
            ammoHitEffect.SetHitEffet(ammoDetails.ammoHitEffect);

            ammoHitEffect.gameObject.SetActive(true);
        }
    }

    public void SetAmmoMaterial(Material material)
    {
        spriteRenderer.material = material;
    }  

    public GameObject GetGameObject()
    {
        return gameObject;
    }

    #region Validation
#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilitie.ValidateCheckNullValue(this, nameof(trailRenderer), trailRenderer);
    }
#endif
    #endregion Validation

}
