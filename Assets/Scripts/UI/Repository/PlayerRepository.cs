using System.Collections.Generic;

public class PlayerRepository
{
    public PlayerData GetCharacter(string slotId)
    {
        CharacterSaveData saveData = FindSlot(slotId);
        return saveData == null || saveData.IsEmpty ? null : ConvertToPlayerData(saveData);
    }
    
    public List<PlayerData> GetAllCharacters()
    {
        List<PlayerData> result = new List<PlayerData>();

        foreach (CharacterSaveData saveData in SaveManager.Instance.CurrentSaveData.Slots)
        {
            if (saveData.IsEmpty == false)
            {
                result.Add(ConvertToPlayerData(saveData));
            }
        }

        return result;
    }

    private CharacterSaveData FindSlot(string slotId)
    {
        foreach (CharacterSaveData slot in SaveManager.Instance.CurrentSaveData.Slots)
        {
            if (slot.SlotId == slotId)
            {
                return slot;
            }
        }

        return null;
    }

    private PlayerData ConvertToPlayerData(CharacterSaveData saveData)
    {
        return new PlayerData(
            saveData.SlotId,
            saveData.Name,
            saveData.Job,
            saveData.Hp,
            saveData.Mp,
            saveData.Atk,
            saveData.Def
            );
    }
}
