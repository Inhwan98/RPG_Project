using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogSystem
{
    DialogData[] _dialogDatas;
    List<DialogData> currentDialog = new List<DialogData>();


    /// <summary>  생성과 동시에 데이터를 불러 온다. </summary>
    public DialogSystem(DialogData[] dialogDatas)
    {
        this._dialogDatas = dialogDatas;
    }


    /// <summary>
    /// Dialog List의 데이터를 ID 매개변수를 기준으로
    /// 정리해서 return 한다.
    /// </summary>
    public List<DialogData> GetDialogDataList(int questID, int brach)
    {
        currentDialog.Clear();

        int size = _dialogDatas.Length;

        for (int i = 0; i < size; i++)
        {
            //퀘스트의 ID와 브랜치가 같은 다이얼로그만 출력하게 한다.
            if(_dialogDatas[i].nQuestID == questID)
            {
                if(_dialogDatas[i].nBranch == brach)
                {
                    currentDialog.Add(_dialogDatas[i]);
                }
            }
        }
        return currentDialog;
    }
}
