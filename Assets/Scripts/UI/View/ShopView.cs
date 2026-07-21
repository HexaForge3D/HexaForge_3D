using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ShopView : BaseOverLayUI
{
    [SerializeField] private TMP_Text Text_Gold;
    [SerializeField] private Transform Transform_SlotParent;
    [SerializeField] private GameObject Prefab_ShopSlot;

    private ShopViewModel _viewModel;
    private readonly List<GameObject> _spawnedSlots = new List<GameObject>();

    public void BindViewModel(ShopViewModel viewModel)
    {
        _viewModel = viewModel;

        viewModel.OnGoldChanged += RefreshGold;

        RefreshGold();
        BuildSlots();
    }

    private void BuildSlots()
    {
        ClearSlots();

        List<ItemData> items = _viewModel.GetShopItems();

        foreach (ItemData item in items)
        {
            GameObject slotObject = Instantiate(Prefab_ShopSlot, Transform_SlotParent);
            ShopSlotView slotView = slotObject.GetComponent<ShopSlotView>();
            slotView.Setup(item, OnBuyClicked);

            _spawnedSlots.Add(slotObject);
        }
    }

    private void ClearSlots()
    {
        foreach (GameObject slot in _spawnedSlots)
        {
            Destroy(slot);
        }

        _spawnedSlots.Clear();
    }

    private void OnBuyClicked(ItemData item)
    {
        _viewModel.BuyItem(item);
    }

    public void RefreshGold()
    {
        Text_Gold.text = $"Gold: {_viewModel.GetCurrentGold()}G";
    }
}