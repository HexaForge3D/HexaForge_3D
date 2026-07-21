using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEngine;

public class CharacterCreateView : BaseOverLayUI
{
    [SerializeField] private TMP_InputField Input_Nickname;
    [SerializeField] private Transform Transform_JobListParent;
    [SerializeField] private GameObject Prefab_JobSlot;
    [SerializeField] private Button Button_Create;
    [SerializeField] private TMP_Text Text_ErrorMessage;

    private CharacterCreateViewModel _viewModel;
    private readonly List<JobSlotView> _spawnedJobSlots = new List<JobSlotView>();

    public void BindViewModel(CharacterCreateViewModel viewModel)
    {
        _viewModel = viewModel;

        viewModel.OnCreateFailed += OnCreateFailed;

        Input_Nickname.onValueChanged.RemoveAllListeners();
        Input_Nickname.onValueChanged.AddListener(OnNickNameChanged);

        Button_Create.onClick.RemoveAllListeners();
        Button_Create.onClick.AddListener(OnClickCreate);

        Text_ErrorMessage.text = string.Empty;

        BuildJobList();
    }

    private void BuildJobList()
    {
        ClearJobList();

        List<PlayerTableData> jobs = _viewModel.GetJobList();

        foreach (PlayerTableData job in jobs)
        {
            GameObject slotObject = Instantiate(Prefab_JobSlot, Transform_JobListParent);
            JobSlotView slotView = slotObject.GetComponent<JobSlotView>();
            slotView.Setup(job, OnJobSelected);

            _spawnedJobSlots.Add(slotView);
        }
    }

    private void ClearJobList()
    {
        foreach (JobSlotView slotView in _spawnedJobSlots)
        {
            Destroy(slotView.gameObject);
        }

        _spawnedJobSlots.Clear();
    }

    private void OnNickNameChanged(string value)
    {
        _viewModel.SetNickName(value);
    }

    private void OnJobSelected(PlayerTableData job, JobSlotView clickedSlot)
    {
        _viewModel.SelectJob(job);

        foreach (JobSlotView slotView in _spawnedJobSlots)
        {
            slotView.SetSelected(slotView == clickedSlot);
        }
    }

    private void OnClickCreate()
    {
        _viewModel?.RequestCreate();
    }

    private void OnCreateFailed(string message)
    {
        Text_ErrorMessage.text = message;
    }
}
