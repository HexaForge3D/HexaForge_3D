using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public enum TransactionResult
{
    Success,
    NotEnoughGold,
    InventoryFull,
    ItemNotFound,
    NotEnoughItems,
    SlotNotFound,
    LevelNotEnough,
}

public class SaveManager : BaseMonoManager<SaveManager>
{
    private const int SlotCount = 3;
    private const string SaveFileName = "CharacterSaveData.json";
    public const float SellPriceRatio = 0.5f;
    private const int SkillPointsPerLevel = 1;
    public const int MaxInventorySlots = 63;

    public SaveData CurrentSaveData { get; private set; }

    protected override void Awake()
    {
        base.Awake();

        if(Instance == this)
        {
            CurrentSaveData = LoadOrCreatePlayerData();
        }
    }

    private SaveData LoadOrCreatePlayerData()
    {
        if (HasSaveFile())
        {
            return LoadFromFile();
        }

        SaveData newData = CreateEmptySaveData();
        SaveToFile(newData);

        return newData;
    }


    // 캐릭터 생성/삭제 + 데이터 최신화
    private SaveData CreateEmptySaveData()
    {
        SaveData data = new SaveData
        {
            Slots = new List<CharacterSaveData>()
        };

        for (int i = 0; i < SlotCount; i++)
        {
            data.Slots.Add(new CharacterSaveData
            {
                SlotId = $"Slot_{i:D2}",
                IsEmpty = true
            });
        }

        return data;
    }

    public bool CreateCharacter(string slotId, string name, string job)
    {
        CharacterSaveData slot = FindSlot(slotId);

        PlayerTableData jobMaster = GameDataManager.Instance.GetData<PlayerTableData>(job);

        if (slot == null)
        {
            Debug.LogError($"[SaveManager] {slotId}를 찾을 수 없습니다.");
            return false;
        }

        if (slot.IsEmpty == false)
        {
            Debug.LogError($"[SaveManager] {slotId}는 이미 캐릭터가 존재합니다.");
            return false;
        }
        
        if (jobMaster == null )
        {
            Debug.LogError($"[SaveManager] {job}에 대한 직업 마스터 데이터를 찾을 수 없습니다.");
            return false;
        }

        slot.IsEmpty = false;
        slot.Name = name;
        slot.Job = job;
        slot.Exp = 0;
        slot.Hp = jobMaster.Hp;
        slot.Mp = jobMaster.Mp;
        slot.Atk = jobMaster.Atk;
        slot.Def = jobMaster.Def;
        slot.Gold = 0;
        slot.CurrentHp = jobMaster.Hp; // 현재 체력도 생성되었을 때는 최대 체력으로
        slot.CurrentMp = jobMaster.Mp; // 현재 마나도 생성되었을 때는 최대 마나로

        slot.Inventory = new InventorySaveData
        {
            Slots = new List<InventorySlotSaveData>()
        };

        EnsureInventorySize(slot);

        slot.Equipped = new EquippedItemsSaveData();

        slot.Skills = new SkillSaveData
        {
            Skills = new List<SkillProgressData>(),
            AvailablePoints = 0
        };

        SaveToFile(CurrentSaveData);

        return true;
    }
    
    public bool DeleteCharacter(string slotId)
    {
        CharacterSaveData slot = FindSlot(slotId);

        if (slot == null)
        {
            Debug.LogError($"[SaveManager] {slotId}를 찾을 수 없습니다.");
            return false;
        }

        slot.IsEmpty = true;
        slot.Name = null;
        slot.Job = null;
        slot.Exp = 0;
        slot.Hp = 0;
        slot.Mp = 0;
        slot.Atk = 0;
        slot.Def = 0;
        slot.Gold = 0;
        slot.Inventory = null;
        slot.Skills = null;
        slot.Equipped = null;

        SaveToFile(CurrentSaveData);
        
        return true;
    }

