using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public enum ButtonSoundType
{
    None,
    Click,
    Buy,
    Sell
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
                SoundManager.Instance.PlayUISound("ClickSound");
                break;
            case ButtonSoundType.Buy:
                SoundManager.Instance.PlayUISound("ItemBuy");
                break;
            case ButtonSoundType.Sell:
                SoundManager.Instance.PlayUISound("ItemSell");
                break;
            case ButtonSoundType.None:
            default:
                break;
        }

    }
}
