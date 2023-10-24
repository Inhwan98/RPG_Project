using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 보스에 근접 공격에 관한 스크립트이다.
/// <para> 보스는 특정 부위들에 콜라이더를 갖고, 공격시에 활성화 / 비활성화 한다. </para>
/// </summary>
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
