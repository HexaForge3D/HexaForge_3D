using UnityEngine;
public class PlayerCamera : MonoBehaviour
{
    public GameObject Target; // 카메라가 따라갈 대상

    public float offsetX = 0f; // 카메라의 X축 오프셋
    public float offsetY = 10f; // 카메라의 Y축 오프셋
    public float offsetZ = -10f; // 카메라의 Z축 오프셋

    public float CameraSpeed = 5f; // 카메라 이동 속도

    Vector3 TargetPos;

    private void FixedUpdate()
    {
        TargetPos = new Vector3(Target.transform.position.x + offsetX, Target.transform.position.y + offsetY, Target.transform.position.z + offsetZ);
        transform.position = Vector3.Lerp(transform.position, TargetPos, CameraSpeed * Time.deltaTime * CameraSpeed);
    }
}
