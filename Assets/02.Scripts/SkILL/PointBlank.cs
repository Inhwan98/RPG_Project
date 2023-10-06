using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointBlank : MonoBehaviour
{
    [SerializeField] private Collider coll;
    [SerializeField] private float attackPerSecond;

    private void OnEnable()
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

    private void OnDisable()
    {
        StopCoroutine("AttacksPerSecond");
    }
}
