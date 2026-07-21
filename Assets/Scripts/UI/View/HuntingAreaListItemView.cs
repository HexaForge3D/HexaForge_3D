using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HuntingAreaListItemView : MonoBehaviour
{
    [SerializeField] private TMP_Text Text_AreaName;
    [SerializeField] private Button Button_Select;

    private HuntingAreaData _data;
    private Action<HuntingAreaData> _onSelected;

    public void Setup(HuntingAreaData data, Action<HuntingAreaData> onSelected)
    {
        _data = data;
        _onSelected = onSelected;

        Text_AreaName.text = data.Name;

        Button_Select.onClick.RemoveListener(OnClickSelect);
        Button_Select.onClick.AddListener(OnClickSelect);
    }

    private void OnClickSelect()
    {
        _onSelected?.Invoke(_data);
    }
}
