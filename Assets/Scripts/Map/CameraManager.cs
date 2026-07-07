using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private Transform _playerTransform;
    [SerializeField] private LayerMask _obstacleMask;

    private List<Renderer> _currentHitRenderers = new List<Renderer>(); 

    private void Update()
    {
        //unitask로 체크시간을 늘려야함
        CheckOcclusion();
    }

    private void CheckOcclusion()
    {
        if (_playerTransform == null)
        {
            Debug.LogError("[CameraManager] _playerTransform이 할당되지 않았습니다!");
            return;
        }

        Vector3 direction = _playerTransform.position - transform.position;
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
            if (!currentHitRenderList.Contains(renderer))
            {
                ChangedHidingObject(renderer, false);
            }
        }

        _currentHitRenderers = currentHitRenderList;
    }

    private void ChangedHidingObject(Renderer renderer, bool isTransparent)
    {
        Color targetColor = renderer.material.color;
        targetColor.a = isTransparent ? 0.2f : 1.0f;
        renderer.material.color = targetColor;
    }
}