using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

// 게임의 흐름 타이밍을 관리할 스크립트
public class GameFlowManager
{
    private string _currentSlotId;
    private string _pendingDeleteSlotId;
    private InGameViewModel _inGameViewModel;

    private string _pendingSellItemId;
    private int _pendingSellCount;
    private Portal _pendingReturnPortal;

    public async UniTask StartAsync()
    {
        await GameDataManager.Instance.WaitUntilReadyAsync();
        await ShowTitleAsync();
    }

    // 요청 메서드 모음
    private void OnGameStartRequested()
    {
        ShowCharacterSelectAsync().Forget();
    }

    private void OnEnterGameRequested(PlayerData data)
    {
        _currentSlotId = data.Id;
        ShowInGameAsync(data).Forget();
    }

    private void OnBackToCharacterSelectRequested()
    {
        PlayerInputSystem.OnInformation -= OnInformationKeyPressed;
        PlayerInputSystem.OnInventory -= OnInventoryKeyPressed;
        Portal.OnPortalInteracted -= OnPortalInteracted;
        PlayerInputSystem.OnSystem -= OnEscapeKeyPressed;
        PlayerBattle.OnHpChanged -= OnPlayerHpChanged;
        PlayerBattle.OnMpChanged -= OnPlayerMpChanged;
        NPC.OnNPCInteracted -= OnNpcInterated;
        PlayerLevel.OnLevelUp -= OnPlayerLevelUp;
        PlayerInputSystem.OnEquipMent -= OnEquipmentKeyPressed;
        PlayerInputSystem.OnMap -= OnMinimapKeyPressed;
        SkillUtil.Instance.OnSkillDataUpdated -= OnSkillDataUpdated;
        SkillUtil.OnSkillCoolTimeStart -= OnSkillCoolTimeStart;
        PlayerBattle.OnPlayerDead -= OnPlayerDead;
        SkillUtil.OnLackMana -= OnLackMana;
        SkillUtil.OnSkillCoolTimeFail -= OnSkillCoolTimeFail;
        PlayerLevel.OnExpChanged -= OnPlayerExpChanged;
        PlayerInputSystem.OnEvasionCoolTimeStarted -= OnEvasionCoolTimeStarted;
        BaseDungeonController.OnDungeonCleared -= OnDungeonCleared;
        BaseDungeonController.OnDungeonFailed -= OnDungeonFailed;
        PlayerLevel.OnLevelUp -= OnPlayerLevelUp;
        RoomFieldManager.OnCurrentMonsterCountChanged -= OnMonsterCountChanged;
        DefenceFieldManager.OnWaveChanged -= OnWaveChanged;
        DefenceFieldManager.OnCountdownChanged -= OnCountdownChanged;
        DefenceTarget.OnTargetHpChanged -= OnTargetHpChanged;
        MonsterHealth.OnMonsterMoney -= OnMonsterMoneyDropped;
        MonsterHealth.OnMonsterItem -= OnMonsterItemDropped;
        PlayerInteraction.OnItemPickup -= OnItemPickup;

        SaveManager.Instance.SaveCurrentState();

        PlayerSpawnManager.Instance.DeSpawnPlayer();
        _currentSlotId = null;
        _inGameViewModel = null;
        ShowCharacterSelectAsync().Forget();
    }

    private void OnMenuCharacterSelectRequested()
    {
        UIManager.Instance.CloseUI(UIType.GameMenuPopup);
        OnBackToCharacterSelectRequested();
    }

    private void OnTeleportRequested(HuntingAreaData data)
    {
        ChangeMapAndCloseAsync(data.Id).Forget();
    }

    private void OnDeleteRequested(string slotId)
    {
        _pendingDeleteSlotId = slotId;
        ShowConfirmAsync("Delete this Character?", OnDeleteConfirmed,"Click_Sound").Forget();
    }

