using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary> �������͸� �̿��� ��ų ���� ��ũ��Ʈ </summary>
public class SpellAttackController : MonoBehaviour
{
    /*****************************************
     *                Fields
     *****************************************/

    #region option Fields
    [SerializeField] private Collider _coll;
    [SerializeField] GameObject _particleGo;
    [SerializeField] GameObject _projectorGo;
    [SerializeField] private float _DelayTime;
    #endregion

    #region privaet Fields
    
    private WaitForSeconds _waitTime;
    private WaitForSeconds _collActiveTime;

    private int _damage;
    #endregion


    /*****************************************
     *                Get, Set
     *****************************************/

    /// <summary> ������ ������ �ܺο��� ó�� �Ѵ�. </summary>
    public void SetDamage(int value) => this._damage = value;


    /*****************************************
     *               Unity Event
     *****************************************/

    #region Unity Event
    private void Awake()
    {
        _waitTime = new WaitForSeconds(_DelayTime);
        _collActiveTime = new WaitForSeconds(0.5f);
    }

    /// <summary> </summary>
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



    #endregion


    /*****************************************
    *               Coroutine
    *****************************************/

    /// <summary>
    /// ���� �ð��� ������, ��ų��ƼŬ(ex : ����) �� �Բ� �ݶ��̴��� Ȱ��ȭ �����ν� �浹 ó�� ����
    /// 
    /// </summary>
    private IEnumerator SpawnParticle()
    {
        yield return _waitTime;
        _coll.enabled = true;
        _particleGo.SetActive(true);
        _projectorGo.SetActive(false);

        yield return _collActiveTime;
        _coll.enabled = false;
    }

}