    public void UpdateCharacter(CharacterSaveData updateSlot)
    {
        int index = FindSlotIndex(updateSlot.SlotId);

        if (index == -1)
        {
            Debug.LogError($"[SaveManager] {updateSlot.SlotId}를 찾을 수 없습니다.");
            return;
        }

        CurrentSaveData.Slots[index] = updateSlot;
        SaveToFile(CurrentSaveData);
    }


    // 슬롯과 인덱스를 찾는 보조 메서드
    private CharacterSaveData FindSlot(string slotId)
    {
        foreach (CharacterSaveData slot in CurrentSaveData.Slots)
        {
            if (slot.SlotId == slotId)
            {
                return slot;
            }
        }

        return null;
    }

    private int FindSlotIndex(string slotId)
    {
        for (int i = 0; i < CurrentSaveData.Slots.Count; i++)
        {
            if (CurrentSaveData.Slots[i].SlotId == slotId)
            {
                return i;
            }
        }

        return -1;
    }

    private InventorySlotSaveData FindAvailableInventorySlot(CharacterSaveData slot, string itemId, int maxStack)
    {
        foreach (InventorySlotSaveData invSlot in slot.Inventory.Slots)
        {
            if (IsEmptySlot(invSlot) == false && invSlot.ItemId == itemId && invSlot.Count < maxStack)
            {
                return invSlot;
            }
        }
        return null;
    }

    private int FindEmptySlotIndex(List<InventorySlotSaveData> slots)
    {
        for (int i = 0; i < slots.Count; i++)
        {
            if (IsEmptySlot(slots[i]))
            {
                return i;
            }
        }

        return -1;
    }


    // 데이터 관리
    private string GetPath()
    {
        return Path.Combine(Application.persistentDataPath, SaveFileName);
    }

    private bool HasSaveFile()
    {
        return File.Exists(GetPath());
    }

    private void SaveToFile(SaveData data)
    {
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(GetPath(), json);
        Debug.Log($"[SaveManager] 세이브 완료: {GetPath()}");
    }

    private SaveData LoadFromFile()
    {
        string json = File.ReadAllText(GetPath());
        SaveData data = JsonUtility.FromJson<SaveData>(json);

        foreach (CharacterSaveData slot in data.Slots)
        {
            if (slot.IsEmpty == false)
            {
                EnsureInventorySize(slot);
            }
        }

        Debug.Log($"[SaveManager] 세이브 로드 완료: {GetPath()}");
        return data;
    }

    // 기능 메서드
    public bool ChangeGold(string slotId, int amount)
    {
        CharacterSaveData slot = FindSlot(slotId);

        if (slot == null)
        {
            Debug.LogError($"[SaveManager] {slotId}를 찾을 수 없습니다.");
            return false;
        }

        if (slot.Gold + amount < 0)
        {
            Debug.LogError($"[SaveManager] 골드가 부족합니다. 현재: {slot.Gold}, 요청: {amount}");
            return false;
        }

        slot.Gold += amount;
        SaveToFile(CurrentSaveData);

        return true;
    }

    public void AddExp(string slotId, int newExp, int levelBefore, int levelAfter)
    {
        CharacterSaveData slot = FindSlot(slotId);

        if (slot == null)
        {
            Debug.LogError($"[SaveManager] {slotId}를 찾을 수 없습니다.");
            return;
        }

        slot.Exp = newExp;

        if (levelAfter > levelBefore)
        {
            int levelsGained = levelAfter - levelBefore;
            
            PlayerTableData playerStats = GameDataManager.Instance.GetData<PlayerTableData>(slot.Job);

            if (playerStats != null)
            {
                int gainedHp = playerStats.HpPerLevel * levelsGained;
                int gainedMp = playerStats.MpPerLevel * levelsGained;
                int gainedAtk = playerStats.AtkPerLevel * levelsGained;
                int gainedDef = playerStats.DefPerLevel * levelsGained;
                // 최대 스탯 영구 증가 (일단은 기초로 잡아놓았습니다 Player.json에서 값 수정하면 됩니다
                slot.Hp += gainedHp;
                slot.Mp += gainedMp;
                slot.Atk += gainedAtk;
                slot.Def += gainedDef;
                // 레벨 업을 할 시 현재 체력과 마나는 풀로 차는 것이 아닌 증가한 값 만큼 회복이 됩니다(추가 됩니다)
                slot.CurrentHp += gainedHp;
                slot.CurrentMp += gainedMp;

                Debug.Log($"[SaveManager] 레벨업! 최대 체력: {slot.Hp} / 최대 마나: {slot.Mp} / 공격력: {slot.Atk} / 방어력: {slot.Def}");
            }

            if (slot.Skills == null)
            {
                slot.Skills = new SkillSaveData
                {
                    Skills = new List<SkillProgressData>(),
                    AvailablePoints = 0
                };
            }

            int skillPointsGained = (levelAfter / 2) - (levelBefore / 2);
            slot.Skills.AvailablePoints += skillPointsGained;
            Debug.Log($"[SaveManager] 레벨업! {levelBefore} > {levelAfter}, 스킬 포인트 +{levelsGained}");
        }

        SaveToFile(CurrentSaveData);
    }

