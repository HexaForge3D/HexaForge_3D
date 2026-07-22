using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class BaseUI : MonoBehaviour
{
    public UIType UIType_This {  get; private set; }

    protected virtual void Awake()
    {
        Button[] allButtons = GetComponentsInChildren<Button>(true);

        foreach (Button btn in allButtons)
        {
            btn.onClick.AddListener(PlayCommonButtonSound);
        }
    }
    public void SetUIType(UIType uIType)
    {
        UIType_This = uIType;
    }

    private void PlayCommonButtonSound()
    {
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlayUISound("ClickSound");
        }        
    }

    public void AddSoundToDynamicButton(Button btn)
    {
        if (btn != null)
        {
            btn.onClick.AddListener(PlayCommonButtonSound);
        }
    }
}
