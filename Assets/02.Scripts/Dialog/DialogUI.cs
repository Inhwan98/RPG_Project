using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogUI : MonoBehaviour
{
    [Header("Dialog")]
    [SerializeField]
    private GameObject _dialogPanel;
    [SerializeField]
    private TMP_Text _textName; //현재 대사중인 캐릭터 이름 출력
    [SerializeField]
    private TMP_Text _textDialogue; //현재 대사 출력 Text UI


    [SerializeField]
    private bool _isAutoStart = true; //자동 시작 여부

    private bool _isFirst = true; //최초 1회만 호출하기 위한 변수
    private int _currentDialogIndex = -1; //현재 대사 순번

    private float _typingSpeed = 0.05f;  //텍스트 타이핑 효과의 재생 속도
    private bool _isTypingEffect = false; //텍스트 타이핑 효과를 재생중인지

    private DialogSystem _dialogSystem;
    private List<DialogData> _currentDialog;

    private void Awake()
    {
        _dialogPanel.SetActive(false);
    }

    private void Start()
    {
        _dialogSystem = GameManager.instance.GetDialogSystem();
    }

    public void SetDialogList(int questID, int branch)
    {
        _currentDialog = _dialogSystem.GetDialogDataList(questID, branch);
    }


    public bool UpdateDialog()
    {
        // 대사 분기가 시작될 때 1회만 호출
        if (_isFirst == true)
        {
            //초기화. 캐릭터 이미지는 활성화하고, 대사 관련 UI는 모두 비활성화
            _dialogPanel.SetActive(false);

            // 자동 재생 (isAutoStat= true) 으로 설정되어 있으면 첫 번째 대사 재생
            if (_isAutoStart) SetNextDialog();

            _isFirst = false;
        }

        
        if (Input.anyKeyDown)
        {
            //텍스트 타이핑 효과를 재생중일 때 마우스 왼쪽 클릭하면 타이핑 효과 종료
            if (_isTypingEffect == true)
            {
                _isTypingEffect = false;

                //타이핑 효과를 중지하고, 현재 대사 전체를 출력한다
                StopCoroutine("OnTypingText");


                _textDialogue.text = _currentDialog[_currentDialogIndex].sDialog;
                //대사가 완료되었을 때 출력되는 커서 활성화

                return false;
            }

            //if (dialogDatas[currentDialogIndex + 1].nBranch != branch)
            //{
            //    dialogPanel.SetActive(false);
            //    return true;
            //}

            //대사가 남아있을 경우 대사 진행
            if (_currentDialog.Count > _currentDialogIndex + 1)
            {
                SetNextDialog();
            }
            //대사가 더이상 없을 경우 모든 오브젝트를 비활성화하고 true  반환
            else
            {
                _dialogPanel.SetActive(false);
                _currentDialogIndex = -1;
                _isFirst = true;
                return true;
            }             
        }
        return false;
    }

    private void SetNextDialog()
    {
        //다음 대사를 진행하도록
        _currentDialogIndex++;

        _dialogPanel.SetActive(true);

        //현재 화자 이름 텍스트 설정
        _textName.text = _currentDialog[_currentDialogIndex].sName;

        StartCoroutine("OnTypingText");
    }

    private IEnumerator OnTypingText()
    {
        int index = 0;

        _isTypingEffect = true;

        //텍스트를 한글자씩 타이핑치듯 재생
        while (index < _currentDialog[_currentDialogIndex].sDialog.Length)
        {
            _textDialogue.text = _currentDialog[_currentDialogIndex].sDialog.Substring(0, index);

            index++;

            yield return new WaitForSeconds(_typingSpeed);
        }

        _isTypingEffect = false;

        // 대사가 완료되었을 때 출력되는 커서 활성화
    }


}
