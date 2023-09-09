using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerUIManager : MonoBehaviour
{
    public static PlayerUIManager instance = null;
    
    [Header("Level UI")]
    [SerializeField] TMP_Text levelText;
    private int m_curLevel;

    [Header("HP UI")]
    [SerializeField] private Image hpBarImage;
    [SerializeField] private RectTransform hpHandleRT;
    [SerializeField] private Text hpText;

    [Header("MP UI")]
    [SerializeField] private Image[] mpBarImage = new Image[2];
    [SerializeField] private RectTransform mpHandleRT;

    [Header("EXP UI")]
    [SerializeField] private Image expBarImage;
    [SerializeField] private RectTransform expHandleRT;
    [SerializeField] private Text expText;

    [Header("Player Info")]
    [SerializeField] private Text MaxHPText;
    [SerializeField] private Text MaxMPText;
    [SerializeField] private Text STRText;
    [SerializeField] private Text AttackSpeedText;
    [SerializeField] private Text AttackRangeText;

    [Header("Skill Info")]
    [SerializeField] private Image[] skill_Image           = new Image[6];
    [SerializeField] private Image[] skill_CoolTimeImage   = new Image[6];
    [SerializeField] private TMP_Text[] skill_CoolTimeText = new TMP_Text[6];

    private PlayerController playerCtr;

    private void Awake()
    {
        if(instance != null)
        {
            Destroy(this.gameObject);
        }
        instance = this;
        DontDestroyOnLoad(this.gameObject);

        for(int i = 0; i < skill_Image.Length; i++)
        {
            skill_CoolTimeText[i] = skill_CoolTimeImage[i].GetComponentInChildren<TMP_Text>();
        }
    }
    void Start()
    {
        playerCtr = PlayerController.instance;
    }

    public void SetHPbar(float _playerHP, float _playerMaxHP)
    {
        float amount = (_playerHP / _playerMaxHP);
        hpBarImage.fillAmount = amount;

        Vector2 vec;
        vec.x = amount;
        vec.y = 0;
        hpHandleRT.anchorMin = vec;
        vec.y = 1;
        hpHandleRT.anchorMax = vec;
        hpText.text = $"{_playerHP}/{_playerMaxHP}";
    }

    public void SetMPbar(float _playerMP, float _playerMaxMP)
    {
        float amount = (_playerMP / _playerMaxMP);
        mpBarImage[0].fillAmount = amount;
        mpBarImage[1].fillAmount = amount;

        Vector2 vec;
        vec.x = amount;
        vec.y = 0;
        mpHandleRT.anchorMin = vec;
        vec.y = 1;
        mpHandleRT.anchorMax = vec;
    }

    public void SetEXPbar(float _playerEXP, float _playerMaxEXP)
    {
        float amount = (_playerEXP / _playerMaxEXP);
        expBarImage.fillAmount = amount;

        Vector2 vec;
        vec.x = amount;
        vec.y = 0;
        expHandleRT.anchorMin = vec;
        vec.y = 1;
        expHandleRT.anchorMax = vec;
        expText.text = $"{_playerEXP}/{_playerMaxEXP} XP";
    }

    public IEnumerator SkillUsedWarring(int _idx)
    {
        WaitForSeconds waitTime = new WaitForSeconds(0.05f);

        for(int i = 0; i < 4; i++)
        {
            if (i % 2 == 0) skill_Image[_idx].color = Color.red;
            else skill_Image[_idx].color = Color.white;
            yield return waitTime;
        }

        yield break;
    }

    public IEnumerator StartSkillCoolTime(int _idx, float _coolTime, SkillStatus _curSkill)
    {
        _curSkill.SetInUse(true); // 현재 스킬은 사용 중으로 bool 값 상태

        skill_CoolTimeImage[_idx].gameObject.SetActive(true);
        skill_CoolTimeImage[_idx].fillAmount = 1;
        float fcoolTimeText = _coolTime;
        float t = 0.0f;

        while (0.0f < fcoolTimeText)
        {
            fcoolTimeText -= Time.deltaTime;
            t = Time.deltaTime / _coolTime;
            skill_CoolTimeImage[_idx].fillAmount -= t;

            skill_CoolTimeText[_idx].text = $"{(int)fcoolTimeText}";
            yield return null;
        }
        //쿨타임이 끝나면 Fill 이미지와 CoolTime Text 비활성화
        //스킬 사용 가능 상태로 변환
        skill_CoolTimeImage[_idx].gameObject.SetActive(false);
        _curSkill.SetInUse(false);
    }

    

    public void UpdateSkill_Image(List<SkillStatus> _skill_List)
    {
        //배운 스킬의 개수만큼 스킬 이미지 활성화
        int size = _skill_List.Count;
        for (int i = 0; i < size; i++)
        {
            skill_Image[i].sprite = _skill_List[i].GetSkill_Sprite();
            skill_Image[i].gameObject.SetActive(true);

            Debug.Log("Update Skill Image");
        }

    }

    public void DisplayInfo(int _level, float _maxHP, float _maxMP, float _str)
    {
        m_curLevel = _level;
        MaxHPText.text = $"최대체력 : {_maxHP}";
        MaxMPText.text = $"최대마나 : {_maxMP}";
        STRText.text = $"공격력 : {_str}";
        levelText.text = $"Lv.{m_curLevel}";
    }
}
