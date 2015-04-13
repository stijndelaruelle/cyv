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

    public void OnPointerDown(PointerEventData eventData)
    {
        //Show movement range
        if (m_Tile != null)
        {
            //if (m_UnitData.UnitDefinition.OrthogonalMoves > 0)
            //{
            //    m_Tile.HighlightNeighbour((int)Direction.Ortohonal1, 1, false);
            //    m_Tile.HighlightNeighbour((int)Direction.Ortohonal2, 1, false);
            //    m_Tile.HighlightNeighbour((int)Direction.Ortohonal3, 1, false);
            //    m_Tile.HighlightNeighbour((int)Direction.Ortohonal4, 1, false);
            //    m_Tile.HighlightNeighbour((int)Direction.Ortohonal5, 1, false);
            //    m_Tile.HighlightNeighbour((int)Direction.Ortohonal6, 1, false);
            //}

            //if (m_UnitData.UnitDefinition.DiagonalMoves > 0)
            //{
            //    m_Tile.HighlightNeighbour((int)Direction.Diagonal1, 1, false);
            //    m_Tile.HighlightNeighbour((int)Direction.Diagonal2, 1, false);
            //    m_Tile.HighlightNeighbour((int)Direction.Diagonal3, 1, false);
            //    m_Tile.HighlightNeighbour((int)Direction.Diagonal4, 1, false);
            //    m_Tile.HighlightNeighbour((int)Direction.Diagonal5, 1, false);
            //    m_Tile.HighlightNeighbour((int)Direction.Diagonal6, 1, false);
            //}
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        m_DraggedUnit = this;

        //Loosen from field so we always render on top!
        transform.SetParent(transform.parent.transform.parent);

        GetComponent<CanvasGroup>().blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        m_DraggedUnit = null;
        GetComponent<CanvasGroup>().blocksRaycasts = true;

        transform.localScale = new Vector3(1.0f, 1.0f, 1.0f); //For some reason this resets
    }

    public void SetTile(VisualTile tile)
    {
        m_Tile = tile;
        transform.SetParent(tile.gameObject.transform);
    }

    public void SetColor(Color color)
    {
        gameObject.GetComponent<Image>().color = color;
    }
}
