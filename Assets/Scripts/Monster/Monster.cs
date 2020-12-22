using System.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Monster : MonoBehaviour
{
    public enum MonsterAction { CHASE, ATTACK, STAND, DEAD, HIT, STAY, NULL }
    [SerializeField]
    protected MonsterAction action;
    protected CharacterState state = new CharacterState();

    protected IMove move;
    protected ISkill attack;
    protected IMove chase;
    protected IMove hit;
    protected IMove dead;
    protected IMove stay;
    protected IStand knockBack;
    protected IMove stand;
    protected ICharacterUpdate update;

    protected MonsterAttackStay monsterAttackStay = new MonsterAttackStay();
    protected MonsterMoveStay monsterMoveStay = new MonsterMoveStay();

    protected float skill1CoolTime;
    protected float skill2CoolTime;
    protected float skill3CoolTime;

    protected float currentHP;
    protected float currentSpeed;
    protected float currentDefense;
    protected float currentDamage;

    protected float attackTime;
    protected bool isAttack;
    protected bool isAttackDecision;

    protected NavMeshAgent nav;
    protected Rigidbody rigid;
    protected Animator anim;
    protected Character character;
    protected MonsterAttackDirection monsterDirection;
    protected MonsterSpawn room;

    [SerializeField]
    protected float attackDistance;
    [SerializeField]
    protected float attackDirection;

    protected bool isStay;

    protected virtual void OnEnable()
    {
        action = MonsterAction.CHASE;
    }

    protected virtual void Start()
    {
        InitData();
    }

    protected virtual void Update()
    {
        Stand();
        Attack();
    }

    //protected virtual void State()
    //{
    //    switch (action)
    //    {
    //        case MonsterAction.STAND:
    //            Stand();
    //            break;

    //        case MonsterAction.CHASE:
    //            Chase();
    //            break;

    //        case MonsterAction.ATTACK:
    //            Attack();
    //            break;

    //        case MonsterAction.HIT:
    //            Hit();
    //            break;

    //        case MonsterAction.STAY:
    //            Stay();
    //            break;

    //        case MonsterAction.DEAD:
    //            Dead();
    //            break;
    //    }
    //}

    protected virtual void InitData()
    {
        EventManager.instance.AddHitEvent(HitDamage);
        character = Character.instance;
        nav = GetComponent<NavMeshAgent>();

        nav.destination = Character.instance.transform.position;
        nav.enabled = true;
        monsterDirection = new MonsterAttackDirection(this);
        anim = GetComponent<Animator>();
        rigid = GetComponent<Rigidbody>();
        attack = monsterAttackStay;
        //rigid.isKinematic = false;
        GetComponent<Collider>().enabled = true;

        ResetAnimation();
    }

    protected virtual void HitDamage(float Damage, int instanceID, bool knockBack = false, float knockBackPower = 0)
    {
        if (transform.GetInstanceID() != instanceID) return;

        Damage -= Damage * currentDefense;
        currentHP -= Damage;

        action = currentHP <= 0 ? MonsterAction.DEAD : MonsterAction.HIT;
        KnockBack(knockBack, knockBackPower);
    }

    protected virtual void BuffDamage(MonsterSpawn _room, float _time, float _magnification)
    {
        if (room != _room) return;

        StartCoroutine(BuffTime(_time, _magnification));
    }

    protected virtual IEnumerator BuffTime(float _time, float _magnification)
    {
        float beforeDamage = currentDamage;
        currentDamage = currentDamage * _magnification;
        yield return new WaitForSeconds(_time);

        currentDamage = beforeDamage;
    }

    protected virtual void Stand()
    {
        move.Move();
        //action = !isAttack ? MonsterAction.CHASE : MonsterAction.STAND;
    }

    protected virtual void Attack()
    {
        //if (!isAttack)
        //{
        //    isAttack = true;
        //    nav.isStopped = true;
        //    attack.Skill();
        //    float time = Random.Range(2.0f, 5.0f);
        //    StartCoroutine(AttackCoolTime(time));
        //    StartCoroutine(attack.SkillCoolTime());
        //}

        //else if (isAttackDecision)
        //{
        //    character.Hit(currentDamage);
        //    isAttackDecision = false;
        //}
    }

    protected virtual void Chase()
    {
        chase.Move();

        float dis = Vector3.Distance(transform.position, character.transform.position);

        action = !isAttack ? dis < attackDistance ? monsterDirection.GetinDirection(attackDirection) ?
            MonsterAction.ATTACK : MonsterAction.CHASE : MonsterAction.CHASE : MonsterAction.STAND;
    }

    protected void Stay()
    {
        if (!isStay)
        {
            nav.isStopped = false;
            Vector3 pos = room.GetMoveTile();
            nav.destination = pos;
            nav.angularSpeed = 180;
            StartCoroutine(IEnumStay());
        }

        //float dis = Vector3.Distance(transform.position, nav.destination);
        //if(dis < nav.stoppingDistance) SetAnimationBool("Walk", false);
        if (nav.velocity.sqrMagnitude < 0.5f) { SetAnimationBool("Walk", false); }
        else SetAnimationBool("Walk", true);
    }

    protected IEnumerator IEnumStay()
    {
        isStay = true;
        float time = Random.Range(10.0f, 20.0f);
        yield return new WaitForSeconds(time);
        isStay = false;
    }

    protected void Hit()
    {
        nav.isStopped = true;
        action = MonsterAction.NULL;
        hit.Move();
    }

    protected virtual void KnockBack(bool active, float power)
    {
        if (!active) return;

        Vector3 dir = (transform.position - character.transform.position).normalized;
        rigid.AddForce(dir * power, ForceMode.Impulse);
    }

    protected void Dead()
    {
        EventManager.instance.SubHitEvent(HitDamage);

        dead.Move();
        StartCoroutine(DeadTime());
        action = MonsterAction.NULL;

        nav.isStopped = true;
        nav.enabled = false;
    }

    protected virtual IEnumerator DeadTime()
    {
        yield return new WaitForSeconds(5.0f);

        Renderer[] renderer = GetComponentsInChildren<Renderer>();
        float num = 0;
        while (true)
        {
            if (num > 1) break;

            for (int i = 0; i < renderer.Length; i++)
                renderer[i].material.SetFloat("_DissolveAmount", num);

            num += 0.005f;
            yield return null;
        }
        gameObject.SetActive(false);
    }

    protected void SpawnMonster()
    {
        InitData();
    }

    public void SetAnimationBool(string name, bool active)
    {
        anim.SetBool(name, active);
    }

    public void SetAnimationTrigger(string name)
    {
        anim.SetTrigger(name);
    }

    public void SetAnimationFloat(string name, float num)
    {
        anim.SetFloat(name, num);
    }

    public void AnimationEndEvent(MonsterAction _action)
    {
        action = _action;
    }

    /// <summary>
    /// 애니메이터의 모든 Bool paramerter false
    /// </summary>
    public void ResetAnimation()
    {
        foreach (var obj in anim.parameters)
        {
            if (obj.type == AnimatorControllerParameterType.Bool)
            {
                anim.SetBool(obj.name, false);
            }
        }
    }

    public Vector3 GetLookCharacterRotation()
    {
        return (character.transform.position - transform.position).normalized;
    }

    public void AttackEnd()
    {
        action = MonsterAction.CHASE;
    }

    public void AttackDecisionOn()
    {
        isAttackDecision = true;
    }

    public void AttackDecisionOff()
    {
        isAttackDecision = false;
    }

    protected IEnumerator AttackCoolTime(float time)
    {
        yield return new WaitForSeconds(time);
        isAttack = false;
    }

    public void SetMonsterRoom(MonsterSpawn _room)
    {
        room = _room;
    }

    public void SetMonsterState(MonsterAction state)
    {
        action = state;
    }

    public MonsterAction GetMonsterState()
    {
        return action;
    }

    public float GetMonsterDamage()
    {
        return currentDamage;
    }

    public void SetMonsterStand(IMove stand)
    {
        move = stand;
    }

    public bool GetMonsterAttackState()
    {
        return isAttack;
    }

    public void SetMonsterAttackState(bool active)
    {
        isAttack = active;
    }
}



