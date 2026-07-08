public class InformationViewModel
{
    private readonly PlayerRepository _repository = new PlayerRepository();

    public PlayerData GetPlayerData()
    {
        return _repository.GetCurrentPlayer();
    }
}