    private void OnQuitGameRequested()
    {
        ShowConfirmAsync("Quit the Game?", OnQuitGameConfirmed,"Click_Sound").Forget();
    }

    private void OnInventorySellRequested(InventoryItemData data, int count)
    {
        _pendingSellItemId = data.Id;
        _pendingSellCount = count;

        int totalPrice = Mathf.FloorToInt(data.Price * SaveManager.SellPriceRatio) * count;
        string message = $"Sell {data.Name} X {count} for {totalPrice}G?";

        ShowConfirmAsync(message, OnSellConfirmed,"Item_Buy_Sound").Forget();
    }

    private void OnCreateCharacterRequested(string slotId)
    {
        ShowCharacterCreateAsync(slotId).Forget();
    }

    private void OnInventoryEquipRequested(InventoryItemData data)
    {
        TransactionResult result = EquipmentManager.Instance.EquipItem(_currentSlotId, data.Id);

        if (result == TransactionResult.Success)
        {
            InventoryView inventoryView = UIManager.Instance.GetUI<InventoryView>(UIType.InventoryPopup);
            inventoryView?.Refresh();

            EquipmentView equipmentView = UIManager.Instance.GetUI<EquipmentView>(UIType.EquipmentPopup);
            equipmentView?.Refresh();

            InformationView informationView = UIManager.Instance.GetUI<InformationView>(UIType.InformationPopup);
            informationView?.Refresh();
        }
        else
        {
            SystemMessageManager.Instance.Show(SaveManager.GetTransactionMessage(result));
        }
    }

    private void OnEquipmentUnequipRequested(string equipSlot)
    {
        TransactionResult result = EquipmentManager.Instance.UnEquipItem(_currentSlotId, equipSlot);

        if (result == TransactionResult.Success)
        {
            EquipmentView equipmentView = UIManager.Instance.GetUI<EquipmentView>(UIType.EquipmentPopup);
            equipmentView?.Refresh();

            InventoryView inventoryView = UIManager.Instance.GetUI<InventoryView>(UIType.InventoryPopup);
            inventoryView?.Refresh();

            InformationView informationView = UIManager.Instance.GetUI<InformationView>(UIType.InformationPopup);
            informationView?.Refresh();
        }
        else
        {
            SystemMessageManager.Instance.Show(SaveManager.GetTransactionMessage(result));
        }
    }

    private void OnReviveRequested()
    {
        UIManager.Instance.CloseUI(UIType.DeathPopup);
        ReviveAndChangeMapAsync().Forget();
    }

    private void OnInventoryUseRequested(InventoryItemData data)
    {
        PlayerBattle playerBattle = PlayerSpawnManager.Instance.GetPlayerBattle();

        if (playerBattle == null) return;

        playerBattle.UsePotion(data.Id);
    }

    private void OnSettingsRequested()
    {
        ShowSettingAsync().Forget();
    }


    private void OnPortalInteracted(Portal portal)
    {
        portal.ExitPortal();

        if (portal.PortalType == PortalType.DungeonStart)
        {
            if (portal.ParentMapName == "Village")
            {
                
                ShowHuntingAreaAsync().Forget();
                return;
            }

            HideDungeonInfoIfExists();

            _pendingReturnPortal = portal;
            ShowConfirmAsync("Return to Village?", OnVillageReturnConfirmed,"Click_Sound").Forget();
            return;
        }

        if (portal.PortalType == PortalType.DungeonClear)
        {
            ChangeMapWithLoadingAsync(portal.TargetMapId, PortalType.DungeonStart, true).Forget();
            return;
        }

        if (portal.PortalType == PortalType.Village)
        {
            ChangeMapWithLoadingAsync(portal.TargetMapId, portal.PortalType, true).Forget();
            return;
        }

        if (string.IsNullOrEmpty(portal.TargetMapId) == false)
        {
            ChangeMapWithLoadingAsync(portal.TargetMapId, portal.PortalType, false).Forget();
        }

        else
        {
            Debug.LogWarning($"[GameFlowManager] 포탈({portal.name})에 TargetMapId{portal.TargetMapId}가 설정되어 있지 않습니다.");
        }

    }

