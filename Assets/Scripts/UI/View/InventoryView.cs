using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InventoryView : BaseOverLayUI
{
    [SerializeField] private int SlotCount = 20;
    [SerializeField] private Transform Transform_SlotParent;
    [SerializeField] private GameObject Prefab_InventorySlot;
    [SerializeField] private TMP_Text Text_Gold;

    private InventoryViewModel _viewModel;
    private readonly List<GameObject> _spawnedSlots = new List<GameObject>();
    private readonly Dictionary<string, InventorySlotView> _slotViewsByItemId = new Dictionary<string, InventorySlotView>();

    public Action<InventoryItemData, int> OnSellRequested;
    public Action<InventoryItemData> OnEquipRequested;
    public Action<InventoryItemData> OnUseRequested;
    public Action<int, int> OnSlotDropped;
    public Action<int> OnDiscardRequested;

    private void OnDisable()
    {
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlayUISound("Inventory_Close_Sound");
        }
    }
    public void BindViewModel(InventoryViewModel viewModel)
    {
        _viewModel = viewModel;
        RefreshGold();
        BuildSlots();
    }

    public void Refresh()
    {
        Debug.Log("[InventoryView] Refresh() 호출됨");
        RefreshGold();
        BuildSlots();
    }

    private void BuildSlots()
    {
        ClearSlot();

        List<InventoryItemData> slots = _viewModel.GetInventorySlots(SlotCount);

        for (int i = 0; i < slots.Count; i++)
        {
            InventoryItemData slotData = slots[i];

            GameObject slotObject = Instantiate(Prefab_InventorySlot, Transform_SlotParent);
            InventorySlotView slotView = slotObject.GetComponent<InventorySlotView>();
            slotView.Setup(slotData, RequestSell, RequestEquip, RequestUse, RequestSlotDrop, RequestDiscard, i);

            if (slotData != null)
            {
                _slotViewsByItemId[slotData.Id] = slotView;
            }

            _spawnedSlots.Add(slotObject);
        }
    }

    public void StartItemCoolDown(string itemId, float duration)
    {
        if (_slotViewsByItemId.TryGetValue(itemId, out InventorySlotView slotView))
        {
            slotView.StartCoolDown(duration);
        }
    }

    private void RequestSell(InventoryItemData data, int count)
    {
        OnSellRequested?.Invoke(data, count);
    }

    private void RequestEquip(InventoryItemData data)
    {
        OnEquipRequested?.Invoke(data);
    }

    private void RequestUse(InventoryItemData data)
    {
        OnUseRequested?.Invoke(data);
    }

    private void RequestSlotDrop(int fromIndex, int toIndex)
    {
        OnSlotDropped?.Invoke(fromIndex, toIndex);
    }

    private void RequestDiscard(int slotIndex)
    {
        OnDiscardRequested?.Invoke(slotIndex);
    }

    private void ClearSlot()
    {
        foreach (GameObject slot in _spawnedSlots)
        {
            Destroy(slot);
        }

        _spawnedSlots.Clear();
    }

    public void RefreshGold()
    {
        Text_Gold.text = $"Gold: {_viewModel.GetCurrentGold()}G";
    }
}