    public CharacterSaveData GetChararcterData(string slotId)
    {
        return FindSlot(slotId);
    }

    public void SaveCurrentState()
    {
        SaveToFile(CurrentSaveData);
    }

    // 시스템 메세지로 문구를 띄울 메서드 모음
    private bool IsEmptySlot(InventorySlotSaveData slot)
    {
        return slot == null || string.IsNullOrEmpty(slot.ItemId);
    }

    public TransactionResult AddItem(string slotId, string itemId, int count)
    {
        CharacterSaveData slot = FindSlot(slotId);

        if (slot == null)
        {
            return TransactionResult.SlotNotFound;
        }

        ItemTableData itemMaster = GameDataManager.Instance.GetData<ItemTableData>(itemId);

        if (itemMaster == null)
        {
            return TransactionResult.ItemNotFound;
        }

        EnsureInventorySize(slot);

        int remaining = count;

        for (int i = 0; i < slot.Inventory.Slots.Count && remaining > 0; i++)
        {
            InventorySlotSaveData existing = slot.Inventory.Slots[i];

            if (IsEmptySlot(existing) == false && existing.ItemId == itemId && existing.Count < itemMaster.MaxStack)
            {
                int availableSpace = itemMaster.MaxStack - existing.Count;
                int addAmount = Mathf.Min(availableSpace, remaining);

                existing.Count += addAmount;
                remaining -= addAmount;
            }
        }

        for (int i = 0; i < slot.Inventory.Slots.Count && remaining > 0; i++)
        {
            if (IsEmptySlot(slot.Inventory.Slots[i]))
            {
                int addAmount = Mathf.Min(itemMaster.MaxStack, remaining);

                slot.Inventory.Slots[i] = new InventorySlotSaveData
                {
                    ItemId = itemId,
                    Count = addAmount
                };

                remaining -= addAmount;
            }
        }

        if (remaining > 0)
        {
            return TransactionResult.InventoryFull;
        }

        SaveToFile(CurrentSaveData);
        return TransactionResult.Success;
    }

    public TransactionResult BuyItem(string slotId, string itemId)
    {
        CharacterSaveData slot = FindSlot(slotId);

        if (slot == null)
        {
            return TransactionResult.SlotNotFound;
        }

        ItemTableData itemMaster = GameDataManager.Instance.GetData<ItemTableData>(itemId);

        if (itemMaster == null)
        {
            return TransactionResult.ItemNotFound;
        }

        if (slot.Gold < itemMaster.Price)
        {
            return TransactionResult.NotEnoughGold;
        }

        EnsureInventorySize(slot);

        InventorySlotSaveData existingSlot = FindAvailableInventorySlot(slot, itemId, itemMaster.MaxStack);

        if (existingSlot != null)
        {
            existingSlot.Count += 1;
        }
        else
        {
            int emptyIndex = FindEmptySlotIndex(slot.Inventory.Slots);

            if (emptyIndex == -1)
            {
                return TransactionResult.InventoryFull;
            }

            slot.Inventory.Slots[emptyIndex] = new InventorySlotSaveData
            {
                ItemId = itemId,
                Count = 1
            };
        }

        slot.Gold -= itemMaster.Price;

        SaveToFile(CurrentSaveData);
        return TransactionResult.Success;
    }

