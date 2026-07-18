using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ShopView : BaseOverLayUI
{
    [SerializeField] private TMP_Text Text_Gold;
    [SerializeField] private TMP_Text Text_Message;
    [SerializeField] private Transform Transform_SlotParent;
    [SerializeField] private GameObject Prefab_ShopSlot;

    private ShopViewModel _viewModel;
    private readonly List<GameObject> _spawnedSlots = new List<GameObject>();

    public void BindViewModel(ShopViewModel viewModel)
    {
        _viewModel = viewModel;

        viewModel.OnGoldChanged += OnGoldChanged;
        viewModel.OnBuyFailed += OnBuyFailed;

        Text_Message.text = string.Empty;

        RefreshGold();
        BuildSlot();
    }

    private void ClearSlots()
    {
        foreach (GameObject slot in _spawnedSlots)
        {
            Destroy(slot);
        }

        _spawnedSlots.Clear();
    }

    private void BuildSlot()
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

    private void OnBuyClicked(ItemData item)
    {
        _viewModel.BuyItem(item);
    }

    private void OnGoldChanged()
    {
        RefreshGold();
        Text_Message.text = string.Empty;
    }

    private void OnBuyFailed(string message)
    {
        Text_Message.text = message;
    }

    public void RefreshGold()
    {
        Text_Gold.text = $"Gold: {_viewModel.GetCurrentGold()}G";
    }
}
