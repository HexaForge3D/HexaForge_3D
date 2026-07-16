using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveManager : BaseMonoManager<SaveManager>
{
    private const int SlotCount = 3;
    private const string SaveFileName = "CharacterSaveData.json";

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
}
