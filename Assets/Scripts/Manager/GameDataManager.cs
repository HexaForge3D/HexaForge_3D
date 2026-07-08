using System.Collections.Generic;
using UnityEngine;
using System;

public class GameDataManager : BaseMonoManager<GameDataManager>
{
    private readonly Dictionary<Type, object> _dataStore = new Dictionary<Type, object>();

    protected override void Awake()
    {
        base.Awake();

        if (Instance == this)
        {
            LoadAllData();
        }
    }

    public void ReloadAlData()
    {
        _dataStore.Clear();
        LoadAllData();
    }

    private void LoadAllData()
    {
        LoadData<HuntingAreaTableData>("HuntingArea");
    }

    public void LoadData<T>(string tableName) where T : GameDataBase
    {
        Dictionary<string, T> table = ParseJson<T>(tableName);
        _dataStore[typeof(T)] = table;
    }

    public T GetData<T>(string id) where T : GameDataBase
    {
        Dictionary<string, T> table = GetTable<T>();

        if (table == null || string.IsNullOrEmpty(id)) return null;

        return table.TryGetValue(id, out T data) ? data : null;
    }

    public Dictionary<string, T> GetAllData<T>() where T : GameDataBase
    {
        return GetTable<T>();
    }

    private Dictionary<string, T> GetTable<T>() where T : GameDataBase
    {
        if (_dataStore.TryGetValue(typeof(T), out object table) == false)
        {
            Debug.LogError($"[GameDataManager] {typeof(T)} 데이터가 로드되어 있지 않습니다.");
            return null;
        }

        return table as Dictionary<string, T>;
    }

    [Serializable]
    private class SerializableWrapper<T>
    {
        public List<T> _data;
    }

    private Dictionary<string, T> ParseJson<T>(string tableName) where T : GameDataBase
    {
        string resourcePath = $"JsonOutput/{tableName}";
        TextAsset textAsset = Resources.Load<TextAsset>(resourcePath);

        if (textAsset == null)
        {
            Debug.LogError($"[GameDataManager] {resourcePath} 경로에 리소스가 없습니다.");
            return new Dictionary<string, T>();
        }

        try
        {
            string wrapperJson = "{\"_data\":" + textAsset.text + "}";
            SerializableWrapper<T> wrapper = JsonUtility.FromJson<SerializableWrapper<T>>(wrapperJson);

            if (wrapper._data == null)
            {
                Debug.LogError($"[GameDataManager] {resourcePath}의 데이터가 없습니다.");
                return new Dictionary<string, T>();
            }

            Dictionary<string, T> result = new Dictionary<string, T>(wrapper._data.Count);

            foreach (T data in wrapper._data)
            {
                result.Add(data.ID, data);
            }

            Debug.LogError($"[GameDataManager] {resourcePath} 데이터 {result.Count}건 로드 완료");

            return result;
        }

        catch (Exception e)
        {
            Debug.LogException(e);
            return new Dictionary<string, T>();
        }
    }
}