/// <summary>
/// 몬스터 공격 범위 판별
/// </summary>
public class MonsterAttackDirection
{
    private Monster monster;
    private bool inDirection;

    public MonsterAttackDirection(Monster _monster) { monster = _monster; }

    private void CheckDirection(float attackDirection)
    {
        Character character = Character.instance;

        Vector3 dir = (character.transform.position - monster.transform.position).normalized;
        float dot = Vector3.Dot(monster.transform.forward, dir);
        float result = Mathf.Cos(attackDirection / 2 * Mathf.Deg2Rad);

        inDirection = dot > result ? true : false;
    }
    /// <summary>
    /// Degree각으로 몬스터 공격 범위 판별
    /// </summary>
    /// <param name="attackDirection">Degree 각</param>
    /// <returns></returns>
    public bool GetinDirection(float attackDirection)
    {
        CheckDirection(attackDirection);
        return inDirection;
    }
}

/// <summary>
/// 캐릭터 몬스터 외적 값
/// </summary>
//public class MonsterCharacterCross
//{
//    private Character character;
//    private Monster monster;
//    private Vector3 dir;
//    private float time;
//    bool right;

//    public MonsterCharacterCross(Monster _monster, float _time)
//    {
//        monster = _monster;
//        character = Character.instance;
//        dir = Vector3.zero;
//        time = _time;
//    }

