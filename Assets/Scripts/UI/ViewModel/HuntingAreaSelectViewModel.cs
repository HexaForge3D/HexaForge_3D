using System;
using System.Collections.Generic;

public class HuntingAreaSelectViewModel
{
    private readonly HuntingAreaRepository _repository = new HuntingAreaRepository();
    
    private List<HuntingAreaData> _areaList;
    private HuntingAreaData _selectedArea;

    public Action<HuntingAreaData> OnAreaSelected;

    public Action<HuntingAreaData> OnTeleportRequested;

    public List<HuntingAreaData> GetAreaList()
    {
        if (_areaList == null)
        {
            _areaList = _repository.GetAllAreas();
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
}
