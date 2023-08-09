using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Weapon : MonoBehaviour
{
    [SerializeField] private IWeaponType weaponType;

    [SerializeField] private Collider meleeArea;

    private void Awake()
    {
        meleeArea = GetComponent<Collider>();
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
        yield return new WaitForSeconds(0.1f);
        meleeArea.enabled = true; //������Ʈ Ȱ��ȭ
                                  //2
        yield return new WaitForSeconds(0.3f);
        meleeArea.enabled = false;
        //3
        yield return new WaitForSeconds(0.3f);
    }
}
