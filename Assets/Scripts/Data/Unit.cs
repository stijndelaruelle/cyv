using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum UnitType
{
    Mountain,
    King,
    Rabble,
    Crossbow,
    Spear,
    LightHorse,
    Catapult,
    Elephant,
    HeavyHorse,
    Dragon
}

//Struct
public class UnitDefinition
{
    protected UnitType m_UnitType = UnitType.Mountain;
    public UnitType UnitType
    {
        get { return m_UnitType; }
    }

    protected int m_OrthogonalMoves = 0; //-1 = infinite
    public int OrthogonalMoves
    {
        get { return m_OrthogonalMoves; }
    }

    protected int m_DiagonalMoves = 0; //-1 = infinite
    public int DiagonalMoves
    {
        get { return m_DiagonalMoves; }
    }

    protected int m_Value = 0; //Value used in the board value calculations
    public int Value
    {
        get { return m_Value; }
    }

    protected int m_StartAmount = 0; //Amount of these units you start with
    public int StartAmount
    {
        get { return m_StartAmount; }
    }

    protected int m_MaxAmount = 0; //The max amount of this unit you can have
    public int MaxAmount
    {
        get { return m_MaxAmount; }
    }

    protected bool m_IgnoreUnits = false;
    public bool IgnoreUnits
    {
        get { return m_IgnoreUnits; }
    }
}

public class MountainUnitDefinition : UnitDefinition
{
    public MountainUnitDefinition()
    {
        m_UnitType = UnitType.Mountain;
        m_OrthogonalMoves = 0;
        m_DiagonalMoves = 0;
        m_Value = 0;

        m_StartAmount = 6;
        m_MaxAmount = 6;
    }
}

public class KingUnitDefinition : UnitDefinition
{
    public KingUnitDefinition()
    {
        m_UnitType = UnitType.King;
        m_OrthogonalMoves = 1;
        m_DiagonalMoves = 1;
        m_Value = 9999; //overrules all, meaning that if we can take the king the AI will always prefer it

        m_StartAmount = 1;
        m_MaxAmount = 1;
    }

    //Special stuff
}

public class Unit
{
    //---------------
    // Datamembers
    //---------------
    private UnitDefinition m_UnitDefinition;
    public UnitDefinition UnitDefinition
    {
        get { return m_UnitDefinition; }
    }

    private PlayerType m_Owner; //The player that owns us
    public PlayerType Owner
    {
        get { return m_Owner; }
    }

    private Tile m_Tile; //The tile we're on

    //Cache so we don't need to check every time
    private int[] m_MoveCounts = new int[12];

    //---------------
    // Functions
    //---------------
    public Unit(UnitDefinition unitDef, PlayerType owner, Tile tile)
    {
        m_UnitDefinition = unitDef;
        m_Owner = owner;
        m_Tile = tile;
    }

    public void SetTile(Tile tile)
    {
        //if tile == null, we died or we promoted
        m_Tile = tile;
    }

    public Tile GetTile()
    {
        return m_Tile;
    }

    public void Copy(Unit otherUnit)
    {
        m_UnitDefinition = otherUnit.UnitDefinition; //This is a reference, but that's not a problem
        m_Owner = otherUnit.Owner; //Copy
        m_Tile = null; //Will be set later.
    }

    //AI
    public int CalculateMovecounts()
    {
        //We calculate all the places we can go
        int totalMoves = 0;

        for (int i = 0; i < 12; ++i) //6 is just the amount of directions
        {
            m_MoveCounts[i] = 0;

            if (m_Tile == null)
            {
                m_MoveCounts[i] = 0;
                continue;
            }

            int movesLeft = UnitDefinition.OrthogonalMoves;
            if (i >= 6) movesLeft = UnitDefinition.DiagonalMoves;

            if (movesLeft > 0)
            {
                m_Tile.CountNeighbours(i, ref m_MoveCounts[i], movesLeft, UnitDefinition.IgnoreUnits, false);
                totalMoves += m_MoveCounts[i];
            }
        }

        Debug.Log(m_Owner.ToString() + "'s " + UnitDefinition.UnitType.ToString() + " has " + totalMoves + " possible moves!");
        return totalMoves;
    }

    public void ProcessMove(int id)
    {
        //Debug.Log("Processing move: " + id);
    }
}
