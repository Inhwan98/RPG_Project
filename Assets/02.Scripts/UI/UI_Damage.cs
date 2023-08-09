using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UI_Damage : MonoBehaviour
{

    //데미지 텍스트
    [SerializeField] private TMP_Text damageText;
    [SerializeField] private float moveDuration = 1.0f; // 이동하는 데 걸리는 시간

    private Vector3 startPos;
    private Vector3 destPos;

    private float currentTime = 0f; // 현재 시간

    void Start()
    {
        
        startPos = transform.position;
        destPos = startPos + (Vector3.up * 0.5f);
    }


    void Update()
    {
        if (currentTime < moveDuration)
        {
            // easeOutBounce 함수를 사용하여 이동 보간값 계산
            float t = currentTime / moveDuration;
            float easeValue = easeOutBounce(t);

            // 시작 위치부터 목표 위치까지 보간하여 이동
            transform.position = Vector3.Lerp(startPos, destPos, easeValue);

            currentTime += Time.deltaTime;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    // easeOutBounce 함수 (https://easings.net/ 참고)
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
