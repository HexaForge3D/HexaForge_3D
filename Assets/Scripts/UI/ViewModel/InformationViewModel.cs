public class InformationViewModel
{
    private readonly PlayerRepository _repository = new PlayerRepository();
    private readonly string _currentSlotId;

    public InformationViewModel(string currentSlotId)
    {
        _currentSlotId = currentSlotId;
    }

    public PlayerData GetPlayerData()
    {
        return _repository.GetCharacter(_currentSlotId);
    }
}
