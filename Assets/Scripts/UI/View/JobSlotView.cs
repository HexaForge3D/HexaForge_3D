using System;
using TMPro;
using UnityEngine.UI;
using UnityEngine;

public class JobSlotView : MonoBehaviour
{
    [SerializeField] private TMP_Text Text_JobName;
    [SerializeField] private Button Button_Select;

    private PlayerTableData _job;
    private Action<PlayerTableData> _onSelected;

    public void Setup(PlayerTableData job, Action<PlayerTableData> onSelected)
    {
        _job = job;
        _onSelected = onSelected;

        Text_JobName.text = job.ID;

        Button_Select.onClick.RemoveListener(OnClickSelect);
        Button_Select.onClick.AddListener(OnClickSelect);
    }

    private void OnClickSelect()
    {
        _onSelected?.Invoke(_job);
    }
}
