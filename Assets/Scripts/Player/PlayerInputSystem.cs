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
    [SerializeField] private KeyCode _skillKey5 = KeyCode.A;
    [SerializeField] private KeyCode _skillKey6 = KeyCode.S;
    [SerializeField] private KeyCode _skillKey7 = KeyCode.D;
    [SerializeField] private KeyCode _skillKey8 = KeyCode.F;
    [SerializeField] private KeyCode _skillinfoKey = KeyCode.K;
    [SerializeField] private KeyCode _evasionKey = KeyCode.Space;

    [Header("Item Key")] // 아이템 버튼
    [SerializeField] private KeyCode _itemKey1 = KeyCode.Alpha1;
    [SerializeField] private KeyCode _itemKey2 = KeyCode.Alpha2;
    [SerializeField] private KeyCode _itemKey3 = KeyCode.Alpha3;
    [SerializeField] private KeyCode _itemKey4 = KeyCode.Alpha4;


    [Header("Inventory Key")] // 인벤토리 버튼
    [SerializeField] private KeyCode _inventoryKey = KeyCode.I;

    [Header("Interaction Key")] // 상호작용 버튼
    [SerializeField] private KeyCode _interactionKey = KeyCode.G;

    [Header("Information Key")] // 정보 버튼
    [SerializeField] private KeyCode _informationKey = KeyCode.C;

    [Header("Map Key")] // 미니 맵 버튼
    [SerializeField] private KeyCode _mapKey = KeyCode.M;

    [Header("System Key")] // 시스템 버튼
    [SerializeField] private KeyCode _systemKey = KeyCode.Escape;

    private PlayerController _playerController;

    public static event Action OnSkill1;
    public static event Action OnSkill2;
    public static event Action OnSkill3;
    public static event Action OnSkill4;
    public static event Action OnSkill5;
    public static event Action OnSkill6;
    public static event Action OnSkill7;
    public static event Action OnSkill8;
    public static event Action OnEvasion;
    public static event Action OnSkillinfo;

    public static event Action OnItem1;
    public static event Action OnItem2;
    public static event Action OnItem3;
    public static event Action OnItem4;

    public static event Action OnInventory;

    public static event Action OnInteract;

    public static event Action OnInformation;
    public static event Action OnMap;
    public static event Action OnSystem;

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
            OnSkill1?.Invoke();
        }

        if (Input.GetKeyDown(_skillKey2))
        {
            Debug.Log("W스킬 발동!");
            // 애니메이션 및, 공격 메서드 추가
            OnSkill2?.Invoke();
        }

        if (Input.GetKeyDown(_skillKey3))
        {
            Debug.Log("E스킬 발동!");
            // 애니메이션 및, 공격 메서드 추가
            OnSkill3?.Invoke();
        }

        if (Input.GetKeyDown(_skillKey4))
        {
            Debug.Log("R스킬 발동!");
            // 애니메이션 및, 공격 메서드 추가
            OnSkill4?.Invoke();
        }

        if (Input.GetKeyDown(_skillKey5))
        {
            Debug.Log("A스킬 발동!");
            // 애니메이션 및, 공격 메서드 추가
            OnSkill5?.Invoke();
        }

        if (Input.GetKeyDown(_skillKey6))
        {
            Debug.Log("S스킬 발동!");
            // 애니메이션 및, 공격 메서드 추가
            OnSkill6?.Invoke();
        }

        if (Input.GetKeyDown(_skillKey7))
        {
            Debug.Log("D스킬 발동!");
            // 애니메이션 및, 공격 메서드 추가
            OnSkill7?.Invoke();
        }

        if (Input.GetKeyDown(_skillKey8))
        {
            Debug.Log("F스킬 발동!");
            // 애니메이션 및, 공격 메서드 추가
            OnSkill8?.Invoke();
        }

        if (Input.GetKeyDown(_evasionKey))
        {
            Debug.Log("회피 실행");
            // 회피 메서드 추가
            OnEvasion?.Invoke();
        }

        if (Input.GetKeyDown(_skillinfoKey))
        {
            Debug.Log("스킬 정보창 출력");
            OnSkillinfo?.Invoke();
        }

        if (Input.GetKeyDown(_itemKey1))
        {
            Debug.Log("아이템1 사용");
            // 아이템1 사용 메서드 추가
            OnItem1?.Invoke();
        }

        if (Input.GetKeyDown(_itemKey2))
        {
            Debug.Log("아이템2 사용");
            // 아이템2 사용 메서드 추가
            OnItem2?.Invoke();
        }

        if (Input.GetKeyDown(_itemKey3))
        {
            Debug.Log("아이템3 사용");
            // 아이템3 사용 메서드 추가
            OnItem3?.Invoke();
        }

        if (Input.GetKeyDown(_itemKey4))
        {
            Debug.Log("아이템4 사용");
            // 아이템4 사용 메서드 추가
            OnItem4?.Invoke();
        }

        if (Input.GetKeyDown(_inventoryKey))
        {
            Debug.Log("인벤토리가 열렸습니다.");
            // 인벤토리 Ui 열고 닫는 내용 추가
            OnInventory?.Invoke();
        }

        if (Input.GetKeyDown(_interactionKey))
        {
            Debug.Log("상호작용 실행");
            OnInteract?.Invoke();
        }

        if (Input.GetKeyDown(_systemKey))
        {
            Debug.Log("시스템창이 열렸습니다.");
            // 시스템 Ui 열고 닫는 내용 추가
            OnSystem?.Invoke();
        }

        if (Input.GetKeyDown(_informationKey))
        {
            if (_playerController != null && _playerController.PlayerData != null)
            {
                CharacterSaveData data = _playerController.PlayerData;

                Debug.Log($"<color=yellow>[플레이어 상태창]</color>\n" +
                          $"이름: {data.Name}\n" +
                          $"직업: {data.Job}\n" +
                          $"HP: {data.Hp}\n" +
                          $"MP: {data.Mp}\n" +
                          $"공격력(ATK): {data.Atk}\n" +
                          $"방어력(DEF): {data.Def}");
            }
            else
            {
                Debug.LogWarning("플레이어 데이터를 아직 불러오지 못했습니다.");
            }
            OnInformation?.Invoke();
        }

        if (Input.GetKeyDown(_mapKey))
        {
            Debug.Log("Map을 열었습니다.");
            // MapUi 출력 => 없을 수 있으므로 지워도 될 듯
            OnMap?.Invoke();
        }
    }

    private void Evasion()
    {
        // 추가 구현 예정
    }
}