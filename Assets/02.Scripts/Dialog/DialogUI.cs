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
    private TMP_Text _textName; //���� ������� ĳ���� �̸� ���
    [SerializeField]
    private TMP_Text _textDialogue; //���� ��� ��� Text UI


    [SerializeField]
    private bool _isAutoStart = true; //�ڵ� ���� ����

    private bool _isFirst = true; //���� 1ȸ�� ȣ���ϱ� ���� ����
    private int _currentDialogIndex = -1; //���� ��� ����

    private float _typingSpeed = 0.05f;  //�ؽ�Ʈ Ÿ���� ȿ���� ��� �ӵ�
    private bool _isTypingEffect = false; //�ؽ�Ʈ Ÿ���� ȿ���� ���������

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
        // ��� �бⰡ ���۵� �� 1ȸ�� ȣ��
        if (_isFirst == true)
        {
            //�ʱ�ȭ. ĳ���� �̹����� Ȱ��ȭ�ϰ�, ��� ���� UI�� ��� ��Ȱ��ȭ
            _dialogPanel.SetActive(false);

            // �ڵ� ��� (isAutoStat= true) ���� �����Ǿ� ������ ù ��° ��� ���
            if (_isAutoStart) SetNextDialog();

            _isFirst = false;
        }

        
        if (Input.anyKeyDown)
        {
            //�ؽ�Ʈ Ÿ���� ȿ���� ������� �� ���콺 ���� Ŭ���ϸ� Ÿ���� ȿ�� ����
            if (_isTypingEffect == true)
            {
                _isTypingEffect = false;

                //Ÿ���� ȿ���� �����ϰ�, ���� ��� ��ü�� ����Ѵ�
                StopCoroutine("OnTypingText");


                _textDialogue.text = _currentDialog[_currentDialogIndex].sDialog;
                //��簡 �Ϸ�Ǿ��� �� ��µǴ� Ŀ�� Ȱ��ȭ

                return false;
            }

            //if (dialogDatas[currentDialogIndex + 1].nBranch != branch)
            //{
            //    dialogPanel.SetActive(false);
            //    return true;
            //}

            //��簡 �������� ��� ��� ����
            if (_currentDialog.Count > _currentDialogIndex + 1)
            {
                SetNextDialog();
            }
            //��簡 ���̻� ���� ��� ��� ������Ʈ�� ��Ȱ��ȭ�ϰ� true  ��ȯ
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
        //���� ��縦 �����ϵ���
        _currentDialogIndex++;

        _dialogPanel.SetActive(true);

        //���� ȭ�� �̸� �ؽ�Ʈ ����
        _textName.text = _currentDialog[_currentDialogIndex].sName;

        StartCoroutine("OnTypingText");
    }

    private IEnumerator OnTypingText()
    {
        int index = 0;

        _isTypingEffect = true;

        //�ؽ�Ʈ�� �ѱ��ھ� Ÿ����ġ�� ���
        while (index < _currentDialog[_currentDialogIndex].sDialog.Length)
        {
            _textDialogue.text = _currentDialog[_currentDialogIndex].sDialog.Substring(0, index);

            index++;

            yield return new WaitForSeconds(_typingSpeed);
        }

        _isTypingEffect = false;

        // ��簡 �Ϸ�Ǿ��� �� ��µǴ� Ŀ�� Ȱ��ȭ
    }


}
