using TMPro;
using UnityEngine;

public class InformationView : BaseOverLayUI
{
    [SerializeField] private TMP_Text Text_Name;
    [SerializeField] private TMP_Text Text_Job;
    [SerializeField] private TMP_Text Text_Level;
    [SerializeField] private TMP_Text Text_Hp;
    [SerializeField] private TMP_Text Text_Mp;
    [SerializeField] private TMP_Text Text_Atk;
    [SerializeField] private TMP_Text Text_Def;

    private InformationViewModel _viewModel;

    public void BindViewModel(InformationViewModel viewModel)
    {
        _viewModel = viewModel;

        Refresh();
    }

    public void Refresh()
    {
        CharacterSaveData data = _viewModel.GetSaveData();

        if (data == null) 
        {
            Debug.LogError("[Information] 플레이어 데이터를 찾을 수 없습니다.");
            return;
        }

        Text_Name.text = data.Name;
        Text_Job.text = data.Job;
        Text_Level.text = $"Lv. {_viewModel.GetLevel(data.Exp)}";
        Text_Hp.text = $"HP {data.Hp}";
        Text_Mp.text = $"MP {data.Mp}";
        Text_Atk.text = $"ATK {_viewModel.GetFinalAtk()}";
        Text_Def.text = $"DEF {_viewModel.GetFinalDef()}";
    }
}
