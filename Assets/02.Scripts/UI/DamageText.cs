using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DamageText : MonoBehaviour
{

    [SerializeField] private float _moveDuration = 1.5f; // 이동하는 데 걸리는 시간

    private TextMesh _textMesh;

    private Camera _cam;
    private Vector3 _startPos;
    private Vector3 _destPos;

    private Color _originColor;
    private Color _destColor;
    private float alpha;

    private float _randomOffset = 0.5f;

    private float _currentTime = 0f; // 현재 시간

    private void Awake()
    {
        _cam = Camera.main;
        _textMesh = GetComponent<TextMesh>();
    }

    void Start()
    {
        //시각적인 연출을 위해 초반 생성 위치를 x,y축 랜덤하게 바꾼다. (-0.5 ~ 0.5 Offset)
        float rand = Random.Range(-_randomOffset, _randomOffset);
        transform.localPosition += Vector3.right * rand;
        transform.localPosition += Vector3.up * rand;

        _startPos = transform.localPosition;
        _destPos = _startPos + (Vector3.up);

        _originColor = _textMesh.color;                                            //초기 칼라에서
        _destColor = new Color(_originColor.r, _originColor.b, _originColor.b, 0); // 목표 알파 값은 zero
    }

    void Update()
    {
        if (_currentTime < _moveDuration)
        {
            // easeOutBounce 함수를 사용하여 이동 보간값 계산
            float t = _currentTime / _moveDuration;
            float easeValue = EaseLerp.easeOutQuint(t); //점점 상승
            float fadeAlphaValue = EaseLerp.easeInCirc(t); //점점 투명하게

            // 시작 위치부터 목표 위치까지 보간하여 이동
            transform.localPosition = Vector3.Lerp(_startPos, _destPos, easeValue);
            _textMesh.color = Color.Lerp(_originColor, _destColor, fadeAlphaValue);

            _currentTime += Time.deltaTime;
        }
        else
        {
            //보간 완료시 삭제
            Destroy(this.gameObject);
        }
    }

    private void LateUpdate()
    {
        transform.LookAt(transform.position + _cam.transform.forward);
    }
}
