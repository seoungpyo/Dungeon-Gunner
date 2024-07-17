using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AmmoHitEffect_", menuName = "Scriptable Object/Weapon/Ammo Hit Effect")]
public class AmmoHitEffectSO : ScriptableObject
{
    #region Header AMMO HIT EFFECT DETAILS
    [Space(10)]
    [Header("AMMO HIT EFFECT DETAILS")]
    #endregion Header AMMO HIT EFFECT DETAILS
    public Gradient colorGradient;
    public float duration = 0.50f;
    public float startParticleSize = 0.25f;
    public float startParticleSpeed = 3f;
    public float startLifetime = 0.5f;
    public int maxParticleNumber = 100;
    public int emissionRate = 100;
    public int burstParticleNumber = 20;
    public float effectGravity = -0.01f;
    public Sprite sprite;
    public Vector3 velocityOverLifetimeMax;
    public Vector3 velocityOverLifetimeMin;
    public GameObject ammoHitEffectPrefab;

    #region Validation
#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilitie.ValidateCheckPositiveValue(this, nameof(duration), duration, false);
        HelperUtilitie.ValidateCheckPositiveValue(this, nameof(startParticleSize), startParticleSize, false);
        HelperUtilitie.ValidateCheckPositiveValue(this, nameof(startParticleSpeed), startParticleSpeed, false);
        HelperUtilitie.ValidateCheckPositiveValue(this, nameof(maxParticleNumber), maxParticleNumber, false);
        HelperUtilitie.ValidateCheckPositiveValue(this, nameof(emissionRate), emissionRate, true);
        HelperUtilitie.ValidateCheckPositiveValue(this, nameof(burstParticleNumber), burstParticleNumber, false);
        HelperUtilitie.ValidateCheckNullValue(this, nameof(ammoHitEffectPrefab), ammoHitEffectPrefab);

    }
#endif
    #endregion Validation
}
