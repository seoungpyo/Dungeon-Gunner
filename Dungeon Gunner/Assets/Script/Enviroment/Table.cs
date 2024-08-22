using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody2D))]
public class Table : MonoBehaviour, IUseable
{
    [SerializeField] private float itemMass;
    private BoxCollider2D boxCollider2D;
    private Animator animator;
    private Rigidbody2D rigidBody2D;
    private bool itemUsed = false;

    private void Awake()
    {
        boxCollider2D = GetComponent<BoxCollider2D>();
        animator = GetComponent<Animator>();
        rigidBody2D = GetComponent<Rigidbody2D>();
    }

    public void UseItem()
    {
        if (!itemUsed)
        {
            Bounds bounds = boxCollider2D.bounds;
            
            // calculate closest point to player on collider bounds
            Vector3 closesPointToPlayer = bounds.ClosestPoint(GameManager.Instance.GetPlayer().GetPlayerPosition());

            // if player is to the right of the table then flip left
            if (closesPointToPlayer.x == bounds.max.x)
            {
                animator.SetBool(Settings.flipLeft, true);
            }
            else if (closesPointToPlayer.x == bounds.min.x)
            {
                animator.SetBool(Settings.flipRight, true);
            }
            else if (closesPointToPlayer.y == bounds.max.y)
            {
                animator.SetBool(Settings.flipDown, true);
            }
            else
            {
                animator.SetBool(Settings.flipUp, true);
            }

            gameObject.layer = LayerMask.NameToLayer("Environment");

            rigidBody2D.mass = itemMass;

            SoundEffectManager.Instance.PlaySoundEffect(GameResources.Instance.tableFlip);

            itemUsed = true;
        }
    }

    #region Validation
#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilitie.ValidateCheckPositiveValue(this, nameof(itemMass), itemMass, false);
    }
#endif
    #endregion Validation
}
