using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public struct DialogDataStruct
{
    public string name; //ĳ���� �̸�
    [TextArea(3, 5)]
    public string dialogue; //���
}

public class DialogSystem : MonoBehaviour
{
    [SerializeField]
    private int branch; //����� �б� ����

    [Header("Dialog")]
    [SerializeField]
    private GameObject dialogPanel;
    [SerializeField]
    private TMP_Text textName; //���� ������� ĳ���� �̸� ���
    [SerializeField]
    private TMP_Text textDialogue; //���� ��� ��� Text UI


    //private DialogDataStruct[] dialogs;

    [SerializeField]
    private bool isAutoStart = true; //�ڵ� ���� ����
    private bool isFirst = true; //���� 1ȸ�� ȣ���ϱ� ���� ����
    private int currentDialogIndex = -1; //���� ��� ����

    private float typingSpeed = 0.05f;  //�ؽ�Ʈ Ÿ���� ȿ���� ��� �ӵ�
    private bool isTypingEffect = false; //�ؽ�Ʈ Ÿ���� ȿ���� ���������

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
        // ��� �бⰡ ���۵� �� 1ȸ�� ȣ��
        if (isFirst == true)
        {
            //�ʱ�ȭ. ĳ���� �̹����� Ȱ��ȭ�ϰ�, ��� ���� UI�� ��� ��Ȱ��ȭ
            dialogPanel.SetActive(false);

            // �ڵ� ��� (isAutoStat= true) ���� �����Ǿ� ������ ù ��° ��� ���
            if (isAutoStart) SetNextDialog();

            isFirst = false;
        }

        
        if (Input.anyKeyDown)
        {
            //�ؽ�Ʈ Ÿ���� ȿ���� ������� �� ���콺 ���� Ŭ���ϸ� Ÿ���� ȿ�� ����
            if (isTypingEffect == true)
            {
                isTypingEffect = false;

                //Ÿ���� ȿ���� �����ϰ�, ���� ��� ��ü�� ����Ѵ�
                StopCoroutine("OnTypingText");


                textDialogue.text = currentDialog[currentDialogIndex].sDialog;
                //��簡 �Ϸ�Ǿ��� �� ��µǴ� Ŀ�� Ȱ��ȭ

                return false;
            }

            //if (dialogDatas[currentDialogIndex + 1].nBranch != branch)
            //{
            //    dialogPanel.SetActive(false);
            //    return true;
            //}

            //��簡 �������� ��� ��� ����
            if (currentDialog.Count > currentDialogIndex + 1)
            {
                SetNextDialog();
            }
            //��簡 ���̻� ���� ��� ��� ������Ʈ�� ��Ȱ��ȭ�ϰ� true  ��ȯ
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
        //���� ��縦 �����ϵ���
        currentDialogIndex++;

        dialogPanel.SetActive(true);

        //���� ȭ�� �̸� �ؽ�Ʈ ����
        textName.text = currentDialog[currentDialogIndex].sName;

        StartCoroutine("OnTypingText");
    }

    private IEnumerator OnTypingText()
    {
        int index = 0;

        isTypingEffect = true;

        //�ؽ�Ʈ�� �ѱ��ھ� Ÿ����ġ�� ���
        while (index < currentDialog[currentDialogIndex].sDialog.Length)
        {
            textDialogue.text = currentDialog[currentDialogIndex].sDialog.Substring(0, index);

            index++;

            yield return new WaitForSeconds(typingSpeed);
        }

        isTypingEffect = false;

        // ��簡 �Ϸ�Ǿ��� �� ��µǴ� Ŀ�� Ȱ��ȭ
    }

    public void SetDialog(DialogData[] datas)
    {
        dialogDatas = datas;
    }
}
