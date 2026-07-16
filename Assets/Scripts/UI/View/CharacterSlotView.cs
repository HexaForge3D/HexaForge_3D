using System;
using TMPro;
using UnityEngine.UI;
using UnityEngine;

public class CharacterSlotView : MonoBehaviour
{
    [SerializeField] private TMP_Text Text_SlotLabel;
    [SerializeField] private Button Button_Select;

    private CharacterSlotData _data;
    private Action<CharacterSlotData> _onSelected;

    public void Setup(CharacterSlotData data, Action<CharacterSlotData> onSelected)
    {
        _data = data;
        _onSelected = onSelected;

        Text_SlotLabel.text = data.IsEmpty ? "Empty Slot (Create)" : data.Character.Name;

        Button_Select.onClick.RemoveAllListeners();
        Button_Select.onClick.AddListener(OnClickSelect);
    }

    private void OnClickSelect()
    {
        _onSelected?.Invoke(_data);
    }
}
