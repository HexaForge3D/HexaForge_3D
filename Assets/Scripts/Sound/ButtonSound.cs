using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public enum ButtonSoundType
{
    None,
    Click,
    Buy
}
[RequireComponent(typeof(Button))]
public class ButtonSound : MonoBehaviour
{
    [Header("Button Sound Type")]
    [SerializeField] private ButtonSoundType _soundType = ButtonSoundType.Click;
    private Button _button;

    private void Awake()
    {
        _button = GetComponent<Button>();
        
        if(_button != null)
        {
            _button.onClick.RemoveListener(PlayButtonSound);
            _button.onClick.AddListener(PlayButtonSound);
        }
    }

    private void PlayButtonSound()
    {
        if (SoundManager.Instance == null) return;

        switch (_soundType)
        {
            case ButtonSoundType.Click:
                SoundManager.Instance.PlayUISound("Click_Sound");
                break;
            case ButtonSoundType.Buy:
                SoundManager.Instance.PlayUISound("Item_Buy_Sound");
                break;
            case ButtonSoundType.None:
            default:
                break;
        }

    }
}
