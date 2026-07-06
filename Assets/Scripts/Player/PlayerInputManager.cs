using UnityEngine;

public class PlayerInputManager : MonoBehaviour
{
    [Header("Movement Stop")] // S/s 버튼을 누르면 움직임 멈춤과 동시에 목적지 초기화
    [SerializeField] private KeyCode _playerStop = KeyCode.S;

    [Header("Skill Key")] // 스킬 버튼에 관한 내용
    [SerializeField] private KeyCode _skillKey1 = KeyCode.Q;
    [SerializeField] private KeyCode _skillKey2 = KeyCode.W;
    [SerializeField] private KeyCode _skillKey3 = KeyCode.E;
    [SerializeField] private KeyCode _skillKey4 = KeyCode.R;

    [Header("Inventory Key")] // 인벤토리 버튼
    [SerializeField] private KeyCode _inventoryKey = KeyCode.I;

    [Header("Interaction Key")] // 상호작용 버튼
    [SerializeField] private KeyCode _interactionKey = KeyCode.F;

    [Header("Information Key")] // 정보 버튼
    [SerializeField] private KeyCode _informationKey = KeyCode.C;

    [Header("Map Key")] // 미니 맵 버튼
    [SerializeField] private KeyCode _mapKey = KeyCode.M;

    private PlayerController _playerController;
   

    private void Start()
    {
        _playerController = GetComponent<PlayerController>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(_playerStop))
        {
            _playerController.MoveStop();
        }

        if (Input.GetKeyDown(_skillKey1))
        {
            Debug.Log("Q스킬 발동!");
            // 애니메이션 및, 공격 메서드 추가
        }

        if (Input.GetKeyDown(_skillKey2))
        {
            Debug.Log("W스킬 발동!");
            // 애니메이션 및, 공격 메서드 추가
        }

        if (Input.GetKeyDown(_skillKey3))
        {
            Debug.Log("E스킬 발동!");
            // 애니메이션 및, 공격 메서드 추가
        }

        if (Input.GetKeyDown(_skillKey4))
        {
            Debug.Log("R스킬 발동!");
            // 애니메이션 및, 공격 메서드 추가
        }

        if (Input.GetKeyDown(_inventoryKey))
        {
            Debug.Log("인벤토리가 열렸습니다.");
            // 인벤토리 Ui 열고 닫는 내용 추가
        }

        if (Input.GetKeyDown(_interactionKey))
        {
            Debug.Log("상호작용 실행");
            // 상호작용 하는 메서드 추가
        }

        if (Input.GetKeyDown(_informationKey))
        {
            Debug.Log("정보창 오픈");
            // InformationUi 출력
        }

        if (Input.GetKeyDown(_mapKey))
        {
            Debug.Log("Map을 열었습니다.");
            // MapUi 출력 => 없을 수 있으므로 지워도 될 듯
        }
    }
}