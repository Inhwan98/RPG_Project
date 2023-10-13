using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DamageText : MonoBehaviour
{

    [SerializeField] private float _moveDuration = 1.5f; // �̵��ϴ� �� �ɸ��� �ð�

    private TextMesh _textMesh;

    private Camera _cam;
    private Vector3 _startPos;
    private Vector3 _destPos;

    private Color _originColor;
    private Color _destColor;
    private float alpha;

    private float _randomOffset = 0.5f;

    private float _currentTime = 0f; // ���� �ð�

    private void Awake()
    {
        _cam = Camera.main;
        _textMesh = GetComponent<TextMesh>();
    }

    void Start()
    {
        //�ð����� ������ ���� �ʹ� ���� ��ġ�� x,y�� �����ϰ� �ٲ۴�. (-0.5 ~ 0.5 Offset)
        float rand = Random.Range(-_randomOffset, _randomOffset);
        transform.localPosition += Vector3.right * rand;
        transform.localPosition += Vector3.up * rand;

        _startPos = transform.localPosition;
        _destPos = _startPos + (Vector3.up);

        _originColor = _textMesh.color;                                            //�ʱ� Į�󿡼�
        _destColor = new Color(_originColor.r, _originColor.b, _originColor.b, 0); // ��ǥ ���� ���� zero
    }

    void Update()
    {
        if (_currentTime < _moveDuration)
        {
            // easeOutBounce �Լ��� ����Ͽ� �̵� ������ ���
            float t = _currentTime / _moveDuration;
            float easeValue = EaseLerp.easeOutQuint(t); //���� ���
            float fadeAlphaValue = EaseLerp.easeInCirc(t); //���� �����ϰ�

            // ���� ��ġ���� ��ǥ ��ġ���� �����Ͽ� �̵�
            transform.localPosition = Vector3.Lerp(_startPos, _destPos, easeValue);
            _textMesh.color = Color.Lerp(_originColor, _destColor, fadeAlphaValue);

            _currentTime += Time.deltaTime;
        }
        else
        {
            //���� �Ϸ�� ����
            Destroy(this.gameObject);
        }
    }

    private void LateUpdate()
    {
        transform.LookAt(transform.position + _cam.transform.forward);
    }
}
