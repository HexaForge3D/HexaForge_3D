using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillSlotView : MonoBehaviour
{
    [SerializeField] private Image Image_Icon;
    [SerializeField] private Image Image_CoolDownOverLay;
    [SerializeField] private TMP_Text Text_KeyLabel;

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
            return;
        }

        Image_Icon.gameObject.SetActive(true);
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
