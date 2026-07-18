using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEngine;

public class SkillTreeView : BaseOverLayUI
{
    [SerializeField] private TMP_Text Text_AvailablePoints;
    [SerializeField] private Transform Transform_SlotParent;
    [SerializeField] private GameObject Prefab_SkillTreeSlot;
    [SerializeField] private Button Button_ResetAll;

    private SkillTreeViewModel _viewModel;
    private readonly List<GameObject> _spawnedSlots = new List<GameObject>();

    public void BindViewModel(SkillTreeViewModel viewModel)
    {
        _viewModel = viewModel;

        SkillUtil.Instance.OnSkillDataUpdated += Refresh;

        Button_ResetAll.onClick.RemoveAllListeners();
        Button_ResetAll.onClick.AddListener(OnClickResetAll);

        Refresh();
    }

    private void OnDestroy()
    {
        if (SkillUtil.Instance != null)
        {
            SkillUtil.Instance.OnSkillDataUpdated -= Refresh;
        }
    }

    private void Refresh()
    {
        Text_AvailablePoints.text = $"Skill Points: {_viewModel.GetAvailablePoints()}";
        BuildSlot();
    }

    private void BuildSlot()
    {
        ClearSlots();

        List<SkillTreeSlotData> slots = _viewModel.GetSkillSlots();

        foreach (SkillTreeSlotData slot in slots)
        {
            GameObject slotObject = Instantiate(Prefab_SkillTreeSlot, Transform_SlotParent);
            SkillTreeSlotView slotView = slotObject.GetComponent<SkillTreeSlotView>();
            slotView.Setup(slot, OnUpgradeClicked, OnDowngradeClicked);

            _spawnedSlots.Add(slotObject);
        }
    }

    private void ClearSlots()
    {
        foreach (GameObject slot in _spawnedSlots)
        {
            Destroy(slot);
        }

        _spawnedSlots.Clear();
    }

    private void OnUpgradeClicked(string skillId)
    {
        _viewModel.RequestUpgrade(skillId);
    }

    private void OnDowngradeClicked(string skillId)
    {
        _viewModel.RequestDowngrade(skillId);
    }

    private void OnClickResetAll()
    {
        _viewModel.RequestResetAll();
    }
}
