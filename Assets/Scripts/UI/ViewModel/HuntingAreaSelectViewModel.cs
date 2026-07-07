using System;
using System.Collections.Generic;

public class HuntingAreaSelectViewModel
{
    private List<HuntingAreaData> _areaList;
    private HuntingAreaData _selectedArea;

    public Action<HuntingAreaData> OnAreaSelected;

    public Action<HuntingAreaData> OnTeleportRequested;

    public List<HuntingAreaData> GetAreaList()
    {
        if (_areaList == null)
        {
            _areaList = CreateDummyData();
        }

        return _areaList;
    }

    public void SelectArea(HuntingAreaData data)
    {
        _selectedArea = data;
        OnAreaSelected?.Invoke(data);
    }

    public void RequestTeleport()
    {
        if (_selectedArea == null)
        {
            return;
        }

        OnTeleportRequested?.Invoke(_selectedArea);
    }

    private List<HuntingAreaData> CreateDummyData()
    {
        return new List<HuntingAreaData>
        {
            new HuntingAreaData("area_a", "Area A", "Area A Description......", ""),
            new HuntingAreaData("area_b", "Area B", "Area B Description......", ""),
            new HuntingAreaData("area_c", "Area C", "Area C Description......", ""),
            new HuntingAreaData("area_d", "Area D", "Area D Description......", ""),
            new HuntingAreaData("area_e", "Area E", "Area E Description......", ""),
        };
    }
}