    private void OnNpcInterated(NPC npc)
    {
        switch (npc.NPCId)
        {
            case NPCId.Store:
                ToggleUI(UIType.ShopUI, ShowShop);
                    break;
            case NPCId.Smithy:
                Debug.Log("smithy 상호작용");
                break;
            case NPCId.MainQuest:
                Debug.Log("MainQuest 상호작용");
                break;
            default:
                Debug.Log("NPCId 설정이 잘못되었습니다.");
                break;
        }
    }

    private void OnCharacterCreated()
    {
        UIManager.Instance.CloseUI(UIType.CharacterCreatePopup);
        ShowCharacterSelectAsync().Forget();
    }

    private void OnDeleteConfirmed()
    {
        SaveManager.Instance.DeleteCharacter(_pendingDeleteSlotId);
        ShowCharacterSelectAsync().Forget();
    }

    private void OnQuitGameConfirmed()
    {
        Application.Quit();
    }

    private void OnSellConfirmed()
    {
        TransactionResult result = SaveManager.Instance.SellItem(_currentSlotId, _pendingSellItemId, _pendingSellCount);

        if (result == TransactionResult.Success)
        {
            InventoryView inventoryView = UIManager.Instance.GetUI<InventoryView>(UIType.InventoryPopup);
            inventoryView?.Refresh();

            ShopView shopView = UIManager.Instance.GetUI<ShopView>(UIType.ShopUI);
            shopView?.RefreshGold();
        }
        else
        {
            SystemMessageManager.Instance.Show(SaveManager.GetTransactionMessage(result));
        }
    }

    private void OnSkillDataUpdated()
    {
        InGameView inGameView = UIManager.Instance.GetUI<InGameView>(UIType.InGameUI);
        inGameView?.RefreshSkillSlots();
    }

    private void OnPlayerHpChanged(int currentHp, int maxHp)
    {
        _inGameViewModel?.HandleHpChanged(currentHp, maxHp);
    }

    private void OnPlayerMpChanged(int currentMp, int maxMp)
    {
        _inGameViewModel?.HandleMpChanged(currentMp, maxMp);
    }

    private void OnPlayerExpChanged(int currentExp, int maxExp)
    {
        _inGameViewModel?.HandleExpChanged(currentExp);
    }

    private void OnPlayerLevelUp()
    {
        InGameView inGameView = UIManager.Instance.GetUI<InGameView>(UIType.InGameUI);
        inGameView?.RefreshSkillSlots();

        CharacterSaveData data = SaveManager.Instance.GetChararcterData(_currentSlotId);

        if (data == null) return;

        _inGameViewModel?.HandleHpChanged(data.CurrentHp, data.Hp);
        _inGameViewModel?.HandleMpChanged(data.CurrentMp, data.Mp);

        InformationView inforamtionView = UIManager.Instance.GetUI<InformationView>(UIType.InformationPopup);
        inforamtionView?.Refresh();
    }

    private void OnSkillCoolTimeStart(string skillId, float coolDown)
    {
        SkillTableData skillData = GameDataManager.Instance.GetData<SkillTableData>(skillId);

        if (skillData == null) return;

        string keyLabel = skillData.Key.Replace("Skill", "");

        InGameView inGameView = UIManager.Instance.GetUI<InGameView>(UIType.InGameUI);
        inGameView?.StartSkillCoolDown(keyLabel, coolDown);
    }

    private void OnPlayerDead()
    {
        if (BaseDungeonController.IsInDungeon) return;

        ShowDeathAsync().Forget();
    }

