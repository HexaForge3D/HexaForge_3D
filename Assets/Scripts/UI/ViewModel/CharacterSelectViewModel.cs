using System;
using System.Collections.Generic;

public class CharacterSelectViewModel
{
    private readonly PlayerRepository _repository = new PlayerRepository();

    private CharacterSlotData _selectedSlot;

    public Action<CharacterSlotData> OnSlotSelected;
    public Action<string> OnCreateCharacterRequested;
    public Action<PlayerData> OnEnterGameRequested;
    
    public List<CharacterSlotData> GetAllSlots()
    {
        return _repository.GetAllSlots();
    }

    public void SelectSlot(CharacterSlotData slot)
    {
        if (slot.IsEmpty)
        {
            OnCreateCharacterRequested?.Invoke(slot.SlotId);
            return;
        }

        _selectedSlot = slot;
        OnSlotSelected?.Invoke(slot);
    }

    public void RequestEnterGame()
    {
        if (_selectedSlot == null || _selectedSlot.IsEmpty) return;

        OnEnterGameRequested?.Invoke(_selectedSlot.Character);
    }
}
