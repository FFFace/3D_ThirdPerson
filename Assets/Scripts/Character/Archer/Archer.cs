using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class Archer : Character
{
    //private ArcherUpdate archerUpdate;
    private ArcherKick archerKick;
    private ArcherArrowActive arrowActive;
    private ArcherRecharge recharge;
    private ArcherRangeAttack attack;

    private ArcherMultiArrow multiArrow;
    private ArcherSpreadArrow spreadArrow;

    [SerializeField]
    protected GameObject modelArrow;

    [SerializeField]
    protected Arrow arrow;
    private Queue<Arrow> arrows = new Queue<Arrow>();

    [SerializeField]
    private Transform arrowFireTR;
    private Transform targetMonster;

    [SerializeField]
    protected float rechargeTime;

    [Space]
    [Header("Skill Image")]

    [SerializeField]
    private Sprite multiArrowImage;
    [SerializeField]
    private Sprite spreadArrowImage;
    [SerializeField]
    private Sprite kickImage;

    private bool isStringPull;
    private Transform bowStringTR;
    private Transform rightHandTR;
    private Vector3 originPos;

    protected override void Start()
    {
        base.Start();
        StartCoroutine(IEnumUpdate());
    }

    protected override void Update()
    {
        base.Update();
        TargetMonster();
    }

    private IEnumerator IEnumUpdate()
    {
        while (true)
        {
            if (isStringPull)
                bowStringTR.position = rightHandTR.position;
            else
                bowStringTR.localPosition = originPos;

            yield return new WaitForSeconds(0.1f);
        }
    }

    public void BowStringReSet()
    {
        isStringPull = false;
        bowStringTR.localPosition = originPos;
        modelArrow.SetActive(false);
    }

    protected override void InitData()
    {
        base.InitData();

        state.hp = 30;
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

        attack = new ArcherRangeAttack(this);
        archerKick = transform.Find("KickDistance").GetComponent<ArcherKick>();

        //Transform bowStringTR = GameObject.Find("WB.string").transform;
        //Transform rightHandTR = GameObject.Find("FollowStringTR").transform;

        //archerUpdate = new ArcherUpdate(bowStringTR, rightHandTR, modelArrow);
        normalAttack = recharge;
        subAttack = new ArcherSubAttack(kickImage);
        //characterUpdate = archerUpdate;
        recharge = new ArcherRecharge(this, rechargeTime);

        multiArrow = new ArcherMultiArrow(10.0f, 1.5f, multiArrowImage);
        spreadArrow = new ArcherSpreadArrow(15.0f, 1.25f, spreadArrowImage);

        mainSkill = multiArrow;
        subSkill = spreadArrow;

        arrowActive = new ArcherArrowActive(this);
        activeArrow = arrowActive;

        bowStringTR = GameObject.Find("WB.string").transform;
        rightHandTR = GameObject.Find("FollowStringTR").transform;
        originPos = bowStringTR.localPosition;

        //UIManager.instance.SetSkillImage(mainSkill.GetImage(), subSkill.GetImage(), subAttack.GetImage());
        //UIManager.instance.SetSkillCoolTime(mainSkill.GetCoolTime(), subSkill.GetCoolTime(), subAttack.GetCoolTime());
        //UIManager.instance.SetPlayerMaxHPBar(state.hp);
    }

    public override void AttackPressDown()
    {
        normalAttack = recharge;
        base.AttackPressDown();
        isAttack = true;
    }

    public override void AttackPressUp()
    {
        normalAttack = attack;
        if (!isAction && isAttack)
        {
            normalAttack.Attack();
        }
    }

    public override void MainSkill()
    {
        mainSkill.Skill();

        if (mainSkill == multiArrow as ISkill || mainSkill == spreadArrow as ISkill)
            subSkill.isActive = mainSkill.isActive ? false : subSkill.isActive;

        if (mainSkill.isActive) activeArrow = mainSkill as IActiveObj;
        else if (!mainSkill.isActive && !subSkill.isActive) activeArrow = arrowActive;
    }

    public override void SubSkill()
    {
        subSkill.Skill();

        if (subSkill == multiArrow as ISkill || subSkill == spreadArrow as ISkill)
            mainSkill.isActive = subSkill.isActive ? false : subSkill.isActive;

        if (subSkill.isActive) activeArrow = subSkill as IActiveObj;
        else if (!mainSkill.isActive && !subSkill.isActive) activeArrow = arrowActive;
    }


    public void BowStringPull()
    {
        isStringPull = true;
    }

    public void BowStringPullOut()
    {
        isStringPull = false;
    }

    public void KickColliderEnable()
    {
        archerKick.ColliderEnable();
    }

    public void KickColliderDisable()
    {
        archerKick.ColliderDisable();
    }

    public void ArrowDraw()
    {
        modelArrow.SetActive(true);
    }

    public void ArrowDrawOut()
    {
        activeArrow.Active();

        if (activeArrow == mainSkill as IActiveObj) UIManager.instance.SetMainSkillCoolTime();
        if (activeArrow == subSkill as IActiveObj) UIManager.instance.SetSubSkillCoolTime();

        activeArrow = arrowActive;
        modelArrow.SetActive(false);

        StartCoroutine(mainSkill.SkillCoolTime());
        StartCoroutine(subSkill.SkillCoolTime());
    }

    public override void AttackEnd()
    {
        base.AttackEnd();
        SetAnimationBool("Recharge", false);
    }

    public Arrow GetArrow()
    {
        return arrow;
    }

    protected virtual Arrow CreateArrow()
    {
        Arrow obj = Instantiate(arrow, transform.position, transform.rotation) as Arrow;
        obj.gameObject.SetActive(false);
        return obj;
    }

    public void ArrowEnqueue(Arrow obj)
    {
        arrows.Enqueue(obj);
    }

    public Arrow ArrowDequeue()
    {
        Arrow obj = arrows.Count > 0 ? arrows.Dequeue() : CreateArrow();
        obj.SetRecharge(isRecharge);
        return obj.gameObject.activeSelf ? CreateArrow() : obj;
    }

    public Transform GetArrowFireTR()
    {
        arrowFireTR.localRotation = Quaternion.Euler(new Vector3(0, 0, 0));

        Vector3 rot = arrowFireTR.rotation.eulerAngles;
        rot += new Vector3(-Random.Range(10.0f, 20.0f), Random.Range(-15.0f, 15.0f), 0);
        arrowFireTR.rotation = Quaternion.Euler(rot);

        return arrowFireTR;
    }

    private void TargetMonster()
    {
        Vector3 pos = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 5));
        Ray ray = new Ray(pos, Camera.main.transform.forward);
        RaycastHit hit = new RaycastHit();
        LayerMask layer = 1 << LayerMask.NameToLayer("Monster");

        Physics.BoxCast(pos, new Vector3(0.25f, 0.25f, 0.25f), transform.forward, out hit, transform.rotation, 20, layer);
        targetMonster = hit.transform;
    }

    public Transform MonsterInCrossHair()
    {
        //Vector3 pos = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 5));
        //Ray ray = new Ray(pos, Camera.main.transform.forward);
        //RaycastHit hit;
        //LayerMask layer = 1 << LayerMask.NameToLayer("Monster");

        //return Physics.BoxCast(pos, new Vector3(0.25f, 0.25f, 0.25f), transform.forward, out hit, transform.rotation, 20, layer) ? hit.transform : null;
        //return Physics.Raycast(ray, out hit, 20, layer) ? hit.transform : null;

        return targetMonster;
    }
}

