using System;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [Header("Audio Sources")]
    [SerializeField] private AudioSource _bgmSource;
    [SerializeField] private AudioSource _uiSource;
    [SerializeField] private AudioSource _sfxSource;

    [Header("Settings")]
    [SerializeField] private AudioClip _buttonClickSound;
    [SerializeField] private AudioMixer _audioMixer;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        DefenceFieldManager.OnStartField += PlayBGMSound;
        NPCEscortFieldManager.OnStartField += PlayBGMSound;
        RoomFieldManager.OnStartField += PlayBGMSound;
        MapManager.OnStartField += PlayBGMSound;
    }

    private void OnDisable()
    {
        DefenceFieldManager.OnStartField -= PlayBGMSound;
        NPCEscortFieldManager.OnStartField -= PlayBGMSound;
        RoomFieldManager.OnStartField -= PlayBGMSound;
        MapManager.OnStartField -= PlayBGMSound;
    }
    public void PlayBGM(AudioClip bgmclip, float volume = 1f)
    {
        if (_bgmSource.clip == bgmclip) return;
        _bgmSource.volume = volume;
        _bgmSource.clip = bgmclip;
        _bgmSource.loop = true;
        _bgmSource.Play();
    }

    public void StopBGM()
    {
        _bgmSource.Stop();
    }

    public void PlayUI(AudioClip uiclip, float volume = 1f)
    {
        if (uiclip == null) return;
        _uiSource.PlayOneShot(uiclip, volume);
    }


    public void SetBGMVolume(float sliderValue)
    {
        float value = Mathf.Clamp(sliderValue, 0.0001f, 1f);
        _audioMixer.SetFloat("BGM_Volume", Mathf.Log10(value) * 20);
    }

    public void SetSFXVolume(float sliderValue)
    {
        float value = Mathf.Clamp(sliderValue, 0.0001f, 1f);
        _audioMixer.SetFloat("SFX_Volume", Mathf.Log10(value) * 20);
    }

    public void SetUIVolume(float sliderValue)
    {
        float value = Mathf.Clamp(sliderValue, 0.0001f, 1f);
        _audioMixer.SetFloat("UI_Volume", Mathf.Log10(value) * 20);
    }
    public void PlayBGMSound(string fileName)
    {
        AudioClip clip = Resources.Load<AudioClip>($"Sounds/BGM/{fileName}");
        if (clip != null)
        {
            PlayBGM(clip);
        }
        else
        {
            Debug.LogWarning($"SoundManager: AudioClip '{fileName}' not found in Resource/Sounds/BGM/");
        }
    }
    public void PlayUISound(string fileName)
    {
        AudioClip clip = Resources.Load<AudioClip>($"Sounds/UI/{fileName}");
        if (clip != null)
        {
            PlayUI(clip);
        }
        else
        {
            Debug.LogWarning($"SoundManager: AudioClip '{fileName}' not found in Resource/Sounds/UI/");
        }
    }

    public void PlaySFXSound(string fileName)
    {
        AudioClip clip = Resources.Load<AudioClip>($"Sounds/SFX/{fileName}");
        if (clip != null)
        {
            _sfxSource.PlayOneShot(clip);
        }
        else
        {
            Debug.LogWarning($"SoundManager: AudioClip '{fileName}' not found in Resource/Sounds/SFX/");
        }
    }
}
