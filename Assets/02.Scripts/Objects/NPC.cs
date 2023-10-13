using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class NPC : MonoBehaviour
{
    [SerializeField]
    private int _nID;

    [SerializeField]
    private Transform _markTr;

    private GameObject _questionMarkGo;
    private GameObject _exclamationMarkGO;

    private List<QuestData> _possibleQuestList;
    private List<QuestData> _InProgressQuestList;

    private QuestSystem _questSystem;
    private ResourcesData _resourcesData;



    public int GetID()
    {
        return _nID;
    }

    private void Start()
    {
        MarkInit();
        StartCoroutine(CheckQuestList());
    }

    /// <summary> 
    /// 퀘스트에 대한 감정마크를 미리 생성하여 활성화, 비활성화 방법으로 사용
    /// </summary>
    public void MarkInit()
    {
        _questSystem = GameManager.instance.GetQuestSystem();
        _resourcesData = GameManager.instance.GetResourcesData();

        _questionMarkGo = _resourcesData.GetQuestionMarkGo();
        _exclamationMarkGO = _resourcesData.GetExclamationMarkGO();

        _questionMarkGo.transform.localScale = Vector3.one * 5.0f;
        _exclamationMarkGO.transform.localScale = Vector3.one * 5.0f;

        _questionMarkGo.SetActive(false);
        _exclamationMarkGO.SetActive(false);

        _questionMarkGo = Instantiate(_questionMarkGo, this.transform);
        _exclamationMarkGO = Instantiate(_exclamationMarkGO, this.transform);

        var markMoveCtr = _questionMarkGo.GetComponent<MarkMovement>();
        markMoveCtr.SetYOffset(_markTr.position);
        markMoveCtr = _exclamationMarkGO.GetComponent<MarkMovement>();
        markMoveCtr.SetYOffset(_markTr.position);
    }

    IEnumerator CheckQuestList()
    {
        while (true)
        {
            yield return null;

            _possibleQuestList = _questSystem.GetPossibleQuest();
            _InProgressQuestList = PlayerController.instance.GetPlayerQuestList();

            if(_InProgressQuestList.Count == 0) _questionMarkGo.SetActive(false);
            if (_possibleQuestList.Count == 0) _exclamationMarkGO.SetActive(false);

            //진행중인 퀘스트중, 퀘스트의 목표ID가 본인ID와 같다면...
            foreach (var a in _InProgressQuestList)
            {
                if (this._nID == a.nDestID)
                {
                    _questionMarkGo.SetActive(true);
                    _exclamationMarkGO.SetActive(false);
                    
                    break;
                }
            }

            // 완성 퀘스트가 있다면 "!" 는 활성화하지 않고 "?"만 활성화한다.
            if (_questionMarkGo.activeSelf) continue;

            // 가능한 퀘스트들 중 하나의 ID가 본인 ID와 같다면
            foreach (var a in _possibleQuestList)
            {
                if (this._nID == a.nID)
                {
                    _exclamationMarkGO.SetActive(true);
                    break;
                }
            }

        }
    }

}

