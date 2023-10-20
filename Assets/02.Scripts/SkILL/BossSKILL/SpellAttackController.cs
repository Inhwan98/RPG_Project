using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellAttackController : MonoBehaviour
{
    [SerializeField] private Collider _coll;
    [SerializeField] GameObject _particleGo;
    [SerializeField] GameObject _projectorGo;
    [SerializeField] private float _DelayTime;

    WaitForSeconds _waitTime;
    WaitForSeconds _collActiveTime;

    private int _damage;

    public void SetDamage(int value) => this._damage = value;

    private void Awake()
    {
        _waitTime = new WaitForSeconds(_DelayTime);
        _collActiveTime = new WaitForSeconds(0.5f);
    }

    private void OnEnable()
    {
        _projectorGo.SetActive(true);

        StartCoroutine(SpawnParticle());
    }

    private void OnTriggerEnter(Collider coll)
    {
        if (coll.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            var playerCtr = coll.gameObject.GetComponent<PlayerController>();
            playerCtr.OnDamage(this._damage, true, transform);
        }
    }

    IEnumerator SpawnParticle()
    {
        yield return _waitTime;
        _coll.enabled = true;
        _particleGo.SetActive(true);
        _projectorGo.SetActive(false);

        yield return _collActiveTime;
        _coll.enabled = false;
    }

}
