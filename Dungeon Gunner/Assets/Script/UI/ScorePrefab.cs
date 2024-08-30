using TMPro;
using UnityEngine;

public class ScorePrefab : MonoBehaviour
{
    public TextMeshProUGUI rankTMP;
    public TextMeshProUGUI nameTMP;
    public TextMeshProUGUI levelTMP;
    public TextMeshProUGUI scoreTMP;

    #region Validation
#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilitie.ValidateCheckNullValue(this, nameof(rankTMP), rankTMP);
        HelperUtilitie.ValidateCheckNullValue(this, nameof(nameTMP), nameTMP);
        HelperUtilitie.ValidateCheckNullValue(this, nameof(levelTMP), levelTMP);
        HelperUtilitie.ValidateCheckNullValue(this, nameof(scoreTMP), scoreTMP);
    }
#endif
    #endregion Validation
}
