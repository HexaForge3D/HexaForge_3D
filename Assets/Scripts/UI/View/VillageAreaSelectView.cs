using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class VillageAreaSelectView : BaseOverLayUI
{
    [SerializeField] private Transform Transform_ListParent;
    [SerializeField] private GameObject Prefab_ListItem;

    [SerializeField] private TMP_Text Text_DetailName;
    [SerializeField] private TMP_Text Text_DetailDescription;
    [SerializeField] private Button Button_Teleport;

    private VillageAreaSelectViewModel _viewModel;
    private readonly List<GameObject> _spawnListItems = new List<GameObject>();

    public void BindViewModel(VillageAreaSelectViewModel viewModel)
    {
        _viewModel = viewModel;

        viewModel.OnAreaSelected += OnAreaSelected;
        Button_Teleport.onClick.RemoveAllListeners();
        Button_Teleport.onClick.AddListener(OnClickTeleport);

        BuildAreaList();
    }

    private void BuildAreaList()
    {
        ClearAreaList();

        List<VillageAreaData> areaList = _viewModel.GetAreaList();

        foreach (VillageAreaData data in areaList)
        {
            GameObject itemObject = Instantiate(Prefab_ListItem, Transform_ListParent);
            VillageAreaListItemView itemView = itemObject.GetComponent<VillageAreaListItemView>();
            itemView.Setup(data, OnAreaItemClicked);

            _spawnListItems.Add(itemObject);
        }
    }

    private void ClearAreaList()
    {
        foreach (GameObject item in _spawnListItems)
        {
            Destroy(item);
        }

        _spawnListItems.Clear();
    }

    private void OnAreaItemClicked(VillageAreaData data)
    {
        _viewModel.SelectArea(data);
    }

    private void OnAreaSelected(VillageAreaData data)
    {
        Text_DetailName.text = data.Name;
        Text_DetailDescription.text = data.Description;
    }

    private void OnClickTeleport()
    {
        _viewModel.RequestTeleport();
    }
}
