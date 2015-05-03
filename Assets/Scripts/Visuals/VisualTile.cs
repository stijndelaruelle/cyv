﻿using UnityEngine;
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

public class VisualTile : MonoBehaviour, IPointerDownHandler, IDropHandler
{
    //This class just visually represents the Unit & the player can interact with it.
    //It does in no way, shape or form hold any game data.
    private VisualTile[] m_Neighbours;
    private Color m_OriginalColor = Color.magenta; //so funny

    private bool m_IsHighlightingMovement = false;
    private bool m_IsHighlighted = false;

    public static VisualTile m_FromTile = null;
    public static VisualTile m_ToTile = null;

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

    public bool HasNeightbourWithUnit(UnitType unitType, bool orthogonal, bool diagonal)
    {
        for (int i = 0; i < m_Neighbours.Length; ++i)
        {
            if (m_Neighbours[i] == null)
                continue;

            //Diagonal
            if (i % 2 == 0 && !diagonal)
                continue;

            //Orthogonal
            if (i % 2 != 0 && !orthogonal)
                continue;

            VisualUnit visualUnit = m_Neighbours[i].GetVisualUnit();
            if (visualUnit != null &&
                visualUnit.GetUnitDefinition().UnitType == unitType)
                return true;
        }

        return false;
    }

    public void HighlightMovement(bool enable, bool from)
    {
        m_IsHighlightingMovement = enable;

        Color color = Color.magenta;
        if (enable)
        {
            if (from)
            {
                //There can only be one!
                if (m_FromTile != null && m_ToTile != m_FromTile) { m_FromTile.HighlightMovement(false, from); }

                color = new Color(1.0f, 0.0f, 1.0f, 0.75f);
                m_FromTile = this;
            }
            else
            {
                //There can only be one!
                if (m_ToTile != null && m_ToTile != m_FromTile) { m_ToTile.HighlightMovement(false, from); }

                color = new Color(1.0f, 1.0f, 0.0f, 0.75f);
                m_ToTile = this;
            }
        }
        else
        {
            //Don't reset the from & to tiles
            color = m_OriginalColor;
        }

        SetColor(color);
    }

    public void Highlight(bool enable)
    {
        //These are the circles indicating the movement range
        m_IsHighlighted = enable;

        if (m_IsHighlightingMovement)
            return;

        Color color = new Color(1.0f, 0.0f, 0.0f, 0.75f);
        if (!enable) { color = m_OriginalColor; }
        SetColor(color);
    }

    public void HighlightNeighbour(int dir, bool enable, int movesLeft, bool canJump, bool mustJump, PlayerColor playerColor, bool recursiveCall = false)
    {
        bool hightLight = enable;

        //Only do certain checks if this is not the first call
        if (recursiveCall == true)
        {
            movesLeft -= 1;

            if (GetVisualUnit() != null)
            {
                VisualUnit visualUnit = GetVisualUnit();

                //This is a unit of the same type, don't go here
                if (visualUnit != null && visualUnit.GetPlayerColor() == playerColor)
                {
                    hightLight = false;
                }

                //This unit is a mountain, don't go here
                if (visualUnit != null && visualUnit.GetUnitDefinition().UnitType == UnitType.Mountain)
                {
                    hightLight = false;
                }

                //Jump!
                if (canJump)
                {
                    movesLeft = 1; //We hop over that unit
                    canJump = false; //No more than 1 jump
                }
                else
                {
                    Highlight(hightLight);
                    return;
                }
            }

            //If we MUST jump, and we haven't jumped yet then this tile is not valid.
            if (mustJump && canJump)
            {
                hightLight = false;
            }
        }

        Highlight(hightLight);

        if (m_Neighbours[dir] != null && movesLeft > 0)
        {
            m_Neighbours[dir].HighlightNeighbour(dir, enable, movesLeft, canJump, mustJump, playerColor, true);
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

    //IPointerDownHandler
    public void OnPointerDown(PointerEventData eventData)
    {
        if (VisualUnit.m_DraggedUnit != null)
        {
            //This means the user used the click & click method to place units
            //Debug.Log("OnPointerDown VisualTile");

            PlaceUnit();
            VisualUnit.m_DraggedUnit.OnEndDrag(eventData);   
        }
    }

    //IDropHandler
    public void OnDrop(PointerEventData eventData)
    { 
        //Can be null when you just click
        if (VisualUnit.m_DraggedUnit != null && VisualUnit.m_DraggedUnit.IsDragged == true)
        {
            //Debug.Log("OnDrop");
            PlaceUnit();
        }
    }

    private void PlaceUnit()
    {
        if (VisualUnit.m_DraggedUnit != null &&
            GameplayManager.Instance.CurrentPlayer == VisualUnit.m_DraggedUnit.GetPlayerColor())
        {
            //Debug.Log("Place Unit!");

            //If we're not highlighted, disallow the player to place a unit here
            //And force it back to where it came from!
            if (!m_IsHighlighted)
            {
                VisualUnit.m_DraggedUnit.SetTile(VisualUnit.m_DraggedUnit.GetTile());
            }
            else
            {
                //Only count as a "move" if we moved to a different tile
                if (this == VisualUnit.m_DraggedUnit.GetTile())
                {
                    VisualUnit.m_DraggedUnit.SetTile(this);
                    return;
                }

                //If we already have a child (which means we already hold a unit), make that move to the corresponding reserve tile
                if (GetVisualUnit() != null)
                {
                    VisualUnit currentUnit = transform.GetChild(0).GetComponent<VisualUnit>();
                    if (currentUnit != null) { currentUnit.SetTile(null); }
                }

                //Only count as a "move" if we moved to a different tile (and not in setup phase of course)
                if (GameplayManager.Instance.GameState != GameState.Setup &&
                    this != VisualUnit.m_DraggedUnit.GetTile())
                {
                    VisualUnit.m_DraggedUnit.SetTile(this);
                    GameplayManager.Instance.SubmitMove();
                }
                else
                {
                    VisualUnit.m_DraggedUnit.SetTile(this);
                }
            }
        }
    }

    private VisualUnit GetVisualUnit()
    {
        if (transform.childCount > 0)
            return transform.GetChild(0).GetComponent<VisualUnit>();
        else
            return null;
    }
}