    private void OnPotionUsed(string itemId, float coolTime)
    {
        InventoryView inventoryView = UIManager.Instance.GetUI<InventoryView>(UIType.InventoryPopup);
        inventoryView?.Refresh();
        inventoryView?.StartItemCoolDown(itemId, coolTime);
    }

    private void OnInformationKeyPressed()
    {
        ToggleUI(UIType.InformationPopup, ShowInformation);
    }

    private void OnInventoryKeyPressed()
    {
        ToggleUI(UIType.InventoryPopup, ShowInventory); 
    }

    private void OnEscapeKeyPressed()
    {
        if (UIManager.Instance.IsActiveUI(UIType.DungeonClearPopup) || UIManager.Instance.IsActiveUI(UIType.DungeonFailPopup)) return;

        if (UIManager.Instance.HasActivePopup())
        {
            UIManager.Instance.CloseAllPopups();
        }
        else
        {
            ShowGameMenuAsync().Forget();
        }
    }

    private void OnSkillTreeKeyPressed()
    {
        ToggleUI(UIType.SkillTreePopup, ShowSkillTree);
    }

    private void OnEquipmentKeyPressed()
    {
        ToggleUI(UIType.EquipmentPopup, ShowEquipment);
    }

    private void OnMinimapKeyPressed()
    {
        ToggleUI(UIType.MinimapPopup, ShowMinimap);
    }

    private void OnLackMana(string skillId)
    {
        SystemMessageManager.Instance.Show("Not enough mana.");
    }

    private void OnSkillCoolTimeFail(string skillId, float remainTime)
    {
        Debug.Log($"[GameFlowManager] OnSkillCoolTimeFail 호출됨: {skillId}, {remainTime}");
        SystemMessageManager.Instance.Show($"Skill is on cooldown. ({remainTime:F1}s)");
    }

    private void OnEvasionCoolTimeStarted(float duration)
    {
        InGameView inGameView = UIManager.Instance.GetUI<InGameView>(UIType.InGameUI);
        inGameView?.StartEvasionCoolDown(duration);
    }

    private void OnDungeonClearConfirmed()
    {
        UIManager.Instance.CloseUI(UIType.DungeonClearPopup);
        ReturnToVillageAsync().Forget();
    }

    private void OnDungeonFailConfirmed()
    {
        UIManager.Instance.CloseUI(UIType.DungeonFailPopup);
        ReturnToVillageAsync().Forget();
    }

    private void OnMonsterCountChanged(int current, int total)
    {
        InGameView inGameView = UIManager.Instance.GetUI<InGameView>(UIType.InGameUI);
        inGameView?.SetMonsterCount(current, total);
    }

    private void OnWaveChanged(int current, int total)
    {
        InGameView inGameView = UIManager.Instance.GetUI<InGameView>(UIType.InGameUI);
        inGameView?.SetWave(current, total);    
    }

    private void OnCountdownChanged(float remainingSeconds)
    {
        InGameView inGameView = UIManager.Instance.GetUI<InGameView>(UIType.InGameUI);
        inGameView.SetCountdown(remainingSeconds);
    }

    private void OnTargetHpChanged(int current, int max)
    {
        InGameView inGameView = UIManager.Instance.GetUI<InGameView>(UIType.InGameUI);
        inGameView.SetTargetHp(current, max);
    }

    private void OnShopTransactionCompleted()
    {
        if (UIManager.Instance.IsActiveUI(UIType.InventoryPopup))
        {
            InventoryView inventoryView = UIManager.Instance.GetUI<InventoryView>(UIType.InventoryPopup);
            inventoryView?.Refresh();
        }
    }

    private void OnMonsterMoneyDropped(int amount)
    {
        SaveManager.Instance.AddGold(_currentSlotId, amount);

        InventoryView inventoryView = UIManager.Instance.GetUI<InventoryView>(UIType.InventoryPopup);
        inventoryView?.RefreshGold();
    }

