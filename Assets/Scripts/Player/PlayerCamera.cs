using UnityEngine;
public class PlayerCamera : MonoBehaviour
{
    public GameObject Target; // 카메라가 따라갈 대상
    private PlayerController _playerController;

    private Vector3 _offset;
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
    }
}