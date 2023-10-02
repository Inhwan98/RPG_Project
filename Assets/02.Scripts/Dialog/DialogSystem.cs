using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogSystem
{
    DialogData[] dialogDatas;
    List<DialogData> currentDialog = new List<DialogData>();


    /// <summary>  ������ ���ÿ� �����͸� �ҷ� �´�. </summary>
    public DialogSystem()
    {
        dialogDatas = SaveSys.LoadAllData().DialogDB;
    }


    /// <summary>
    /// Dialog List�� �����͸� ID �Ű������� ��������
    /// �����ؼ� return �Ѵ�.
    /// </summary>
    public List<DialogData> GetDialogDataList(int questID, int brach)
    {
        currentDialog.Clear();

        int size = dialogDatas.Length;

        for (int i = 0; i < size; i++)
        {
            //����Ʈ�� ID�� �귣ġ�� ���� ���̾�α׸� ����ϰ� �Ѵ�.
            if(dialogDatas[i].nQuestID == questID)
            {
                if(dialogDatas[i].nBranch == brach)
                {
                    currentDialog.Add(dialogDatas[i]);
                }
            }
        }
        return currentDialog;
    }
}
