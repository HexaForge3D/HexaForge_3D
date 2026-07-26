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

        await FadeInAsync(target);
    }

    private static async UniTask FadeInAsync(Image target)
    {
        float duration = 0.2f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            if (target == null) return;

            elapsed += Time.deltaTime;
            Color color = target.color;
            color.a = Mathf.Clamp01(elapsed / duration);
            target.color = color;

            await UniTask.Yield();
        }

        if (target != null)
        {
            Color color = target.color;
            color.a = 1f;
            target.color = color;
        }
    }
}
