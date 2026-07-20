using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillSlotView : MonoBehaviour
{
    [SerializeField] private Image Image_Icon;
    [SerializeField] private Image Image_CoolDownOverLay;
    [SerializeField] private TMP_Text Text_KeyLabel;
    [SerializeField] private TooltipTrigger TooltipTrigger;

    private float _coolDownRemaining;
    private float _coolDownDuration;

    private void Update()
    {
        if (_coolDownRemaining <= 0f)
        {
            return;
        }

        _coolDownRemaining -= Time.deltaTime;
        Image_CoolDownOverLay.fillAmount = Mathf.Clamp01(_coolDownRemaining / _coolDownDuration);

        if (_coolDownRemaining <= 0f)
        {
            Image_CoolDownOverLay.gameObject.SetActive(false);
        }
    }


    public void Setup(SkillData skill, string keyLabel)
    {
        Text_KeyLabel.text = keyLabel;
        Image_CoolDownOverLay.gameObject.SetActive(false);

        if (skill == null)
        {
            Image_Icon.gameObject.SetActive(false);
            TooltipTrigger.SetData(null);
            return;
        }

        Image_Icon.gameObject.SetActive(true);
        SpriteLoaderUtil.LoadAsync(Image_Icon, skill.IconAddress).Forget();

        int skillLevel = SkillUtil.Instance.GetSkillLevel(skill.Id);
        SkillTableData skillTableData = GameDataManager.Instance.GetData<SkillTableData>(skill.Id);

        int damage = SkillUtil.Instance.GetCalculatedDamage(skillTableData, skillLevel);
        int manaCost = SkillUtil.Instance.GetCalculatedManaCost(skillTableData, skillLevel);
        float coolDown = SkillUtil.Instance.GetCalculatedCoolDown(skillTableData, skillLevel);

        string statsText = $"Damage: {damage}\nMana Cost: {manaCost}\nCooldown: {coolDown}s";

        TooltipData tooltipData = new TooltipData(
            skill.IconAddress,
            skill.Name,
            skill.Description,
            null,
            null,
            null,
            statsText
            );

        TooltipTrigger.SetData(tooltipData);
    }

    public void StartCoolDown(float duration)
    {
        if (duration <= 0f)
        {
            return;
        }

        _coolDownRemaining = duration;
        _coolDownDuration = duration;

        Image_CoolDownOverLay.gameObject.SetActive(true);
        Image_CoolDownOverLay.fillAmount = 1f;
    }
}
