using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBar : MonoBehaviour
{
    #region Header GameObject References
    [Space(10)]
    [Header("GameObject References")]
    #endregion Header GameObject References
    [SerializeField] private GameObject healthBar;

    public void EnableHealthBar()
    {
        gameObject.SetActive(true);
    }

    public void DisableHealthBar()
    {
        gameObject.SetActive(false);
    }

    public void SetHealthBarValue(float healthPercent)
    {
        healthBar.transform.localScale = new Vector3(healthPercent, 1f, 1f);
    }
}
