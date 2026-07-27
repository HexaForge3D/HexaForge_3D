using UnityEngine;
using UnityEngine.UI;

public class CursorManager : BaseMonoManager<CursorManager>
{
    [SerializeField] private Image CursorImage;
    [SerializeField] private Sprite DefaultCursorSprite;
    [SerializeField] private Sprite ClickCursorSprite;
    [SerializeField] private Vector2 CursorOffset = Vector2.zero; 

    
    protected override void Awake()
    {
        base.Awake();
        Cursor.visible = false;
        CursorImage.sprite = DefaultCursorSprite;
    }

    private void Update()
    {
        CursorImage.rectTransform.position = (Vector2)Input.mousePosition + CursorOffset;

        if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
        {
            CursorImage.sprite = ClickCursorSprite;
        }
        else if (Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(1))
        {
            CursorImage.sprite = DefaultCursorSprite;
        }
    }
}