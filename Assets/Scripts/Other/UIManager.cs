using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("-Skill Image-")]

    [SerializeField]
    private Image mainSKill;
    private float mainSkillCoolTime = 1;
    private float mainSkillCurrentTime=0;
    [SerializeField]
    private Image subSkill;
    private float subSkillCoolTime = 1;
    private float subSkillCurrentTime=0;
    [SerializeField]
    private Image subAttack;
    private float subAttackCoolTime = 1;
    private float subAttackCurrentTime=0;

    [Space, Header("-Skill AmountImage-")]
    [SerializeField]
    private Image skill1Amount;
    [SerializeField]
    private Image skill2Amount;
    [SerializeField]
    private Image skill3Amount;

    public static UIManager instance;


    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        StartCoroutine(SkillCoolTime());
    }

    private IEnumerator SkillCoolTime()
    {
        while(true)
        {
            if (mainSkillCurrentTime > 0) mainSkillCurrentTime -= 0.1f;
            if (subSkillCurrentTime > 0) subSkillCurrentTime -= 0.1f;
            if (subAttackCurrentTime > 0) subAttackCurrentTime -= 0.1f;

            skill1Amount.fillAmount = mainSkillCurrentTime / mainSkillCoolTime;
            skill2Amount.fillAmount = subSkillCurrentTime / subSkillCoolTime;
            skill3Amount.fillAmount = subAttackCurrentTime / subAttackCoolTime;

            yield return new WaitForSeconds(0.1f);
        }
    }

    public void SetSkillImage(Sprite _skill1, Sprite _skill2, Sprite _skill3)
    {
        mainSKill.sprite = _skill1;
        subSkill.sprite = _skill2;
        subAttack.sprite = _skill3;
    }

    public void SetSkillCoolTime(float _mainSkill, float _subSkill, float _subAttack)
    {
        mainSkillCoolTime = _mainSkill;
        subSkillCoolTime = _subSkill;
        subAttackCoolTime = _subAttack;
    }

    public void SetMainSkillCoolTime()
    {
        mainSkillCurrentTime = mainSkillCoolTime;
    }

    public void SetSubSkillCoolTime()
    {
        subSkillCurrentTime = subSkillCoolTime;
    }

    public void SetSubAttackCoolTime( )
    {
        subAttackCurrentTime = subAttackCoolTime;
    }
}
