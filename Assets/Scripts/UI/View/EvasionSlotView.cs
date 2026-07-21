using UnityEngine;
using UnityEngine.UI;

public class EvasionSlotView : MonoBehaviour
{
    [SerializeField] private Image Image_CoolDownOverlay;

    private float _coolDownRemaining;
    private float _coolDownDuration;

    private void Awake()
    {
        gameObject.SetActive(false);
    }

    private void Update()
    {
        if (_coolDownRemaining <= 0f) return;

        _coolDownRemaining -= Time.deltaTime;
        Image_CoolDownOverlay.fillAmount = Mathf.Clamp01(_coolDownRemaining / _coolDownDuration);

        if (_coolDownRemaining <= 0f)
        {
            gameObject.SetActive(false);
        }
    }

    public void StartCoolDown(float duration)
    {
        if (duration <= 0f) return;

        _coolDownDuration = duration;
        _coolDownRemaining = duration;

        gameObject.SetActive(true);
        Image_CoolDownOverlay.fillAmount = 1f;
    }
}
