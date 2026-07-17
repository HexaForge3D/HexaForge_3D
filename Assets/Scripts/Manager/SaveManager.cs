using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveManager : BaseMonoManager<SaveManager>
{
    private const int SlotCount = 3;
    private const string SaveFileName = "CharacterSaveData.json";
    public const float SellPriceRatio = 0.8f;
    private const int SkillPointsPerLevel = 4;

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

        slot.Inventory = new InventorySaveData
        {
            Slots = new List<InventorySlotSaveData>()
        };

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

        SaveToFile(CurrentSaveData);
        
        return true;
    }

    public void UpdataCharacter(CharacterSaveData updateSlot)
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

    private InventorySlotSaveData FindInventorySlot(CharacterSaveData slot, string itemId)
    {
        foreach (InventorySlotSaveData invSlot in slot.Inventory.Slots)
        {
            if (invSlot.ItemId == itemId) return invSlot;

        }

        return null;
    }

    private InventorySlotSaveData FindAvailableInventorySlot(CharacterSaveData slot, string itemId, int maxStack)
    {
        foreach (InventorySlotSaveData invSlot in slot.Inventory.Slots)
        {
            if (invSlot.ItemId == itemId && invSlot.Count < maxStack) return invSlot;
        }

        return null;
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

    public bool AddItem(string slotId, string itemId, int count)
    {
        CharacterSaveData slot = FindSlot(slotId);

        if (slot == null)
        {
            Debug.LogError($"[SaveManager] {slotId}를 찾을 수 없습니다.");
            return false;
        }

        ItemTableData itemMaster = GameDataManager.Instance.GetData<ItemTableData>(itemId);

        if (itemMaster == null)
        {
            Debug.LogError($"[SaveManager] {itemId}에 대한 아이템 데이터를 찾을 수 없습니다.");
            return false;
        }

        if (slot.Inventory == null)
        {
            slot.Inventory = new InventorySaveData
            {
                Slots = new List<InventorySlotSaveData>()
            };
        }

        InventorySlotSaveData existingSlot = FindInventorySlot(slot, itemId);

        if (existingSlot != null && existingSlot.Count + count <= itemMaster.MaxStack)
        {
            existingSlot.Count += count;
        }
        else
        {
            slot.Inventory.Slots.Add(new InventorySlotSaveData
            {
                ItemId = itemId,
                Count = count
            });
        }

        SaveToFile(CurrentSaveData);

        return true;
    }

    public bool RemoveItem(string slotId, string itemId, int count)
    {
        CharacterSaveData slot = FindSlot(slotId);

        if (slot == null)
        {
            Debug.LogError($"[SaveManager] {slotId}를 찾을 수 없습니다.");
            return false;
        }

        InventorySlotSaveData existingSlot = FindInventorySlot(slot, itemId);

        if (existingSlot == null || existingSlot.Count < count)
        {
            Debug.LogError($"[SaveManager] {itemId} 보유 수량이 부족합니다.");
            return false;
        }

        existingSlot.Count -= count;

        if (existingSlot.Count <= 0 )
        {
            slot.Inventory.Slots.Remove(existingSlot);
        }

        SaveToFile(CurrentSaveData);

        return true;
    }

    public bool BuyItem(string slotId, string itemId)
    {
        CharacterSaveData slot = FindSlot(slotId);

        if (slot == null)
        {
            Debug.LogError($"[SaveManager] {slotId}를 찾을 수 없습니다.");
            return false;
        }

        ItemTableData itemMaster = GameDataManager.Instance.GetData<ItemTableData>(itemId);

        if (itemMaster == null)
        {
            Debug.LogError($"[SaveManager] {itemId}에 대한 아이템 데이터를 찾을 수 없습니다.");
            return false;
        }

        if (slot.Gold < itemMaster.Price)
        {
            Debug.LogWarning($"[SaveManager] 골드가 부족합니다. 현재: {slot.Gold}, 필요: {itemMaster.Price}");
            return false;
        }

        if (slot.Inventory == null)
        {
            slot.Inventory = new InventorySaveData
            {
                Slots = new List<InventorySlotSaveData>()
            };
        }

        slot.Gold -= itemMaster.Price;

        InventorySlotSaveData existingSlot = FindAvailableInventorySlot(slot, itemId, itemMaster.MaxStack);

        Debug.Log($"[SaveManager] BuyItem - itemId: {itemId}, MaxStack: {itemMaster.MaxStack}, existingSlot count: {existingSlot?.Count ?? -1}");

        if (existingSlot != null && existingSlot.Count < itemMaster.MaxStack)
        {
            existingSlot.Count += 1;
        }
        else
        {
            slot.Inventory.Slots.Add(new InventorySlotSaveData
            {
                ItemId = itemId,
                Count = 1
            });
        }

        SaveToFile(CurrentSaveData);

        return true;
    }

    public bool SellItem(string slotId, string itemId, int count)
    {
        CharacterSaveData slot = FindSlot(slotId);

        if (slot == null)
        {
            Debug.LogError($"[SaveManager] {slotId}를 찾을 수 없습니다.");
            return false;
        }
        
        ItemTableData itemMaster = GameDataManager.Instance.GetData<ItemTableData>(itemId);

        if (itemMaster == null)
        {
            Debug.LogError($"[SaveManager] {itemId}에 대한 아이템 데이터를 찾을 수 없습니다.");
            return false;
        }

        InventorySlotSaveData existingSlot = FindInventorySlot(slot, itemId);

        if (existingSlot == null || existingSlot.Count < count)
        {
            Debug.LogWarning($"[SaveManager] {itemId} 보유 수량이 부족합니다.");
            return false;
        }

        existingSlot.Count -= count;

        if (existingSlot.Count <= 0)
        {
            slot.Inventory.Slots.Remove(existingSlot);
        }

        int sellPrice = Mathf.FloorToInt(itemMaster.Price * SellPriceRatio) * count;
        slot.Gold += sellPrice;

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
            slot.Skills.AvailablePoints += levelsGained * SkillPointsPerLevel;
            Debug.Log($"[SaveManager] 레벨업! {levelBefore} > {levelAfter}, 스킬 포인트 +{levelsGained}");
        }

        SaveToFile(CurrentSaveData);
    }
}
