using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class CharacterCreateViewModel
{
    private readonly JobRepository _jobRepository = new JobRepository();
    private readonly string _slotId;

    private string _nickName;
    private PlayerTableData _selectedJob;

    public Action OnCharacterCreated;
    public Action<string> OnCreateFailed;

    public CharacterCreateViewModel(string slotId)
    {
        _slotId = slotId;
    }

    public List<PlayerTableData> GetJobList()
    {
        return _jobRepository.GetAllJobs();
    }

    public void SetNickName(string nickName)
    {
        _nickName = nickName;
    }

    public void SelectJob(PlayerTableData job)
    {
        _selectedJob = job;
    }

    public void RequestCreate()
    {
        if (IsValidNickName(_nickName) == false)
        {
            OnCreateFailed?.Invoke("Only 1~10 ENG words");
            return;
        }

        if (_selectedJob == null)
        {
            OnCreateFailed?.Invoke("Please Choose Your Job");
            return;
        }

        bool success = SaveManager.Instance.CreateCharacter(_slotId, _nickName, _selectedJob.ID);

        if (success)
        {
            OnCharacterCreated?.Invoke();
        }
        else
        {
            OnCreateFailed?.Invoke("Failed Create Character");
        }
    }

    public bool IsValidNickName(string nickName)
    {
        if (string.IsNullOrEmpty(nickName))
        {
            return false;
        }

        if (nickName.Length > 10)
        {
            return false;
        }

        return Regex.IsMatch(nickName, "^[a-zA-Z]+$");
    }
}
