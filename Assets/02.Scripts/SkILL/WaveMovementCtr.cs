using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveMovementCtr : MonoBehaviour
{
    [SerializeField] private Transform waveTr;
    [SerializeField] private Transform destTr;

    private Transform startTr;

    // Start is called before the first frame update
    void Start()
    {
        startTr = waveTr;
    }

    // Update is called once per frame
    void Update()
    {
        waveTr.transform.position = Vector3.Lerp(startTr.position, destTr.position, Time.deltaTime);
    }
}
