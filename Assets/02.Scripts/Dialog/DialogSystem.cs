using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public struct DialogDataStruct
{
    public string name; //캐릭터 이름
    [TextArea(3, 5)]
    public string dialogue; //대사
}

public class DialogSystem : MonoBehaviour
{
    [SerializeField]
    private int branch; //대사의 분기 변수

    [Header("Dialog")]
    [SerializeField]
    private GameObject dialogPanel;
    [SerializeField]
    private TMP_Text textName; //현재 대사중인 캐릭터 이름 출력
    [SerializeField]
    private TMP_Text textDialogue; //현재 대사 출력 Text UI


    //private DialogDataStruct[] dialogs;

    [SerializeField]
    private bool isAutoStart = true; //자동 시작 여부
    private bool isFirst = true; //최초 1회만 호출하기 위한 변수
    private int currentDialogIndex = -1; //현재 대사 순번

    private float typingSpeed = 0.05f;  //텍스트 타이핑 효과의 재생 속도
    private bool isTypingEffect = false; //텍스트 타이핑 효과를 재생중인지

    DialogData[] dialogDatas;
    List<DialogData> currentDialog = new List<DialogData>();

    private void Awake()
    {
        dialogDatas = SaveSys.LoadDialogData().Dialog;


        dialogPanel.SetActive(false);
    }

    public void SetDialogData(int nQuestID)
    {
        currentDialog.Clear();

        int size = dialogDatas.Length;

        for (int i = 0; i < size; i++)
        {
            if(dialogDatas[i].nQuestID == nQuestID)
            {
                currentDialog.Add(dialogDatas[i]);
            }
        }
    }

    public bool UpdateDialog()
    {
        // 대사 분기가 시작될 때 1회만 호출
        if (isFirst == true)
        {
            //초기화. 캐릭터 이미지는 활성화하고, 대사 관련 UI는 모두 비활성화
            dialogPanel.SetActive(false);

            // 자동 재생 (isAutoStat= true) 으로 설정되어 있으면 첫 번째 대사 재생
            if (isAutoStart) SetNextDialog();

            isFirst = false;
        }

        
        if (Input.anyKeyDown)
        {
            //텍스트 타이핑 효과를 재생중일 때 마우스 왼쪽 클릭하면 타이핑 효과 종료
            if (isTypingEffect == true)
            {
                isTypingEffect = false;

                //타이핑 효과를 중지하고, 현재 대사 전체를 출력한다
                StopCoroutine("OnTypingText");


                textDialogue.text = currentDialog[currentDialogIndex].sDialog;
                //대사가 완료되었을 때 출력되는 커서 활성화

                return false;
            }

            //if (dialogDatas[currentDialogIndex + 1].nBranch != branch)
            //{
            //    dialogPanel.SetActive(false);
            //    return true;
            //}

            //대사가 남아있을 경우 대사 진행
            if (currentDialog.Count > currentDialogIndex + 1)
            {
                SetNextDialog();
            }
            //대사가 더이상 없을 경우 모든 오브젝트를 비활성화하고 true  반환
            else
            {
                dialogPanel.SetActive(false);

                return true;
            }             
        }
        return false;
    }

    private void SetNextDialog()
    {
        //다음 대사를 진행하도록
        currentDialogIndex++;

        dialogPanel.SetActive(true);

        //현재 화자 이름 텍스트 설정
        textName.text = currentDialog[currentDialogIndex].sName;

        StartCoroutine("OnTypingText");
    }

    private IEnumerator OnTypingText()
    {
        int index = 0;

        isTypingEffect = true;

        //텍스트를 한글자씩 타이핑치듯 재생
        while (index < currentDialog[currentDialogIndex].sDialog.Length)
        {
            textDialogue.text = currentDialog[currentDialogIndex].sDialog.Substring(0, index);

            index++;

            yield return new WaitForSeconds(typingSpeed);
        }

        isTypingEffect = false;

        // 대사가 완료되었을 때 출력되는 커서 활성화
    }

    public void SetDialog(DialogData[] datas)
    {
        dialogDatas = datas;
    }
}
