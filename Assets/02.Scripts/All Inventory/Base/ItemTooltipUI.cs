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
    private Text _titleText; //������ �̸� �ؽ�Ʈ

    [SerializeField]
    private TMP_Text _contentText; //������ ���� �ؽ�Ʈ

    [SerializeField]
    private TMP_Text _returnPriceText; //���ȱ� ����

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

    /// <summary> ��� �ڽ� UI�� ����ĳ��Ʈ Ÿ�� ���� </summary>
    private void DisableAllChildrenRaycastTraget(Transform tr)
    {
        // ������ Graphic(UI)�� ����ϸ� ����ĳ��Ʈ Ÿ�� ����
        tr.TryGetComponent(out Graphic gr);
        if (gr != null)
            gr.raycastTarget = false;

        // �ڽ��� ������ ����
        int childCount = tr.childCount;
        if (childCount == 0) return;

        for(int i = 0; i < childCount; i++)
        {
            DisableAllChildrenRaycastTraget(tr.GetChild(i));
        }
    }

    ///<summary> ���� UI�� ������ ���� ��� </summary>
    public void SetItemInfo(ItemData data)
    {
        _titleText.text = data.GetName();

        

        //�κ� UI�� ��Ÿ���� ���淹������ ǥ��. �÷��̾ ���ٸ� ���������� ǥ��
        if (PlayerController.instance.GetLevel() < data.GetUsedLevel())
            sb.Append($"���� ���� : <color=#ff0000>{data.GetUsedLevel()}</color><br>");
        else
            sb.Append($"���� ���� : {data.GetUsedLevel()}<br>");

        if (data is PortionItemData portionData)
            sb.Append($"ȸ���� :{portionData.GetValue()}<br>");
        else if (data is ArmorItemData armorData)
            sb.Append($"���� : {armorData.GetDefence()}<br>");
        else if (data is WeaponItemData weaponData)
            sb.Append($"���ݷ� : {weaponData.GetDamage()}<br>");

        sb.Append($"<br><br> {data.GetToolTip()}");

        _contentText.text = sb.ToString();

        sb.Clear();
        int returnPrice = data.GetReturnPrice();
        sb.Append($"�Ǹ� ���� : {returnPrice} ���");
        _returnPriceText.text = sb.ToString();

        sb.Clear();
    }

    ///<summary> ���� UI�� ��ų ���� ��� </summary>
    public void SetItemInfo(SkillData data)
    {
        _titleText.text = data.GetSKillName();

        //��ų UI�� ��Ÿ���� ���淹������ ǥ��. �÷��̾ ���ٸ� ���������� ǥ��
        if(PlayerController.instance.GetLevel() < data.GetSkillUsedLevel())
            sb.Append($"���� ���� : <color=#ff0000>{data.GetSkillUsedLevel()}</color><br>");
        else
            sb.Append($"���� ���� : {data.GetSkillUsedLevel()}<br><br>");

        sb.Append($"��ų ���� : {data.GetSkillLevel()}<br>"); ;
        sb.Append($"���� ���ð� : {data.GetCoolDown()}<br>");
        sb.Append($"���� �Ҹ� : <color=#0000ff>{data.GetSkillManaAmount()}</color><br>");
        sb.Append($"���ط� : ���ݷ��� <color=#ff7f00>{data.GetSkillDamagePer()}% </color><br>");
        sb.Append($"<br><color=#ffff00>��ų���� </color> : <br>");
        sb.Append($"{data.GetToolTip()}");

        _contentText.text = sb.ToString();

        sb.Clear();
    }

    /// <summary> ������ ��ġ ���� </summary>
    public void SetRectPosition(RectTransform slotRect)
    {
        // ĵ���� �����Ϸ��� ���� �ػ� ����
        float wRatio = Screen.width / _canvasScaler.referenceResolution.x;
        float hRatio = Screen.height / _canvasScaler.referenceResolution.y;
        float ratio =
            wRatio * (1f - _canvasScaler.matchWidthOrHeight) +
            hRatio * (_canvasScaler.matchWidthOrHeight);

        float slotWidth = slotRect.rect.width * ratio;
        float slotHeight = slotRect.rect.height * ratio;

        // ���� �ʱ� ��ġ(���� ���ϴ�) ����
        _rt.position = slotRect.position + new Vector3(slotWidth * 1.5f, -slotHeight * 1.5f);
        Vector2 pos = _rt.position;

        //������ ũ��
        float width = _rt.rect.width * ratio;
        float height = _rt.rect.height * ratio;

        // ����, �ϴ��� �߷ȴ��� ����
        bool rightTruncated = pos.x + width > Screen.width;
        bool bottomTruncated = pos.y - height < 0f;

        ref bool R = ref rightTruncated;
        ref bool B = ref bottomTruncated;

        // �����ʸ� �߸� => ������ Left Bottom �������� ǥ��
        if(R && !B)
        {
            _rt.position = new Vector2(pos.x - width - slotWidth, pos.y);
        }
        // �Ʒ��ʸ� �߸� => ������ Right Top �������� ǥ��
        else if(!R && B)
        {
            _rt.position = new Vector2(pos.x, pos.y + height + slotHeight);
        }
        // ��� �߸� => ������ Left Top �������� ǥ��
        else if(R && B)
        {
            _rt.position = new Vector2(pos.x - width - slotWidth, pos.y + height + slotHeight);
        }
        // �߸��� ���� => ������ Right Bottom �������� ǥ��
        // Do Nothing

    }
}
