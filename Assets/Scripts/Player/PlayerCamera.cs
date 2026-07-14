using UnityEngine;
using System.Collections.Generic;

public class PlayerCamera : MonoBehaviour
{
    [Header("Camera Setting")]
    [SerializeField] private Transform Target;
    [SerializeField] private LayerMask _obstacleMask;
    [SerializeField] private float _checkRadius = 1f; // 레이케스트 두께

    [Header("Camera Position")]
    [SerializeField] private Vector3 _offset;
    [SerializeField] private Vector3 _cameraAngle;

    [Header("Material Swap Setting")]
    [SerializeField] private Material _transparentMaterial;

    private PlayerController _playerController;

    // 원래 있던 Material과 바꿀 Material을 위해서 원래 값을 담는 Dictionary를 생성
    private Dictionary<Renderer, Material[]> _originalMaterials = new Dictionary<Renderer, Material[]>();
    private HashSet<Renderer> _currentHitRenderers = new HashSet<Renderer>();

    private void Start()
    {
        if (Target != null)
        {
            _playerController = Target.GetComponent<PlayerController>();
            _offset = transform.position - Target.transform.position;
            _cameraAngle = transform.eulerAngles;
        }
    }

    private void LateUpdate()
    {
        if (Target == null || _playerController == null) return;

        // 높이, 거리 조절
        Vector3 targetPos = Target.transform.position + _offset;

        float playerSpeed = _playerController.MoveSpeed;
        transform.position = Vector3.Lerp(transform.position, targetPos, playerSpeed * Time.deltaTime);

        // 각도 조절
        Quaternion targetRotation = Quaternion.Euler(_cameraAngle);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, playerSpeed * Time.deltaTime);

        CheckOcclusion();
    }

    private void CheckOcclusion()
    {
        Vector3 direction = Target.transform.position - transform.position;
        float distance = direction.magnitude;
        Vector3 rayOrigin = transform.position;
        Vector3 rayDirection = direction.normalized;

        RaycastHit[] hits = Physics.SphereCastAll(rayOrigin, _checkRadius, rayDirection, distance, _obstacleMask);

        HashSet<Renderer> currentHitRenderList = new HashSet<Renderer>();

        foreach (RaycastHit hit in hits)
        {
            Renderer[] targetRenderers = hit.collider.GetComponentsInChildren<Renderer>();

            foreach (Renderer targetRenderer in targetRenderers)
            {
                if (targetRenderer != null)
                {
                    currentHitRenderList.Add(targetRenderer);

                    if (_currentHitRenderers.Contains(targetRenderer) == false)
                    {
                        ChangedHidingObject(targetRenderer, true);
                    }
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
        // 투명 머티리얼이 Inspector에 안 들어가 있으면 작동하지 않음
        if (renderer == null || _transparentMaterial == null) return;

        if (isTransparent)
        {
            // 1. 처음 가려진 거라면 원래 머티리얼을 딕셔너리에 안전하게 보관
            if (_originalMaterials.ContainsKey(renderer) == false)
            {
                _originalMaterials.Add(renderer, renderer.sharedMaterials);

                // 2. 투명 머티리얼로 옷 갈아입히기
                Material[] transparentMats = new Material[renderer.sharedMaterials.Length];
                for (int i = 0; i < transparentMats.Length; i++)
                {
                    transparentMats[i] = _transparentMaterial;
                }
                renderer.sharedMaterials = transparentMats;
            }
        }
        else
        {
            // 3. 카메라 밖으로 벗어나면 보관해둔 원래 머티리얼로 다시 복구
            if (_originalMaterials.ContainsKey(renderer))
            {
                renderer.sharedMaterials = _originalMaterials[renderer];
                _originalMaterials.Remove(renderer);
            }
        }
    }
}