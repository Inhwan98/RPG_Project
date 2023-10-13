using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarkMovement : MonoBehaviour
{
    private float _theta = 0f;

    [SerializeField]  private float _fSpeedY = 3.0f;
    [SerializeField]  private float _fHeight = 0.5f;

    private Vector3 _offset;

    void Update()
    {
        Vector3 pos = transform.position;
        _theta += _fSpeedY * Time.deltaTime;
        pos.y = Mathf.Sin(_theta) * _fHeight;

        transform.position = pos + (Vector3.up * _offset.y);
    }

    public void SetYOffset(Vector3 offset) => this._offset = offset;
}