    private void OnMonsterItemDropped(ItemTableData itemData, int amount)
    {
        TransactionResult result = SaveManager.Instance.AddItem(_currentSlotId, itemData.ID, amount);

        if (result == TransactionResult.Success)
        {
            InventoryView inventoryView = UIManager.Instance.GetUI<InventoryView>(UIType.InventoryPopup);
            inventoryView?.Refresh();
        }
        else
        {
            SystemMessageManager.Instance.Show(SaveManager.GetTransactionMessage(result));
        }
    }

    private void OnVillageReturnConfirmed()
    {
        if (_pendingReturnPortal == null) return;

        ChangeMapWithLoadingAsync(_pendingReturnPortal.TargetMapId, _pendingReturnPortal.PortalType, true).Forget();
        _pendingReturnPortal = null;
    }

    private void OnItemPickup(string slotId, DroppedItem droppedItem)
    {
        if (droppedItem == null || droppedItem.ItemData == null) return;

        TransactionResult result = SaveManager.Instance.AddItem(slotId, droppedItem.ItemData.ID, droppedItem.Amount);

        if (result == TransactionResult.Success)
        {
            UnityEngine.Object.Destroy(droppedItem.gameObject);

            InventoryView inventoryView = UIManager.Instance.GetUI<InventoryView>(UIType.InventoryPopup);
            inventoryView?.Refresh();
        }
        else
        {
            SystemMessageManager.Instance.Show(SaveManager.GetTransactionMessage(result));
        }
    }

    private void RefreshGoldUI()
    {
        InventoryView inventoryView = UIManager.Instance.GetUI<InventoryView>(UIType.InventoryPopup);
        inventoryView?.RefreshGold();

        ShopView shopView = UIManager.Instance.GetUI<ShopView>(UIType.ShopUI);
        shopView?.RefreshGold();
    }


    // 요청 수행 메서드 모음
    private async UniTask ShowTitleAsync()
    {
        TitleView view = await UIManager.Instance.OpenUIAsync<TitleView>(UIType.TitleUI);

        TitleViewModel viewModel = new TitleViewModel();
        viewModel.OnGameStartRequested += OnGameStartRequested;

        view.BindViewModel(viewModel);
    }

    private async UniTask ShowCharacterSelectAsync()
    {
        CharacterSelectView view = await UIManager.Instance.OpenUIAsync<CharacterSelectView>(UIType.CharacterSelectUI, useFullScreenLoading: false);

        CharacterSelectViewModel viewModel = new CharacterSelectViewModel();
        viewModel.OnEnterGameRequested += OnEnterGameRequested;
        viewModel.OnCreateCharacterRequested += OnCreateCharacterRequested;
        viewModel.OnDeleteRequested += OnDeleteRequested;

        view.BindViewModel(viewModel);
    }

