using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("-Skill Image-")]

    [SerializeField]
    private Image skill1;
    private float skill1CoolTime;
    [SerializeField]
    private Image skill2;
    private float skill2CoolTime;
    [SerializeField]
    private Image skill3;
    private float skill3CoolTime;

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
        
    }

    private IEnumerator SkillCoolTime()
    {
        while(true)
        {
            if (skill1CoolTime > 1) skill1CoolTime -= 0.1f;
            if (skill2CoolTime > 1) skill2CoolTime -= 0.1f;
            if (skill3CoolTime > 1) skill3CoolTime -= 0.1f;

            skill1Amount.fillAmount = 1 / skill1CoolTime;
            skill2Amount.fillAmount = 1 / skill2CoolTime;
            skill3Amount.fillAmount = 1 / skill3CoolTime;

            yield return new WaitForSeconds(0.1f);
        }
    }

    public void SetSkillImage(Sprite _skill1, Sprite _skill2, Sprite _skill3)
    {
        skill1.sprite = _skill1;
        skill2.sprite = _skill2;
        skill3.sprite = _skill3;
    }

    public void SetSkillCoolTime(float _skill1, float _skill2, float _skill3)
    {
        skill1CoolTime = _skill1;
        skill2CoolTime = _skill2;
        skill3CoolTime = _skill3;
    }
}
