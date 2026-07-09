using System;
using UnityEngine;

public class CharacterSelectViewModel
{
    private readonly PlayerRepository _repository = new PlayerRepository();
    
    public Action<PlayerData> OnEnterGameRequested;

    public PlayerData GetSelectableCharacter()
    {
        return _repository.GetCurrentPlayer();
    }

    public void RequestEnterGame()
    {
        PlayerData data = GetSelectableCharacter();

        if (data == null) return;

        OnEnterGameRequested?.Invoke(data);
    }
}