    private async UniTask ShowInGameAsync(PlayerData data)
    {
        PlayerTableData jobMaster = GameDataManager.Instance.GetData<PlayerTableData>(data.Job);
        
        if (jobMaster == null)
        {
            Debug.LogError($"[GameFlowManager] {data.Job}에 대한 직업 마스터 데이터를 찾을 수 없습니다.");
            return;
        }

        await MapManager.Instance.ChangeMapAsync("area_village");
        await PlayerSpawnManager.Instance.SpawnPlayerAsync(data, jobMaster.PrefabAddress);

        InGameView view = await UIManager.Instance.OpenUIAsync<InGameView>(UIType.InGameUI, useFullScreenLoading: true);

        view.ResetEvasionSlot();

        _inGameViewModel = new InGameViewModel(_currentSlotId);

        view.BindViewModel(_inGameViewModel);

        view.OnCharacterInfoButtonClicked -= OnInformationKeyPressed;
        view.OnCharacterInfoButtonClicked += OnInformationKeyPressed;

        view.OnEquipmentButtonClicked -= OnEquipmentKeyPressed;
        view.OnEquipmentButtonClicked += OnEquipmentKeyPressed;

        view.OnInventoryButtonClicked -= OnInventoryKeyPressed;
        view.OnInventoryButtonClicked += OnInventoryKeyPressed;

        view.OnSkillButtonClicked -= OnSkillTreeKeyPressed;
        view.OnSkillButtonClicked += OnSkillTreeKeyPressed; 

        view.OnMinimapButtonClicked -= OnMinimapKeyPressed;
        view.OnMinimapButtonClicked += OnMinimapKeyPressed;

        PlayerInputSystem.OnInformation += OnInformationKeyPressed;
        PlayerInputSystem.OnInventory += OnInventoryKeyPressed;
        PlayerInputSystem.OnSkillinfo += OnSkillTreeKeyPressed;
        PlayerInputSystem.OnEquipMent += OnEquipmentKeyPressed;
        PlayerInputSystem.OnMap += OnMinimapKeyPressed;
        Portal.OnPortalInteracted += OnPortalInteracted;
        PlayerInputSystem.OnSystem += OnEscapeKeyPressed;
        PlayerBattle.OnHpChanged += OnPlayerHpChanged;
        PlayerBattle.OnMpChanged += OnPlayerMpChanged;
        NPC.OnNPCInteracted += OnNpcInterated;
        PlayerLevel.OnLevelUp += OnPlayerLevelUp;
        SkillUtil.Instance.OnSkillDataUpdated += OnSkillDataUpdated;
        SkillUtil.OnSkillCoolTimeStart += OnSkillCoolTimeStart;
        PlayerBattle.OnPlayerDead += OnPlayerDead;
        PlayerBattle.OnPotionUsed += OnPotionUsed;
        SkillUtil.OnLackMana += OnLackMana;
        SkillUtil.OnSkillCoolTimeFail += OnSkillCoolTimeFail;
        PlayerLevel.OnExpChanged += OnPlayerExpChanged;
        PlayerInputSystem.OnEvasionCoolTimeStarted += OnEvasionCoolTimeStarted;
        BaseDungeonController.OnDungeonCleared += OnDungeonCleared;
        BaseDungeonController.OnDungeonFailed += OnDungeonFailed;
        PlayerLevel.OnLevelUp += OnPlayerLevelUp;
        RoomFieldManager.OnCurrentMonsterCountChanged += OnMonsterCountChanged;
        DefenceFieldManager.OnWaveChanged += OnWaveChanged;
        DefenceFieldManager.OnCountdownChanged += OnCountdownChanged;
        DefenceTarget.OnTargetHpChanged += OnTargetHpChanged;
        MonsterHealth.OnMonsterMoney += OnMonsterMoneyDropped;
        MonsterHealth.OnMonsterItem += OnMonsterItemDropped;
        PlayerInteraction.OnItemPickup += OnItemPickup;
    }

    private async UniTask ChangeMapAndCloseAsync(string mapId)
    {
        HideDungeonInfoIfExists();

        bool isRequiredLevel = MapManager.Instance.CheckRequiredLevelForDungeon(mapId);

        if (isRequiredLevel == false)
        {
            //[TODO] 레벨부족 메시지 띄우기
            return;
        }

        await ChangeMapWithLoadingAsync(mapId, PortalType.Village, true);
        UIManager.Instance.CloseUI(UIType.HuntingAreaSelectUI);

        PlayerBattle playerBattle = PlayerSpawnManager.Instance.GetPlayerBattle();
        playerBattle?.RestoreFull();
    }

    private async UniTask ReviveAndChangeMapAsync()
    {
        HideDungeonInfoIfExists();

        await ChangeMapWithLoadingAsync("area_village", PortalType.Village, true);

        PlayerBattle playerBattle = PlayerSpawnManager.Instance.GetPlayerBattle();
        playerBattle?.Revive();

        RefreshGoldUI();
    }

