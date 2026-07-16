using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

public static class SpriteLoaderUtil
{
    public static async UniTask LoadAsync(Image target, string iconAddress)
    {
        if (target == null || string.IsNullOrEmpty(iconAddress)) return;

        Sprite sprite = await Addressables.LoadAssetAsync<Sprite>(iconAddress).ToUniTask();

        if (sprite != null)
        {
            target.sprite = sprite;
        }
    }
}
