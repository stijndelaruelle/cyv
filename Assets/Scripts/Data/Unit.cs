using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum UnitType
{
    Mountain,
    King,
    Rabble,
    LightHorse,
    Spear,
    Crossbow,
    HeavyHorse,
    Elephant,
    Catapult,
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
}

public class LightHorseUnitDefinition : UnitDefinition
{
    public LightHorseUnitDefinition()
    {
        m_UnitType = UnitType.LightHorse;
        m_OrthogonalMoves = 2;
        m_DiagonalMoves = 0;
        m_Value = 3;

        m_StartAmount = 2;
        m_MaxAmount = 2;
    }
}

public class SpearUnitDefinition : UnitDefinition
{
    public SpearUnitDefinition()
    {
        m_UnitType = UnitType.Spear;
        m_OrthogonalMoves = 0;
        m_DiagonalMoves = 1;
        m_Value = 3;

        m_StartAmount = 0;
        m_MaxAmount = 2;
    }
}

public class CrossbowUnitDefinition : UnitDefinition
{
    public CrossbowUnitDefinition()
    {
        m_UnitType = UnitType.Crossbow;
        m_OrthogonalMoves = 1;
        m_DiagonalMoves = 0;
        m_Value = 3;

        m_StartAmount = 0;
        m_MaxAmount = 2;
    }
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

    private Tile m_Tile; //The tile we're currently on

    //Cache so we don't need to check every time
    private List<Tile>[] m_PossibleMoves = new List<Tile>[BoardState.DIR_NUM];

    private BoardState m_BoardState; //The boardstate we are parented to

    //---------------
    // Functions
    //---------------
    public Unit(BoardState boardState, UnitDefinition unitDef, PlayerType owner, Tile tile)
    {
        m_BoardState = boardState;
        m_UnitDefinition = unitDef;
        m_Owner = owner;
        m_Tile = tile;

        for (int i = 0; i < m_PossibleMoves.Length; ++i)
        {
            m_PossibleMoves[i] = new List<Tile>(); 
        }
    }

    public void SetTile(Tile tile)
    {
        //Swap tile
        Tile oldTile = m_Tile;
        m_Tile = tile;

        //Clear our last tile
        if (oldTile != null && oldTile.GetUnit() == this)
            oldTile.SetUnit(null);

        //Inform our new tile
        if (m_Tile != null)
            m_Tile.SetUnit(this);
    }

    public Tile GetTile()
    {
        return m_Tile;
    }

    //AI
    public void Copy(Unit otherUnit)
    {
        m_UnitDefinition = otherUnit.UnitDefinition; //This is a reference, but that's not a problem as it's static data
        m_Owner = otherUnit.Owner;

        //Clear old unit data 
        if (m_Tile != null) { m_Tile.SetUnit(null); }
        m_Tile = null;

        Tile tile = otherUnit.GetTile();
        if (tile != null)
        {
            m_Tile = m_BoardState.GetTile(tile.ID);
            m_Tile.SetUnit(this);
        }

        for (int i = 0; i < BoardState.DIR_NUM; ++i)
        {
            m_PossibleMoves[i].Clear();

            for (int j = 0; j < otherUnit.m_PossibleMoves[i].Count; ++j)
            {
                m_PossibleMoves[i].Add(m_BoardState.GetTile(otherUnit.m_PossibleMoves[i][j].ID));
            }     
        }
    }

    public int CalculateMovecounts()
    {
        //We calculate all the places we can go
        if (m_Tile == null) return 0;

        int totalMoves = 0;
        for (int i = 0; i < BoardState.DIR_NUM; ++i)
        {
            m_PossibleMoves[i].Clear();

            int movesLeft = UnitDefinition.OrthogonalMoves;
            if (i % 2 == 0) movesLeft = UnitDefinition.DiagonalMoves; //Even directions are diagonal
            
            if (movesLeft > 0)
            {
                m_Tile.CountNeighbours(i, ref m_PossibleMoves[i], movesLeft, UnitDefinition.IgnoreUnits, false, m_Owner);
                totalMoves += m_PossibleMoves[i].Count;
            }
        }

        //Debug.Log(m_Owner.ToString() + "'s " + UnitDefinition.UnitType.ToString() + " has " + totalMoves + " possible moves!");
        return totalMoves;
    }

    public void ProcessMove(int id)
    {
        //Figure out which tile to go to
        //STIJN: Must be done more performant
        int totalFoundId = 0;
        Tile foundTile = null;
        for (int i = 0; i < BoardState.DIR_NUM; ++i)
        {
            totalFoundId += m_PossibleMoves[i].Count;

            if (id < totalFoundId && m_PossibleMoves[i].Count != 0)
            {
                int diff = totalFoundId - id;
                foundTile = m_PossibleMoves[i][m_PossibleMoves[i].Count - diff];
                break;
            }
        }

        if (m_Tile != null && foundTile != null)
        {
            //Remove ourselves from our old tile
            m_Tile.SetUnit(null);

            //And go to a new one
            SetTile(foundTile);
        }
    }
}
