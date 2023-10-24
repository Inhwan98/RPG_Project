using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveMovementCtr : MonoBehaviour
{
    [SerializeField] private float waitTime;
    [SerializeField] private Transform waveTr;
    [SerializeField] private Transform destTr;
    [SerializeField] private float destTime;


    //[SerializeField] private Transform startTr;
    private Vector3 startPos;

    private Collider coll;

    private float time;
    private float t;

    private bool isStart;


    // Start is called before the first frame update
    void Awake()
    {
        coll = waveTr.gameObject.GetComponent<Collider>();
        coll.enabled = false;
    }

    private void OnEnable()
    {
        startPos = waveTr.position;
        StartCoroutine(WaitingWave(waitTime));
    }

    // Update is called once per frame
    void Update()
    {
        if(isStart)
        {
            if (time < destTime)
            {
                time += Time.deltaTime;
                t = time / destTime;
            }

            waveTr.position = Vector3.Lerp(startPos, destTr.position, t);
        }
    }

    private IEnumerator WaitingWave(float time)
    {
        yield return new WaitForSeconds(time);
        isStart = true;
        coll.enabled = true;
    }

    private void OnDisable()
    {
        StopAllCoroutines();
        isStart = false;
        time = 0;
        t = 0;
        waveTr.position = startPos;
    }


}
