using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public static class RandomTr
{
    /// <summary>
    /// 특정 위치 주변으로 랜덤한 위치값을 반환한다.
    /// </summary>
    /// <param name="tr"> 주변에 랜덤 위치값을 가져올 기준 위치 </param>
    /// <param name="distance"> 기준점에서 떨어질 거리 </param>
    public static Vector3 GetRandomPos(Transform tr, float distance)
    {
        float randRotNum = Random.Range(0, 360.0f);
        Quaternion randRot = Quaternion.Euler(tr.up * randRotNum);

        //생성 스테이지 주변으로 respawnDistance 만큼 떨어짐. 각도는 랜덤
        Vector3 randPos = tr.rotation * randRot * (tr.forward * distance) + tr.position;

        return randPos;
    }
}


