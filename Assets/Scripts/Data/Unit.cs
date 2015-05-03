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

    //The unit we will promote in
    protected UnitType m_PromotedType = UnitType.Mountain;
    public UnitType PromotedType
    {
        get { return m_PromotedType; }

    }

    protected int[] m_MaxMoveCount = new int[BoardState.DIR_NUM]; //-1 = infinite
    public int GetMoveCount(int dir, PlayerColor playerColor)
    {
        //These are as seen from the white player (upwards, flip around for the other player)
        if (playerColor == PlayerColor.Black) { dir += (int)(BoardState.DIR_NUM * 0.5f); }

        if (dir >= BoardState.DIR_NUM) { dir -= BoardState.DIR_NUM; }
        if (dir < 0)                   { dir += BoardState.DIR_NUM; }

        return m_MaxMoveCount[dir];
    }

    protected int m_Value = 0; //Value used in the board value calculations
    public int Value
    {
        get { return m_Value; }
    }

    protected int m_Tier = 0; //Used for promotions
    public int Tier
    {
        get { return m_Tier; }
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

    protected bool m_CanJump = false;
    public bool CanJump
    {
        get { return m_CanJump; }
    }

    protected bool m_MustJump = false;
    public bool MustJump
    {
        get { return m_MustJump; }
    }
}

public class MountainUnitDefinition : UnitDefinition
{
    public MountainUnitDefinition()
    {
        m_UnitType = UnitType.Mountain;
        m_PromotedType = UnitType.Mountain;
        m_Value = 0;
        m_Tier = 1;

        for (int i = 0; i < m_MaxMoveCount.Length; ++i)
        {
            m_MaxMoveCount[i] = 0;
        }

        m_CanJump = false;
        m_MustJump = false;
        m_StartAmount = 6;
        m_MaxAmount = 6;
    }
}

public class KingUnitDefinition : UnitDefinition
{
    public KingUnitDefinition()
    {
        m_UnitType = UnitType.King;
        m_PromotedType = UnitType.King;
        m_Value = 9999; //overrules all, meaning that if we can take the king the AI will always prefer it
        m_Tier = 1;

        for (int i = 0; i < m_MaxMoveCount.Length; ++i)
        {
            m_MaxMoveCount[i] = 1;
        }

        m_CanJump = false;
        m_MustJump = false;
        m_StartAmount = 1;
        m_MaxAmount = 1;
    }
}

public class RabbleUnitDefinition : UnitDefinition
{
    public RabbleUnitDefinition()
    {
        m_UnitType = UnitType.Rabble;
        m_PromotedType = UnitType.Rabble;
        m_Value = 1;
        m_Tier = 1;

        for (int i = 0; i < m_MaxMoveCount.Length; ++i)
        {
            if (i == (int)Direction.Ortohonal1 || i == (int)Direction.Ortohonal6)
            {
                m_MaxMoveCount[i] = 1;
            }
            else
            {
                m_MaxMoveCount[i] = 0;
            }  
        }

        m_CanJump = false;
        m_MustJump = false;
        m_StartAmount = 6;
        m_MaxAmount = 6;
    }
}

public class LightHorseUnitDefinition : UnitDefinition
{
    public LightHorseUnitDefinition()
    {
        m_UnitType = UnitType.LightHorse;
        m_PromotedType = UnitType.HeavyHorse;
        m_Value = 3;
        m_Tier = 2;

        for (int i = 0; i < m_MaxMoveCount.Length; ++i)
        {
            //Diagonal
            if (i % 2 == 0) { m_MaxMoveCount[i] = 0; }

            //Orthogonal
            else { m_MaxMoveCount[i] = 2; }
        }

        m_CanJump = false;
        m_MustJump = false;
        m_StartAmount = 2;
        m_MaxAmount = 2;
    }
}

public class SpearUnitDefinition : UnitDefinition
{
    public SpearUnitDefinition()
    {
        m_UnitType = UnitType.Spear;
        m_PromotedType = UnitType.Elephant;
        m_Value = 3;
        m_Tier = 2;

        for (int i = 0; i < m_MaxMoveCount.Length; ++i)
        {
            //Diagonal
            if (i % 2 == 0) { m_MaxMoveCount[i] = 1; }

            //Orthogonal
            else { m_MaxMoveCount[i] = 0; }
        }

        m_CanJump = false;
        m_MustJump = false;
        m_StartAmount = 2;
        m_MaxAmount = 2;
    }
}

