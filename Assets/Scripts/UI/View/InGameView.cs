using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InGameView : BaseUI
{
    [SerializeField] private Image Image_HpBar;
    [SerializeField] private Image Image_MpBar;

    [SerializeField] private Transform Transform_SkillSlotParent;
    [SerializeField] private GameObject Prefab_SkillSlot;

    private static readonly string[] SkillKeyLabels = { "Q", "W", "E", "R", "A", "S", "D", "F" };

    private InGameViewModel _viewModel;
    private readonly List<GameObject> _spawnedSkillSlots = new List<GameObject>();

    public void BindViewModel(InGameViewModel viewModel)
    {
        _viewModel = viewModel;

        Image_HpBar.fillAmount = 1f;
        Image_MpBar.fillAmount = 1f;

        BuildSkillSlots();
    }

    private void BuildSkillSlots()
    {
        ClearSkillSlots();

        List<SkillData> skills = _viewModel.GetSkillSlots();

        for (int i = 0;  i < skills.Count; i++)
        {
            GameObject slotObject = Instantiate(Prefab_SkillSlot, Transform_SkillSlotParent);
            SkillSlotView slotView = slotObject.GetComponent<SkillSlotView>();
            slotView.Setup(skills[i], SkillKeyLabels[i]);

            _spawnedSkillSlots.Add(slotObject);
        }
    }

    private void ClearSkillSlots()
    {
        foreach (GameObject slot in _spawnedSkillSlots) Destroy(slot);

        _spawnedSkillSlots.Clear();
    }
}
