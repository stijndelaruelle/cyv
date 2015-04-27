using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class VisualUnit : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    //This class just visually represents the Unit & the player can interact with it.
    //It does in no way, shape or form hold any game data.
    public static VisualUnit m_DraggedUnit = null;
    private VisualTile m_Tile = null;
    private VisualTile m_ReserveTile = null; //The tile we need to go to when we die

    private UnitDefinition m_UnitDefinition = null; //Used for movement ranges etc.
    private PlayerColor m_PlayerColor = PlayerColor.White;

    private Transform m_ParentTransform;
    private bool m_IsDragged = false; //Used to distinguish between click movement & drag movement
    public bool IsDragged
    {
        get { return m_IsDragged; }
        set { m_IsDragged = value; }
    }

    public void Select(VisualUnit unit)
    {
        if (m_DraggedUnit != null) { m_DraggedUnit.Deselect(); }
        m_DraggedUnit = unit;

        if (unit != null)
        {
            transform.localScale = new Vector3(2.0f, 2.0f, 2.0f);
        }
    }

    public void Deselect()
    {
        m_DraggedUnit.ShowMovementRange(false);
        transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);

        if (GameplayManager.Instance.GameState == GameState.Setup ||
            GameplayManager.Instance.CurrentPlayer == m_PlayerColor)
        {
            EnableDragging(true);
        }

        //We placed the unit out of the valid fields
        if (transform.parent == m_ParentTransform)
        {
            SetTile(m_Tile);
        }
    }

    public void SetTile(VisualTile tile)
    {
        ShowMovementRange(false);

        if (tile == null) { tile = m_ReserveTile; }
        m_Tile = tile;

        transform.SetParent(tile.gameObject.transform);
        transform.localScale = new Vector3(1.0f, 1.0f, 1.0f); //For some reason this resets
    }

    public void SetReserveTile(VisualTile reserveTile)
    {
        m_ReserveTile = reserveTile;
    }

    public VisualTile GetTile()
    {
        return m_Tile;
    }

    public void SetPlayerColor(PlayerColor playerColor)
    {
        m_PlayerColor = playerColor;

        Color color = Color.white;
        if (playerColor == PlayerColor.Black)
        {
            color = Color.black;
        }

        gameObject.GetComponent<Image>().color = color;
    }

    public PlayerColor GetPlayerColor()
    {
        return m_PlayerColor;
    }

    public void SetUnitDefinition(UnitDefinition unitDefinition)
    {
        m_UnitDefinition = unitDefinition;
    }

    public void SetParentTransform(Transform transform)
    {
        m_ParentTransform = transform;
    }

    private void ShowMovementRange(bool enable)
    {
        //Very special case, you can always go to your entire half of the board
        if (GameplayManager.Instance.GameState == GameState.Setup)
        {
            GameplayManager.Instance.HighlightSetupZone(enable);
            return;
        }

        //Show movement range
        if (m_Tile != null && m_UnitDefinition != null)
        {
            for (int i = 0; i < BoardState.DIR_NUM; ++i)
            {
                m_Tile.HighlightNeighbour(i, enable, m_UnitDefinition.GetMoveCount(i, m_PlayerColor), false, m_PlayerColor);
            }
        }
    }

    public void EnableDragging(bool state)
    {
        gameObject.GetComponent<CanvasGroup>().blocksRaycasts = state;
    }

    public void Show(bool state)
    {
        gameObject.SetActive(state);
    }

    //----------------
    // INTERFACES
    //----------------

    //IPointerDownHandler
    public void OnPointerDown(PointerEventData eventData)
    {
        //This means the user used the click & click method to place units
        //Debug.Log("OnPointerDown VisualUnit");
        OnBeginDrag(eventData);
        m_IsDragged = false;
    }

    //IBeginDragHandler
    public void OnBeginDrag(PointerEventData eventData)
    {
        //Debug.Log("Begin Drag!");
        Select(this);

        //Loosen from field so we always render on top!
        transform.SetParent(m_ParentTransform);
        ShowMovementRange(true);

        EnableDragging(false);
        m_IsDragged = true;
    }

    //IDragHandler
    public void OnDrag(PointerEventData eventData)
    {
        transform.position = Input.mousePosition;
    }

    //IEndDragHandler
    public void OnEndDrag(PointerEventData eventData)
    {
        //Debug.Log("End Drag!");
        Select(null);
    }

}
