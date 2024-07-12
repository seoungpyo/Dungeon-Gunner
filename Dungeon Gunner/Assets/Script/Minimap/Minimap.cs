using Cinemachine;
using UnityEngine;

[DisallowMultipleComponent]
public class Minimap : MonoBehaviour
{
    [SerializeField] private GameObject miniMapPlayer;

    private Transform playerTransform;

    private void Start()
    {
        playerTransform = GameManager.Instance.GetPlayer().transform;

        // populate player as cinemachine camera target
        CinemachineVirtualCamera cinemachineVirtualCamera = GetComponentInChildren<CinemachineVirtualCamera>();
        cinemachineVirtualCamera.Follow = playerTransform;

        // set minimap player icon
        SpriteRenderer spriteRenderer = miniMapPlayer.GetComponent<SpriteRenderer>();
        if(spriteRenderer != null)
        {
            spriteRenderer.sprite = GameManager.Instance.GetPlayerMiniMapIcon();
        }
    }

    private void Update()
    {
        if(playerTransform != null && miniMapPlayer != null)
        {
            miniMapPlayer.transform.position = playerTransform.position;
        }
    }

    #region Validation
#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilitie.ValidateCheckNullValue(this, nameof(miniMapPlayer), miniMapPlayer);
    }
#endif
    #endregion Validation
}