public class ArcherRangeAttack : IAttackAction
{
    public Archer archer;

    public ArcherRangeAttack(Archer _archer) { archer = _archer;}

    public virtual void Attack()
    {
        archer.SetAnimationBool("Recharge", false);
    }
}

public class ArcherRecharge : IAttackAction
{
    protected Archer character;
    private float rechargeTime;
    private float currentRechargeTime;

    public ArcherRecharge(Archer _character, float _time) { character = _character; rechargeTime = _time; currentRechargeTime = 0; }

    public void Attack()
    {
        currentRechargeTime = currentRechargeTime < rechargeTime ? currentRechargeTime + Time.deltaTime : rechargeTime;
        character.SetAnimationBool("Recharge", true);
        character.SetAnimationLayerWeight(1, 1);
        character.SetRecharge(currentRechargeTime == rechargeTime ? true : false);
    }
}

public class ArcherArrowActive : IActiveObj
{
    private Archer character;
    private Arrow arrow;
    private float damage;

    public ArcherArrowActive(Archer _character)
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
        arrow.SetItemEffect(character.GetItemEffectList());
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
    private Archer character;
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
        character = Character.instance as Archer;
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
            arrow.SetSpread(true);
            float damage = GetDamage();
            arrow.SetArrowDamage(damage);
            arrow.SetItemEffect(character.GetItemEffectList());

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
            isActive = false;
            isAttack = true;
            yield return new WaitForSeconds(coolTime);
            isAttack = false;
        }
    }

    public float GetDamage()
    {
        return character.GetCharacterCurrentDamage() * damageMagnification;
    }

    public float GetDamageMagnification()
    {
        return damageMagnification;
    }

    public Sprite GetImage()
    {
        return image;
    }

    public float GetCoolTime() { return coolTime; }

    public string GetExplain()
    {
        return "적중 시 사방으로 퍼지는 화살을 날립니다. " + (damageMagnification * 100).ToString() + "% 만큼의 데미지를 줍니다.";
    }
}

