using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillSlotView : MonoBehaviour
{
    [SerializeField] private Image Image_Icon;
    [SerializeField] private Image Image_CoolDownOverLay;
    [SerializeField] private TMP_Text Text_KeyLabel;

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
}
