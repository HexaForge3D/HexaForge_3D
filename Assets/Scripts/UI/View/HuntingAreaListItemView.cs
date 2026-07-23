using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HuntingAreaListItemView : MonoBehaviour
{
    [SerializeField] private TMP_Text Text_AreaName;
    [SerializeField] private Button Button_Select;
    [SerializeField] private GameObject Image_SelectedOverlay;

    private HuntingAreaData _data;
    private Action<HuntingAreaData, HuntingAreaListItemView> _onSelected;

    public void Setup(HuntingAreaData data, Action<HuntingAreaData, HuntingAreaListItemView> onSelected)
    {
        _data = data;
        _onSelected = onSelected;

        Text_AreaName.text = data.Name;

        Button_Select.onClick.RemoveListener(OnClickSelect);
        Button_Select.onClick.AddListener(OnClickSelect);
    }

    private void OnClickSelect()
    {
        _onSelected?.Invoke(_data, this);
    }

    public void SetSelected(bool isSelected)
    {
        Image_SelectedOverlay.SetActive(isSelected);
    }
}
