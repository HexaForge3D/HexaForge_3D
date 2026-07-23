using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MonsterHealth : MonoBehaviour
{
    [Header("체력 설정")]
    public int maxHealth = 150;
    private int currentHealth;
    private bool isDead = false;

    [Header("Reward")]
    public int dropExp = 30;
    public int minGold = 10;
    public int maxGold = 500;

    public static event Action<int> OnMonsterDied;
    public event Action<MonsterHealth> OnMonsterDieCount;
    public static event Action<int> OnMonsterMoney;
    public static event Action<ItemTableData, int> OnMonsterItem;

    private static List<ItemTableData> droppableItemsCache = null;
    private static bool isInitializingCache = false;

    private void OnEnable()
    {
        PlayerBattle.OnPlayerDead += StopAttackOnPlayerDeath;
    }

    private void OnDisable()
    {
        PlayerBattle.OnPlayerDead -= StopAttackOnPlayerDeath;
    }

    private void Start()
    {
        currentHealth = maxHealth;

        if (droppableItemsCache == null && !isInitializingCache)
        {
            InitializeDroppableItemsAsync().Forget();
        }

    }

    private async UniTaskVoid InitializeDroppableItemsAsync()
    {

        isInitializingCache = true;

        await GameDataManager.Instance.WaitUntilReadyAsync();

        droppableItemsCache = new List<ItemTableData>();

        Dictionary<string, ItemTableData> allItemsDict = GameDataManager.Instance.GetAllData<ItemTableData>();

        if (allItemsDict != null)
        {
            foreach (ItemTableData item in allItemsDict.Values)
            {
                if (item.UsageType == "Consumable" || item.UsageType == "Material")
                {
                    droppableItemsCache.Add(item);
                }
            }
            Debug.Log($"드랍 가능 아이템 {droppableItemsCache.Count}개 캐싱 완료!");
        }
        else
        {
            Debug.LogError("아이템 데이터를 불러오지 못했습니다.");
        }
    }

    public void TakeDamage(int damageAmount)
    {
        if (isDead) return;

        currentHealth -= damageAmount;
        currentHealth = Mathf.Max(0, currentHealth);

        Debug.Log($"[몬스터 피격] -{damageAmount} 데미지 (남은체력: {currentHealth} / {maxHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        if (isDead) return;
        isDead = true;
        Debug.Log("몬스터 사망");

        NavMeshAgent agent = GetComponent<NavMeshAgent>();
        if (agent != null) agent.enabled = false;

        Collider coll = GetComponent<Collider>();
        if (coll != null) coll.enabled = false;

        Unity.Behavior.BehaviorGraphAgent bt = GetComponent<Unity.Behavior.BehaviorGraphAgent>();
        if (bt != null) bt.enabled = false;

        Animator anim = GetComponent<Animator>();
        if (anim != null)
        {
            anim.ResetTrigger("Attack");
            anim.SetTrigger("Die");
        }

        CalculateAndDropRewards();

        OnMonsterDieCount?.Invoke(this);
        OnMonsterDied?.Invoke(dropExp);
        
        Destroy(gameObject, 3f);
    }

    private void CalculateAndDropRewards()
    {
        int randomGold = UnityEngine.Random.Range(minGold, maxGold + 1);
        OnMonsterMoney?.Invoke(randomGold);

        if (droppableItemsCache != null && droppableItemsCache.Count > 0)
        {
            float dropChance = 0.6f;

            if (UnityEngine.Random.value <= dropChance)
            {
                int randomIndex = UnityEngine.Random.Range(0, droppableItemsCache.Count);
                ItemTableData randomItem = droppableItemsCache[randomIndex];
                int randomAmount = UnityEngine.Random.Range(1, 11);

                string dropItemPrefabAddress = "DroppedItem";

                SpawnDropItemAsync(dropItemPrefabAddress, randomItem, randomAmount).Forget();
            }
        }
    }

    private async UniTaskVoid SpawnDropItemAsync(string address, ItemTableData item, int amount)
    {
        Vector3 spawnPos = transform.position + new Vector3(0f, 0.3f, 0f);

        GameObject dropObj = await ResourceManager.Inst.InstantiateAsync(address, null, true);

        if (dropObj != null)
        {
            dropObj.transform.position = spawnPos;

            DroppedItem droppedItem = dropObj.GetComponent<DroppedItem>();
            if (droppedItem != null)
            {
                droppedItem.SetUp(item, amount);
            }
        }
    }

    private void StopAttackOnPlayerDeath()
    {
        if (isDead) return;

        NavMeshAgent agent = GetComponent<NavMeshAgent>();
        if ( agent != null && agent.isOnNavMesh)
        {
            agent.ResetPath();
            agent.velocity = Vector3.zero;
        }

        Animator anim = GetComponent<Animator>();
        if (anim != null)
        {
            anim.ResetTrigger("Attack");
            anim.Play("idle_");
        }

        Unity.Behavior.BehaviorGraphAgent bt = GetComponent<Unity.Behavior.BehaviorGraphAgent>() ;
        if (bt != null)
        {
            bt.enabled = false;
        }

        Debug.Log($"{gameObject.name} : 플레이어 사망감지, 공격을 중지합니다");

        Invoke("RebootBrain", 0.5f);
    }

    private void RebootBrain()
    {
        if (isDead) return;

        Unity.Behavior.BehaviorGraphAgent bt = GetComponent<Unity.Behavior.BehaviorGraphAgent>();
        if (bt != null)
        {
            bt.enabled = true;
        }

        Debug.Log($"{gameObject.name} : 뇌 재부팅 완료! 기억이 지워졌으므로 순찰로 복귀합니다.");
    }
}
