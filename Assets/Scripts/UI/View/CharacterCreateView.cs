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
    private readonly List<GameObject> _spawnedJobSlots = new List<GameObject>();

    public void BindViewModel(CharacterCreateViewModel viewModel)
    {
        _viewModel = viewModel;

        viewModel.OnCreateFailed += OnCreateFailed;

        Input_Nickname.onValueChanged.RemoveListener(OnNickNameChanged);
        Input_Nickname.onValueChanged.AddListener(OnNickNameChanged);

        Button_Create.onClick.RemoveListener(OnClickCreate);
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

            _spawnedJobSlots.Add(slotObject);
        }
    }

    private void ClearJobList()
    {
        foreach (GameObject slot in _spawnedJobSlots)
        {
            Destroy(slot);
        }

        _spawnedJobSlots.Clear();
    }

    private void OnNickNameChanged(string value)
    {
        _viewModel.SetNickName(value);
    }

    private void OnJobSelected(PlayerTableData job)
    {
        _viewModel.SelectJob(job);
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
