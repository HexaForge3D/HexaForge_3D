using Cysharp.Threading.Tasks;
using UnityEngine;

// 게임의 흐름 타이밍을 관리할 스크립트
public class GameFlowManager
{
    public async UniTask StartAsync()
    {
        await ShowTitleAsync();
    }

    private async UniTask ShowTitleAsync()
    {

    }


}
