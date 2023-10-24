using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary> 투사체 스킬 컨트롤러 </summary>
public class ProjectileController : MonoBehaviour
{
    [SerializeField] private float _moveDuration = 1.0f; // 이동하는 데 걸리는 시간
    [SerializeField] private GameObject collisionEffectGo;

    private Rigidbody _rigid;

    [SerializeField] private float force = 1200.0f;

    private Vector3 _startPos;
    private Vector3 _destPos;

    private int _damage;

    public void SetDamage(int value) => _damage = value;

    private void Awake()
    {
        _rigid = GetComponent<Rigidbody>();
    }

    // Start is called before the first frame update
    private void OnEnable()
    {
        _rigid.WakeUp();
        Vector3 dir = _destPos - _startPos;
        _rigid.AddForce(dir.normalized * force);
    }

    private void OnCollisionEnter(Collision coll)
    {
        if (coll.gameObject.layer == LayerMask.NameToLayer("Cave"))
        {
            CollisionEffect();
        }
        else if (coll.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            var playerCtr = coll.gameObject.GetComponent<PlayerController>();
            playerCtr.OnDamage(this._damage, true, transform);
            CollisionEffect();
        }
    }


    private void OnDisable()
    {
        _rigid.velocity = Vector3.zero;
        _rigid.angularVelocity = Vector3.zero;

        _rigid.Sleep();
    }

    public void CollisionEffect()
    {
        this.gameObject.SetActive(false);
        var colGO = Instantiate(collisionEffectGo, transform.position, Quaternion.identity);
        Destroy(colGO, 1.5f);
    }


    public void Init(Vector3 startPos, Vector3 destPos)
    {
        this._startPos = startPos;
        this._destPos = destPos;
    }
}
