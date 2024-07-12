using UnityEngine;

[CreateAssetMenu(fileName ="SoundEffect_",menuName ="Scriptable Object/Sounds/SoundEffect")]
public class SoundEffectSO : ScriptableObject
{

    #region Header SOUND EFFECT DETAILS
    [Space(10)]
    [Header("SOUND EFFECT DETAILS")]
    #endregion Header SOUND EFFECT DETAILS
    public string soundEffectName;
    public GameObject soundPrefab;
    public AudioClip soundEffectClip;
    [Range(0.1f, 1.5f)]
    public float soundEffectPitchRandomVariationMin = 0.8f;
    [Range(0.1f, 1.5f)]
    public float soundEffectPitchRandomVariationMax = 1.2f;
    [Range(0f, 1f)]
    public float soundEffectVolume = 1f;
    
    #region Valitation
#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilitie.ValidateCheckEmptyString(this, nameof(soundEffectName), soundEffectName);
        HelperUtilitie.ValidateCheckNullValue(this, nameof(soundPrefab), soundPrefab);
        HelperUtilitie.ValidateCheckNullValue(this, nameof(soundEffectClip), soundEffectClip);
        HelperUtilitie.ValidateCheckPositiveRange(this, nameof(soundEffectPitchRandomVariationMin), soundEffectPitchRandomVariationMin,
            nameof(soundEffectPitchRandomVariationMax), soundEffectPitchRandomVariationMax, false);
        HelperUtilitie.ValidateCheckPositiveValue(this, nameof(soundEffectVolume), soundEffectVolume,true);
    }
#endif
    #endregion
}
