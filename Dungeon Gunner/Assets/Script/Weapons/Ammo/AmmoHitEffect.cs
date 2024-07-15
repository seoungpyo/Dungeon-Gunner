using UnityEngine;

[DisallowMultipleComponent]
public class AmmoHitEffect : MonoBehaviour
{
    private ParticleSystem ammoHitEffectParticleSystem;

    private void Awake()
    {
        ammoHitEffectParticleSystem = GetComponent<ParticleSystem>();
    }

    public void SetHitEffet(AmmoHitEffectSO ammoHitEffect)
    {
        SetHitEffectColorGradient(ammoHitEffect.colorGradient);

        SetHitEffectParticleStartingValue(ammoHitEffect.duration, ammoHitEffect.startParticleSize, ammoHitEffect.startParticleSpeed,
            ammoHitEffect.startLifetime, ammoHitEffect.effectGravity, ammoHitEffect.maxParticleNumber);

        SetHitEffectParticleEmission(ammoHitEffect.emissionRate, ammoHitEffect.burstParticleNumber);

        SetHitEffectParticleSprite(ammoHitEffect.sprite);

        SetHitEffectVelocityOverLifeTime(ammoHitEffect.velocityOverLifetimeMin, ammoHitEffect.velocityOverLifetimeMax);
    }

    /// <summary>
    /// Set the shoot effect particel system color gradient
    /// </summary>
    /// <param name="gradient"></param>
    private void SetHitEffectColorGradient(Gradient gradient)
    {
        ParticleSystem.ColorOverLifetimeModule colorOverLifetimeModule = ammoHitEffectParticleSystem.colorOverLifetime;
        colorOverLifetimeModule.color = gradient;
    }

    /// <summary>
    /// Set hit effect particle system starting values
    /// </summary>
    /// <param name="duration"></param>
    /// <param name="startParticleSize"></param>
    /// <param name="startParticeSpeed"></param>
    /// <param name="startLifetime"></param>
    /// <param name="effectGravity"></param>
    /// <param name="maxParticles"></param>
    private void SetHitEffectParticleStartingValue(float duration, float startParticleSize, float startParticeSpeed, float startLifetime,
        float effectGravity, int maxParticles)
    {
        ParticleSystem.MainModule mainModule = ammoHitEffectParticleSystem.main;

        mainModule.duration = duration;
        mainModule.startSize = startParticleSize;
        mainModule.startSpeed = startParticeSpeed;
        mainModule.startLifetime = startLifetime;
        mainModule.gravityModifier = effectGravity;
        mainModule.maxParticles = maxParticles;
    }

    /// <summary>
    /// Set hit effect particle system particel burst particles number
    /// </summary>
    /// <param name="emissionRate"></param>
    /// <param name="burstParticleNumber"></param>
    private void SetHitEffectParticleEmission(int emissionRate, float burstParticleNumber)
    {
        ParticleSystem.EmissionModule emissionModule = ammoHitEffectParticleSystem.emission;

        // set particle burst number
        ParticleSystem.Burst burst = new ParticleSystem.Burst(0f, burstParticleNumber);
        emissionModule.SetBurst(0, burst);

        emissionModule.rateOverTime = emissionRate;
    }

    /// <summary>
    ///  Set hit effect particle system sprite
    /// </summary>
    /// <param name="sprite"></param>
    private void SetHitEffectParticleSprite(Sprite sprite)
    {
        ParticleSystem.TextureSheetAnimationModule textureSheetAnimationModule = ammoHitEffectParticleSystem.textureSheetAnimation;

        textureSheetAnimationModule.SetSprite(0, sprite);
    }

    /// <summary>
    /// Set the hit effect velocity over lifetime
    /// </summary>
    /// <param name="minVelocity"></param>
    /// <param name="maxVelocity"></param>
    private void SetHitEffectVelocityOverLifeTime(Vector3 minVelocity, Vector3 maxVelocity)
    {
        ParticleSystem.VelocityOverLifetimeModule velocityOverLifetimeModule = ammoHitEffectParticleSystem.velocityOverLifetime;

        ParticleSystem.MinMaxCurve minMaxCurveX = new ParticleSystem.MinMaxCurve();
        minMaxCurveX.mode = ParticleSystemCurveMode.TwoConstants;
        minMaxCurveX.constantMin = minVelocity.x;
        minMaxCurveX.constantMax = maxVelocity.x;
        velocityOverLifetimeModule.x = minMaxCurveX;

        ParticleSystem.MinMaxCurve minMaxCurveY = new ParticleSystem.MinMaxCurve();
        minMaxCurveY.mode = ParticleSystemCurveMode.TwoConstants;
        minMaxCurveY.constantMin = minVelocity.y;
        minMaxCurveY.constantMax = maxVelocity.y;
        velocityOverLifetimeModule.y = minMaxCurveY;

        ParticleSystem.MinMaxCurve minMaxCurveZ = new ParticleSystem.MinMaxCurve();
        minMaxCurveZ.mode = ParticleSystemCurveMode.TwoConstants;
        minMaxCurveZ.constantMin = minVelocity.z;
        minMaxCurveZ.constantMax = maxVelocity.z;
        velocityOverLifetimeModule.z = minMaxCurveZ;
    }
}
