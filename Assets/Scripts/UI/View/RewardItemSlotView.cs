using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RewardItemSlotView : MonoBehaviour
{
    [SerializeField] private Image Image_Icon;
    [SerializeField] private TMP_Text Text_Count;

    public void Setup(ItemData item, int count)
    {
        SpriteLoaderUtil.LoadAsync(Image_Icon, item.IconAddress).Forget();

        bool showCount = count > 1;
        Text_Count.gameObject.SetActive(showCount);
        if (showCount)
        {
            Text_Count.text = $"x{count}";
        }
    }
}