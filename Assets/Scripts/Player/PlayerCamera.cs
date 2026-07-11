using UnityEngine;
using System.Collections.Generic;

public class PlayerCamera : MonoBehaviour
{
    [Header("Camera Setting")]
    [SerializeField] private Transform Target;
    [SerializeField] private LayerMask _obstacleMask;
   
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

        Debug.DrawRay(rayOrigin, rayDirection * distance, Color.yellow, 0.1f);

        List<Renderer> currentHitRenderList = new List<Renderer>();

        foreach (RaycastHit hit in Physics.RaycastAll(rayOrigin, rayDirection, distance, _obstacleMask))
        {
            Renderer targetRenderer = hit.collider.GetComponent<Renderer>();
            if (targetRenderer != null)
            {
                currentHitRenderList.Add(targetRenderer);

                if (!_currentHitRenderers.Contains(targetRenderer))
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
        targetColor.a = isTransparent ? 0.2f : 1.0f;
        renderer.material.color = targetColor;
    }
}