    public TransactionResult SellItem(string slotId, int slotIndex, int count)
    {
        CharacterSaveData slot = FindSlot(slotId);

        if (slot == null || slot.Inventory == null)
        {
            return TransactionResult.SlotNotFound;
        }

        if (slotIndex < 0 || slotIndex >= slot.Inventory.Slots.Count)
        {
            return TransactionResult.ItemNotFound;
        }

        InventorySlotSaveData targetSlot = slot.Inventory.Slots[slotIndex];

        if (IsEmptySlot(targetSlot))
        {
            return TransactionResult.ItemNotFound;
        }

        ItemTableData itemMaster = GameDataManager.Instance.GetData<ItemTableData>(targetSlot.ItemId);

        if (itemMaster == null)
        {
            return TransactionResult.ItemNotFound;
        }

        if (targetSlot.Count < count)
        {
            return TransactionResult.NotEnoughItems;
        }

        targetSlot.Count -= count;

        if (targetSlot.Count <= 0)
        {
            slot.Inventory.Slots[slotIndex] = new InventorySlotSaveData { ItemId = "", Count = 0 };
        }

        int sellPrice = Mathf.FloorToInt(itemMaster.Price * SellPriceRatio) * count;
        slot.Gold += sellPrice;

        SaveToFile(CurrentSaveData);
        return TransactionResult.Success;
    }

    public TransactionResult RemoveItem(string slotId, string itemId, int count)
    {
        CharacterSaveData slot = FindSlot(slotId);

        if (slot == null || slot.Inventory == null)
        {
            return TransactionResult.SlotNotFound;
        }

        int remaining = count;

        for (int i = 0; i < slot.Inventory.Slots.Count && remaining > 0; i++)
        {
            InventorySlotSaveData existing = slot.Inventory.Slots[i];

            if (IsEmptySlot(existing) == false && existing.ItemId == itemId)
            {
                int removeAmount = Mathf.Min(existing.Count, remaining);
                existing.Count -= removeAmount;
                remaining -= removeAmount;

                if (existing.Count <= 0)
                {
                    slot.Inventory.Slots[i] = new InventorySlotSaveData { ItemId = "", Count = 0 };
                }
            }
        }

        if (remaining > 0)
        {
            return TransactionResult.NotEnoughItems;
        }

        SaveToFile(CurrentSaveData);
        return TransactionResult.Success;
    }

    public TransactionResult RemoveItemAtSlot(string slotId, int slotIndex)
    {
        CharacterSaveData slot = FindSlot(slotId);

        if (slot == null || slot.Inventory == null)
        {
            return TransactionResult.SlotNotFound;
        }

        if (slotIndex < 0 || slotIndex >= slot.Inventory.Slots.Count)
        {
            return TransactionResult.ItemNotFound;
        }

        if (slot.Inventory.Slots[slotIndex] == null)
        {
            return TransactionResult.ItemNotFound;
        }

        slot.Inventory.Slots[slotIndex] = null;

        SaveToFile(CurrentSaveData);
        return TransactionResult.Success;
    }

    public TransactionResult SwapInventorySlot(string slotId, string oldItemId, string newItemId)
    {
        CharacterSaveData slot = FindSlot(slotId);

        if (slot == null || slot.Inventory == null)
        {
            return TransactionResult.SlotNotFound;
        }

        int index = slot.Inventory.Slots.FindIndex(s => s.ItemId == oldItemId);

        if (index == -1)
        {
            return TransactionResult.ItemNotFound;
        }

        if (string.IsNullOrEmpty(newItemId) == false)
        {
            slot.Inventory.Slots[index] = new InventorySlotSaveData
            {
                ItemId = newItemId,
                Count = 1
            };
        }
        else
        {
            slot.Inventory.Slots.RemoveAt(index);
        }

        SaveToFile(CurrentSaveData);
        return TransactionResult.Success;
    }

