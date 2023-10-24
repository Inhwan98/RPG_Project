using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
    * 부모 *
    SlotUIBase(abstract) : 하나의 슬롯에 대한 모든 정보, 이벤트를 가지고 있다.
                           아이콘의 변경, 하이라이트 표시 등

     [상속 구조]

       - ItemInvenSlotUI - 아이템 인벤토리에 포함되는 슬롯
       - MerChantSlotUI  - 아이템 상인 인벤토리에 포함되는 슬롯
       - PlayerEquipUI   - 플레이어가 장착하는 장비 슬롯

       - SKillSlotUI (abstract)
            -InvenSkillUI  - 스킬트리 인벤에 포함될 슬롯 (모든스킬슬롯)
            -PlayerSkillUI - 플레이어가 습득한 스킬슬롯

*/

public class SlotUIBase : MonoBehaviour
{
    /***********************************************************************
    *                               Fields 
    ***********************************************************************/
    #region Option Fields
    [Tooltip("슬롯 내에서 아이콘과 슬롯 사이의 여백")]
    [SerializeField] protected float _padding = 1f;

    [Tooltip("아이템 아이콘 이미지 게임오브젝트")]
    [SerializeField] protected GameObject _iconGo;

    [Tooltip("아이템 아이콘 이미지")]
    [SerializeField] protected Image _iconImage;

    [Tooltip("슬롯이 포커스될 때 나타나는 하이라이트 이미지")]
    [SerializeField] protected Image _highlightImage;

    [Space]
    [Tooltip("하이라이트 이미지 알파 값")]
    [SerializeField] private float _highlightAlpha = 0.5f;

