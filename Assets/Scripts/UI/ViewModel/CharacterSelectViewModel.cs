using System;
using System.Collections.Generic;

public class CharacterSelectViewModel
{
    private readonly PlayerRepository _repository = new PlayerRepository();

    private PlayerData _selectedCharacter;

    public Action<PlayerData> OnCharacterSelected;
    public Action<PlayerData> OnEnterGameRequested;
    
    public void SelectCharacter(PlayerData data)
    {
        _selectedCharacter = data;
        OnCharacterSelected?.Invoke(data);
    }

    public List<PlayerData> GetSelecttableCharacters()
    {
        return _repository.GetAllPlayers();
    }

    public void RequestEnterGame()
    {
        if (_selectedCharacter == null) return;

        OnEnterGameRequested?.Invoke(_selectedCharacter);
    }
}
