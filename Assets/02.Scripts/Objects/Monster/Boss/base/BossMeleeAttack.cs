using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BossMeleeAttack : MonoBehaviour
{
    [SerializeField]
    private Collider[] meleeArea;

    private void Awake()
    {
        //meleeArea = GetComponent<Collider>();

        foreach(var col in meleeArea)
        {
            col.enabled = false;
        }
    }

    public void Use(int index)
    {
        StartCoroutine(Attack(index));
    }

    private IEnumerator Attack(int index)
    {
        //공격의 단계를 코루틴으로 구분
        //1
        meleeArea[index].enabled = true; //컴포넌트 활성화
        //2
        yield return new WaitForSeconds(1.0f);
        meleeArea[index].enabled = false;
    }
}
