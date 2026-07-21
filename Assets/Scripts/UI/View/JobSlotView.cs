using System;
using TMPro;
using UnityEngine.UI;
using UnityEngine;

public class JobSlotView : MonoBehaviour
{
    [SerializeField] private TMP_Text Text_JobName;
    [SerializeField] private Button Button_Select;
    [SerializeField] private GameObject Image_SelectedOverlay;

    private PlayerTableData _job;
    private Action<PlayerTableData, JobSlotView> _onSelected;

    public void Setup(PlayerTableData job, Action<PlayerTableData, JobSlotView> onSelected)
    {
        _job = job;
        _onSelected = onSelected;

        Text_JobName.text = job.ID;

        Button_Select.onClick.RemoveAllListeners();
        Button_Select.onClick.AddListener(OnClickSelect);

        SetSelected(false);
    }

    public void SetSelected(bool isSelected)
    {
        Image_SelectedOverlay.SetActive(isSelected);
    }

    private void OnClickSelect()
    {
        _onSelected?.Invoke(_job, this);
    }
}
