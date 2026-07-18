using UnityEngine;
using UnityEngine.EventSystems;

public class WindowDragHandle : MonoBehaviour, IDragHandler
{
    [SerializeField] private RectTransform TargetRectTransform;

    private void Awake()
    {
        if (TargetRectTransform == null)
        {
            TargetRectTransform = transform.parent as RectTransform;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        TargetRectTransform.anchoredPosition += eventData.delta;
    }
}