using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NPCData
{
    public int _nNPCID;
    public string _sNPCName;
    public string _sNPCJob;
    public int[] _itemIDArray;
}

public enum NPCState
{
    일반,
    장비상인,
    잡화상인
}

public class NPC : MonoBehaviour
{
    [SerializeField]
    private int _nID;
    private string _sNPCName;
    private string _sNPCJob;


    [SerializeField]
    private Transform _markTr;

    protected ResourcesData _resourcesData;
    protected NPCData _npcData;

    private GameObject _questionMarkGo;
    private GameObject _exclamationMarkGO;

    private List<QuestData> _possibleQuestList;
    private List<QuestData> _InProgressQuestList;

    private QuestSystem _questSystem;
    
    private int _unsolvedQuestId;
    private int _nCompleteIndex;
    QuestData _questData;

    protected TextMesh _npcNameTextMesh;

    private bool _isPossibleQuest;
    private bool _isCompleteQuest;
    private bool _isInProgressQuest;

    public int GetID() => _nID;
    public int SetID(int nID) => this._nID = nID;

    public bool GetIsPossibleQuest() => _isPossibleQuest;
    public bool GetIsCompleteQuest() => _isCompleteQuest;
    public bool GetIsInProgressQuest() => _isInProgressQuest;

    public QuestData GetQuestData() => _questData;
    public int GetCompleteQuestIndex() => _nCompleteIndex;
    public int GetUnSolvedQuestID() => _unsolvedQuestId;

    

    private void Awake()
    {
        LoadData();
        NPCNameTextInit();
    }

    protected virtual void Start()
    {
        MarkInit();
        CheckQuest();

        PlayerController.OnSettingNPCQuest += this.CheckQuest;
    }

    protected virtual void NPCNameTextInit()
    {
        _npcNameTextMesh = Resources.Load<TextMesh>("Prefab/UI/NPCNameText");
        
        _npcNameTextMesh = Instantiate(_npcNameTextMesh, _markTr.position, Quaternion.identity);
        _npcNameTextMesh.transform.localScale = (Vector3.one * 0.1f);
        _npcNameTextMesh.transform.parent = this.transform;
        _npcNameTextMesh.text = $"{_sNPCName}";
        _npcNameTextMesh.transform.GetChild(0).GetComponent<TextMesh>().text = $"[{_sNPCJob}]";
    }

    /// <summary> 
    /// 퀘스트에 대한 감정마크를 미리 생성하여 활성화, 비활성화 방법으로 사용
    /// </summary>
    protected void MarkInit()
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
        markMoveCtr.SetYOffset(_markTr.position + Vector3.up);
        markMoveCtr = _exclamationMarkGO.GetComponent<MarkMovement>();
        markMoveCtr.SetYOffset(_markTr.position + Vector3.up);
    }

    public void CheckQuest()
    {
        this._possibleQuestList = _questSystem.GetPossibleQuest();
        this._InProgressQuestList = PlayerController.instance.GetPlayerQuestList();

        if (_InProgressQuestList.Count == 0)
        {
            _isCompleteQuest = false;
            _isInProgressQuest = false;
            _questionMarkGo.SetActive(false);
        }
        if (_possibleQuestList.Count == 0)
        {
            _isPossibleQuest = false;
            _exclamationMarkGO.SetActive(false);
        }


        foreach (var poQuest in _possibleQuestList)
        {
            for (int i = 0; i < _InProgressQuestList.Count; i++)
            {
                QuestData inProgressQuest = _InProgressQuestList[i];

                if (this._nID == inProgressQuest.nDestID)
                {
                    _isCompleteQuest = true;
                    _questionMarkGo.SetActive(true);
                    _exclamationMarkGO.SetActive(false);

                    _questData = inProgressQuest;
                    _nCompleteIndex = i;
                    break;
                }
            }

            if (this._nID == poQuest.nID)
            {
                if (!_InProgressQuestList.Contains(poQuest))
                {
                    _isPossibleQuest = true;
                    _exclamationMarkGO.SetActive(true);
                    _questData = poQuest;
                }
                else
                {
                    _isInProgressQuest = true; //진행중인 퀘스트인가
                    _exclamationMarkGO.SetActive(true);
                    _unsolvedQuestId = poQuest.nQuestID;
                }
                break;
            }

        }
    }

    protected virtual void LoadData()
    {
        //NPC의 ID는 1부터 시작.
        _npcData = SaveSys.LoadAllData().NPCDB[_nID];
        _nID = _npcData._nNPCID;
        _sNPCName = _npcData._sNPCName;
        _sNPCJob = _npcData._sNPCJob;
    }

    private void OnDestroy()
    {
        PlayerController.OnSettingNPCQuest -= this.CheckQuest;
    }
}

