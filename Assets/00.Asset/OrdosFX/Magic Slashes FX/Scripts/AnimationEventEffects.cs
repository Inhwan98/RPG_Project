using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEventEffects : MonoBehaviour {
    //public GameObject EffectPrefab;
    //public Transform EffectStartPosition;
    //public float DestroyAfter = 10;
    //[Space]
    //public GameObject EffectPrefabWorldSpace;
    //public Transform EffectStartPositionWorld;
    //public float DestroyAfterWorld = 10;

    public EffectInfo[] Effects;

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

    //   // Update is called once per frame
    //   void CreateEffect () {
    //       var effectOBJ = Instantiate(EffectPrefab, EffectStartPosition);
    //       effectOBJ.transform.localPosition = Vector3.zero;
    //       Destroy(effectOBJ, DestroyAfter);        		
    //}

    //   void CreateEffectWorldSpace()
    //   {
    //       var effectOBJ = Instantiate(EffectPrefabWorldSpace, EffectStartPositionWorld.transform.position, EffectStartPositionWorld.transform.rotation);

    //       Destroy(effectOBJ, DestroyAfterWorld);
    //   }
    void Start() {
    }
            
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