//    public void CrossUpdate()
//    {
//        Vector3 normal = (character.transform.position - monster.transform.position).normalized;
//        dir = Vector3.Cross(monster.transform.up, normal);
//        dir.y = 0;
//        dir = right ? dir.normalized : -dir.normalized;
//    }

//    public IEnumerator DirUpdate()
//    {
//        while (true)
//        {
//            right = Random.Range(0, 2) == 0 ? true : false;

//            yield return new WaitForSeconds(time);
//        }
//    }

//    public Vector3 GetCross()
//    {
//        return dir;
//    }
//}

public class MonsterAttack : ISkill
{
    private Monster monster;
    private float coolTime;
    public bool isActive { get; set; }

    public MonsterAttack(Monster _monster, float _coolTime)
    {
        monster = _monster;
    }

    public void Skill()
    {
        monster.SetAnimationBool("Walk", false);
        monster.SetAnimationBool("Run", false);

        monster.SetAnimationTrigger("Attack");
    }
    public IEnumerator SkillCoolTime()
    {
        yield return null;
    }

    public float GetDamage()
    {
        return 0;
    }

    public Sprite GetImage() { return null; }
}

public class MonsterAttackStay : ISkill
{

    public bool isActive { get; set; }

    public MonsterAttackStay() { }

    public void Skill() { }
    public IEnumerator SkillCoolTime()
    {
        yield return null;
    }

    public float GetDamage()
    {
        return 0;
    }

    public Sprite GetImage() { return null; }
}

public class MonsterMoveStay : IMove
{

    public MonsterMoveStay()
    {

    }

    public void Move()
    {

    }
}

public class MonsterHit : IMove
{
    private Monster monster;

    public MonsterHit(Monster _monster) { monster = _monster; }

    public void Move()
    {
        monster.SetAnimationTrigger("Hit");
    }
}

public class MonsterDead : IMove
{
    private Monster monster;

    public MonsterDead(Monster _monster) { monster = _monster; }

    public void Move()
    {
        monster.SetAnimationTrigger("Dead");
        monster.GetComponent<Collider>().enabled = false;
        //monster.GetComponent<Rigidbody>().isKinematic = true;
    }
}
