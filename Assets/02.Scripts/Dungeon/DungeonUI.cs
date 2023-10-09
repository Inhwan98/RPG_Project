using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Text;

public class DungeonUI : MonoBehaviour
{
    [SerializeField] private TMP_Text _dungeonName;
    [SerializeField] private TMP_Text _dungeonProgress;
    [SerializeField] private TMP_Text _disCriptions;



    public void UpdateDungeonUI(StageData stagedata, string dungeonName)
    {
        _dungeonName.text = dungeonName;

        //보스가 아니라면 일반 스테이지
     
        _dungeonProgress.text = $"<color=purple>[던전]</color> 스테이지 진행도 ({stagedata.nCurProgressNum}/{stagedata.nMaxProgressNum})";

        if (stagedata.nDeadMonsterCount >= stagedata.nMonsterAmount)
        {
            _disCriptions.text = $"<color=grey> -{stagedata.sMonsterName} 처치 완료";
        }
        else
        {
            _disCriptions.text = $" -{stagedata.sMonsterName} 처치 ({stagedata.nDeadMonsterCount}/{stagedata.nMonsterAmount})";
        }

    }
}
