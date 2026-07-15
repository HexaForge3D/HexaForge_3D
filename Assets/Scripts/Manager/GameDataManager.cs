using System.Collections.Generic;
using UnityEngine;
using System;
using Cysharp.Threading.Tasks;
using UnityEngine.AddressableAssets;

public class GameDataManager : BaseMonoManager<GameDataManager>
{
    private readonly Dictionary<Type, object> _dataStore = new Dictionary<Type, object>();
    private UniTaskCompletionSource _loadingCompletionSource = new UniTaskCompletionSource();

    protected override void Awake()
    {
        base.Awake();

        if (Instance == this)
        {
            InitializeAsync().Forget();
        }
    }

    private async UniTask LoadAllDataAsync()
    {
        await LoadData<VillageAreaTableData>("VillageArea");
        await LoadData<HuntingAreaTableData>("HuntingArea");
        await LoadData<PlayerTableData>("Player");
        await LoadData<SkillTableData>("Skill");
    }

    private async UniTask InitializeAsync()
    {
        await LoadAllDataAsync();
        _loadingCompletionSource.TrySetResult();
    }

    public UniTask WaitUntilReadyAsync()
    {
        return _loadingCompletionSource.Task;
    }

    public async UniTask ReloadAIDataAsync()
    {
        _dataStore.Clear();
        _loadingCompletionSource = new UniTaskCompletionSource();
        await LoadAllDataAsync();
        _loadingCompletionSource.TrySetResult();
    }

    public async UniTask LoadData<T>(string tableName) where T : GameDataBase
    {
        Dictionary<string, T> table = await ParseJsonAsync<T>(tableName);
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

    private async UniTask<Dictionary<string, T>> ParseJsonAsync<T>(string tableName) where T : GameDataBase
    {
        string address = $"JsonOutput/{tableName}";
        TextAsset textAsset = await LoadTextAssetAsync(address);

        if (textAsset == null)
        {
            Debug.LogError($"[GameDataManager] {address} 경로에 리소스가 없습니다.");
            return new Dictionary<string, T>();
        }

        try
        {
            string wrapperJson = "{\"_data\":" + textAsset.text + "}";
            SerializableWrapper<T> wrapper = JsonUtility.FromJson<SerializableWrapper<T>>(wrapperJson);

            if (wrapper._data == null)
            {
                Debug.LogError($"[GameDataManager] {address}의 데이터가 없습니다.");
                return new Dictionary<string, T>();
            }

            Dictionary<string, T> result = new Dictionary<string, T>(wrapper._data.Count);

            foreach (T data in wrapper._data)
            {
                result.Add(data.ID, data);
            }

            Debug.Log($"[GameDataManager] {address} 데이터 {result.Count}건 로드 완료");

            return result;
        }

        catch (Exception e)
        {
            Debug.LogException(e);
            return new Dictionary<string, T>();
        }
    }

    private async UniTask<TextAsset> LoadTextAssetAsync(string address)
    {
        return await Addressables.LoadAssetAsync<TextAsset>(address).ToUniTask();
    }
}
