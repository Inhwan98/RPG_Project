using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveMovementCtr : MonoBehaviour
{
    [SerializeField] private float waitTime;
    [SerializeField] private Transform waveTr;
    [SerializeField] private Transform destTr;
    [SerializeField] private float destTime;


    private Transform startTr;

    private float time;
    private float t;

    private bool isStart;


    // Start is called before the first frame update
    void Start()
    {
        startTr = waveTr;

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
            else
            {
                Destroy(this.gameObject);
            }

            waveTr.transform.position = Vector3.Lerp(startTr.position, destTr.position, t);
        }
    }

    private IEnumerator WaitingWave(float time)
    {
        yield return new WaitForSeconds(time);
        isStart = true;
    }
}
