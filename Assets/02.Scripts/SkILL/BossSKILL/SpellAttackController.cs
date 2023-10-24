using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary> 프로젝터를 이용한 스킬 구현 스크립트 </summary>
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

    /// <summary> 데미지 설정은 외부에서 처리 한다. </summary>
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
    /// 일정 시간이 지난뒤, 스킬파티클(ex : 폭발) 과 함께 콜라이더가 활성화 됨으로써 충돌 처리 가능
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
