using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using Random = UnityEngine.Random;

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

    private Dictionary<string, float> _soundPlayTimes = new Dictionary<string, float>();
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

    private void Start()
    {
        float savedBGM = PlayerPrefs.GetFloat("Saved_BGM_Volume", 1f);
        float savedSFX = PlayerPrefs.GetFloat("Saved_SFX_Volume", 1f);
        float savedUI = PlayerPrefs.GetFloat("Saved_UI_Volume", 1f);

        SetBGMVolume(savedBGM);
        SetSFXVolume(savedSFX);
        SetUIVolume(savedUI);
    }

    public void StopBGM()
    {
        _bgmSource.Stop();
    }
    public void PlayBGM(AudioClip bgmclip, float volume = 1f)
    {
        if (_bgmSource.clip == bgmclip) return;
        _bgmSource.volume = volume;
        _bgmSource.clip = bgmclip;
        _bgmSource.loop = true;
        _bgmSource.Play();
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

        PlayerPrefs.SetFloat("Saved_BGM_Volume", sliderValue);
    }

    public void SetSFXVolume(float sliderValue)
    {
        float value = Mathf.Clamp(sliderValue, 0.0001f, 1f);
        _audioMixer.SetFloat("SFX_Volume", Mathf.Log10(value) * 20);

        PlayerPrefs.SetFloat("Saved_SFX_Volume", sliderValue);
    }

    public void SetUIVolume(float sliderValue)
    {
        float value = Mathf.Clamp(sliderValue, 0.0001f, 1f);
        _audioMixer.SetFloat("UI_Volume", Mathf.Log10(value) * 20);

        PlayerPrefs.SetFloat("Saved_UI_Volume", sliderValue);
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

        if (_soundPlayTimes.TryGetValue(fileName, out float lastPlayedTime))
        {
            if (Time.time - lastPlayedTime < 0.1f) return;
        }
        if (clip != null)
        {
            PlayUI(clip);
        }
        else
        {
            Debug.LogWarning($"SoundManager: AudioClip '{fileName}' not found in Resource/Sounds/UI/");
        }
    }
    public void PlaySFX(AudioClip sfxclip, float volume = 1f, bool useRandomPitch = false)
    {
        if (sfxclip == null) return;
        {
            _sfxSource.pitch = useRandomPitch ? Random.Range(0.9f, 1f) : 1f;
            _sfxSource.PlayOneShot(sfxclip, volume);
        }
    }
    public void PlaySFXSound(string fileName, float volume = 1f, bool useRandomPitch = false)
    {
        AudioClip clip = Resources.Load<AudioClip>($"Sounds/SFX/{fileName}");
        if (clip != null)
        {
            PlaySFX(clip, volume, useRandomPitch);
        }
        else
        {
            Debug.LogWarning($"SoundManager: AudioClip '{fileName}' not found in Resource/Sounds/SFX/");
        }
    }

    public void PlaySFX(AudioClip clip, Transform targetTransform,float volume = 1f, bool useRandomPitch = false)
    {
        if (clip == null) return;

        GameObject tempAudioObj = new GameObject($"TempSFX_{clip.name}");
        tempAudioObj.transform.position = targetTransform.position;
        tempAudioObj.transform.SetParent(targetTransform);

        AudioSource tempSource = tempAudioObj.AddComponent<AudioSource>();
        tempSource.clip = clip;
        tempSource.volume = volume;
        tempSource.spatialBlend = 1f;
        tempSource.pitch = useRandomPitch ? Random.Range(0.9f, 1.1f) : 1f;

        tempSource.Play();

        Destroy( tempAudioObj, clip.length );
    }

    public void PlaySFXSound(string fileName, Transform targetTransform, float volume = 1f, bool useRandomPitch = false)
    {
        AudioClip clip = Resources.Load<AudioClip>($"Sound/SFX/{fileName}");
        if (clip != null)
        {
            PlaySFX(clip, targetTransform, volume, useRandomPitch);
        }
        else
        {
            Debug.LogWarning($"PlaySFX sound: {fileName} = null");
        }
    }
}
