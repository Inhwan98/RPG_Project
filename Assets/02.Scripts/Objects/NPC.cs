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

/*
    [상속 구조]

    NPC : NPC에 대한 이름, 정보, 퀘스트 정보를 지니고 있다.
     - MerChantNPC : 상인 NPC에 대한 정보를 추가로 갖는다. (ex - 판매할 아이템 목록)
*/

public class NPC : MonoBehaviour
{
    /*******************************************
     *              Fields
     *******************************************/

    #region option Fields
    [SerializeField] private Transform _markTr;
    [SerializeField] private int _nID;
    #endregion

    #region private Fields
    private GameObject _questionMarkGo;
    private GameObject _exclamationMarkGO;

    private List<QuestData> _possibleQuestList;
    private List<QuestData> _InProgressQuestList;

    private QuestSystem _questSystem;
    private QuestData _questData;

    private string _sNPCName;
    private string _sNPCJob;

    private bool _isPossibleQuest;
    private bool _isCompleteQuest;
    private bool _isInProgressQuest;

    private int _unsolvedQuestId;
    private int _nCompleteIndex;
    #endregion

    #region protected Fields

    protected ResourcesData _resourcesData;
    protected TextMesh _npcNameTextMesh;
    protected NPCData _npcData;

    #endregion

    /*******************************************
    *              Get, Set Methods
    *******************************************/

    #region Get Methods
    public int GetID() => _nID;
    public bool GetIsPossibleQuest() => _isPossibleQuest;
    public bool GetIsCompleteQuest() => _isCompleteQuest;
    public bool GetIsInProgressQuest() => _isInProgressQuest;
    public QuestData GetQuestData() => _questData;
    public int GetCompleteQuestIndex() => _nCompleteIndex;
    public int GetUnSolvedQuestID() => _unsolvedQuestId;
    #endregion

    #region Set Methods
    public int SetID(int nID) => this._nID = nID;
    #endregion

    /*******************************************
    *                Unity Evenet
    *******************************************/
    #region Unity Event
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
    private void OnDestroy()
    {
        PlayerController.OnSettingNPCQuest -= this.CheckQuest;
    }

    #endregion

    /*******************************************
    *                Methods
    *******************************************/
    #region virtual Methods
    /// <summary> 화면상에서 보일 NPC의 Name Text 세팅 </summary>
    protected virtual void NPCNameTextInit()
    {
        _npcNameTextMesh = Resources.Load<TextMesh>("Prefab/UI/NPCNameText");
        
        _npcNameTextMesh = Instantiate(_npcNameTextMesh, _markTr.position, Quaternion.identity);
        _npcNameTextMesh.transform.localScale = (Vector3.one * 0.1f);
        _npcNameTextMesh.transform.parent = this.transform;
        _npcNameTextMesh.text = $"{_sNPCName}";
        _npcNameTextMesh.transform.GetChild(0).GetComponent<TextMesh>().text = $"[{_sNPCJob}]";
    }
    /// <summary> NPC의 데이터 로드 </summary>
    protected virtual void LoadData()
    {
        //NPC의 ID는 1부터 시작.
        _npcData = SaveSys.LoadAllData().NPCDB[_nID];
        _nID = _npcData._nNPCID;
        _sNPCName = _npcData._sNPCName;
        _sNPCJob = _npcData._sNPCJob;
    }
    #endregion

    #region protected Methods
    /// <summary> 
    /// 퀘스트에 대한 감정마크를 미리 생성하여 활성화, 비활성화 방법으로 사용
    /// </summary>
    protected void MarkInit()
    {
        _questSystem = GameManager.instance.GetQuestSystem();
        _resourcesData = GameManager.instance.GetResourcesData();

        _questionMarkGo = _resourcesData.GetQuestionMarkGo();
        _exclamationMarkGO = _resourcesData.GetExclamationMarkGO();

        //도널드촌장 전용
        if(_nID == 1)
        {
            _questionMarkGo.transform.localScale = Vector3.one * 5.0f;
            _exclamationMarkGO.transform.localScale = Vector3.one * 5.0f;
        }
        else
        {
            _questionMarkGo.transform.localScale = Vector3.one * 1.5f;
            _exclamationMarkGO.transform.localScale = Vector3.one * 1.5f;
        }
       
        _questionMarkGo.SetActive(false);
        _exclamationMarkGO.SetActive(false);

        _questionMarkGo = Instantiate(_questionMarkGo, this.transform);
        _exclamationMarkGO = Instantiate(_exclamationMarkGO, this.transform);

        var markMoveCtr = _questionMarkGo.GetComponent<MarkMovement>();
        markMoveCtr.SetYOffset(_markTr.position + Vector3.up);
        markMoveCtr = _exclamationMarkGO.GetComponent<MarkMovement>();
        markMoveCtr.SetYOffset(_markTr.position + Vector3.up);
    }
    #endregion

    #region public Methods
    /// <summary> 필요 시 마다 NPC의 Quest를 Update함 </summary>
    public void CheckQuest()
    {
        this._possibleQuestList = _questSystem.GetPossibleQuest();
        this._InProgressQuestList = PlayerController.instance.GetPlayerQuestList();

        QuestReset();

        //1. 수행가능한 전체 퀘스트 목록 탐색
        foreach (var poQuest in _possibleQuestList)
        {
            //2. 진행 중인 퀘스트 안에서
            for (int i = 0; i < _InProgressQuestList.Count; i++)
            {
                QuestData inProgressQuest = _InProgressQuestList[i];

                //2-1. 목표ID와 NPCID가 같다면 완료 처리
                if (this._nID == inProgressQuest.nDestID)
                {
                    _isCompleteQuest = true;
                    _questionMarkGo.SetActive(true);
                    _exclamationMarkGO.SetActive(false);

                    _questData = inProgressQuest;
                    _nCompleteIndex = i;
                    return;
                }
            }

            //3. 퀘스트시작 ID와 NPCID가 같다면
            if (this._nID == poQuest.nID)
            {
                //3-1. 진행중인 퀘스트에 포함되어 있지 않다면
                if (!_InProgressQuestList.Contains(poQuest))
                {
                    //3-2. 가능 상태
                    _isPossibleQuest = true;
                    _exclamationMarkGO.SetActive(true);
                    _questData = poQuest;
                }
                //4.1 포함되어 있다면 진행중인 상태로 수정
                else
                {
                    _isInProgressQuest = true; //진행중인 퀘스트인가
                    _exclamationMarkGO.SetActive(true);
                    _unsolvedQuestId = poQuest.nQuestID;
                }
                break;
            }
        }

        //매번 설정을 0으로 초기화하고, 이후에 상태를 점검
        void QuestReset()
        {
            _isCompleteQuest = false;
            _isPossibleQuest = false;
            _isInProgressQuest = false;
            _questionMarkGo.SetActive(false);
            _exclamationMarkGO.SetActive(false);
        }
    }
    #endregion

}
