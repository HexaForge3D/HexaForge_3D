using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

public class LoadingOverLayView : MonoBehaviour
{
    [SerializeField] private List<string> LoadingImageAddresses;
    [SerializeField] private float FakeLoadingDuration = 5f;

    [SerializeField] private Image Image_Spinner;
    [SerializeField] private Image Image_FullBackGround;
    [SerializeField] private Image Image_DimBackGround;
    [SerializeField] private Image Image_ProgressBar;

    private readonly List<Sprite> _cachedLoadingImages = new List<Sprite>();

    public async UniTask PreLoadImagesAsync()
    {
        foreach (string address in LoadingImageAddresses)
        {
            Sprite sprite = await Addressables.LoadAssetAsync<Sprite>(address).ToUniTask();

            if (sprite != null)
            {
                _cachedLoadingImages.Add(sprite);
            }
        }
    }

    public UniTask ShowAsync(bool useFullScreen)
    {
        gameObject.SetActive(true);

        if (useFullScreen)
        {
            ApplyRandomBackGround();
            Image_FullBackGround.gameObject.SetActive(true);
            Image_DimBackGround.gameObject.SetActive(false);
            Image_Spinner.gameObject.SetActive(false); 

            return PlayFakeProgressAsync();
        }

        Image_FullBackGround.gameObject.SetActive(false);
        Image_DimBackGround.gameObject.SetActive(true);
        Image_Spinner.gameObject.SetActive(true);
        Image_ProgressBar.gameObject.SetActive(false);

        return UniTask.CompletedTask;
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    private void ApplyRandomBackGround()
    {
        if (_cachedLoadingImages.Count == 0) return;

        int randomIndex = Random.Range(0, _cachedLoadingImages.Count);
        Image_FullBackGround.sprite = _cachedLoadingImages[randomIndex];
    }

    private async UniTask PlayFakeProgressAsync()
    {
        Image_ProgressBar.gameObject.SetActive(true);
        Image_ProgressBar.fillAmount = 0f;

        float elapsed = 0f;

        while (elapsed < FakeLoadingDuration)
        {
            elapsed += Time.deltaTime;
            Image_ProgressBar.fillAmount = Mathf.Clamp01(elapsed / FakeLoadingDuration);
            await UniTask.Yield();
        }

        Image_ProgressBar.fillAmount = 1f;
    }
}
