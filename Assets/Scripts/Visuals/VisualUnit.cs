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
    private PlayerType m_PlayerType = PlayerType.White;

    public void Select(VisualUnit unit)
    {
        if (m_DraggedUnit != null) { m_DraggedUnit.Deselect(); }

        transform.localScale = new Vector3(2.0f, 2.0f, 2.0f);
        m_DraggedUnit = unit;
    }

    public void Deselect()
    {
        m_DraggedUnit.ShowMovementRange(false);
        transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
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

    public void SetPlayerType(PlayerType playerType)
    {
        m_PlayerType = playerType;

        Color color = Color.white;
        if (playerType == PlayerType.Black)
        {
            color = Color.black;
        }

        gameObject.GetComponent<Image>().color = color;
    }

    public PlayerType GetPlayerType()
    {
        return m_PlayerType;
    }

    public void SetUnitDefinition(UnitDefinition unitDefinition)
    {
        m_UnitDefinition = unitDefinition;
    }

    private void ShowMovementRange(bool enable)
    {
        //Show movement range
        if (m_Tile != null && m_UnitDefinition != null)
        {
            for (int i = 0; i < BoardState.DIR_NUM; ++i)
            {
                m_Tile.HighlightNeighbour(i, enable, m_UnitDefinition.GetMoveCount(i, m_PlayerType), false, m_PlayerType);
            }
        }
    }

    public void EnableDragging(bool state)
    {
        gameObject.GetComponent<CanvasGroup>().blocksRaycasts = state;
    }

    //----------------
    // INTERFACES
    //----------------

    //IPointerDownHandler
    public void OnPointerDown(PointerEventData eventData)
    {
        Select(this);
        ShowMovementRange(true);

        //Loosen from field so we always render on top!
        transform.SetParent(transform.parent.transform.parent);
    }

    //IBeginDragHandler
    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log("Begin Drag!");
        //Loosen from field so we always render on top!
        //transform.SetParent(transform.parent.transform.parent);

        GetComponent<CanvasGroup>().blocksRaycasts = false;
    }

    //IDragHandler
    public void OnDrag(PointerEventData eventData)
    {
        transform.position = Input.mousePosition;
    }

    //IEndDragHandler
    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log("End Drag!");
        Select(null);
        GetComponent<CanvasGroup>().blocksRaycasts = true;

        transform.localScale = new Vector3(1.0f, 1.0f, 1.0f); //For some reason this resets
    }

}
