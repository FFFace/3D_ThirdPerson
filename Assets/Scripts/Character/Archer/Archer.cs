using System.Collections;
using TMPro;
using UnityEngine;

public class Archer : Character
{
    private ArcherUpdate archerUpdate;
    private ArcherKick archerKick;
    private ArcherArrowActive arrowActive;

    private ISkill skill1;
    private ISkill skill2;

    protected override void InitData()
    {
        base.InitData();

        state.hp = 5;
        state.attackDamage = 1;
        state.attackSpeed = 1;
        state.moveSpeed = 5;
        state.jumpPower = 5;

        currentMoveSpeed = state.moveSpeed;
        currentAttackDamage = state.attackDamage;
        currentAttackSpeed = state.attackSpeed;
        currentJumpPower = state.jumpPower;
        currentCharacterHP = state.hp;
        currentDefense = state.defense;
        currentKnockBackPower = 0;

        subAttackCoolTime = 3.0f;

        ArcherRangeAttack attack = new ArcherRangeAttack(this);
        archerKick = transform.Find("KickDistance").GetComponent<ArcherKick>();

        Transform bowStringTR = GameObject.Find("WB.string").transform;
        Transform rightHandTR = GameObject.Find("FollowStringTR").transform;

        archerUpdate = new ArcherUpdate(bowStringTR, rightHandTR, modelArrow);
        nomalAttack = attack;
        subAttack = new ArcherSubAttack(archerUpdate);
        characterUpdate = archerUpdate;
        recharge = new ArcherRecharge(this, rechargeTime);
        mainSkill = new ArcherSkillArrow(10.0f, 1.5f);
        subSkill = new ArcherSkillArrow(15.0f, 1.25f);

        arrowActive = new ArcherArrowActive(this, mainSkill, subSkill);
        activeArrow = arrowActive;
    }

    public void BowStringPull()
    {
        archerUpdate.isStringPull = true;
    }

    public void BowStringPullOut()
    {
        archerUpdate.isStringPull = false;
    }

    public void KickColliderEnable()
    {
        archerKick.ColliderEnable();
    }

    public void KickColliderDisable()
    {
        archerKick.ColliderDisable();
    }

    public override void AttackEnd()
    {
        base.AttackEnd();
        StartCoroutine(mainSkill.SkillCoolTime());
        StartCoroutine(subSkill.SkillCoolTime());
    }
}

public class ArcherRangeAttack : IAttackAction
{
    private Archer archer;

    public ArcherRangeAttack(Archer _archer) { archer = _archer;}

    public virtual void Attack()
    {
        archer.SetAnimationBool("Recharge", false);
    }
}

public class ArcherRecharge : IRecharge
{
    protected Archer archer;
    private float rechargeTime;
    private float currentRechargeTime;

    public ArcherRecharge(Archer _archer, float _time) { archer = _archer; rechargeTime = _time; currentRechargeTime = 0; }

    public void Recharge()
    {
        currentRechargeTime = currentRechargeTime < rechargeTime ? currentRechargeTime + Time.deltaTime : rechargeTime;
        archer.SetAnimationBool("Recharge", true);
        archer.SetAnimationLayerWeight(1, 1);
        archer.SetRecharge(currentRechargeTime == rechargeTime ? true : false);
    }
}

public class ArcherArrowActive : IActiveObj
{
    private Archer archer;
    private Arrow[] arrows;
    private float damage;

    private ISkill mainSkill;
    private ISkill subSkill;

    public ArcherArrowActive(Archer _archer, ISkill _mainSkill, ISkill _subSkill) 
    {
        archer = _archer;
        mainSkill = _mainSkill;
        subSkill = _subSkill;
        arrows = new Arrow[5];
    }

    public void Active()
    {
        Transform MonsterTR = archer.MonsterInCrossHair();

        if (mainSkill.isActive) 
        {
            for (int i = 0; i < 5; i++) 
                arrows[i] = archer.ArrowDequeue();

            damage = mainSkill.GetDamage(); 
        }

        if (subSkill.isActive)
        {
            arrows[0] = archer.ArrowDequeue();
            arrows[0].SetSkillSpread(true);
            damage = subSkill.GetDamage();
        }

        else arrows[0] = archer.ArrowDequeue();

        for (int i = 0; i < arrows.Length; i++)
        {
            if (!arrows[i]) break;

            Transform fireTR = archer.GetArrowFireTR();
            arrows[i].SetTarget(MonsterTR);

            arrows[i].transform.position = fireTR.position;
            arrows[i].transform.rotation = fireTR.rotation;

            arrows[i].SetArrowDamage(damage);
            arrows[i].gameObject.SetActive(true);

            arrows[i] = null;
        }

        //obj = archer.ArrowDequeue();
        //obj.SetTarget(MonsterTR);

        //obj.transform.position = fireTR.position;
        //obj.transform.rotation = fireTR.rotation;
        //obj.gameObject.SetActive(true);
    }
}

public class ArcherUpdate : ICharacterUpdate
{
    public bool isStringPull { get; set; }
    private Transform bowStringTR;
    private Transform rightHandTR;
    private GameObject modelArrow;

    private Vector3 originPos;

    public ArcherUpdate(Transform bow, Transform rightHand, GameObject _modelArrow)
    {
        bowStringTR = bow;
        rightHandTR = rightHand;
        originPos = bowStringTR.localPosition;
        modelArrow = _modelArrow;
    }

    public void CharacterUpdate()
    {
        if (isStringPull)
            bowStringTR.position = rightHandTR.position;
        else
            bowStringTR.localPosition = originPos;
    }

    public void BowStringReSet()
    {
        isStringPull = false;
        bowStringTR.localPosition = originPos;
        modelArrow.SetActive(false);
    }
}

public class ArcherSubAttack : ISkill
{
    private Character character;
    private ArcherUpdate update;

    private float coolTime = 3.0f;

    public bool isActive { get; set; }

    public ArcherSubAttack(ArcherUpdate _update) { character = Character.instance; update = _update; }

    public void Skill()
    {
        character.ResetAnimation();
        character.SetAnimationTrigger("SubAttack");
        update.BowStringReSet();
    }

    public IEnumerator SkillCoolTime()
    {
        isActive = true;
        yield return new WaitForSeconds(coolTime);
        isActive = false;
    }

    public float GetDamage()
    {
        return 0;
    }
}


public class ArcherSkillArrow : ISkill
{
    private Character character;
    private float coolTime;
    private float damageMagnification;
    private bool isAttack = false;
    public bool isActive { get; set; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="_coolTime">스킬 쿨타임</param>
    /// <param name="_damageMagnification">스킬 데미지 배율</param>
    public ArcherSkillArrow(float _coolTime, float _damageMagnification) 
    {
        character = Character.instance;
        coolTime = _coolTime;
        damageMagnification = _damageMagnification;
    }

    public void Skill()
    {
        if (isAttack == false)
            isActive = !isActive;
    }

    public IEnumerator SkillCoolTime()
    {
        if (!isActive) yield return null;
        else
        {
            isAttack = true;
            isActive = false;
            yield return new WaitForSeconds(coolTime);
            isAttack = false;
        }
    }

    public float GetDamage()
    {
        return character.GetCharacterCurrentDamage() * damageMagnification;
    }
}