using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Archer : Character
{
    private ArcherUpdate archerUpdate;
    private ArcherKick archerKick;
    private ArcherArrowActive arrowActive;

    private ArcherMultiArrow multiArrow;
    private ArcherSpreadArrow spreadArrow;

    [Space]
    [Header("Skill Image")]

    [SerializeField]
    private Sprite multiArrowImage;
    [SerializeField]
    private Sprite spreadArrowImage;
    [SerializeField]
    private Sprite kickImage;

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
        subAttack = new ArcherSubAttack(archerUpdate, kickImage);
        characterUpdate = archerUpdate;
        recharge = new ArcherRecharge(this, rechargeTime);

        multiArrow = new ArcherMultiArrow(10.0f, 1.5f, multiArrowImage);
        spreadArrow = new ArcherSpreadArrow(15.0f, 1.25f, spreadArrowImage);

        mainSkill = multiArrow;
        subSkill = spreadArrow;

        arrowActive = new ArcherArrowActive(this);
        activeArrow = arrowActive;

        UIManager.instance.SetSkillImage(mainSkill.GetImage(), subSkill.GetImage(), subAttack.GetImage());
    }

    public override void MainSkill()
    {
        base.MainSkill();

        if (mainSkill == multiArrow as ISkill || mainSkill == spreadArrow as ISkill)
            subSkill.isActive = mainSkill.isActive ? false : subSkill.isActive;
    }

    public override void SubSkill()
    {
        base.SubSkill();

        if (subSkill == multiArrow as ISkill || subSkill == spreadArrow as ISkill)
            mainSkill.isActive = subSkill.isActive ? false : subSkill.isActive;
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
    private Character character;
    private Arrow arrow;
    private float damage;

    public ArcherArrowActive(Character _character)
    {
        character = _character;
    }

    public void Active()
    {
        Transform MonsterTR = character.MonsterInCrossHair();

        arrow = character.ArrowDequeue();

        Transform fireTR = character.GetArrowFireTR();
        arrow.SetTarget(MonsterTR);

        arrow.transform.position = fireTR.position;
        arrow.transform.rotation = fireTR.rotation;

        damage = character.GetCharacterCurrentDamage();
        arrow.SetArrowDamage(damage);
        arrow.gameObject.SetActive(true);

        arrow = null;

        //obj = archer.ArrowDequeue();
        //obj.SetTarget(MonsterTR);

        //obj.transform.position = fireTR.position;
        //obj.transform.rotation = fireTR.rotation;
        //obj.gameObject.SetActive(true);
    }
}

public class ArcherSpreadArrow : ISkill, IActiveObj
{
    private Character character;
    private Arrow arrow;
    private float coolTime;
    private float damageMagnification;
    private bool isAttack = false;
    private Sprite image;
    public bool isActive { get; set; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="_coolTime">스킬 쿨타임</param>
    /// <param name="_damageMagnification">스킬 데미지 배율</param>
    public ArcherSpreadArrow(float _coolTime, float _damageMagnification, Sprite _image)
    {
        character = Character.instance;
        coolTime = _coolTime;
        damageMagnification = _damageMagnification;
        image = _image;
    }

    public void Skill()
    {
        if (isAttack == false)
            isActive = !isActive;
    }

    public void Active()
    {
        if (isActive)
        {
            Transform MonsterTR = character.MonsterInCrossHair();

            arrow = character.ArrowDequeue();
            arrow.SetSkillSpread(true);
            float damage = GetDamage();
            arrow.SetArrowDamage(damage);

            Transform fireTR = character.GetArrowFireTR();
            arrow.SetTarget(MonsterTR);

            arrow.transform.position = fireTR.position;
            arrow.transform.rotation = fireTR.rotation;

            arrow.gameObject.SetActive(true);
        }
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

    public Sprite GetImage()
    {
        return image;
    }
}

public class ArcherMultiArrow : ISkill, IActiveObj
{
    private Character character;
    private Arrow[] arrows;
    private float coolTime;
    private float damageMagnification;
    private bool isAttack = false;
    private Sprite image;
    public bool isActive { get; set; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="_coolTime">스킬 쿨타임</param>
    /// <param name="_damageMagnification">스킬 데미지 배율</param>
    public ArcherMultiArrow(float _coolTime, float _damageMagnification, Sprite _image)
    {
        character = Character.instance;
        coolTime = _coolTime;
        damageMagnification = _damageMagnification;
        image = _image;
        arrows = new Arrow[5];
    }

    public void Skill()
    {
        if (isAttack == false)
            isActive = !isActive;
    }

    public void Active()
    {
        if (isActive)
        {
            Transform MonsterTR = character.MonsterInCrossHair();

            for (int i = 0; i < 5; i++)
            {
                arrows[i] = character.ArrowDequeue();
                float damage = GetDamage();
                arrows[i].SetArrowDamage(damage);

                Transform fireTR = character.GetArrowFireTR();
                arrows[i].SetTarget(MonsterTR);

                arrows[i].transform.position = fireTR.position;
                arrows[i].transform.rotation = fireTR.rotation;

                arrows[i].gameObject.SetActive(true);
            }
        }
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

    public Sprite GetImage()
    {
        return image;
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
    private Sprite image;

    private float coolTime = 3.0f;

    public bool isActive { get; set; }

    public ArcherSubAttack(ArcherUpdate _update, Sprite _image) { character = Character.instance; update = _update; image = _image; }

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

    public Sprite GetImage() { return image; }
}


//public class ArcherSkillArrow : ISkill
//{
//    private Character character;
//    private float coolTime;
//    private float damageMagnification;
//    private bool isAttack = false;
//    private Sprite image;
//    public bool isActive { get; set; }

//    /// <summary>
//    /// 
//    /// </summary>
//    /// <param name="_coolTime">스킬 쿨타임</param>
//    /// <param name="_damageMagnification">스킬 데미지 배율</param>
//    public ArcherSkillArrow(float _coolTime, float _damageMagnification, Sprite _image) 
//    {
//        character = Character.instance;
//        coolTime = _coolTime;
//        damageMagnification = _damageMagnification;
//        image = _image;
//    }

//    public void Skill()
//    {
//        if (isAttack == false)
//            isActive = !isActive;
//    }

//    public IEnumerator SkillCoolTime()
//    {
//        if (!isActive) yield return null;
//        else
//        {
//            isAttack = true;
//            isActive = false;
//            yield return new WaitForSeconds(coolTime);
//            isAttack = false;
//        }
//    }

//    public float GetDamage()
//    {
//        return character.GetCharacterCurrentDamage() * damageMagnification;
//    }

//    public Sprite GetImage()
//    {
//        return image;
//    }
//}