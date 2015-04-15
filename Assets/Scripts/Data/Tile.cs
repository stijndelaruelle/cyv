using UnityEngine;
using System.Collections;

public class Tile
{
    //Bolow a diagram of how the neighbour ID's work
    //   5  /\ 0
    //   4 |  | 1
    //   3  \/ 2
    private Tile[] m_Neighbours;
    private Unit m_Unit;
    public Unit Unit
    {
        get { return m_Unit; }
    }

    public Tile()
    {
        m_Neighbours = new Tile[12];
        m_Unit = null;
    }

    public void SetUnit(Unit unit)
    {
        //If this tile wasn't empty, we killed that unit
        if (m_Unit != null)
        {
            //Illegal move
            if (m_Unit.Owner == unit.Owner) return;
            m_Unit.SetTile(null);
        }

        m_Unit = unit;
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

    public void CountNeighbours(int id, ref int counter, int movesLeft, bool ignoreUnits, bool ignoreMountains, bool recursiveCall = false)
    {
        if (movesLeft <= 0) return;
        
        //Only do certain checks if this is not the first call
        if (recursiveCall == true)
        {
            if (!ignoreMountains && m_Unit != null && m_Unit.UnitDefinition.UnitType == UnitType.Mountain) return;

            movesLeft -= 1;
            ++counter;

            if (!ignoreUnits && m_Unit != null) return;
        }

        if (m_Neighbours[id] != null && movesLeft > 0)
        {
            m_Neighbours[id].CountNeighbours(id, ref counter, movesLeft, ignoreUnits, ignoreMountains, true);
        }
    }
}
