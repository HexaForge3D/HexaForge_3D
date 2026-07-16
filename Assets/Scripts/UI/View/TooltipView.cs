using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TooltipView : MonoBehaviour
{
    [SerializeField] private RectTransform RectTransform_Self;
    [SerializeField] private CanvasGroup CanvasGroup_Self;
    [SerializeField] private Image Image_Icon;
    [SerializeField] private TMP_Text Text_Name;
    [SerializeField] private TMP_Text Text_Description;
    [SerializeField] private TMP_Text Text_UsageHint;
    [SerializeField] private TMP_Text Text_Count;

    [SerializeField] private Vector2 CursorOffset = new Vector2(20f, -20f);

    private void Awake()
    {
        CanvasGroup_Self.blocksRaycasts = false;
        Hide();
    }

    public void Show(TooltipData data, Vector2 screenPosition)
    {
        Text_Name.text = data.Name;
        Text_Description.text = data.Description;

        bool hasUsageHint = string.IsNullOrEmpty(data.UsageHint) == false;
        Text_UsageHint.gameObject.SetActive(hasUsageHint);

        if (hasUsageHint)
        {
            Text_UsageHint.text = data.UsageHint;
        }

        bool hasCount = string.IsNullOrEmpty(data.CountText) == false;
        Text_Count.gameObject.SetActive(hasCount);

        if (hasCount)
        {
            Text_Count.text = data.CountText;
        }

        SpriteLoaderUtil.LoadAsync(Image_Icon, data.IconAddress).Forget();

        gameObject.SetActive(true);
        UpdatePosition(screenPosition);
    }

    public void UpdatePosition(Vector2 screenPosition)
    {
        RectTransform_Self.position = screenPosition + CursorOffset;
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