    private void HideDungeonInfoIfExists()
    {
        InGameView inGameView = UIManager.Instance.GetUI<InGameView>(UIType.InGameUI);
        inGameView?.HideDungeonInfo();
    }

    private async UniTask ShowHuntingAreaAsync()
    {
        HuntingAreaSelectView view = await UIManager.Instance.OpenUIAsync<HuntingAreaSelectView>(UIType.HuntingAreaSelectUI, useFullScreenLoading: false);

        HuntingAreaSelectViewModel viewModel = new HuntingAreaSelectViewModel(_currentSlotId);
        viewModel.OnTeleportRequested += OnTeleportRequested;

        view.BindViewModel(viewModel);
    }

    private async UniTask ShowInformationAsync()
    {
        InformationView view = await UIManager.Instance.OpenUIAsync<InformationView>(UIType.InformationPopup);

        InformationViewModel viewModel = new InformationViewModel(_currentSlotId);
        
        view.BindViewModel(viewModel);

    }

    private async UniTask ShowCharacterCreateAsync(string slotId)
    {
        CharacterCreateView view  = await UIManager.Instance.OpenUIAsync<CharacterCreateView>(UIType.CharacterCreatePopup);
        CharacterCreateViewModel viewModel = new CharacterCreateViewModel(slotId);
        viewModel.OnCharacterCreated += OnCharacterCreated;

        view.BindViewModel(viewModel);
    }

    private async UniTask ShowConfirmAsync(string message, Action onConfirmed,string confirmUISoundName)
    {
        ConfirmView view = await UIManager.Instance.OpenUIAsync<ConfirmView>(UIType.ConfirmPopup);
        ConfirmViewModel viewModel = new ConfirmViewModel(message, onConfirmed, confirmUISoundName);
        view.BindViewModel(viewModel);
    }

    private async UniTask ShowGameMenuAsync()
    {
        GameMenuView view = await UIManager.Instance.OpenUIAsync<GameMenuView>(UIType.GameMenuPopup);
        GameMenuViewModel viewModel = new GameMenuViewModel();
        viewModel.OnBackToCharacterSelectRequested += OnMenuCharacterSelectRequested;
        viewModel.OnQuitGameRequested += OnQuitGameRequested;
        viewModel.OnSettingsRequested += OnSettingsRequested;

        view.BindViewModel(viewModel);
    }

    private async UniTask ShowInventoryAsync()
    {
        InventoryView view = await UIManager.Instance.OpenUIAsync<InventoryView>(UIType.InventoryPopup);
        InventoryViewModel viewModel = new InventoryViewModel(_currentSlotId);

        view.OnSellRequested -= OnInventorySellRequested;
        view.OnSellRequested += OnInventorySellRequested;

        view.OnEquipRequested -= OnInventoryEquipRequested;
        view.OnEquipRequested += OnInventoryEquipRequested;

        view.OnUseRequested -= OnInventoryUseRequested;
        view.OnUseRequested += OnInventoryUseRequested;

        view.BindViewModel(viewModel);
    }

    private async UniTask ShowShopAsync()
    {
        ShopView view = await UIManager.Instance.OpenUIAsync<ShopView>(UIType.ShopUI);
        ShopViewModel viewModel = new ShopViewModel(_currentSlotId);
        viewModel.OnGoldChanged -= OnShopTransactionCompleted;
        viewModel.OnGoldChanged += OnShopTransactionCompleted;
        view.BindViewModel(viewModel);
    }

    private async UniTask ShowSkillTreeAsync()
    {
        SkillTreeView view = await UIManager.Instance.OpenUIAsync<SkillTreeView>(UIType.SkillTreePopup);
        SkillTreeViewModel viewModel = new SkillTreeViewModel(_currentSlotId);
        view.BindViewModel(viewModel);
    }

