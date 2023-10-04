using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogSystem
{
    DialogData[] _dialogDatas;
    List<DialogData> currentDialog = new List<DialogData>();


    /// <summary>  ������ ���ÿ� �����͸� �ҷ� �´�. </summary>
    public DialogSystem(DialogData[] dialogDatas)
    {
        this._dialogDatas = dialogDatas;
    }


    /// <summary>
    /// Dialog List�� �����͸� ID �Ű������� ��������
    /// �����ؼ� return �Ѵ�.
    /// </summary>
    public List<DialogData> GetDialogDataList(int questID, int brach)
    {
        currentDialog.Clear();

        int size = _dialogDatas.Length;

        for (int i = 0; i < size; i++)
        {
            //����Ʈ�� ID�� �귣ġ�� ���� ���̾�α׸� ����ϰ� �Ѵ�.
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
