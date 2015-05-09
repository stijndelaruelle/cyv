using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class VisualTutorialBoard : MonoBehaviour
{
    public static int TUTORIAL_BOARD_SIZE = 4;
    public static int TUTORIAL_SKIP_ROWS = 2;

    [SerializeField]
    private UnitType m_UnitType;

    [SerializeField]
    private int[] m_MountainTileID;

    [SerializeField]
    private GameObject m_TilePrefab = null;

    [SerializeField]
    private GameObject m_CentralUnit = null;

    [SerializeField]
    private GameObject m_MountainUnit = null;

    [SerializeField]
    private Color[] m_Colors;

    //Not sure why we need this
    private List<VisualTile> m_VisualTiles;
    private VisualUnit m_VisualUnit;
    private VisualTile m_CenterTile;

    public void Awake()
    {
        GenerateHexBoard(TUTORIAL_BOARD_SIZE);
        GenerateUnits();
    }

    public void OnEnable()
    {
        //Reset the unit's position
        m_VisualUnit.SetTile(m_CenterTile);
    }

    private void GenerateHexBoard(int boardSize)
    {
        if (m_VisualTiles == null) m_VisualTiles = new List<VisualTile>();
        m_VisualTiles.Clear();

        float width = 44;
        float height = 38;

        GameObject obj = null;
        RectTransform rectTransform = null;

        //Calculate board size
        int rowWidth = boardSize;
        float startX = -(width * (boardSize * 0.5f)) + (width / 2);
        float startY = height * (boardSize - 1);
        int startColor = 0;

        //Rows
        for (int i = 0; i < (boardSize * 2) - 1 - TUTORIAL_SKIP_ROWS; ++i)
        {
            float x = startX;
            int color = startColor;

            //Columns
            for (int j = 0; j < rowWidth; ++j)
            {
                obj = GameObject.Instantiate(m_TilePrefab) as GameObject;

                rectTransform = obj.GetComponent<RectTransform>();
                rectTransform.SetParent(gameObject.GetComponent<RectTransform>()); //Parent this to us
                rectTransform.anchoredPosition = new Vector2(x, startY);
                rectTransform.localScale = new Vector2(1.0f, 1.0f); //Always resets for some reason

                VisualTile newTile = obj.GetComponent<VisualTile>();
                newTile.SetColor(m_Colors[color]);
                newTile.SetID(m_VisualTiles.Count);
                m_VisualTiles.Add(newTile);

                x += width;

                //-----------------------------------------------------------------
                // Manage neighbours backwards (so we are sure they already exist)
                //-----------------------------------------------------------------

                //Bolow a diagram of how the neighbour ID's work (d = diagonal)
                //   11    0d   1
                //   10d   /\   2d 
                //   9    |  |  3
                //   8d    \/   4d 
                //   7     6d   5

                //--------------
                // ORTHOGONAL
                //--------------
                //Left neighbour (9), only exists if you're not the first one in the row
                if (j != 0)
                {
                    int id = (m_VisualTiles.Count - 2);

                    VisualTile neigbourTile = m_VisualTiles[id];
                    newTile.SetNeightbour(9, neigbourTile);
                    neigbourTile.SetNeightbour(3, newTile);
                }

                //Top right corner (1)
                if (i == 0) { /*Nothing*/ }
                else if (j == (rowWidth - 1) && i < boardSize) { /*Nothing*/ }
                else
                {
                    //Get id
                    int id = (m_VisualTiles.Count - 1);
                    id -= rowWidth;
                    if (i < boardSize)
                        id += 1; //On the growing side remove an extra id

                    VisualTile neigbourTile = m_VisualTiles[id];
                    newTile.SetNeightbour(1, neigbourTile);
                    neigbourTile.SetNeightbour(7, newTile);
                }

                //Top left neightbour (11)
                if (i == 0) { /*Nothing*/ }
                else if (j == 0 && i < boardSize) { /*Nothing*/ }
                else
                {
                    //Get id
                    int id = (m_VisualTiles.Count - 1);
                    id -= rowWidth;
                    if (i >= boardSize) id -= 1; //On the shrinking side remove an extra id

                    VisualTile neigbourTile = m_VisualTiles[id];
                    newTile.SetNeightbour(11, neigbourTile);
                    neigbourTile.SetNeightbour(5, newTile);
                }

                //--------------
                // DIAGONAL
                //--------------

                //Top neighbour (0)
                if (i < 2)
                {
                    /*Nothing*/
                }
                else if (i < boardSize && (j == 0 || j == (rowWidth - 1)))
                {
                    /*Nothing*/
                }
                else
                {
                    //Get id
                    int id = (m_VisualTiles.Count - 1);
                    id -= rowWidth;

                    if (i > boardSize) { id -= (rowWidth + 2); }
                    else if (i < boardSize) { id -= (rowWidth - 2); }
                    else if (i == boardSize) { id -= (rowWidth + 1); }

                    VisualTile neigbourTile = m_VisualTiles[id];
                    newTile.SetNeightbour(0, neigbourTile);
                    neigbourTile.SetNeightbour(6, newTile);
                }

                //Top right diagonal corner (2)
                if (i == 0) { /*Nothing*/ }
                else if (j == (rowWidth - 1)) { /*Nothing*/ }
                else if (j == (rowWidth - 2) && i < boardSize) { /*Nothing*/ }
                else
                {
                    //Get id
                    int id = (m_VisualTiles.Count - 1);
                    id -= rowWidth - 1;
                    if (i < boardSize)
                        id += 1; //On the shrinking side remove an extra id

                    VisualTile neigbourTile = m_VisualTiles[id];
                    newTile.SetNeightbour(2, neigbourTile);
                    neigbourTile.SetNeightbour(8, newTile);
                }

                //Top left neightbour (10)
                if (i == 0) { /*Nothing*/ }
                else if (j == 0) { /*Nothing*/ }
                else if (j == 1 && i < boardSize) { /*Nothing*/ }
                else
                {
                    //Get id
                    int id = (m_VisualTiles.Count - 1);
                    id -= rowWidth + 1;
                    if (i >= boardSize) id -= 1; //On the shrinking side remove an extra id

                    VisualTile neigbourTile = m_VisualTiles[id];
                    newTile.SetNeightbour(10, neigbourTile);
                    neigbourTile.SetNeightbour(4, newTile);
                }

                //-------------------
                // Increase colour
                //-------------------
                color += 1;
                if (color >= m_Colors.Length) color = 0;
            }

            //Alter the width & start X coord of the new row
            if (i < (boardSize - 1))
            {
                startX -= (width * 0.5f);

                rowWidth += 1;
                startColor += 1;
                if (startColor >= m_Colors.Length) startColor = 0;
            }
            else
            {
                startX += (width * 0.5f);
                rowWidth -= 1;

                startColor -= 1;
                if (startColor < 0) startColor = m_Colors.Length - 1;
            }

            //Alter the y coord of the next row
            startY -= height;
        }
    }

    private void GenerateUnits()
    {
        //Determine the unitdefninition
        UnitDefinition unitDefinition;

        switch (m_UnitType)
        {
            case UnitType.Mountain:     unitDefinition = new MountainUnitDefinition();    break;
            case UnitType.King:         unitDefinition = new KingUnitDefinition();        break;
            case UnitType.Rabble:       unitDefinition = new RabbleUnitDefinition();      break;
            case UnitType.LightHorse:   unitDefinition = new LightHorseUnitDefinition();  break;
            case UnitType.Spear:        unitDefinition = new SpearUnitDefinition();       break;
            case UnitType.Crossbow:     unitDefinition = new CrossbowUnitDefinition();    break;
            case UnitType.HeavyHorse:   unitDefinition = new HeavyHorseUnitDefinition();  break;
            case UnitType.Elephant:     unitDefinition = new ElephantUnitDefinition();    break;
            case UnitType.Catapult:     unitDefinition = new CatapultUnitDefinition();    break;
            case UnitType.Dragon:       unitDefinition = new DragonUnitDefinition();      break;

            default:                    unitDefinition = new MountainUnitDefinition();    break;

        }

        GameObject obj = GameObject.Instantiate(m_CentralUnit) as GameObject;

        VisualUnit visualUnit = obj.GetComponent<VisualUnit>();
        visualUnit.SetPlayerColor(PlayerColor.White);
        visualUnit.SetUnitDefinition(unitDefinition);
        visualUnit.SetParentTransform(gameObject.transform);

        //Get the central tile
        CalculateCenterTile();
        visualUnit.SetTile(m_CenterTile); //So it's positionned a the reserve tile

        m_VisualUnit = visualUnit;

        //Generate some mountains (player can use that to jump over
        foreach(int i in m_MountainTileID)
        {
            if (m_VisualTiles.Count < i)
                continue;

            GameObject mountainObj = GameObject.Instantiate(m_MountainUnit) as GameObject;

            VisualUnit visualMountainUnit = mountainObj.GetComponent<VisualUnit>();
            visualMountainUnit.SetPlayerColor(PlayerColor.Black);
            visualMountainUnit.SetUnitDefinition(new MountainUnitDefinition());
            visualMountainUnit.SetParentTransform(gameObject.transform);

            visualMountainUnit.SetTile(m_VisualTiles[i]);
        }
    }

    private void CalculateCenterTile()
    {
        int skippedTiles = 0;
        for (int i = 0; i < TUTORIAL_SKIP_ROWS; ++i)
        {
            skippedTiles += (TUTORIAL_BOARD_SIZE + i);
        }

        int centerID = (int)((m_VisualTiles.Count + skippedTiles) / 2.0f);
        m_CenterTile = m_VisualTiles[centerID];
    }
}
