using UnityEngine;

public class PlayerSound : MonoBehaviour
{

    public void PlayerFootStepSound(string footSide)
    {
        string currentMapTag = MapManager.Instance.GetCurrentMapTag();
        string soundFileName = $"FootStep_{currentMapTag}_{footSide}";

        SoundManager.Instance.PlaySFXSound(soundFileName, 1f, true);
    }

    public void PlayerAttackSound()
    {
        SoundManager.Instance.PlaySFXSound("Player_Attack_Sound", 1f, true);
    }

    public void PlayerEvasionSound()
    {
        SoundManager.Instance.PlaySFXSound("Player_Evasion_Sound", 1f, true);
    }
    
    public void PlayerLevelUpSound()
    {
        SoundManager.Instance.PlaySFXSound("Player_Level_Up_Sound", 1f, false);
    }

    public void PlayerReviveSound()
    {
        SoundManager.Instance.PlaySFXSound("Player_Revive_Sound", 1f, false);
    }

    public void PlayerSkillVoiceSound()
    {
        SoundManager.Instance.PlaySFXSound("Player_Skill_Voice_Sound", 1f, true);
    }
}
