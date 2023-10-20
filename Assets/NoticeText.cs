using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoticeText : MonoBehaviour
{
    ResourcesData _resourcesData;

    private static GameObject _npcConversationKeyGo;

    void Start()
    {
        _resourcesData = GameManager.instance.GetResourcesData();
        _npcConversationKeyGo = _resourcesData.GetConversationKey();

        _npcConversationKeyGo = Instantiate(_npcConversationKeyGo, this.transform);
        _npcConversationKeyGo.SetActive(false);
    }

    public static void ConversationKeyGo(bool value, Vector3 pos)
    {
        _npcConversationKeyGo.SetActive(value);
        _npcConversationKeyGo.transform.position = pos;
    }
}
