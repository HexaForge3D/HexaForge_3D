using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectView : BaseUI       
{
    [SerializeField] private Transform Transform_SlotParent;
    [SerializeField] private GameObject Prefab_CharacterSlot;

    [SerializeField] private Image Image_CharacterPreview;
    [SerializeField] private Button Button_EnterGame;

    private CharacterSelectViewModel _viewModel;
    private readonly List<GameObject> _spawnedSlots = new List<GameObject>();

    public void BindViewModel(CharacterSelectViewModel viewModel)
    {
        _viewModel = viewModel;

        viewModel.OnCharacterSelected += OnCharacterSeleted;

        Button_EnterGame.onClick.RemoveAllListeners();
        Button_EnterGame.onClick.AddListener(OnClickEnterGame);

        BuildSlotList();
    }

    private void BuildSlotList()
    {
        ClearSlotList();

        List<PlayerData> characters = _viewModel.GetSelecttableCharacters();

        foreach (PlayerData data in characters)
        {
            GameObject slotObject = Instantiate(Prefab_CharacterSlot, Transform_SlotParent);
            CharacterSlotView slotView = slotObject.GetComponent<CharacterSlotView>();
            slotView.Setup(data, OnSlotClicked);

            _spawnedSlots.Add(slotObject);
        }
    }
    
    private void ClearSlotList()
    {
        foreach (GameObject slot in _spawnedSlots)
        {
            Destroy(slot);
        }

        _spawnedSlots.Clear();  
    }

    private void OnSlotClicked(PlayerData data)
    {
        _viewModel.SelectCharacter(data);
        Debug.Log("선택됨");
    }

    private void OnCharacterSeleted(PlayerData data)
    {

    }

    private void OnClickEnterGame()
    {
        _viewModel?.RequestEnterGame();
    }
}
