public class PlayerRepository
{
    private const string TempCurrentPlayerId = "Player_01";

    public PlayerData GetCurrentPlayer()
    {
        return GetPlayer(TempCurrentPlayerId);
    }

    public PlayerData GetPlayer(string id)
    {
        PlayerTableData tableData = GameDataManager.Instance.GetData<PlayerTableData>(id);
        return tableData == null ? null : ConvertToPlayerData(tableData);
    }

    private PlayerData ConvertToPlayerData(PlayerTableData tableData)
    {
        return new PlayerData(
            tableData.ID,
            tableData.Name,
            tableData.Job,
            tableData.Hp,
            tableData.Mp,
            tableData.Atk,
            tableData.Def
            );
    }
}
