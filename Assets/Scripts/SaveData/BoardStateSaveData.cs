using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BoardStateSaveData
{
    protected PlayerColor m_CurrentPlayer = PlayerColor.White;
    protected int m_FromTileID = -1;
    protected int m_ToTileID = -1;
    protected int m_PromotionTileID = -1;

    //unit tiles
    protected List<int> m_UnitTiles = new List<int>();

    public int GetUnitTile(int unitID)
    {
        return m_UnitTiles[unitID];
    }
}

public class BoardStateSaveDataWhiteSymmetric : BoardStateSaveData
{
    public BoardStateSaveDataWhiteSymmetric()
    {
        m_UnitTiles.Clear();

        SetupWhite();
        SetupBlack();
    }

    private void SetupWhite()
    {
        //The mountains (6)
        m_UnitTiles.Add(64);
        m_UnitTiles.Add(66);
        m_UnitTiles.Add(71);
        m_UnitTiles.Add(76);
        m_UnitTiles.Add(80);
        m_UnitTiles.Add(82);

        //The king (1)
        m_UnitTiles.Add(87);

        //The rabble (6)
        m_UnitTiles.Add(52);
        m_UnitTiles.Add(53);
        m_UnitTiles.Add(54);
        m_UnitTiles.Add(57);
        m_UnitTiles.Add(58);
        m_UnitTiles.Add(59);

        //The light horse (2)
        m_UnitTiles.Add(55);
        m_UnitTiles.Add(56);

        //The spears (2)
        m_UnitTiles.Add(51);
        m_UnitTiles.Add(60);

        //The crossbows (2)
        m_UnitTiles.Add(63);
        m_UnitTiles.Add(67);

        //The heavy horse (2)
        m_UnitTiles.Add(-1);
        m_UnitTiles.Add(-1);

        //The elephants (2)
        m_UnitTiles.Add(-1);
        m_UnitTiles.Add(-1);

        //The trebuchets (2)
        m_UnitTiles.Add(-1);
        m_UnitTiles.Add(-1);

        //The dragon (1)
        m_UnitTiles.Add(65);
    }

    private void SetupBlack()
    {
        for (int i = 0; i < 26; ++i)
        {
            //-2 means not set up!
            m_UnitTiles.Add(-2);
        }
    }
}

public class BoardStateSaveDataWhiteAggressive : BoardStateSaveData
{
    public BoardStateSaveDataWhiteAggressive()
    {
        m_UnitTiles.Clear();

        SetupWhite();
        SetupBlack();
    }

    private void SetupWhite()
    {
        //The mountains (6)
        m_UnitTiles.Add(61);
        m_UnitTiles.Add(63);
        m_UnitTiles.Add(67);
        m_UnitTiles.Add(69);
        m_UnitTiles.Add(80);
        m_UnitTiles.Add(82);

        //The king (1)
        m_UnitTiles.Add(87);

        //The rabble (6)
        m_UnitTiles.Add(53);
        m_UnitTiles.Add(55);
        m_UnitTiles.Add(56);
        m_UnitTiles.Add(58);
        m_UnitTiles.Add(62);
        m_UnitTiles.Add(68);

        //The light horse (2)
        m_UnitTiles.Add(54);
        m_UnitTiles.Add(57);

        //The spears (2)
        m_UnitTiles.Add(52);
        m_UnitTiles.Add(59);

        //The crossbows (2)
        m_UnitTiles.Add(64);
        m_UnitTiles.Add(66);

        //The heavy horse (2)
        m_UnitTiles.Add(-1);
        m_UnitTiles.Add(-1);

        //The elephants (2)
        m_UnitTiles.Add(-1);
        m_UnitTiles.Add(-1);

        //The trebuchets (2)
        m_UnitTiles.Add(-1);
        m_UnitTiles.Add(-1);

        //The dragon (1)
        m_UnitTiles.Add(65);
    }

    private void SetupBlack()
    {
        for (int i = 0; i < 26; ++i)
        {
            //-2 means not set up!
            m_UnitTiles.Add(-2);
        }
    }
}

public class BoardStateSaveDataWhiteDefensive : BoardStateSaveData
{
    public BoardStateSaveDataWhiteDefensive()
    {
        m_UnitTiles.Clear();

        SetupWhite();
        SetupBlack();
    }

    private void SetupWhite()
    {
        //The mountains (6)
        m_UnitTiles.Add(53);
        m_UnitTiles.Add(61);
        m_UnitTiles.Add(64);
        m_UnitTiles.Add(71);
        m_UnitTiles.Add(81);
        m_UnitTiles.Add(86);

        //The king (1)
        m_UnitTiles.Add(78);

        //The rabble (6)
        m_UnitTiles.Add(54);
        m_UnitTiles.Add(56);
        m_UnitTiles.Add(57);
        m_UnitTiles.Add(58);
        m_UnitTiles.Add(59);
        m_UnitTiles.Add(65);

        //The light horse (2)
        m_UnitTiles.Add(55);
        m_UnitTiles.Add(74);

        //The spears (2)
        m_UnitTiles.Add(52);
        m_UnitTiles.Add(66);

        //The crossbows (2)
        m_UnitTiles.Add(63);
        m_UnitTiles.Add(67);

        //The heavy horse (2)
        m_UnitTiles.Add(-1);
        m_UnitTiles.Add(-1);

        //The elephants (2)
        m_UnitTiles.Add(-1);
        m_UnitTiles.Add(-1);

        //The trebuchets (2)
        m_UnitTiles.Add(-1);
        m_UnitTiles.Add(-1);

        //The dragon (1)
        m_UnitTiles.Add(70);
    }

