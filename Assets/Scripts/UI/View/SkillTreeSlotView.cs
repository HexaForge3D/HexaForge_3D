using Cysharp.Threading.Tasks;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillTreeSlotView : MonoBehaviour
{
    [SerializeField] private Image Image_Icon;
    [SerializeField] private GameObject Image_LockOverLay;
    [SerializeField] private TMP_Text Text_Level;
    [SerializeField] private TMP_Text Text_RequiredLevel;
    [SerializeField] private Button Button_Plus;
    [SerializeField] private Button Button_Minus;
    [SerializeField] private TooltipTrigger TooltipTrigger;

    private SkillTreeSlotData _data;
    private Action<string> _onUpgradeClicked;
    private Action<string> _onDowngradeClicked;

    public void Setup(SkillTreeSlotData data, Action<string> onUpgradeClicked, Action<string> onDowngradeClicked)
    {
        _data = data;
        _onUpgradeClicked = onUpgradeClicked;
        _onDowngradeClicked = onDowngradeClicked;

        SpriteLoaderUtil.LoadAsync(Image_Icon, data.IconAddress).Forget();

        Image_LockOverLay.SetActive(data.IsUnlocked == false);

        bool showRequiredLevel = data.IsUnlocked == false;
        Text_RequiredLevel.gameObject.SetActive(showRequiredLevel);

        if (showRequiredLevel)
        {
            Text_RequiredLevel.text = $"Req. Lv {data.RequiredLevel}";
        }

        Text_Level.text = $"{data.CurrentLevel}/{data.MaxLevel}";

        Button_Plus.onClick.RemoveAllListeners();
        Button_Plus.onClick.AddListener(OnClickPlus);
        Button_Plus.interactable = data.IsUnlocked;

        Button_Minus.onClick.RemoveAllListeners();
        Button_Minus.onClick.AddListener(OnClickMinus);
        Button_Minus.interactable = data.IsUnlocked && data.CurrentLevel > 1;

        SkillTableData skillTableData = GameDataManager.Instance.GetData<SkillTableData>(data.Id);

        int damage = SkillUtil.Instance.GetCalculatedDamage(skillTableData, data.CurrentLevel);
        int manaCost = SkillUtil.Instance.GetCalculatedManaCost(skillTableData, data.CurrentLevel);
        float coolDown = SkillUtil.Instance.GetCalculatedCoolDown(skillTableData, data.CurrentLevel);

        string statsText = $"Damage: {damage}\nMana Cost: {manaCost}\nCooldown: {coolDown}s";

        TooltipData tooltipData = new TooltipData(
            data.IconAddress,
            data.Name,
            data.Description,
            null,
            null,
            null,
            statsText
            );

        TooltipTrigger.SetData(tooltipData);
    }

    private void OnClickPlus()
    {
        _onUpgradeClicked?.Invoke(_data.Id);
    }

    private void OnClickMinus()
    {
        _onDowngradeClicked?.Invoke(_data.Id);
    }
}
