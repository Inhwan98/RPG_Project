using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointBlank : MonoBehaviour
{
    [SerializeField] private Collider coll;
    [SerializeField] private float attackPerSecond;

    void Start()
    {
        StartCoroutine(AttacksPerSecond());
    }

    IEnumerator AttacksPerSecond()
    {
        while(true)
        {
            yield return new WaitForSeconds(attackPerSecond);
            coll.enabled = !(coll.enabled);
        }
    }
}
