using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class DealContactDamage : MonoBehaviour
{
    #region Header DEAL DAMAGE
    [Header("DEAL DAMAGE")]
    #endregion Header DEAL DAMAGE
    [SerializeField] private int contactDamageAmount;
    [SerializeField] private LayerMask layerMask;
    private bool isColliding = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isColliding) return;

        ContactDamage(collision);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (isColliding) return;

        ContactDamage(collision);
    }

    private void ContactDamage(Collider2D collision)
    {
        // if the collision object isn't in the specified layer then return (use bitwise comparison)
        int collisionObjectLayerMask = (1 << collision.gameObject.layer);


        if ((layerMask.value & collisionObjectLayerMask) == 0) return;

        ReceiveContactDamage receiveContactDamage = collision.gameObject.GetComponent<ReceiveContactDamage>();

        if(receiveContactDamage != null)
        {
            isColliding = true;

            Invoke("ResetContactCollision", Settings.contactDamageCollisionResetDelay);

            receiveContactDamage.TakeContactDamage(contactDamageAmount);
        }

    }

    private void ResetContactCollision()
    {
        isColliding = false;
    }

    #region Validation
#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilitie.ValidateCheckPositiveValue(this, nameof(contactDamageAmount), contactDamageAmount, true);
    }
#endif
    #endregion Validation

}
