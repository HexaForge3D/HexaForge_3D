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

        viewModel.OnSlotSelected += OnSlotSeleted;

        Button_EnterGame.onClick.RemoveAllListeners();
        Button_EnterGame.onClick.AddListener(OnClickEnterGame);

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

    private void OnSlotClicked(CharacterSlotData slot)
    {
        _viewModel.SelectSlot(slot);
    }

    private void OnSlotSeleted(CharacterSlotData slot)
    {

    }

    private void OnClickEnterGame()
    {
        _viewModel?.RequestEnterGame();
    }
}
