using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointBlank : MonoBehaviour
{
    [SerializeField] protected Collider coll;
    [SerializeField] private float attackSecond;
    [SerializeField] private int repeatCount;

    private int count;

    protected virtual void OnEnable()
    {
        StartCoroutine("AttacksPerSecond");
    }

    IEnumerator AttacksPerSecond()
    {
        while(count < repeatCount)
        {
            yield return new WaitForSeconds(attackSecond);
            coll.enabled = !(coll.enabled);
            count++;
        }
       //0 2 4

        coll.enabled = false;
    }

    protected virtual void OnDisable()
    {
        StopCoroutine("AttacksPerSecond");
        count = 0;
        coll.enabled = true;
    }
}
