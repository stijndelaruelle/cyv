using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum Direction
{
    Diagonal1,  //0 
    Ortohonal1, //1

    Diagonal2,  //2
    Ortohonal2,

    Diagonal3,
    Ortohonal3,

    Diagonal4,
    Ortohonal4,

    Diagonal5,
    Ortohonal5,

    Diagonal6,
    Ortohonal6,
}

public class VisualTile : MonoBehaviour, IDropHandler
{
    //This class just visually represents the Unit & the player can interact with it.
    //It does in no way, shape or form hold any game data.
    private VisualTile[] m_Neighbours;
    private Color m_OriginalColor = Color.magenta; //so funny

    private bool m_IsHighlighted = false;

    private int m_ID;
    public int ID
    {
        get { return m_ID; }
    }

    private void Awake()
    {
        m_Neighbours = new VisualTile[BoardState.DIR_NUM];
    }

    public void SetID(int id)
    {
        m_ID = id;
    }

    public void SetNeightbour(int dir, VisualTile tile)
    {
        if (dir >= m_Neighbours.Length) return;
        m_Neighbours[dir] = tile;
    }

    public VisualTile GetNeighbour(int dir)
    {
        if (dir >= m_Neighbours.Length) return null;
        return m_Neighbours[dir];
    }

    public void HighlightNeighbour(int dir, bool enable, int movesLeft, bool ignoreUnits, bool recursiveCall = false)
    {
        m_IsHighlighted = enable;

        Color color = new Color(1.0f, 0.0f, 0.0f, 0.75f);
        if (!enable) { color = m_OriginalColor; }
        SetColor(color);

        //Only do certain checks if this is not the first call
        if (recursiveCall == true)
        {
            if (!ignoreUnits && transform.childCount != 0) return;
            movesLeft -= 1;
        }

        if (m_Neighbours[dir] != null && movesLeft > 0)
        {
            m_Neighbours[dir].HighlightNeighbour(dir, enable, movesLeft, ignoreUnits, true);
        }
    }

    public void SetColor(Color color)
    {
        if (m_OriginalColor == Color.magenta)
        {
            m_OriginalColor = color; 
        }

        gameObject.GetComponent<Image>().color = color;
    }

    //----------------
    // INTERFACES
    //----------------

    //IDropHandler
    public void OnDrop(PointerEventData eventData)
    {
        //Can be null when you just click
        if (VisualUnit.m_DraggedUnit != null)
        {
            //If we're not highlighted, disallow the player to place a unit here
            //And force it back to where it came from!
            if (!m_IsHighlighted)
            {
                VisualUnit.m_DraggedUnit.SetTile(VisualUnit.m_DraggedUnit.GetTile());
            }
            else
            {
                //If we already have a child (which means we already hold a unit), make that move to the corresponding reserve tile
                if (transform.childCount > 0)
                {
                    VisualUnit currentUnit = transform.GetChild(0).GetComponent<VisualUnit>();
                    if (currentUnit != null) { currentUnit.SetTile(null); }
                }

                VisualUnit.m_DraggedUnit.SetTile(this);
                GameplayManager.Instance.Move();
            }
        }
    }
}
