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
        m_DraggedUnit = unit;
    }

    public void Deselect()
    {
        m_DraggedUnit.ShowMovementRange(false);
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
            int orthMoves = m_UnitDefinition.OrthogonalMoves;
            int diagMoves = m_UnitDefinition.DiagonalMoves;

            if (orthMoves > 0)
            {
                m_Tile.HighlightNeighbour((int)Direction.Ortohonal1, enable, orthMoves, false);
                m_Tile.HighlightNeighbour((int)Direction.Ortohonal2, enable, orthMoves, false);
                m_Tile.HighlightNeighbour((int)Direction.Ortohonal3, enable, orthMoves, false);
                m_Tile.HighlightNeighbour((int)Direction.Ortohonal4, enable, orthMoves, false);
                m_Tile.HighlightNeighbour((int)Direction.Ortohonal5, enable, orthMoves, false);
                m_Tile.HighlightNeighbour((int)Direction.Ortohonal6, enable, orthMoves, false);
            }

            if (diagMoves > 0)
            {
                m_Tile.HighlightNeighbour((int)Direction.Diagonal1, enable, diagMoves, false);
                m_Tile.HighlightNeighbour((int)Direction.Diagonal2, enable, diagMoves, false);
                m_Tile.HighlightNeighbour((int)Direction.Diagonal3, enable, diagMoves, false);
                m_Tile.HighlightNeighbour((int)Direction.Diagonal4, enable, diagMoves, false);
                m_Tile.HighlightNeighbour((int)Direction.Diagonal5, enable, diagMoves, false);
                m_Tile.HighlightNeighbour((int)Direction.Diagonal6, enable, diagMoves, false);
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
    }

    //IBeginDragHandler
    public void OnBeginDrag(PointerEventData eventData)
    {
        //Loosen from field so we always render on top!
        transform.SetParent(transform.parent.transform.parent);

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
        Select(null);
        GetComponent<CanvasGroup>().blocksRaycasts = true;

        transform.localScale = new Vector3(1.0f, 1.0f, 1.0f); //For some reason this resets
    }

}
