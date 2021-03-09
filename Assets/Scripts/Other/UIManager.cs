using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("-Player HP-")]
    [SerializeField]
    private Image hpBar;
    [SerializeField]
    private Image hpBarBG;
    [SerializeField]
    private float speed;
    private float playerMaxHP = 1;
    private float playerCurrentHP = 1;
    private float playerCurrentHPBG;
    private bool isHit;

    [Space, Header("-Skill Image-")]
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

    public void PlayerHit(float _currentHP)
    {
        playerCurrentHP = _currentHP;
        hpBar.fillAmount = playerCurrentHP / playerMaxHP;
        if (!isHit) StartCoroutine(IEnumAmoutHP());
    }

    private IEnumerator IEnumAmoutHP()
    {
        isHit = true;
        yield return new WaitForSeconds(1.0f);

        while (playerCurrentHPBG > playerCurrentHP)
        {
            playerCurrentHPBG -= speed * Time.deltaTime;
            hpBarBG.fillAmount = playerCurrentHPBG / playerMaxHP;

            yield return null;
        }

        playerCurrentHPBG = playerCurrentHP;
        isHit = false;
    }

    public void SetMainSkill(ISkill _skill)
    {
        mainSKill.sprite = _skill.GetImage();
        mainSkillCoolTime = _skill.GetCoolTime();
    }

    public void SetSubSkill(ISkill _skill)
    {
        subSkill.sprite = _skill.GetImage();
        subSkillCoolTime = _skill.GetCoolTime();
    }

    public void SetSubAttack(ISkill _skill)
    {
        subAttack.sprite = _skill.GetImage();
        subAttackCoolTime = _skill.GetCoolTime();
    }

    public void SetMainSkillCoolTime(bool active)
    {
        Debug.Log("UIManager MainSkillCoolTime");
        if (!active || mainSkillCurrentTime > 0) { Debug.Log("UIManager MainSkillCoolTime Return"); return; }

        Debug.Log("UIManager MainSkillCoolTime Start");
        mainSkillCurrentTime = mainSkillCoolTime;
    }

    public void SetSubSkillCoolTime(bool active)
    {
        if (!active || subSkillCurrentTime > 0) return;
        subSkillCurrentTime = subSkillCoolTime;
    }

    public void SetSubAttackCoolTime(bool active)
    {
        Debug.Log("UIManager SubAttackCoolTime");
        if (!active || subAttackCurrentTime > 0) { Debug.Log("UIManager SubAttackCoolTime Return"); return; }

        Debug.Log("UIManager SubAttackCoolTime Start");
        subAttackCurrentTime = subAttackCoolTime;
    }

    public void SetPlayerMaxHPBar(float _hp)
    {
        playerMaxHP = _hp;
        playerCurrentHP = _hp;
        playerCurrentHPBG = _hp;
    }
}
