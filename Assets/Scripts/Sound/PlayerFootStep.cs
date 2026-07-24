using UnityEngine;

public class PlayerFootStep : MonoBehaviour
{

    public void PlayerFootStepSound(string footSide)
    {
        string currentMapTag = MapManager.Instance.GetCurrentMapTag();
        string soundFileName = $"FootStep_{currentMapTag}_{footSide}";

        SoundManager.Instance.PlaySFXSound(soundFileName, 1f, true);
    }

}
