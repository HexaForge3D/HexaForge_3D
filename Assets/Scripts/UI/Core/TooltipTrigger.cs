using UnityEngine;
using UnityEngine.EventSystems;

public class TooltipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private TooltipData _data;
    private bool _isHovering;

    private void Update()
    {
        if (_isHovering)
        {
            TooltipManager.Instance.UpdatePosition(Input.mousePosition);
        }
    }

    public void SetData(TooltipData data)
    {
        _data = data;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_data == null) return;

        _isHovering = true;
        TooltipManager.Instance.Show(_data, eventData.position);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _isHovering = false;
        TooltipManager.Instance.Hide();
    }


}
