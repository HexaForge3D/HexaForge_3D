using UnityEngine;

// 게임의 시작지점을 확인함
public class GameEntryPoint : MonoBehaviour
{
    private GameFlowManager _gameFlowManager;

    private async void Start()
    {
        _gameFlowManager = new GameFlowManager();
        await _gameFlowManager.StartAsync();
    }
}
