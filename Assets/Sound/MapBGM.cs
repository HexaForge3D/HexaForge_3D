using UnityEngine;

public class MapBGM : MonoBehaviour
{
    [Header("Map BGM Settings")]
    [SerializeField] private AudioClip _mapBGMClip;

    [SerializeField] private string _bgmFileName;

    [Range(0f, 1f)]
    [SerializeField] private float _bgmVolume = 1f;

    private void Start()
    {
        if (SoundManager.Instance == null)
        {
            Debug.LogError("SoundManager instance is not found in the scene.");
            return;
        }
        if (_mapBGMClip != null)
        {
            SoundManager.Instance.PlayBGM(_mapBGMClip, _bgmVolume);
        }
        else if (!string.IsNullOrEmpty(_bgmFileName))
        {
            SoundManager.Instance.PlayBGMSound(_bgmFileName);
        }

    }
}
