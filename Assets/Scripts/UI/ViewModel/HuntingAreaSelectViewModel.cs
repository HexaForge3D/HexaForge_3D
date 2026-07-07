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
            new HuntingAreaData("area_a", "사냥터 A", "사냥터A 설명......", ""),
            new HuntingAreaData("area_b", "사냥터 B", "사냥터B 설명......", ""),
            new HuntingAreaData("area_c", "사냥터 C", "사냥터C 설명......", ""),
            new HuntingAreaData("area_d", "사냥터 D", "사냥터D 설명......", ""),
            new HuntingAreaData("area_e", "사냥터 E", "사냥터E 설명......", ""),
        };
    }
}
