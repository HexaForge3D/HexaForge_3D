using System;
using System.Collections.Generic;

public class VillageAreaSelectViewModel
{
    private readonly VillageAreaRepository _repository = new VillageAreaRepository();
    
    private List<VillageAreaData> _areaList;
    private VillageAreaData _selectedArea;

    public Action<VillageAreaData> OnAreaSelected;

    public Action<VillageAreaData> OnTeleportRequested;

    public List<VillageAreaData> GetAreaList()
    {
        if (_areaList == null)
        {
            _areaList = _repository.GetAllAreas();
        }

        return _areaList;
    }

    public void SelectArea(VillageAreaData data)
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
