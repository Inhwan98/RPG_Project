using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC_Dialog : MonoBehaviour
{
    [SerializeField]
    private DialogUI dialogSystem01;

    private IEnumerator Start()
    {
        // 첫 번째 대사 분기 시작
        yield return new WaitUntil(() => dialogSystem01.UpdateDialog());
    }
}
