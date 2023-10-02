using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC_Dialog : MonoBehaviour
{
    [SerializeField]
    private DialogUI dialogSystem01;

    private IEnumerator Start()
    {
        // ù ��° ��� �б� ����
        yield return new WaitUntil(() => dialogSystem01.UpdateDialog());
    }
}
