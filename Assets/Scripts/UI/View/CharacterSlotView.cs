using System;
using TMPro;
using UnityEngine.UI;
using UnityEngine;

public class CharacterSlotView : MonoBehaviour
{
    [SerializeField] private TMP_Text Text_CharacterName;
    [SerializeField] private Button Button_Select;

    private PlayerData _data;
    private Action<PlayerData> _onSelected;

    public void Setup(PlayerData data, Action<PlayerData> onSelected)
    {
        _data = data;
        _onSelected = onSelected;

        Text_CharacterName.text = data.Name;

        Button_Select.onClick.RemoveAllListeners();
        Button_Select.onClick.AddListener(OnClickSelect);
    }

    private void OnClickSelect()
    {
        _onSelected?.Invoke(_data);
    }
}
