using UnityEngine;

public class BaseUI : MonoBehaviour
{
    public UIType UIType_This {  get; private set; }

    public void SetUIType(UIType uIType)
    {
        UIType_This = uIType;
    }
}
