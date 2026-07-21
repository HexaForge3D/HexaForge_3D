using System;
using TMPro;
using UnityEngine.UI;
using UnityEngine;
using Unity.VisualScripting;

public class CharacterSlotView : MonoBehaviour
{
    [SerializeField] private TMP_Text Text_SlotLabel;
    [SerializeField] private Button Button_Select;
    [SerializeField] private GameObject Image_SelectedOverlay;

    private CharacterSlotData _data;
    private Action<CharacterSlotData, CharacterSlotView> _onSelected;

    public void Setup(CharacterSlotData data, Action<CharacterSlotData, CharacterSlotView> onSelected)
    {
        _data = data;
        _onSelected = onSelected;

        Text_SlotLabel.text = data.IsEmpty ? "Empty Slot" : data.Character.Name;

        Button_Select.onClick.RemoveListener(OnClickSelect);
        Button_Select.onClick.AddListener(OnClickSelect);

        SetSelected(false);
    }

    public void SetSelected(bool isSelected)
    {
        Image_SelectedOverlay.SetActive(isSelected);
    }

    private void OnClickSelect()
    {
        _onSelected?.Invoke(_data, this);
    }
}