    [Tooltip("하이라이트 소요 시간")]
    [SerializeField] private float _highlightFadeDuration = 0.2f;
    #endregion
    #region private Fields
    private int index;
    private RectTransform _slotRect;
    private RectTransform _highlightRect;
    private Image _slotImage;
    private float _currentAlpha = 0f;    // 현재 하이라이트 알파값
    #endregion
    #region protected Fields
    protected RectTransform _iconRect;
    protected GameObject _highlightGo;
    protected bool _isHasItem = false;
    protected bool _isAccessibleSlot = true; // 슬롯 접근가능 여부
    protected bool _isAccessibleItem = true; // 아이템 접근가능 여부
    /// <summary> 비활성화된 슬롯의 색상 </summary>
    protected static readonly Color InaccessibleSlotColor = new Color(0.2f, 0.2f, 0.2f, 0.5f);
    /// <summary> 비활성화된 아이콘 색상 </summary>
    protected static readonly Color InaccessibleIconColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);
    #endregion

    /**********************************************************************
    *                           Get, Set Methods                          *
    ***********************************************************************/
    #region Get
    /// <summary> 슬롯의 인덱스 /// </summary>
    public int GetIndex() => index;
    public RectTransform GetSlotRect() => _slotRect;
    public RectTransform GetIconRect() => _iconRect;
    /// <summary> 접근 가능한 슬롯인지 여부  </summary>
    public bool GetIsAccessible() => _isAccessibleSlot && _isAccessibleItem;
    /// <summary> 슬롯이 아이템을 보유하고 있는지 여부  </summary>
    public bool GetHasItem() => _iconImage.sprite != null;
    public Image GetIconImage() => _iconImage;
    #endregion
    #region Set
    /// <summary> 슬롯이 아이템을 보유하고 있는지 여부 세팅  </summary>
    public void SetHasItem(bool value) => _isHasItem = value;
    public void SetSlotIndex(int _slotIndex) => index = _slotIndex;
    #endregion

    /***********************************************************************
    *                               Unity Events
    ***********************************************************************/
    #region Unity Evnets
    protected virtual void Awake()
    {
        InitComponents();
        InitValues();
    }
    #endregion


    /***********************************************************************
    *                               Methods
    ***********************************************************************/

    #region private Methods
    //아이콘 활성화 / 비활성화
    private void ShowIcon() => _iconGo.SetActive(true);
    private void HideIcon() => _iconGo.SetActive(false);
    private void InitValues()
    {
        // 1. Item Icon, Highlight Rect
        _iconRect.pivot = new Vector2(0.5f, 0.5f); // 피벗은 중앙
        _iconRect.anchorMin = Vector2.zero;        // 앵커는 Top Left
        _iconRect.anchorMax = Vector2.one;

        // 패딩 조절
        _iconRect.offsetMin = Vector2.one * (_padding);
        _iconRect.offsetMax = Vector2.one * (-_padding);

        // 아이콘과 하이라이트 크기가 동일하도록
        _highlightRect.pivot = _iconRect.pivot;
        _highlightRect.anchorMin = _iconRect.anchorMin;
        _highlightRect.anchorMax = _iconRect.anchorMax;
        _highlightRect.offsetMin = _iconRect.offsetMin;
        _highlightRect.offsetMax = _iconRect.offsetMax;

        // 2. Image
        _iconImage.raycastTarget = false;
        _highlightImage.raycastTarget = false;

        // 3. Deactivate Icon
        HideIcon();
        _highlightGo.SetActive(false);
    }
    #endregion
    #region protected Methods
    protected virtual void InitComponents()
    {
        // Rects
        _slotRect = GetComponent<RectTransform>();
        _iconRect = _iconImage.rectTransform;
        _highlightRect = _highlightImage.rectTransform;

        // Game Objects
        _highlightGo = _highlightImage.gameObject;

        // Images
        _slotImage = GetComponent<Image>();
    }
    #endregion
    #region public Methods
    /// <summary> 슬롯 자체의 활성화 / 비활성화 여부 설정  </summary>
    public void SetSlotAccessibleState(bool value)
    {
        //중복 처리는 지양
        if (_isAccessibleSlot == value) return;

        if (value) _slotImage.color = Color.black;
        else
        {
            _slotImage.color = InaccessibleSlotColor;
            HideIcon();
            //HideText();
        }
        _isAccessibleSlot = value;
    }
    /// <summary> 다른 슬롯과 아이템 교환 </summary>
    public void SwapOnMoveIcon(SlotUIBase other, bool isJustCopy = false)
    {
        if (other == null) return;
        if (other == this) return; // 자기 자신과 교환 불가
        if (other is InvenSkillSlotUI) return;
        if (!this.GetIsAccessible()) return;
        if (!other.GetIsAccessible()) return;

        var temp = _iconImage.sprite;

        other.SetItem(temp);

        //단순히 스킬 습득위한 복사면 리턴
        if (isJustCopy) return;

        // 1. 대상에 아이템이 있는 경우 : 교환
        if (other.GetHasItem()) SetItem(other.GetIconImage().sprite);
        // 2. 없는 경우 : 이동
        else RemoveItem();


    }
    /// <summary> 아이템 스프라이트 등록 (아이템 등록) </summary>
    public void SetItem(Sprite itemSprite)
    {
        if (itemSprite != null)
        {
            _iconImage.sprite = itemSprite;
            ShowIcon();
        }
        else
        {
            RemoveItem();
        }
    }
    /// <summary> 아이템 스프라이트 삭제 (아이템삭제) </summary>
    public void RemoveItem()
    {
        _iconImage.sprite = null;
        HideIcon();
        //HideText();
    }
    /// <summary> 아이템 이미지 투명도 설정  </summary>
    public void SetIconAlpha(float alpha)
    {
        _iconImage.color = new Color(
            _iconImage.color.r, _iconImage.color.g, _iconImage.color.b, alpha);
    }
    /// <summary> 슬롯에 하이라이트 표시/해제 </summary>
    public void Highlight(bool show)
    {
        if (show)
            StartCoroutine(nameof(HighlightFadeInRoutine));
        else
            StartCoroutine(nameof(HighlightFadeOutRoutine));
    }
    /// <summary> 하이라이트를 먼저 보이게 하기 위한 함수 </summary>
    public void SetHighlightOnTop(bool value)
    {
        if (value)
            _highlightRect.SetAsLastSibling();
        else
            _highlightRect.SetAsFirstSibling();
    }
    #endregion

    /***********************************************************************
    *                             Coroutines
    ***********************************************************************/
    #region private Coroutines
    /// <summary> 하이라이트 알파값 서서히 증가 </summary>
    private IEnumerator HighlightFadeInRoutine()
    {
        StopCoroutine(nameof(HighlightFadeOutRoutine));
        _highlightGo.SetActive(true);

        float unit = _highlightAlpha / _highlightFadeDuration;

        for (; _currentAlpha <= _highlightAlpha; _currentAlpha += unit * Time.deltaTime)
        {
            _highlightImage.color = new Color(
                _highlightImage.color.r,
                _highlightImage.color.g,
                _highlightImage.color.b,
                _currentAlpha
                );

            yield return null;
        }
    }
    /// <summary> 하이라이트 알파값 0%까지 서서히 감소 </summary>
    private IEnumerator HighlightFadeOutRoutine()
    {
        StopCoroutine(nameof(HighlightFadeInRoutine));

        float unit = _highlightAlpha / _highlightFadeDuration;

        for (; _currentAlpha >= 0f; _currentAlpha -= unit * Time.deltaTime)
        {
            _highlightImage.color = new Color(
                _highlightImage.color.r,
                _highlightImage.color.g,
                _highlightImage.color.b,
                _currentAlpha
            );

            yield return null;
        }

        _highlightGo.SetActive(false);
    }
    #endregion

}