public class ArcherMultiArrow : ISkill, IActiveObj
{
    private Archer character;
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
    public ArcherMultiArrow(float _coolTime, float _damageMagnification, Sprite _image )
    {
        character = Character.instance as Archer;
        coolTime = _coolTime;
        damageMagnification = _damageMagnification;
        image = _image;
        arrows = new Arrow[5];
    }

    public void Skill()
    {
        if (!isAttack)
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
                arrows[i].SetItemEffect(character.GetItemEffectList());

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
            isActive = false;
            isAttack = true;
            yield return new WaitForSeconds(coolTime);
            isAttack = false;
        }
    }

    public float GetDamage()
    {
        return character.GetCharacterCurrentDamage() * damageMagnification;
    }

    public float GetDamageMagnification()
    {
        return damageMagnification;
    }

    public Sprite GetImage()
    {
        return image;
    }

    public float GetCoolTime() { return coolTime; }

    public string GetExplain()
    {
        return "한번에 5발의 화살을 날립니다. 각 화살 당 " + (damageMagnification * 100).ToString() + "%만큼의 데미지를 줍니다.";
    }
}

//public class ArcherUpdate : ICharacterUpdate
//{
//    public bool isStringPull { get; set; }
//    private Transform bowStringTR;
//    private Transform rightHandTR;
//    private GameObject modelArrow;

//    private Vector3 originPos;

//    public ArcherUpdate(Transform bow, Transform rightHand, GameObject _modelArrow)
//    {
//        bowStringTR = bow;
//        rightHandTR = rightHand;
//        originPos = bowStringTR.localPosition;
//        modelArrow = _modelArrow;
//    }

//    public void CharacterUpdate()
//    {
//        if (isStringPull)
//            bowStringTR.position = rightHandTR.position;
//        else
//            bowStringTR.localPosition = originPos;
//    }

//    public void BowStringReSet()
//    {
//        isStringPull = false;
//        bowStringTR.localPosition = originPos;
//        modelArrow.SetActive(false);
//    }
//}

public class ArcherSubAttack : ISkill
{
    private Archer character;
    private Sprite image;

    private float coolTime = 3.0f;

    public bool isActive { get; set; }

    public ArcherSubAttack(Sprite _image) { character = Character.instance as Archer; image = _image; }

    public void Skill()
    {
        character.ResetAnimation();
        character.SetAnimationTrigger("SubAttack");
        character.BowStringReSet();
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

    public float GetDamageMagnification()
    {
        return 0;
    }

    public Sprite GetImage() { return image; }

    public float GetCoolTime() { return coolTime; }

    public string GetExplain()
    {
        return "전방을 발로 차 120% 의 데미지를 줍니다.";
    }
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