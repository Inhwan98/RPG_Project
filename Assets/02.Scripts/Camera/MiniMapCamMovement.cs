using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniMapCamMovement : MonoBehaviour
{
    [SerializeField] private bool x, y, z;

    private Transform target;

    private void Start()
    {
        target = PlayerController.instance.transform;
    }

    // Update is called once per frame
    void Update()
    {
        //�Ѿư����� ����� ������ ����
        if (!target) return;

        transform.position = new Vector3(
            (x ? target.position.x : transform.position.x),
            (y ? target.position.y : transform.position.y),
            (z ? target.position.z : transform.position.z));
    }
}
