using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InGameView : BaseUI
{
    [SerializeField] private Image Image_HpBar;
    [SerializeField] private Image Image_MpBar;

    [SerializeField] private TMP_Text Text_HpValue;
    [SerializeField] private TMP_Text Text_MpValue;

    [SerializeField] private Transform Transform_SkillSlotParent;
    [SerializeField] private GameObject Prefab_SkillSlot;

    private static readonly string[] SkillKeyLabels = { "Q", "W", "E", "R", "A", "S", "D", "F" };

    private InGameViewModel _viewModel;
    private readonly Dictionary<string, SkillSlotView> _skillSlotViews = new Dictionary<string, SkillSlotView>();

    public void BindViewModel(InGameViewModel viewModel)
    {
        _viewModel = viewModel;
        viewModel.OnHpRatioChanged += OnHpRatioChanged;
        viewModel.OnHpValueChanged += OnHpValueChanged;
        viewModel.OnMpRatioChanged += OnMpRatioChanged;

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

            _skillSlotViews.Add(SkillKeyLabels[i], slotView);           
        }
    }

    private void ClearSkillSlots()
    {
        foreach (SkillSlotView slotView in _skillSlotViews.Values) Destroy(slotView.gameObject);

        _skillSlotViews.Clear();
    }

    private void OnHpRatioChanged(float ratio)
    {
        Image_HpBar.fillAmount = ratio;
    }

    private void OnHpValueChanged(int currentHp, int maxHp)
    {
        Text_HpValue.text = $"{currentHp}/{maxHp}";  
    }

    private void OnMpRatioChanged(float ratio)
    {
        Image_MpBar.fillAmount = ratio;
    }

    public void StartSkillCoolDown(string keyLabel, float duration)
    {
        if (_skillSlotViews.TryGetValue(keyLabel, out SkillSlotView slotView))
        {
            slotView.StartCoolDown(duration);
        }
    }
}
