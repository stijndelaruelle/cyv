using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

public class VisualUnit : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    //This class just visually represents the Unit & the player can interact with it.
    //It does in no way, shape or form hold any game data.

    //Events
    public VisualUnitDelegate OnPromote;

    public static VisualUnit m_DraggedUnit = null;

    //Our sprite, as it has to be higher up while draggin
    [SerializeField]
    private RectTransform m_SpriteTransform;

    //The big blob that is used for indicating the tile we're on
    [SerializeField]
    private RectTransform m_DragIndicator;

    private VisualTile m_Tile = null;
    private VisualTile m_ReserveTile = null; //The tile we need to go to when we die

    private UnitDefinition m_UnitDefinition = null; //Used for movement ranges etc.
    private PlayerColor m_PlayerColor = PlayerColor.White;
    
    private Transform m_ParentTransform;
    private bool m_IsDragged = false; //Used to distinguish between click movement & drag movement
    public bool IsDragged
    {
        get { return m_IsDragged; }
        set { m_IsDragged = value; }
    }

    private RectTransform m_CanvasTransform; //The canvas transform, used for drag & drop
    private bool m_IsAnimating = false;
    public bool IsAnimating
    {
        get { return m_IsAnimating; }
    }

    public void Awake()
    {
        m_CanvasTransform = GameObject.Find("Canvas").GetComponent<RectTransform>();
    }

    public void Select(VisualUnit unit)
    {
        if (m_DraggedUnit != null) { m_DraggedUnit.Deselect(); }
        m_DraggedUnit = unit;

        //Only enlarge units that we actually can pick up
        if (GameplayManager.Instance.CurrentPlayerType == PlayerType.AI)
            return;

        if (unit != null)
        {
            if (GameplayManager.Instance.CurrentPlayer == m_PlayerColor || MenuManager.Instance.IsInManual())
            {
                transform.localScale = new Vector3(2.0f, 2.0f, 1.0f);
            }
        }
    }

    public void Deselect()
    {
        m_DraggedUnit.ShowMovementRange(false);
        transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);

        EnableDragging(true);

        //We placed the unit out of the valid fields
        if (transform.parent == m_ParentTransform)
        {
            SetTile(m_Tile);
        }

        if (m_DragIndicator != null && m_DragIndicator.gameObject.activeSelf)
        {
            Vector3 newPoint = GetConvertedPosition();
            transform.position = newPoint;

            if (m_SpriteTransform != null)
                m_SpriteTransform.position = new Vector3(newPoint.x, newPoint.y, newPoint.z - 0.02f);

            m_DragIndicator.gameObject.SetActive(false);
        }
    }

    public void SetTile(VisualTile tile)
    {
        if (tile == null)
        {
            tile = m_ReserveTile;
        }
        else
        {
            ShowMovementRange(false);
        }

        transform.SetParent(tile.GetUnitParent());
        transform.position = new Vector3(transform.position.x, transform.position.y, -1.0f);
        transform.localScale = new Vector3(1.0f, 1.0f, 1.0f); //For some reason this resets

        if (!m_IsAnimating && m_Tile != null && m_Tile != tile)
        {
            StartCoroutine(AnimateUnitRoutine(tile.transform.position));
        }

        m_Tile = tile;
    }

    public void SetReserveTile(VisualTile reserveTile)
    {
        m_ReserveTile = reserveTile;
    }

    public VisualTile GetTile()
    {
        return m_Tile;
    }

    public VisualTile GetReserveTile()
    {
        return m_ReserveTile;
    }

    public void SetPlayerColor(PlayerColor playerColor)
    {
        m_PlayerColor = playerColor;

        //Color primColor = Color.white;
        //Color secColor = Color.black;
        //if (playerColor == PlayerColor.Black)
        //{
        //    primColor = Color.black;
        //    secColor = Color.white;
        //}

        //gameObject.GetComponent<Image>().color = primColor;

        ////if (m_UnitSprite != null)
        ////{
        ////    m_UnitSprite.color = secColor;
        ////}
    }

    public PlayerColor GetPlayerColor()
    {
        return m_PlayerColor;
    }

    public void SetUnitDefinition(UnitDefinition unitDefinition)
    {
        m_UnitDefinition = unitDefinition;
    }

    public UnitDefinition GetUnitDefinition()
    {
        return m_UnitDefinition;
    }

    public void SetParentTransform(Transform transform)
    {
        m_ParentTransform = transform;
    }

    private void ShowMovementRange(bool enable)
    {
        //Very special case, you can always go to your entire half of the board
        if (GameplayManager.Instance.GameState == GameState.Setup && !MenuManager.Instance.IsInManual())
        {
            GameplayManager.Instance.HighlightSetupZone(enable);
            return;
        }

        if (enable)
        {
            VisualTile.UnHighlightMovementHistory(true);
            VisualTile.UnHighlightMovementHistory(false);
        }

        if (!enable && GameplayManager.Instance.GameState != GameState.Promotion)
        {
            VisualTile.HighlightMovementHistory(true);
            VisualTile.HighlightMovementHistory(false);
        }

        //Show movement range
        if (m_Tile != null && m_UnitDefinition != null)
        {
            for (int i = 0; i < BoardState.DIR_NUM; ++i)
            {
                m_Tile.HighlightNeighbour(i, enable, m_UnitDefinition.GetMoveCount(i, m_PlayerColor), m_UnitDefinition.CanJump, m_UnitDefinition.MustJump, m_PlayerColor);
            }
        }
    }

    public void EnableDragging(bool state)
    {
        gameObject.GetComponent<CanvasGroup>().blocksRaycasts = state;
    }

    public void Show(bool state)
    {
        //m_SpriteTransform.gameObject.SetActive(state);
        gameObject.SetActive(state);
    }

    public void Flip(bool state)
    {
        float angle = 0.0f;
        if (state) angle = 180.0f;

        gameObject.transform.localRotation = Quaternion.Euler(new Vector3(0.0f, 0.0f, angle));
    }

    private IEnumerator AnimateUnitRoutine(Vector3 newPos)
    {
        if (m_IsAnimating)
            yield return null;

        Vector3 prevPos = m_SpriteTransform.position;

        if (prevPos == newPos)
            yield return null;

        m_IsAnimating = true;

        //Calculate the direction we're going to be moving in
        Vector3 dir = newPos - prevPos;
        dir.Normalize();

        //Set starting values
        GameObject prevParent = m_SpriteTransform.transform.parent.gameObject;
        m_SpriteTransform.position = prevPos;
        m_SpriteTransform.SetParent(m_ParentTransform); //Set parent to the board so you can see us!

        //Calculating the distance from the target
        Vector3 dist = newPos - prevPos;
        float speed = 10.0f;

        //Loop until we reached the correct point
        Vector3 tempPos = m_SpriteTransform.position;
        while (Math.Sign(dist.x) == Mathf.Sign(dir.x) && Math.Sign(dist.y) == Mathf.Sign(dir.y))
        {
            //Yield here so we don't see the final "snap" back
            yield return null;

            tempPos = new Vector3(tempPos.x + (dir.x * Time.deltaTime * speed), tempPos.y + (dir.y * Time.deltaTime * speed), -1.0f);
            m_SpriteTransform.position = tempPos;
            
            dist = newPos - tempPos;
        }

        //Once reached, snap again to the 100% correct values
        m_SpriteTransform.SetParent(prevParent.transform);

        m_SpriteTransform.localPosition = new Vector3(0.0f, 0.0f, -2.0f); //Center yourself on the tile
        m_SpriteTransform.localScale = new Vector3(1.0f, 1.0f, 1.0f); //And reset scale, because it always screws up otherwise.

        m_IsAnimating = false;
    }

    //----------------
    // INTERFACES
    //----------------

    //IPointerDownHandler
    public void OnPointerDown(PointerEventData eventData)
    {
        //If we're allowed to promote this unit
        if (GameplayManager.Instance.GameState == GameState.Promotion && !MenuManager.Instance.IsInManual())
        {
            if (OnPromote != null) OnPromote(this);
            return;
        }

        //This means the user used the click & click method to place units
        if (GameplayManager.Instance.CurrentPlayer != m_PlayerColor &&
            GameplayManager.Instance.CurrentPlayerType != PlayerType.AI && 
            !MenuManager.Instance.IsInManual())
        {
            //If we are moving a character here, 
            if (m_DraggedUnit != null)
            {
                if (GameplayManager.Instance.CurrentPlayerType != PlayerType.AI)
                {
                    if (m_DraggedUnit.GetPlayerColor() == GameplayManager.Instance.CurrentPlayer || MenuManager.Instance.IsInManual())
                    {
                        m_Tile.OnPointerDown(eventData);
                    }
                }
            }
            //If we're just checking the movement range
            else
            {
                Select(this);
                ShowMovementRange(true);
            }

            return;
        }

        OnBeginDrag(eventData);
        m_IsDragged = false;

        //Play a sound effect
        AudioManager.Instance.PlaySound(AudioManager.SoundType.GrabUnit);
    }

    //IBeginDragHandler
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (GameplayManager.Instance.GameState == GameState.Promotion && !MenuManager.Instance.IsInManual())
            return;

        if (GameplayManager.Instance.CurrentPlayerType == PlayerType.AI)
            return;

        if (GameplayManager.Instance.CurrentPlayer != m_PlayerColor && !MenuManager.Instance.IsInManual())
            return;

        //Debug.Log("Begin Drag!");
        Select(this);

        //Loosen from field so we always render on top!
        transform.SetParent(m_ParentTransform);
        ShowMovementRange(true);

        EnableDragging(false);
        m_IsDragged = true;

        Vector3 newPoint = GetConvertedPosition();
        transform.position = newPoint;

        if (m_SpriteTransform != null)
        {
            float offset = 1.0f;
            if (transform.rotation.z != 0.0f &&
                GameplayManager.Instance.GameMode == GameMode.MirroredPlay &&
                GameplayManager.Instance.NumAIPlayers() == 0)
            {
                offset = -1.0f;
            }

            m_SpriteTransform.position = new Vector3(newPoint.x, newPoint.y + offset, newPoint.z - 0.02f);
        }

        if (m_DragIndicator != null)
        {
            m_DragIndicator.gameObject.SetActive(true);
            m_DragIndicator.position = new Vector3(newPoint.x, newPoint.y, -0.01f);
        }
    }

    //IDragHandler
    public void OnDrag(PointerEventData eventData)
    {
        if (GameplayManager.Instance.GameState == GameState.Promotion && !MenuManager.Instance.IsInManual())
            return;

        if (GameplayManager.Instance.CurrentPlayerType == PlayerType.AI)
            return;

        if (GameplayManager.Instance.CurrentPlayer != m_PlayerColor && !MenuManager.Instance.IsInManual())
            return;

        Vector3 newPoint = GetConvertedPosition();
        transform.position = newPoint;

        if (m_SpriteTransform != null)
        {
            float offset = 1.0f;
            if (transform.rotation.z != 0.0f &&
                GameplayManager.Instance.GameMode == GameMode.MirroredPlay &&
                GameplayManager.Instance.NumAIPlayers() == 0)
            {
                offset = -1.0f;
            }

            m_SpriteTransform.position = new Vector3(newPoint.x, newPoint.y + offset, newPoint.z - 0.02f);
        }

        if (m_DragIndicator != null)
        {
            //Make the drag object snap to tiles
            var raycastResults = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, raycastResults);

            if (raycastResults.Count > 0)
            {
                GameObject go = raycastResults[0].gameObject;

                //First is just an empty tile, second is when there is an unit on that tile (LAME but w/e)
                VisualTile visualTile = go.transform.parent.GetComponent<VisualTile>();
                if (visualTile == null) { visualTile = go.transform.parent.parent.parent.GetComponent<VisualTile>(); }

                if (visualTile != null && visualTile.IsHighlighted)
                {
                    m_DragIndicator.gameObject.SetActive(true);
                    m_DragIndicator.localScale = Vector3.one;
                    m_DragIndicator.position = new Vector3(go.transform.position.x, go.transform.position.y, -0.01f);
                }
                else
                {
                    m_DragIndicator.localScale = Vector3.zero; // Don't do this, it skrews the OnEndDrag logic -> LAME .SetActive(false);
                }
            }
        }
    }

    //IEndDragHandler
    public void OnEndDrag(PointerEventData eventData)
    {
        //We should always be able to deselect, otherwise showing the enemies movement range bugs out on the last used unit.
        Select(null);
    }

    private Vector3 GetConvertedPosition()
    {
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(m_CanvasTransform, Input.mousePosition, Camera.main, out localPoint);
        return m_CanvasTransform.TransformPoint(localPoint);
    }

}