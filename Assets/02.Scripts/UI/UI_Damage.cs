using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UI_Damage : MonoBehaviour
{

    //������ �ؽ�Ʈ
    [SerializeField] private TMP_Text damageText;
    [SerializeField] private float moveDuration = 1.0f; // �̵��ϴ� �� �ɸ��� �ð�

    private Vector3 startPos;
    private Vector3 destPos;

    private float currentTime = 0f; // ���� �ð�

    void Start()
    {
        
        startPos = transform.position;
        destPos = startPos + (Vector3.up * 0.5f);
    }


    void Update()
    {
        if (currentTime < moveDuration)
        {
            // easeOutBounce �Լ��� ����Ͽ� �̵� ������ ���
            float t = currentTime / moveDuration;
            float easeValue = easeOutBounce(t);

            // ���� ��ġ���� ��ǥ ��ġ���� �����Ͽ� �̵�
            transform.position = Vector3.Lerp(startPos, destPos, easeValue);

            currentTime += Time.deltaTime;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    // easeOutBounce �Լ� (https://easings.net/ ����)
    public static float easeOutBounce(float x)
    {
        const float n1 = 7.5625f;
        const float d1 = 2.75f;

        if (x < 1f / d1)
        {
            return n1 * x * x;
        }
        else if (x < 2f / d1)
        {
            x -= 1.5f / d1;
            return n1 * x * x + 0.75f;
        }
        else if (x < 2.5f / d1)
        {
            x -= 2.25f / d1;
            return n1 * x * x + 0.9375f;
        }
        else
        {
            x -= 2.625f / d1;
            return n1 * x * x + 0.984375f;
        }
    }

    public void SetDamageText(float _damage)
    {
        damageText.text = $"{_damage}";
    }


}
