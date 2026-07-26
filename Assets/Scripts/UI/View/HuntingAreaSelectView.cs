using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HuntingAreaSelectView : BaseOverLayUI
{
    [SerializeField] private Transform Transform_ListParent;
    [SerializeField] private GameObject Prefab_ListItem;

    [SerializeField] private TMP_Text Text_DetailName;
    [SerializeField] private TMP_Text Text_DetailDescription;
    [SerializeField] private Button Button_Teleport;

    private HuntingAreaSelectViewModel _viewModel;
    private readonly List<HuntingAreaListItemView> _spawnListItems = new List<HuntingAreaListItemView>();

    public void BindViewModel(HuntingAreaSelectViewModel viewModel)
    {
        _viewModel = viewModel;

        viewModel.OnAreaSelected += OnAreaSelected;
        Button_Teleport.onClick.RemoveListener(OnClickTeleport);
        Button_Teleport.onClick.AddListener(OnClickTeleport);

        BuildAreaList();
    }

    private void BuildAreaList()
    {
        ClearAreaList();

        List<HuntingAreaData> areaList = _viewModel.GetAreaList();
        HuntingAreaListItemView firstItem = null;
        HuntingAreaData firstData = null;

        foreach (HuntingAreaData data in areaList)
        {
            GameObject itemObject = Instantiate(Prefab_ListItem, Transform_ListParent);
            HuntingAreaListItemView itemView = itemObject.GetComponent<HuntingAreaListItemView>();
            itemView.Setup(data, OnAreaItemClicked);

            if (firstItem == null)
            {
                firstItem = itemView;
                firstData = data;
            }

            _spawnListItems.Add(itemView);
        }

        if (firstItem != null)
        {
            OnAreaItemClicked(firstData, firstItem);
        }
    }

    private void ClearAreaList()
    {
        foreach (HuntingAreaListItemView item in _spawnListItems)
        {
            Destroy(item.gameObject);
        }

        _spawnListItems.Clear();
    }

    private void OnAreaItemClicked(HuntingAreaData data, HuntingAreaListItemView clickedItem)
    {
        _viewModel.SelectArea(data);

        foreach (HuntingAreaListItemView item in _spawnListItems)
        {
            item.SetSelected(item == clickedItem);
        }
    }

    private void OnAreaSelected(HuntingAreaData data)
    {
        Text_DetailName.text = data.Name;
        Text_DetailDescription.text = data.Description;
    }

    private void OnClickTeleport()
    {
        _viewModel.RequestTeleport();
    }
}
