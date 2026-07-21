using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectView : BaseUI       
{
    [SerializeField] private Transform Transform_SlotParent;
    [SerializeField] private GameObject Prefab_CharacterSlot;

    [SerializeField] private Image Image_CharacterPreview;
    [SerializeField] private Button Button_EnterGame;
    [SerializeField] private Button Button_Delete;

    private CharacterSelectViewModel _viewModel;
    private readonly List<CharacterSlotView> _spawnedSlots = new List<CharacterSlotView>();

    public void BindViewModel(CharacterSelectViewModel viewModel)
    {
        _viewModel = viewModel;

        viewModel.OnSlotSelected += OnSlotSeleted;

        Button_EnterGame.onClick.RemoveAllListeners();
        Button_EnterGame.onClick.AddListener(OnClickEnterGame);

        Button_Delete.onClick.RemoveAllListeners();
        Button_Delete.onClick.AddListener(OnClickDelete);
        Button_Delete.interactable = false;

        BuildSlotList();
    }

    private void BuildSlotList()
    {
        ClearSlotList();

        List<CharacterSlotData> slots = _viewModel.GetAllSlots();

        foreach (CharacterSlotData slot in slots)
        {
            GameObject slotObject = Instantiate(Prefab_CharacterSlot, Transform_SlotParent);
            CharacterSlotView slotView = slotObject.GetComponent<CharacterSlotView>();
            slotView.Setup(slot, OnSlotClicked);

            _spawnedSlots.Add(slotView);
        }
    }
    
    private void ClearSlotList()
    {
        foreach (CharacterSlotView slotView in _spawnedSlots)
        {
            Destroy(slotView.gameObject);
        }

        _spawnedSlots.Clear();  
    }

    private void OnSlotClicked(CharacterSlotData slot, CharacterSlotView clickedSlot)
    {
        _viewModel.SelectSlot(slot);

        if (slot.IsEmpty == false)
        {
            foreach (CharacterSlotView slotView in _spawnedSlots)
            {
                slotView.SetSelected(slotView == clickedSlot);
            }
        }
    }

    private void OnSlotSeleted(CharacterSlotData slot)
    {
        Button_Delete.interactable = true;
    }

    private void OnClickEnterGame()
    {
        _viewModel?.RequestEnterGame();
    }

    private void OnClickDelete()
    {
        _viewModel?.RequestDelete();
    }
}
