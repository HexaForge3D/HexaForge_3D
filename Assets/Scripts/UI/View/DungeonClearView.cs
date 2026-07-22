using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DungeonClearView : BaseUI
{
    [SerializeField] private TMP_Text Text_Gold;
    [SerializeField] private Transform Transform_ItemRewardParent;
    [SerializeField] private GameObject Prefab_RewardItemSlot;
    [SerializeField] private Button Button_Confirm;

    private Action _onConfirmClicked;
    private readonly List<GameObject> _spawnedRewardSlots = new List<GameObject>();

    public void Setup(int gold, List<string> itemIds, Action onConfirmClicked)
    {
        _onConfirmClicked = onConfirmClicked;

        Text_Gold.text = $"+{gold}G";

        BuildRewardSlots(itemIds);

        Button_Confirm.onClick.RemoveAllListeners();
        Button_Confirm.onClick.AddListener(OnClickConfirm);
    }

    private void BuildRewardSlots(List<string> itemIds)
    {
        ClearRewardSlots();

        if (itemIds == null)
        {
            return;
        }

        ItemRepository itemRepository = new ItemRepository();

        foreach (string itemId in itemIds)
        {
            ItemData item = itemRepository.GetItem(itemId);

            if (item == null)
            {
                continue;
            }

            GameObject slotObject = Instantiate(Prefab_RewardItemSlot, Transform_ItemRewardParent);
            RewardItemSlotView slotView = slotObject.GetComponent<RewardItemSlotView>();
            slotView.Setup(item);

            _spawnedRewardSlots.Add(slotObject);
        }
    }

    private void ClearRewardSlots()
    {
        foreach (GameObject slot in _spawnedRewardSlots)
        {
            Destroy(slot);
        }

        _spawnedRewardSlots.Clear();
    }

    private void OnClickConfirm()
    {
        _onConfirmClicked?.Invoke();
    }
}