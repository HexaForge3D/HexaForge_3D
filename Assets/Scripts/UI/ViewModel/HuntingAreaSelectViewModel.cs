using System;
using System.Collections.Generic;

public class HuntingAreaSelectViewModel
{
    private readonly HuntingAreaRepository _repository = new HuntingAreaRepository();
    private readonly string _slotId;
    
    private List<HuntingAreaData> _areaList;
    private HuntingAreaData _selectedArea;

    public Action<HuntingAreaData> OnAreaSelected;

    public Action<HuntingAreaData> OnTeleportRequested;

    public HuntingAreaSelectViewModel(string slotId)
    {
        _slotId = slotId;
    }

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

        CharacterSaveData saveData = SaveManager.Instance.GetChararcterData(_slotId);

        if (saveData == null) return;

        int currentLevel = PlayerLevel.LevelFromExp(saveData.Exp);

        if (currentLevel < _selectedArea.RequiredLevel)
        {
            SystemMessageManager.Instance.Show($"Required Level {_selectedArea.RequiredLevel}");
            return;
        }

        OnTeleportRequested?.Invoke(_selectedArea);
    }
}
