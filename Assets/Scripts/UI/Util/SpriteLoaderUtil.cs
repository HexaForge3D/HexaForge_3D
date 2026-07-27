using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

public static class SpriteLoaderUtil
{
    public static async UniTask LoadAsync(Image target, string iconAddress)
    {
        if (target == null || string.IsNullOrEmpty(iconAddress)) return;

        Color color = target.color;
        color.a = 0f;
        target.color = color;

        Sprite sprite = await Addressables.LoadAssetAsync<Sprite>(iconAddress).ToUniTask();

        if (target == null) return;

        if (sprite != null)
        {
            target.sprite = sprite;
        }

        Color finalColor = target.color;
        finalColor.a = 1f;
        target.color = finalColor;
    }
}
