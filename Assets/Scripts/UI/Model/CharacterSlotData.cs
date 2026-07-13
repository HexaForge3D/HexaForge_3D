public class CharacterSlotData
{
    public string SlotId {  get; private set; }
    public bool IsEmpty { get; private set; }
    public PlayerData Character {  get; private set; }

    public CharacterSlotData(string slotId, bool isEmpty, PlayerData character)
    {
        SlotId = slotId; 
        IsEmpty = isEmpty; 
        Character = character;
    }
}
