using UnityEngine;

[CreateAssetMenu(fileName = "WeaponShootEffect_", menuName ="Scriptable Object/Weapon/Weapon Shoot Effect")]
public class WeaponShootEffectSO : ScriptableObject
{
    #region Header WEAPON SHOOT EFFECT DETAILS
    [Space(10)]
    [Header("WEAPON SHOOT EFFECT DETAILS")]
    #endregion Header WEAPON SHOOT EFFECT DETAILS
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
    public GameObject weaponShootEffectPrefab;

    #region Validation
#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilitie.ValidateCheckPositiveValue(this, nameof(duration), duration, false);
        HelperUtilitie.ValidateCheckPositiveValue(this, nameof(startParticleSize), startParticleSize, false);
        HelperUtilitie.ValidateCheckPositiveValue(this, nameof(startParticleSpeed), startParticleSpeed, false);
        HelperUtilitie.ValidateCheckPositiveValue(this, nameof(maxParticleNumber), maxParticleNumber, false);
        HelperUtilitie.ValidateCheckPositiveValue(this, nameof(emissionRate), emissionRate, false);
        HelperUtilitie.ValidateCheckPositiveValue(this, nameof(burstParticleNumber), burstParticleNumber, false);
        HelperUtilitie.ValidateCheckNullValue(this, nameof(weaponShootEffectPrefab), weaponShootEffectPrefab);

    }
#endif
    #endregion Validation
}
