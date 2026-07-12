using UnityEngine;
using System.Collections.Generic;

public class PlayerCamera : MonoBehaviour
{
    [Header("Camera Setting")]
    [SerializeField] private Transform Target;
    [SerializeField] private LayerMask _obstacleMask;
    [SerializeField] private float _checkRadius = 1f; // 레이케스트 두께
    [SerializeField] private float _transfparentAlpha = 0.2f; // 투명해질 때의 알파 값

    private PlayerController _playerController;
    private Vector3 _offset;
    private List<Renderer> _currentHitRenderers = new List<Renderer>();

    private void Start()
    {
        if (Target != null)
        {
            _playerController = Target.GetComponent<PlayerController>();
            _offset = transform.position - Target.transform.position;
        }
    }

    private void LateUpdate()
    {
        if (Target == null || _playerController == null) return;
        Vector3 targetPos = Target.transform.position + _offset;
        float playerSpeed = _playerController.MoveSpeed;
        transform.position = Vector3.Lerp(transform.position, targetPos, playerSpeed * Time.deltaTime);

        CheckOcclusion();
    }

    private void CheckOcclusion()
    {
        Vector3 direction = Target.transform.position - transform.position;
        float distance = direction.magnitude;
        Vector3 rayOrigin = transform.position;
        Vector3 rayDirection = direction.normalized;

        RaycastHit[] hits = Physics.SphereCastAll(rayOrigin, _checkRadius, rayDirection, distance, _obstacleMask);

        List<Renderer> currentHitRenderList = new List<Renderer>();

        foreach (RaycastHit hit in hits)
        {
            Renderer targetRenderer = hit.collider.GetComponent<Renderer>();
            if (targetRenderer != null)
            {
                currentHitRenderList.Add(targetRenderer);

                if (_currentHitRenderers.Contains(targetRenderer) == false)
                {
                    ChangedHidingObject(targetRenderer, true);
                }
            }
        }

        foreach (Renderer renderer in _currentHitRenderers)
        {
            if (currentHitRenderList.Contains(renderer) == false)
            {
                ChangedHidingObject(renderer, false);
            }
        }
        _currentHitRenderers = currentHitRenderList;
    }

    private void ChangedHidingObject(Renderer renderer, bool isTransparent)
    {
        if (renderer == null) return;

        Color targetColor = renderer.material.color;
        targetColor.a = isTransparent ? _transfparentAlpha : 1.0f;
        renderer.material.color = targetColor;
    }
}