    public TransactionResult MoveOrMergeInventorySlot(string slotId, int fromIndex, int toIndex)
    {
        CharacterSaveData slot = FindSlot(slotId);

        if (slot == null || slot.Inventory == null)
        {
            return TransactionResult.SlotNotFound;
        }

        List<InventorySlotSaveData> slots = slot.Inventory.Slots;

        if (fromIndex < 0 || fromIndex >= MaxInventorySlots || toIndex < 0 || toIndex >= MaxInventorySlots)
        {
            return TransactionResult.ItemNotFound;
        }

        InventorySlotSaveData fromSlot = slots[fromIndex];

        if (IsEmptySlot(fromSlot))
        {
            return TransactionResult.ItemNotFound;
        }

        InventorySlotSaveData toSlot = slots[toIndex];

        if (IsEmptySlot(toSlot))
        {
            slots[toIndex] = fromSlot;
            slots[fromIndex] = new InventorySlotSaveData { ItemId = "", Count = 0 };
        }
        else
        {
            ItemTableData itemMaster = GameDataManager.Instance.GetData<ItemTableData>(fromSlot.ItemId);

            if (fromSlot.ItemId == toSlot.ItemId && itemMaster != null)
            {
                int availableSpace = itemMaster.MaxStack - toSlot.Count;
                int moveAmount = Mathf.Min(availableSpace, fromSlot.Count);

                toSlot.Count += moveAmount;
                fromSlot.Count -= moveAmount;

                if (fromSlot.Count <= 0)
                {
                    slots[fromIndex] = new InventorySlotSaveData { ItemId = "", Count = 0 };
                }
            }
            else
            {
                slots[fromIndex] = toSlot;
                slots[toIndex] = fromSlot;
            }
        }

        SaveToFile(CurrentSaveData);
        return TransactionResult.Success;
    }

    public TransactionResult SwapInventorySlot(string slotId, int slotIndex, string newItemId)
    {
        CharacterSaveData slot = FindSlot(slotId);

        if (slot == null || slot.Inventory == null)
        {
            return TransactionResult.SlotNotFound;
        }

        if (slotIndex < 0 || slotIndex >= slot.Inventory.Slots.Count)
        {
            return TransactionResult.ItemNotFound;
        }

        if (string.IsNullOrEmpty(newItemId) == false)
        {
            slot.Inventory.Slots[slotIndex] = new InventorySlotSaveData
            {
                ItemId = newItemId,
                Count = 1
            };
        }
        else
        {
            slot.Inventory.Slots[slotIndex] = new InventorySlotSaveData { ItemId = "", Count = 0 };
        }

        SaveToFile(CurrentSaveData);
        return TransactionResult.Success;
    }

    public void AddGold(string slotId, int amount)
    {
        CharacterSaveData slot = FindSlot(slotId);

        if (slot == null) return;

        slot.Gold += amount;
        SaveToFile(CurrentSaveData);
    }

    public static string GetTransactionMessage(TransactionResult result)
    {
        switch (result)
        {
            case TransactionResult.NotEnoughGold: return "Not enough gold.";
            case TransactionResult.InventoryFull: return "Inventory is full.";
            case TransactionResult.NotEnoughItems: return "Not enough items.";
            case TransactionResult.ItemNotFound: return "Item not found.";
            case TransactionResult.SlotNotFound: return "Character not found.";
            case TransactionResult.LevelNotEnough: return "Level is not Enough.";
            default: return null;
        }
    }

    private void EnsureInventorySize(CharacterSaveData slot)
    {
        if (slot.Inventory == null)
        {
            slot.Inventory = new InventorySaveData { Slots = new List<InventorySlotSaveData>() };
        }

        while (slot.Inventory.Slots.Count < MaxInventorySlots)
        {
            slot.Inventory.Slots.Add(new InventorySlotSaveData { ItemId = "", Count = 0 });
        }
    }
}
