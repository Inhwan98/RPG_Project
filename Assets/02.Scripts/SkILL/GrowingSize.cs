using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrowingSize : PointBlank
{
    [SerializeField] private Vector3 destSize;
    [SerializeField] private float destTime;

    private BoxCollider _boxColl;
    private Vector3 originSize;
    private float time;
    private float t;

    private void Awake()
    {
        if (coll is BoxCollider boxColl)
        {
            _boxColl = boxColl;
        }
        originSize = _boxColl.size;
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        StartCoroutine("StartGrowingSize");
    }

    /// <summary> 비활성화 시 재사용하기 위해 초기화 </summary>
    protected override void OnDisable()
    {
        base.OnDisable();
        _boxColl.size = originSize;
        _boxColl.enabled = true;
        time = 0;
        t = 0;
    }


    IEnumerator StartGrowingSize()
    {
        while (time < destTime)
        {
            time += Time.deltaTime;
            t = time / destTime;
            _boxColl.size = Vector3.Lerp(originSize, destSize, t);
            yield return null;
        }
        _boxColl.size = originSize;
        _boxColl.enabled = false;
    }

}
