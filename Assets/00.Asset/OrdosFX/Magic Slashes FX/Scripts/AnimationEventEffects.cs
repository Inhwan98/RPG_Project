using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EffectInfo
{
    [SerializeField] private string name;
    [SerializeField] private GameObject effectObj;
    [SerializeField] private Transform startPositionRotation;
    [SerializeField] private float destroyAfter = 10;
    [SerializeField] private bool useLocalPosition = true;

    public GameObject GetEffect() => effectObj;

    public Transform GetStartTr() => startPositionRotation;
    public float GetDestroyAfter() => destroyAfter;
    public bool GetUseLocalPos() => useLocalPosition;
}


public class AnimationEventEffects : MonoBehaviour
{
    [SerializeField] private EffectInfo[] Effects;

    void InstantiateEffect(int EffectNumber)
    {
        if(Effects == null || Effects.Length <= EffectNumber)
        {
            Debug.Log(EffectNumber);
            Debug.LogError("Incorrect effect number or effect is null");
        }

        var instance = Instantiate(Effects[EffectNumber].GetEffect(), Effects[EffectNumber].GetStartTr().position, Effects[EffectNumber].GetStartTr().rotation);

        if (Effects[EffectNumber].GetUseLocalPos())
        {
            instance.transform.parent = Effects[EffectNumber].GetStartTr().transform;
            instance.transform.localPosition = Vector3.zero;
            instance.transform.localRotation = new Quaternion();
        }
        Destroy(instance, Effects[EffectNumber].GetDestroyAfter());
    }
}
