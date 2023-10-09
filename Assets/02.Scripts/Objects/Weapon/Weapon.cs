using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Weapon : MonoBehaviour
{
    [SerializeField] private IWeaponType weaponType;

    private Collider meleeArea;
    private TrailRenderer weaponTrail;

    private void Awake()
    {
        meleeArea   = GetComponent<Collider>();
        weaponTrail = GetComponentInChildren<TrailRenderer>();
    }

    public void Use()
    {
        switch (weaponType)
        {
            case IWeaponType.Sword:
                StartCoroutine(Swing());
                break;
        }
    }

    IEnumerator Swing()
    {
        //공격의 단계를 코루틴으로 구분
        //1
        meleeArea.enabled   = true; //컴포넌트 활성화
        weaponTrail.enabled = true; //트레일렌더러 활성화
        //2
        yield return new WaitForSeconds(0.3f);
        meleeArea.enabled   = false;
        //3
        yield return new WaitForSeconds(0.3f);
        weaponTrail.enabled = false; //트레일렌더러 비활성화
    }
}
