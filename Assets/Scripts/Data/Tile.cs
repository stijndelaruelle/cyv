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
}
