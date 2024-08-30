using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class PlayerSelectionUI : MonoBehaviour
{
    public SpriteRenderer playerHandSpriteRenderer;
    public SpriteRenderer playerHandNoWeaponSpriteRenderer;
    public SpriteRenderer playerWeaponSPriteRenderer;
    public Animator animator;

    #region Validation 
#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilitie.ValidateCheckNullValue(this, nameof(playerHandSpriteRenderer), playerHandSpriteRenderer);
        HelperUtilitie.ValidateCheckNullValue(this, nameof(playerHandNoWeaponSpriteRenderer), playerHandNoWeaponSpriteRenderer);
        HelperUtilitie.ValidateCheckNullValue(this, nameof(playerWeaponSPriteRenderer), playerWeaponSPriteRenderer);
        HelperUtilitie.ValidateCheckNullValue(this, nameof(animator), animator);
    }
#endif
    #endregion Validation
}
