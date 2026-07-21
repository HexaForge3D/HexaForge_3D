using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

[Serializable]
public class SpawnPair
{
    public Transform _spawnPoint;
    public GameObject _monsterPrefab;
}

public class DefenceFieldManager : MonoBehaviour
{
    [SerializeField] private SpawnPair[] _spawnPairs;
    [SerializeField] private GameObject _clearPortal;
    [SerializeField] private GameObject _defenceTarget;
    [SerializeField] private int _waveCount = 5;
    [SerializeField] private float _countdownDuration = 20f;

    private void Start()
    {
        _clearPortal.SetActive(false);
        StartDefenceSequence().Forget();
    }
    private async UniTask StartDefenceSequence()
    {
        Debug.Log($"{_countdownDuration}초 후 디펜스 시작");

        await UniTask.Delay((int)(_countdownDuration * 1000));

        for (int i = 0; i < _waveCount; i++)
        {
            if (_defenceTarget == null)
            {
                Debug.Log("목표가 파괴되었습니다. 실패!");
                return;
            }

            SpawnAllDefinedMonsters();
            Debug.Log($"{i + 1} 웨이브 완료");

            Debug.Log($"{i + 2} 웨이브 {(_countdownDuration / 2)}초뒤 시작");
            await UniTask.Delay((int)(_countdownDuration * 500));
        }

        if (_defenceTarget != null)
        {
            _clearPortal.SetActive(true);
            Debug.Log("모든 웨이브 클리어! 클리어 포탈 활성화!");
        } 
    }

    private void SpawnAllDefinedMonsters()
    {
        foreach (var pair in _spawnPairs)
        {
            if (pair._spawnPoint != null && pair._monsterPrefab != null)
            {
                Instantiate(pair._monsterPrefab, pair._spawnPoint.position, pair._spawnPoint.rotation);
            }
        }
    }


}
