using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum Direction
{
    Ortohonal1,
    Diagonal1,

    Ortohonal2,
    Diagonal2,

    Ortohonal3,
    Diagonal3,

    Ortohonal4,
    Diagonal4,

    Ortohonal5,
    Diagonal5,

    Ortohonal6,
    Diagonal6,
}

public class VisualTile : MonoBehaviour, IDropHandler
{
    //This class just visually represents the Unit & the player can interact with it.
    //It does in no way, shape or form hold any game data.
    private VisualTile[] m_Neighbours;
    private Color m_PreviousColor;

    private void Awake()
    {
        m_Neighbours = new VisualTile[12];
    }

    public void OnDrop(PointerEventData eventData)
    {
        //Can be null when you just click
        if (VisualUnit.m_DraggedUnit != null)
        {
            VisualUnit.m_DraggedUnit.SetTile(this);
        }
    }

    public void SetNeightbour(int id, VisualTile tile)
    {
        if (id >= m_Neighbours.Length) return;
        m_Neighbours[id] = tile;
    }

    public VisualTile GetNeighbour(int id)
    {
        if (id >= m_Neighbours.Length) return null;
        return m_Neighbours[id];
    }



    public void HighlightNeighbour(int id, int movesLeft, bool ignoreUnits, bool recursiveCall = false)
    {
        SetColor(Color.red);

        //Only do certain checks if this is not the first call
        if (recursiveCall == true)
        {
            if (!ignoreUnits && transform.childCount != 0) return;
            movesLeft -= 1;
        }

        if (m_Neighbours[id] != null && movesLeft > 0)
        {
            m_Neighbours[id].HighlightNeighbour(id, movesLeft, ignoreUnits, true);
        }
    }

    public void SetColor(Color color)
    {
        if (m_PreviousColor.a == 0.0f) //empty color
            m_PreviousColor = color;
        else
            m_PreviousColor = gameObject.GetComponent<Image>().color;

        gameObject.GetComponent<Image>().color = color;
    }

    public void ResetColor()
    {
        gameObject.GetComponent<Image>().color = m_PreviousColor;
    }
}
