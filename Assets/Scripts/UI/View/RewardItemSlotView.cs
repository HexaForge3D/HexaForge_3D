using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class RewardItemSlotView : MonoBehaviour
{
    [SerializeField] private Image Image_Icon;

    public void Setup(ItemData item)
    {
        SpriteLoaderUtil.LoadAsync(Image_Icon, item.IconAddress).Forget();
    }
}