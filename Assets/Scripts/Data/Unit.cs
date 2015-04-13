using UnityEngine;
using System.Collections;

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
}

public class MountainUnitDefinition : UnitDefinition
{
    public MountainUnitDefinition()
    {
        m_UnitType = UnitType.Mountain;
        m_OrthogonalMoves = 0;
        m_DiagonalMoves = 0;
    }
}

public class KingUnitDefinition : UnitDefinition
{
    public KingUnitDefinition()
    {
        m_UnitType = UnitType.King;
        m_OrthogonalMoves = 1;
        m_DiagonalMoves = 1;
    }

    //Special stuff
}

public class Unit
{
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

    public Unit(UnitDefinition unitDef, PlayerType owner, Tile tile)
    {
        m_UnitDefinition = unitDef;
        m_Owner = owner;
        m_Tile = tile;
    }

    public void SetTile(Tile tile)
    {
        //if tile == null, we died
        m_Tile = tile;
    }
}
