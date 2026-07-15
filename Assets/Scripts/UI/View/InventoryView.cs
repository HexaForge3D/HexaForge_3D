using System.Collections.Generic;
using UnityEngine;

public class InventoryView : BaseOverLayUI
{
    [SerializeField] private int SlotCount = 20;
    [SerializeField] private Transform Transform_SlotParent;
    [SerializeField] private GameObject Prefab_InventorySlot;

    private InventoryViewModel _viewModel;
    private readonly List<GameObject> _spawnedSlots = new List<GameObject>();

    public void BindViewModel(InventoryViewModel viewModel)
    {
        _viewModel = viewModel;
        BuildSlots();
    }

    private void BuildSlots()
    {
        ClearSlot();

        List<ItemData> slots = _viewModel.GetInventorySlots(SlotCount);

        foreach (ItemData item in slots)
        {
            GameObject slotObject = Instantiate(Prefab_InventorySlot, Transform_SlotParent);
            InventorySlotView slotView = slotObject.GetComponent<InventorySlotView>();

            //임시 카운트 처리 추후 세이브데이터로 받아올 예정
            int count = item != null ? 1 : 0;
            slotView.Setup(item, count);

            _spawnedSlots.Add(slotObject);
        }
    }

    private void ClearSlot()
    {
        foreach (GameObject slot in _spawnedSlots)
        {
            Destroy(slot);
        }

        _spawnedSlots.Clear();
    }
}
