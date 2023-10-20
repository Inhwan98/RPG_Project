using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text;
using TMPro;

public class ItemTooltipUI : MonoBehaviour
{
    [SerializeField]
    private Text _titleText; //아이템 이름 텍스트

    [SerializeField]
    private TMP_Text _contentText; //아이템 설명 텍스트

    [SerializeField]
    private TMP_Text _returnPriceText; //되팔기 가격

    StringBuilder sb = new StringBuilder();

    private RectTransform _rt;
    private CanvasScaler _canvasScaler;


    private void Awake()
    {
        Init();
        Hide();
    }

    public void Show() => gameObject.SetActive(true);
    public void Hide() => gameObject.SetActive(false);

    private void Init()
    {
        TryGetComponent(out _rt);
        _rt.pivot = new Vector2(0f, 1f); //Left Top
        _canvasScaler = GetComponentInParent<CanvasScaler>();

        DisableAllChildrenRaycastTraget(transform);
    }

    /// <summary> 모든 자식 UI에 레이캐스트 타겟 해제 </summary>
    private void DisableAllChildrenRaycastTraget(Transform tr)
    {
        // 본인이 Graphic(UI)를 상속하면 레이캐스트 타겟 해제
        tr.TryGetComponent(out Graphic gr);
        if (gr != null)
            gr.raycastTarget = false;

        // 자식이 없으면 종료
        int childCount = tr.childCount;
        if (childCount == 0) return;

        for(int i = 0; i < childCount; i++)
        {
            DisableAllChildrenRaycastTraget(tr.GetChild(i));
        }
    }

    ///<summary> 툴팁 UI에 아이템 정보 등록 </summary>
    public void SetItemInfo(ItemData data)
    {
        _titleText.text = data.GetName();

        

        //인벤 UI에 나타나는 습득레벨제한 표시. 플레이어가 낮다면 빨간색으로 표시
        if (PlayerController.instance.GetLevel() < data.GetUsedLevel())
            sb.Append($"제한 레벨 : <color=#ff0000>{data.GetUsedLevel()}</color><br>");
        else
            sb.Append($"제한 레벨 : {data.GetUsedLevel()}<br>");

        if (data is PortionItemData portionData)
            sb.Append($"회복량 :{portionData.GetValue()}<br>");
        else if (data is ArmorItemData armorData)
            sb.Append($"방어력 : {armorData.GetDefence()}<br>");
        else if (data is WeaponItemData weaponData)
            sb.Append($"공격력 : {weaponData.GetDamage()}<br>");

        sb.Append($"<br><br> {data.GetToolTip()}");

        _contentText.text = sb.ToString();

        sb.Clear();
        int returnPrice = data.GetReturnPrice();
        sb.Append($"판매 가격 : {returnPrice} 루비");
        _returnPriceText.text = sb.ToString();

        sb.Clear();
    }

    ///<summary> 툴팁 UI에 스킬 정보 등록 </summary>
    public void SetItemInfo(SkillData data)
    {
        _titleText.text = data.GetSKillName();

        //스킬 UI에 나타나는 습득레벨제한 표시. 플레이어가 낮다면 빨간색으로 표시
        if(PlayerController.instance.GetLevel() < data.GetSkillUsedLevel())
            sb.Append($"제한 레벨 : <color=#ff0000>{data.GetSkillUsedLevel()}</color><br>");
        else
            sb.Append($"제한 레벨 : {data.GetSkillUsedLevel()}<br><br>");

        sb.Append($"스킬 레벨 : {data.GetSkillLevel()}<br>"); ;
        sb.Append($"재사용 대기시간 : {data.GetCoolDown()}<br>");
        sb.Append($"마나 소모 : <color=#0000ff>{data.GetSkillManaAmount()}</color><br>");
        sb.Append($"피해량 : 공격력의 <color=#ff7f00>{data.GetSkillDamagePer()}% </color><br>");
        sb.Append($"<br><color=#ffff00>스킬설명 </color> : <br>");
        sb.Append($"{data.GetToolTip()}");

        _contentText.text = sb.ToString();

        sb.Clear();
    }

    /// <summary> 툴팁의 위치 조정 </summary>
    public void SetRectPosition(RectTransform slotRect)
    {
        // 캔버스 스케일러에 따른 해상도 대응
        float wRatio = Screen.width / _canvasScaler.referenceResolution.x;
        float hRatio = Screen.height / _canvasScaler.referenceResolution.y;
        float ratio =
            wRatio * (1f - _canvasScaler.matchWidthOrHeight) +
            hRatio * (_canvasScaler.matchWidthOrHeight);

        float slotWidth = slotRect.rect.width * ratio;
        float slotHeight = slotRect.rect.height * ratio;

        // 툴팁 초기 위치(슬롯 우하단) 설정
        _rt.position = slotRect.position + new Vector3(slotWidth * 1.5f, -slotHeight * 1.5f);
        Vector2 pos = _rt.position;

        //툴팁의 크기
        float width = _rt.rect.width * ratio;
        float height = _rt.rect.height * ratio;

        // 우측, 하단이 잘렸는지 여부
        bool rightTruncated = pos.x + width > Screen.width;
        bool bottomTruncated = pos.y - height < 0f;

        ref bool R = ref rightTruncated;
        ref bool B = ref bottomTruncated;

        // 오른쪽만 잘림 => 슬롯의 Left Bottom 방향으로 표시
        if(R && !B)
        {
            _rt.position = new Vector2(pos.x - width - slotWidth, pos.y);
        }
        // 아래쪽만 잘림 => 슬롯의 Right Top 방향으로 표시
        else if(!R && B)
        {
            _rt.position = new Vector2(pos.x, pos.y + height + slotHeight);
        }
        // 모두 잘림 => 슬롯의 Left Top 방향으로 표시
        else if(R && B)
        {
            _rt.position = new Vector2(pos.x - width - slotWidth, pos.y + height + slotHeight);
        }
        // 잘리지 않음 => 슬롯의 Right Bottom 방향으로 표시
        // Do Nothing

    }
}
