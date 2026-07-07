using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private Transform _playerTransform;
    [SerializeField] private LayerMask _obstacleMask;
    //다중투명화시
    //private List<Renderer> _currentHitRenderers = new List<Renderer>(); 
    private Renderer _lastHitObjectRenderer;

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

        if (Physics.Raycast(rayOrigin, rayDirection, out RaycastHit hit, distance, _obstacleMask))
        {
            Renderer hitRenderer = hit.collider.GetComponent<Renderer>();

            if (_lastHitObjectRenderer != null && _lastHitObjectRenderer != hitRenderer)
            {
                ChangedHidingObject(_lastHitObjectRenderer, false);
            }

            if (hitRenderer != null)
            {
                _lastHitObjectRenderer = hitRenderer;
                ChangedHidingObject(_lastHitObjectRenderer, true);
            }
        }
        else
        {
            if (_lastHitObjectRenderer != null)
            {
                ChangedHidingObject(_lastHitObjectRenderer, false);
                _lastHitObjectRenderer = null;
            }
        }
    }

    private void ChangedHidingObject(Renderer renderer, bool isTransparent)
    {
        Color targetColor = renderer.material.color;
        targetColor.a = isTransparent ? 0.2f : 1.0f;
        renderer.material.color = targetColor;
    }
}