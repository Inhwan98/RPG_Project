using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NextSceneController : MonoBehaviour
{
    private void OnTriggerEnter(Collider coll)
    {
        if(coll.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            LodingSceneController.LoadScene("Dungeon");
        }
    }
}
