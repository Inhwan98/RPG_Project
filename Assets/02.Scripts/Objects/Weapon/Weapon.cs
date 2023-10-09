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
        //������ �ܰ踦 �ڷ�ƾ���� ����
        //1
        meleeArea.enabled   = true; //������Ʈ Ȱ��ȭ
        weaponTrail.enabled = true; //Ʈ���Ϸ����� Ȱ��ȭ
        //2
        yield return new WaitForSeconds(0.3f);
        meleeArea.enabled   = false;
        //3
        yield return new WaitForSeconds(0.3f);
        weaponTrail.enabled = false; //Ʈ���Ϸ����� ��Ȱ��ȭ
    }
}
