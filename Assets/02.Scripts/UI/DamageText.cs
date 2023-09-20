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
            float easeValue = easeOutQuint(t); //���� ���
            float fadeAlphaValue = easeInCirc(t); //���� �����ϰ�

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

    // easeOutBounce �Լ� (https://easings.net/ ����)
    public float easeOutBounce(float x)
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

    /// <summary> ��� ȿ�� </summary>
    public float easeOutQuint(float x)
    {
        return 1 - Mathf.Pow(1 - x, 5); 
    }

    /// <summary> ���� �����ϰ� ���� �Լ� </summary>
    public float easeInCirc(float x)
    {
        return 1 - Mathf.Sqrt(1 - Mathf.Pow(x, 2));
    }

}
