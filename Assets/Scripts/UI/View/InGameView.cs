using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InGameView : BaseUI
{
    [SerializeField] private Image Image_HpBar;
    [SerializeField] private Image Image_MpBar;
    [SerializeField] private Image Image_ExpBar;

    [SerializeField] private TMP_Text Text_HpValue;
    [SerializeField] private TMP_Text Text_MpValue;
    [SerializeField] private TMP_Text Text_ExpValue;

    [SerializeField] private Transform Transform_SkillSlotParent;
    [SerializeField] private GameObject Prefab_SkillSlot;
    [SerializeField] private EvasionSlotView EvasionSlotView;

    [SerializeField] private Button Button_Equipment;
    [SerializeField] private Button Button_Skill;
    [SerializeField] private Button Button_Inventory;
    [SerializeField] private Button Button_CharacterInfo;

    public Action OnCharacterInfoButtonClicked;
    public Action OnSkillButtonClicked;
    public Action OnInventoryButtonClicked;
    public Action OnEquipmentButtonClicked;

    private static readonly string[] SkillKeyLabels = { "Q", "W", "E", "R", "A", "S", "D", "F" };

    private InGameViewModel _viewModel;
    private readonly Dictionary<string, SkillSlotView> _skillSlotViews = new Dictionary<string, SkillSlotView>();

    public void BindViewModel(InGameViewModel viewModel)
    {
        _viewModel = viewModel;
        viewModel.OnHpRatioChanged += OnHpRatioChanged;
        viewModel.OnHpValueChanged += OnHpValueChanged;
        viewModel.OnMpRatioChanged += OnMpRatioChanged;
        viewModel.OnMpValueChanged += OnMpValueChanged;
        viewModel.OnExpRatioChanged += OnExpRatioChanged;
        viewModel.OnExpValueChanged += OnExpValueChanged;

        Button_CharacterInfo.onClick.RemoveListener(OnClickCharacterInfo);
        Button_CharacterInfo.onClick.AddListener(OnClickCharacterInfo);

        Button_Inventory.onClick.RemoveListener(OnClickInventory);
        Button_Inventory.onClick.AddListener(OnClickInventory);

        Button_Skill.onClick.RemoveListener(OnClickSkill);
        Button_Skill.onClick.AddListener(OnClickSkill);

        Button_Equipment.onClick.RemoveListener(OnClickEquipment);
        Button_Equipment.onClick.AddListener(OnClickEquipment);

        _viewModel.GetInitialHpMp(out int currentHp, out int maxHp, out int currentMp, out int  maxMp);

        float hpRatio = maxHp > 0 ? (float)currentHp / maxHp : 0f;
        Image_HpBar.fillAmount = hpRatio;
        Text_HpValue.text = $"{currentHp}/{maxHp}";

        float mpRatio = maxMp > 0 ? (float)currentMp / maxMp : 0f;
        Image_MpBar.fillAmount = mpRatio;
        Text_MpValue.text = $"{currentMp}/{maxMp}";

        int initialExp = _viewModel.GetInitialExp();
        _viewModel.HandleExpChanged(initialExp);

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

    private void OnMpValueChanged(int currentMp, int maxMp)
    {
        Text_MpValue.text = $"{currentMp}/{maxMp}";
    }

    private void OnExpRatioChanged(float ratio)
    {
        Image_ExpBar.fillAmount = ratio;
    }

    private void OnExpValueChanged(int current, int max)
    {
        Text_ExpValue.text = $"{current}/{max}";
    }

    public void StartSkillCoolDown(string keyLabel, float duration)
    {
        if (_skillSlotViews.TryGetValue(keyLabel, out SkillSlotView slotView))
        {
            slotView.StartCoolDown(duration);
        }
    }

    public void RefreshSkillSlots()
    {
        BuildSkillSlots();
    }
    
    public void StartEvasionCoolDown(float duration)
    {
        EvasionSlotView?.StartCoolDown(duration);
    }

    private void OnClickCharacterInfo()
    {
        OnCharacterInfoButtonClicked?.Invoke();
    }

    private void OnClickSkill()
    {
        OnSkillButtonClicked?.Invoke();
    }

    private void OnClickInventory()
    {
        OnInventoryButtonClicked?.Invoke();
    }

    private void OnClickEquipment()
    {
        OnEquipmentButtonClicked?.Invoke(); 
    }
}
