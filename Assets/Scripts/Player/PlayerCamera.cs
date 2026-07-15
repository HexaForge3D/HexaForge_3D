using UnityEngine;
using System.Collections.Generic;

public class PlayerCamera : MonoBehaviour
{
    [Header("Camera Setting")]
    [SerializeField] private Transform Target;
    [SerializeField] private LayerMask _obstacleMask;
    [SerializeField] private float _checkRadius = 1f; // 구 콜라이더 반지름 설정

    [Header("Camera Position")]
    [SerializeField] private Vector3 _offset;
    [SerializeField] private Vector3 _cameraAngle;

    [Header("Material Swap Setting")]
    [SerializeField] private Material _transparentMaterial;

    private PlayerController _playerController;

    private CapsuleCollider _capsuleCollider;
    private Transform _colliderTransform;

    // 원래 있던 Material과 바꿀 Material을 위해서 원래 값을 담는 Dictionary를 생성
    private Dictionary<Renderer, Material[]> _originalMaterials = new Dictionary<Renderer, Material[]>();

    private void Start()
    {
        if (Target != null)
        {
            _playerController = Target.GetComponent<PlayerController>();
            _offset = transform.position - Target.transform.position;
            _cameraAngle = transform.eulerAngles;
        }

        GameObject colliderObj = new GameObject("SightCollider");
        colliderObj.transform.SetParent(this.transform);
        colliderObj.transform.localPosition = Vector3.zero; // 위치를 카메라와 똑같이 맞춤
        _colliderTransform = colliderObj.transform;

        _capsuleCollider = colliderObj.AddComponent<CapsuleCollider>();
        _capsuleCollider.isTrigger = true;
        _capsuleCollider.direction = 2; // Z축(앞 방향)으로 길어지도록 설정 => 0 이면 좌우로, 1 이면 위아래
        _capsuleCollider.radius = _checkRadius;
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

        if (_capsuleCollider != null && _colliderTransform != null)
        {
            _capsuleCollider.radius = _checkRadius; // 콜라이더 두께 적용

            _colliderTransform.LookAt(Target.position); // 캡슐 콜라이더가 플레이어 쪽을 향해 바라볼 수 있도록
            // 카메라와 플레이어 사이의 거리를 재서 캡슐의 길이로 설정
            float distance = Vector3.Distance(transform.position, Target.position);
            _capsuleCollider.height = distance;

            _capsuleCollider.center = new Vector3(0, 0, distance / 2f);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // 충돌한 오브젝트의 레이어가 _obstacleMask에 포함되어 있는지 확인
        if (_obstacleMask.Contains(other.gameObject.layer))
        {
            Renderer[] targetRenderers = other.GetComponentsInChildren<Renderer>();

            foreach (Renderer targetRenderer in targetRenderers)
            {
                if (targetRenderer != null)
                {
                    ChangedHidingObject(targetRenderer, true);
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {

        if (_obstacleMask.Contains(other.gameObject.layer))
        {
            Renderer[] targetRenderers = other.GetComponentsInChildren<Renderer>();

            foreach (Renderer targetRenderer in targetRenderers)
            {
                if (targetRenderer != null)
                {
                    ChangedHidingObject(targetRenderer, false);
                }
            }
        }
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

    private void OnDrawGizmos()
    {
        if (Target == null) return;

        Gizmos.color = Color.purple;

        Vector3 startPos = transform.position;
        Vector3 endPos = Target.position;

        Gizmos.DrawWireSphere(startPos, _checkRadius);
        Gizmos.DrawWireSphere(endPos, _checkRadius);

        Vector3 direction = (endPos - startPos).normalized;
        Vector3 up = Vector3.Cross(direction, Vector3.right).normalized;

        if (up == Vector3.zero) up = Vector3.Cross(direction, Vector3.forward).normalized;

        Vector3 right = Vector3.Cross(up, direction).normalized;

        Gizmos.DrawLine(startPos + up * _checkRadius, endPos + up * _checkRadius);
        Gizmos.DrawLine(startPos - up * _checkRadius, endPos - up * _checkRadius);
        Gizmos.DrawLine(startPos + right * _checkRadius, endPos + right * _checkRadius);
        Gizmos.DrawLine(startPos - right * _checkRadius, endPos - right * _checkRadius);
    }
}