public class CrossbowUnitDefinition : UnitDefinition
{
    public CrossbowUnitDefinition()
    {
        m_UnitType = UnitType.Crossbow;
        m_PromotedType = UnitType.Catapult;
        m_Value = 3;
        m_Tier = 2;

        for (int i = 0; i < m_MaxMoveCount.Length; ++i)
        {
            //Diagonal
            if (i % 2 == 0) { m_MaxMoveCount[i] = 1; }

            //Orthogonal
            else { m_MaxMoveCount[i] = 1; }

        }

        m_CanJump = true;
        m_MustJump = true;
        m_StartAmount = 2;
        m_MaxAmount = 2;
    }
}

public class HeavyHorseUnitDefinition : UnitDefinition
{
    public HeavyHorseUnitDefinition()
    {
        m_UnitType = UnitType.HeavyHorse;
        m_PromotedType = UnitType.HeavyHorse;
        m_Value = 5;
        m_Tier = 3;

        for (int i = 0; i < m_MaxMoveCount.Length; ++i)
        {
            //Diagonal
            if (i % 2 == 0) { m_MaxMoveCount[i] = 0; }

            //Orthogonal
            else { m_MaxMoveCount[i] = 9999; }
        }

        m_CanJump = false;
        m_MustJump = false;
        m_StartAmount = 2;
        m_MaxAmount = 2;
    }
}

public class ElephantUnitDefinition : UnitDefinition
{
    public ElephantUnitDefinition()
    {
        m_UnitType = UnitType.Elephant;
        m_PromotedType = UnitType.Elephant;
        m_Value = 5;
        m_Tier = 3;

        for (int i = 0; i < m_MaxMoveCount.Length; ++i)
        {
            //Diagonal
            if (i % 2 == 0) { m_MaxMoveCount[i] = 9999; }

            //Orthogonal
            else { m_MaxMoveCount[i] = 0; }
        }

        m_CanJump = false;
        m_MustJump = false;
        m_StartAmount = 2;
        m_MaxAmount = 2;
    }
}

public class CatapultUnitDefinition : UnitDefinition
{
    public CatapultUnitDefinition()
    {
        m_UnitType = UnitType.Catapult;
        m_PromotedType = UnitType.Catapult;
        m_Value = 5;
        m_Tier = 3;

        for (int i = 0; i < m_MaxMoveCount.Length; ++i)
        {
            //Diagonal
            if (i % 2 == 0) { m_MaxMoveCount[i] = 9999; }

            //Orthogonal
            else { m_MaxMoveCount[i] = 9999; }

        }

        m_CanJump = true;
        m_MustJump = true;
        m_StartAmount = 2;
        m_MaxAmount = 2;
    }
}

public class DragonUnitDefinition : UnitDefinition
{
    public DragonUnitDefinition()
    {
        m_UnitType = UnitType.Dragon;
        m_PromotedType = UnitType.Dragon;
        m_Value = 9;
        m_Tier = 4;

        for (int i = 0; i < m_MaxMoveCount.Length; ++i)
        {
            m_MaxMoveCount[i] = 9999;
        }

        m_CanJump = true;
        m_MustJump = false;
        m_StartAmount = 1;
        m_MaxAmount = 1;
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

    private PlayerColor m_Owner; //The player that owns us
    public PlayerColor Owner
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
    public Unit(BoardState boardState, UnitDefinition unitDef, PlayerColor owner, Tile tile)
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

            int movesLeft = UnitDefinition.GetMoveCount(i, m_Owner);        
            if (movesLeft > 0)
            {
                m_Tile.CountNeighbours(i, ref m_PossibleMoves[i], movesLeft, UnitDefinition.CanJump, UnitDefinition.MustJump, m_Owner);
                totalMoves += m_PossibleMoves[i].Count;
            }
        }

        return totalMoves;
    }

    public bool ProcessMove(int id)
    {
        //Figure out which tile to go to
        //STIJN: Must be done more performant
        bool promote = false;

        int totalFoundId = 0;
        Tile foundTile = null;
        for (int i = 0; i < BoardState.DIR_NUM; ++i)
        {
            totalFoundId += m_PossibleMoves[i].Count;

            if (id < totalFoundId && m_PossibleMoves[i].Count > 0)
            {
                int diff = totalFoundId - id;
                foundTile = m_PossibleMoves[i][m_PossibleMoves[i].Count - diff];
                break;
            }
        }

        if (m_Tile != null && foundTile != null)
        {
            Unit currentUnit = m_Tile.GetUnit();

            //Do we promote by doing this move?
            if (this.UnitDefinition.Tier <= currentUnit.UnitDefinition.Tier)
            {
                promote = true;
            }

            //Remove ourselves from our old tile
            m_Tile.SetUnit(null);

            //And go to a new one
            SetTile(foundTile);
        }

        return promote;
    }
}
