using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MusicTrack_",menuName ="Scriptable Object/Sounds/MusicTrack")]
public class MusicTrackSO : ScriptableObject
{
    #region Header MUSIC TRACK DEFAILS
    [Space(10)]
    [Header("MUSIC TRACK DEFAILS")]
    #endregion Header MUSIC TRACK DEFAILS
    public string musicName;
    public AudioClip musicClip;

    [Range(0, 1)] public float musicVolume = 1f;

    #region Validation
#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilitie.ValidateCheckEmptyString(this, nameof(musicName), musicName);
        HelperUtilitie.ValidateCheckNullValue(this, nameof(musicClip), musicClip);
        HelperUtilitie.ValidateCheckPositiveValue(this, nameof(musicVolume), musicVolume, true);
    }
#endif
    #endregion Validation
}