    private void SetupBlack()
    {
        for (int i = 0; i < 26; ++i)
        {
            //-2 means not set up!
            m_UnitTiles.Add(-2);
        }
    }
}

public class BoardStateSaveDataBlackSymmetric : BoardStateSaveData
{
    public BoardStateSaveDataBlackSymmetric()
    {
        m_UnitTiles.Clear();

        SetupWhite();
        SetupBlack();
    }

    private void SetupWhite()
    {
        for (int i = 0; i < 26; ++i)
        {
            //-2 means not set up!
            m_UnitTiles.Add(-2);
        }
    }

    private void SetupBlack()
    {
        //The mountains (6)
        m_UnitTiles.Add(8);
        m_UnitTiles.Add(10);
        m_UnitTiles.Add(14);
        m_UnitTiles.Add(19);
        m_UnitTiles.Add(24);
        m_UnitTiles.Add(26);

        //The king (1)
        m_UnitTiles.Add(3);

        //The rabble (6)
        m_UnitTiles.Add(31);
        m_UnitTiles.Add(32);
        m_UnitTiles.Add(33);
        m_UnitTiles.Add(36);
        m_UnitTiles.Add(37);
        m_UnitTiles.Add(38);

        //The light horse (2)
        m_UnitTiles.Add(34);
        m_UnitTiles.Add(35);

        //The spears (2)
        m_UnitTiles.Add(30);
        m_UnitTiles.Add(39);

        //The crossbows (2)
        m_UnitTiles.Add(23);
        m_UnitTiles.Add(27);

        //The heavy horse (2)
        m_UnitTiles.Add(-1);
        m_UnitTiles.Add(-1);

        //The elephants (2)
        m_UnitTiles.Add(-1);
        m_UnitTiles.Add(-1);

        //The trebuchets (2)
        m_UnitTiles.Add(-1);
        m_UnitTiles.Add(-1);

        //The dragon (1)
        m_UnitTiles.Add(25);
    }
}

public class BoardStateSaveDataBlackAggressive : BoardStateSaveData
{
    public BoardStateSaveDataBlackAggressive()
    {
        m_UnitTiles.Clear();

        SetupWhite();
        SetupBlack();
    }

    private void SetupWhite()
    {
        for (int i = 0; i < 26; ++i)
        {
            //-2 means not set up!
            m_UnitTiles.Add(-2);
        }
    }

    private void SetupBlack()
    {
        //The mountains (6)
        m_UnitTiles.Add(8);
        m_UnitTiles.Add(10);
        m_UnitTiles.Add(21);
        m_UnitTiles.Add(23);
        m_UnitTiles.Add(27);
        m_UnitTiles.Add(29);

        //The king (1)
        m_UnitTiles.Add(3);

        //The rabble (6)
        m_UnitTiles.Add(22);
        m_UnitTiles.Add(28);
        m_UnitTiles.Add(32);
        m_UnitTiles.Add(34);
        m_UnitTiles.Add(35);
        m_UnitTiles.Add(37);

        //The light horse (2)
        m_UnitTiles.Add(33);
        m_UnitTiles.Add(36);

        //The spears (2)
        m_UnitTiles.Add(31);
        m_UnitTiles.Add(38);

        //The crossbows (2)
        m_UnitTiles.Add(24);
        m_UnitTiles.Add(26);

        //The heavy horse (2)
        m_UnitTiles.Add(-1);
        m_UnitTiles.Add(-1);

        //The elephants (2)
        m_UnitTiles.Add(-1);
        m_UnitTiles.Add(-1);

        //The trebuchets (2)
        m_UnitTiles.Add(-1);
        m_UnitTiles.Add(-1);

        //The dragon (1)
        m_UnitTiles.Add(25);
    }
}

public class BoardStateSaveDataBlackDefensive : BoardStateSaveData
{
    public BoardStateSaveDataBlackDefensive()
    {
        m_UnitTiles.Clear();

        SetupWhite();
        SetupBlack();
    }

    private void SetupWhite()
    {
        for (int i = 0; i < 26; ++i)
        {
            //-2 means not set up!
            m_UnitTiles.Add(-2);
        }
    }

    private void SetupBlack()
    {
        //The mountains (6)
        m_UnitTiles.Add(4);
        m_UnitTiles.Add(9);
        m_UnitTiles.Add(19);
        m_UnitTiles.Add(26);
        m_UnitTiles.Add(29);
        m_UnitTiles.Add(37);

        //The king (1)
        m_UnitTiles.Add(12);

        //The rabble (6)
        m_UnitTiles.Add(25);
        m_UnitTiles.Add(31);
        m_UnitTiles.Add(32);
        m_UnitTiles.Add(33);
        m_UnitTiles.Add(34);
        m_UnitTiles.Add(36);

        //The light horse (2)
        m_UnitTiles.Add(16);
        m_UnitTiles.Add(35);

        //The spears (2)
        m_UnitTiles.Add(24);
        m_UnitTiles.Add(38);

        //The crossbows (2)
        m_UnitTiles.Add(23);
        m_UnitTiles.Add(27);

        //The heavy horse (2)
        m_UnitTiles.Add(-1);
        m_UnitTiles.Add(-1);

        //The elephants (2)
        m_UnitTiles.Add(-1);
        m_UnitTiles.Add(-1);

        //The trebuchets (2)
        m_UnitTiles.Add(-1);
        m_UnitTiles.Add(-1);

        //The dragon (1)
        m_UnitTiles.Add(20);
    }
}