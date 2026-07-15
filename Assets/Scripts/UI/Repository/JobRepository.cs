using System.Collections.Generic;
using UnityEngine;

public class JobRepository : MonoBehaviour
{
    public List<PlayerTableData> GetAllJobs()
    {
        Dictionary<string, PlayerTableData> table = GameDataManager.Instance.GetAllData<PlayerTableData > ();
        List<PlayerTableData> result = new List<PlayerTableData>();

        if (table == null) return result;

        foreach (PlayerTableData job in table.Values)
        {
            result.Add(job);
        }

        return result;
    }

    public PlayerTableData GetJob(string jobId)
    {
        return GameDataManager.Instance.GetData<PlayerTableData>(jobId);
    }
}
