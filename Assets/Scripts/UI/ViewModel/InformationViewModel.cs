public class InformationViewModel
{
    private readonly string _currentSlotId;

    public InformationViewModel(string currentSlotId)
    {
        _currentSlotId = currentSlotId;
    }

    public CharacterSaveData GetSaveData()
    {
        return SaveManager.Instance.GetChararcterData(_currentSlotId);
    }

    public int GetLevel(int exp)
    {
        return PlayerLevel.LevelFromExp(exp);
    }

    public int GetFinalAtk()
    {
        return SaveManager.Instance.GetFinalAtk(_currentSlotId);
    }

    public int GetFinalDef()
    {
        return SaveManager.Instance.GetFinalDef(_currentSlotId);
    }

}
