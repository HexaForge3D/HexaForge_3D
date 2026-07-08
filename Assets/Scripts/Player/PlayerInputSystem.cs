using UnityEngine;
using System;

public class PlayerInputSystem : MonoBehaviour
{
    [Header("Movement Stop")] // S/s 버튼을 누르면 움직임 멈춤과 동시에 목적지 초기화
    [SerializeField] private KeyCode _playerStop = KeyCode.S;

    [Header("Skill Key")] // 스킬 버튼에 관한 내용
    [SerializeField] private KeyCode _skillKey1 = KeyCode.Q;
    [SerializeField] private KeyCode _skillKey2 = KeyCode.W;
    [SerializeField] private KeyCode _skillKey3 = KeyCode.E;
    [SerializeField] private KeyCode _skillKey4 = KeyCode.R;
    [SerializeField] private KeyCode _evasionKey = KeyCode.Space;

    [Header("Item Key")] // 아이템 버튼
    [SerializeField] private KeyCode _itemKey1 = KeyCode.Alpha1;
    [SerializeField] private KeyCode _itemKey2 = KeyCode.Alpha2;    
    [SerializeField] private KeyCode _itemKey3 = KeyCode.Alpha3;
    [SerializeField] private KeyCode _itemKey4 = KeyCode.Alpha4;


    [Header("Inventory Key")] // 인벤토리 버튼
    [SerializeField] private KeyCode _inventoryKey = KeyCode.I;

    [Header("Interaction Key")] // 상호작용 버튼
    [SerializeField] private KeyCode _interactionKey = KeyCode.F;

    [Header("Information Key")] // 정보 버튼
    [SerializeField] private KeyCode _informationKey = KeyCode.C;

    [Header("Map Key")] // 미니 맵 버튼
    [SerializeField] private KeyCode _mapKey = KeyCode.M;

    private PlayerController _playerController;

    public static event Action OnInteract;

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

        if (Input.GetKeyDown(_evasionKey))
        {
            Debug.Log("회피 실행");
            // 회피 메서드 추가
        }

        if (Input.GetKeyDown(_itemKey1))
        {
            Debug.Log("아이템1 사용");
            // 아이템1 사용 메서드 추가
        }

        if (Input.GetKeyDown(_itemKey2)) 
        {
            Debug.Log("아이템2 사용");
            // 아이템2 사용 메서드 추가
        }

        if (Input.GetKeyDown(_itemKey3))
        {
            Debug.Log("아이템3 사용");
            // 아이템3 사용 메서드 추가
        }

        if (Input.GetKeyDown(_itemKey4))
        {
            Debug.Log("아이템4 사용");
            // 아이템4 사용 메서드 추가
        }

        if (Input.GetKeyDown(_inventoryKey))
        {
            Debug.Log("인벤토리가 열렸습니다.");
            // 인벤토리 Ui 열고 닫는 내용 추가
        }

        if (Input.GetKeyDown(_interactionKey))
        {
            Debug.Log("상호작용 실행");
            OnInteract?.Invoke();
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

    private void Evasion()
    {
        //앙기모띠
    }
}