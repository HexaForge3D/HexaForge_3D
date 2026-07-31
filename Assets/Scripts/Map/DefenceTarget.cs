using System;
using UnityEngine;

public class DefenceTarget : InteractObject
{
    [SerializeField] private int _maxHp = 500;
    private int _currentHp;

    private bool _isDestroyed = false;
    private float _lastHitSoundTime = -2f;

    public static event Action OnTargetDestroyed;
    public static event Action OnDefenceStartRequested;
    public static event Action<int, int> OnTargetHpChanged;

    private AudioSource _myAudioSource;

    private void Awake()
    {
        _myAudioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        _currentHp = _maxHp;
        OnTargetHpChanged?.Invoke(_currentHp, _maxHp);
    }

    protected override void OnEnable()
    {
        base.OnEnable();
    }

    protected override void OnDisable()
    {
        base.OnDisable();
    }

    protected override bool CanInteract()
    {
        if (_isDestroyed)
        {
            return false;
        }
        return base.CanInteract();
    }

    protected override void OnInteract()
    {
        SoundManager.Instance?.PlaySFXWithSource(_myAudioSource, "Defence_Target_Help_Sound", 1f);
        Debug.Log("<color=green>[방어 목표 상호작용] 디펜스 시작 요청을 보냅니다.");
        OnDefenceStartRequested?.Invoke();
    }

    public void TakeDamage(int damage)
    {
        if (_isDestroyed) return;

        _currentHp -= damage;
        _currentHp = Mathf.Max(0, _currentHp);

        Debug.Log($"<color=orange>[방어 목표 피격]</color> 남은 체력: {_currentHp} / {_maxHp}");

        OnTargetHpChanged?.Invoke(_currentHp, _maxHp);

        if(Time.time - _lastHitSoundTime >= 2f)
        {
            SoundManager.Instance?.PlaySFXWithSource(_myAudioSource, "Defence_Target_TakeDamage_Sound", 1f);

            _lastHitSoundTime = Time.time;
        }

        if (_currentHp <= 0)
        {
            SoundManager.Instance?.PlaySFXSound("Defence_Target_Die_Sound", this.transform, 1f);
            DestroyTarget();        
        }
    }

    private void DestroyTarget()
    {
        if (_isDestroyed) return;
        _isDestroyed = true;

        Debug.Log("<color=red>[방어 목표 파괴]</color> 디펜스 실패!");
        OnTargetDestroyed?.Invoke();

        Destroy(gameObject);
    }
}