using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Tile
{
    //Bolow a diagram of how the neighbour ID's work
    //   5  /\ 0
    //   4 |  | 1
    //   3  \/ 2
    private Tile[] m_Neighbours;
    private Unit m_Unit;

    private int m_ID;
    public int ID
    {
        get { return m_ID; }
    }

    public Tile(int ID)
    {
        m_Neighbours = new Tile[BoardState.DIR_NUM];
        m_Unit = null;
        m_ID = ID;
    }

    public void SetUnit(Unit unit)
    {
        //Remove our old unit
        if (m_Unit != null && unit != null)
        {
            m_Unit.SetTile(null);
        }

        //Accept the new
        m_Unit = unit;
    }

    public Unit GetUnit()
    {
        return m_Unit;
    }

    public void SetNeightbour(int id, Tile tile)
    {
        if (id >= m_Neighbours.Length) return;
        m_Neighbours[id] = tile;
    }

    public Tile GetNeighbour(int id)
    {
        if (id >= m_Neighbours.Length) return null;
        return m_Neighbours[id];
    }

    public void CountNeighbours(int id, ref List<Tile> movealbeTiles, int movesLeft, bool canJump, bool mustJump, PlayerColor playerColor, bool recursiveCall = false)
    {
        if (movesLeft <= 0) return;
        bool add = true;

        //Only do certain checks if this is not the first call
        if (recursiveCall == true)
        {
            movesLeft -= 1;

            if (m_Unit != null)
            {
                //Don't even include this tile if the unit is from the same player
                if (m_Unit.Owner == playerColor)
                {
                    add = false;
                }

                //This unit is a mountain, don't go here
                if (m_Unit.UnitDefinition.UnitType == UnitType.Mountain)
                {
                    add = false;
                }

                //Jump!
                if (canJump)
                {
                    movesLeft = 1; //We hop over that unit
                    canJump = false; //No more than 1 jump
                }
                else
                {
                    if (add) movealbeTiles.Add(this);
                    return;
                }
            }

            //If we MUST jump, and we haven't jumped yet then this tile is not valid.
            if (mustJump && canJump)
            {
                add = false;
            }

            if (add) movealbeTiles.Add(this);
        }

        if (m_Neighbours[id] != null && movesLeft > 0)
        {
            m_Neighbours[id].CountNeighbours(id, ref movealbeTiles, movesLeft, canJump, mustJump, playerColor, true);
        }
    }
}