    private async UniTask ShowEquipmentAsync()
    {
        EquipmentView view = await UIManager.Instance.OpenUIAsync<EquipmentView>(UIType.EquipmentPopup);
        EquipmentViewModel viewModel = new EquipmentViewModel(_currentSlotId);

        view.OnUnequipRequested -= OnEquipmentUnequipRequested;
        view.OnUnequipRequested += OnEquipmentUnequipRequested;

        view.BindViewModel(viewModel);
    }

    private async UniTask ShowMinimapAsync()
    {
        await UIManager.Instance.OpenUIAsync<MinimapView>(UIType.MinimapPopup);
    }

    private async UniTask ShowDeathAsync()
    {
        DeathView view = await UIManager.Instance.OpenUIAsync<DeathView>(UIType.DeathPopup);
        view.Setup(OnReviveRequested);
    }

    private async UniTask ShowSettingAsync()
    {
        SettingView view = await UIManager.Instance.OpenUIAsync<SettingView>(UIType.SettingPopup);
        SettingsViewModel viewModel = new SettingsViewModel();
        view.BindViewModel(viewModel);
    }

    private async UniTask ShowDungeonClearAsync(DungeonReward reward)
    {
        DungeonClearView view = await UIManager.Instance.OpenUIAsync<DungeonClearView>(UIType.DungeonClearPopup);
        view.Setup(reward.Gold, reward.ItemIds, OnDungeonClearConfirmed);
    }

    private async UniTask ShowDungeonFailAsync(DungeonFailReason reason)
    {
        DungeonFailView view = await UIManager.Instance.OpenUIAsync<DungeonFailView>(UIType.DungeonFailPopup);
        view.Setup(OnDungeonFailConfirmed);
    }

    private async UniTask ReturnToVillageAsync()
    {
        HideDungeonInfoIfExists();

        await ChangeMapWithLoadingAsync("area_village", PortalType.Village, true);

        PlayerBattle playerBattle = PlayerSpawnManager.Instance.GetPlayerBattle();

        if (playerBattle != null)
        {
            playerBattle.Revive();
            playerBattle.RestoreFull();

            PlayerSpawnManager.Instance.MoveToSpawnPoint(playerBattle.gameObject);
        }

        RefreshGoldUI();
    }

    private async UniTask ChangeMapWithLoadingAsync(string mapId, PortalType entryPortalType, bool useFullScreenLoading)
    {
        UniTask loadingTask = UIManager.Instance.ShowLoadingAsync(useFullScreenLoading);
        UniTask minDisplayTask = UniTask.Delay(500);

        await loadingTask;

        UniTask changeMapTask = MapManager.Instance.ChangeMapAsync(mapId, entryPortalType);

        await UniTask.WhenAll(changeMapTask, minDisplayTask);

        UIManager.Instance.HideLoading();
    }


    // UI 토글화
    private void ToggleUI(UIType uiType, Action showAction)
    {
        if (UIManager.Instance.IsActiveUI(uiType))
        {
            UIManager.Instance.CloseUI(uiType);
        }
        else
        {
            showAction();
        }
    }

    private void ShowInformation()
    {
        ShowInformationAsync().Forget();
    }

    private void ShowInventory()
    {
        ShowInventoryAsync().Forget();
    }

    private void ShowShop()
    {
        ShowShopAsync().Forget();
    }

    private void ShowSkillTree()
    {
        ShowSkillTreeAsync().Forget();
    }

    private void ShowEquipment()
    {
        ShowEquipmentAsync().Forget();
    }

    private void ShowMinimap()
    {
        ShowMinimapAsync().Forget();
    }

    private void OnDungeonCleared(DungeonReward reward)
    {
        HideDungeonInfoIfExists();
        ShowDungeonClearAsync(reward).Forget();
    }

    private void OnDungeonFailed(DungeonFailReason reason)
    {
        HideDungeonInfoIfExists();
        ShowDungeonFailAsync(reason).Forget();
    }

}
