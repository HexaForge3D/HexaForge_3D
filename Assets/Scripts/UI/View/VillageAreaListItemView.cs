using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class VillageAreaListItemView : MonoBehaviour
{

    private VillageAreaData _data;
    private Action<VillageAreaData> _onSelected;

    public void Setup(VillageAreaData data, Action<VillageAreaData> onSelected)
    {
        _data = data;
        _onSelected = onSelected;
    }
}
