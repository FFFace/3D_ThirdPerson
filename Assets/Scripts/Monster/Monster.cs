using System.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Monster : MonoBehaviour
{
    protected CharacterState state = new CharacterState();

    protected IMove move;
    protected ISkill attack;

    protected MonsterAttackStay monsterAttackStay = new MonsterAttackStay();
    protected MonsterMoveStay monsterMoveStay;

    protected float currentHP;
    protected float currentSpeed;
    protected float currentDefense;
    protected float currentDamage;

    protected float attackTime;
    protected bool isAttackDecision;
    protected bool isSummonBoss;

    protected NavMeshAgent nav;
    protected Rigidbody rigid;
    protected Animator anim;
    protected Character character;
    protected MonsterAttackDirection monsterDirection;
    public MonsterSpawn room;

    [SerializeField]
    protected float attackDistance;
    [SerializeField]
    protected float attackDirection;
    [Space, Header("-Monster Effect-"), SerializeField]
    protected ParticleSystem buffParticle;

    protected bool isStay;

    protected virtual void OnEnable()
    {
        InitData();
        StartCoroutine(IEnumSummon());
    }

    protected virtual void Start()
    {

    }

    protected virtual void Update()
    {
        Stand();
    }

    private IEnumerator IEnumSummon()
    {
        float time = 1;
        Renderer[] renderer = GetComponentsInChildren<Renderer>();
        GetComponent<Collider>().enabled = false;

        yield return new WaitForSeconds(2f);

        while (time > 0)
        {
            for (int i = 0; i < renderer.Length; i++)
                renderer[i].material.SetFloat("_DissolveAmount", time);

            time -= 0.5f * Time.deltaTime;
            yield return null;
        }

        GetComponent<Collider>().enabled = true;
    }

    protected virtual void InitData()
    {
        EventManager.instance.AddHitEvent(HitDamage);
        EventManager.instance.AddMonsterBuffDamageEvent(BuffDamage);
        character = Character.instance;
        nav = GetComponent<NavMeshAgent>();

        nav.destination = Character.instance.transform.position;
        nav.enabled = true;
        monsterDirection = new MonsterAttackDirection(this);
        anim = GetComponent<Animator>();
        rigid = GetComponent<Rigidbody>();
        GetComponent<Collider>().enabled = true;
        attack = monsterAttackStay;

        monsterMoveStay = new MonsterMoveStay(nav);

        ResetAnimation();

        Renderer[] renderer = GetComponentsInChildren<Renderer>();
        for (int i = 0; i < renderer.Length; i++)
            renderer[i].material.SetFloat("_DissolveAmount", 1);
    }

    protected virtual void HitDamage(float Damage, int instanceID, bool knockBack = false, float knockBackPower = 0)
    {
        Damage -= Damage * currentDefense;
        currentHP -= Damage;
    }

    protected virtual void BuffDamage(MonsterSpawn _room, float _time, float _magnification)
    {
        if (room != _room) return;

        StartCoroutine(IEnumBuffTime(_magnification, _time));
    }

    protected virtual IEnumerator IEnumBuffTime(float _magnification, float _time)
    {
        yield return new WaitForSeconds(1.0f);
        buffParticle.gameObject.SetActive(true);
        currentDamage += state.attackDamage * _magnification;

        Renderer[] renderer = GetComponentsInChildren<Renderer>();
        for (int i = 0; i < renderer.Length; i++)
            renderer[i].material.SetFloat("_RedColor", 1);
        yield return new WaitForSeconds(_time);

        for (int i = 0; i < renderer.Length; i++)
            renderer[i].material.SetFloat("_RedColor", 0);
        currentDamage -= state.attackDamage;
        buffParticle.gameObject.SetActive(false);
    }

    protected virtual void Stand()
    {
        move.Move();
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

    protected virtual IEnumerator DeadTime()
    {
        GetComponent<Collider>().enabled = false;
        nav.enabled = false;

        yield return new WaitForSeconds(5.0f);

        Renderer[] renderer = GetComponentsInChildren<Renderer>();
        float num = 0;
        while (num < 1)
        {
            for (int i = 0; i < renderer.Length; i++)
                renderer[i].material.SetFloat("_DissolveAmount", num);

            num += 0.5f * Time.deltaTime;
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

    /// <summary>
    /// 애니메이터의 모든 Bool paramerter false
    /// </summary>
    public void ResetAnimation()
    {
        foreach (var obj in anim.parameters)
        {
            if (obj.type == AnimatorControllerParameterType.Bool)
            {
                SetAnimationBool(obj.name, false);
            }
        }
    }

    public Vector3 GetLookCharacterRotation()
    {
        return (character.transform.position - transform.position).normalized;
    }

    public void AttackEnd()
    {
        isAttackDecision = false;
    }

    protected IEnumerator AttackCoolTime(float time)
    {
        yield return new WaitForSeconds(time);
    }

    public void SetMonsterRoom(MonsterSpawn _room)
    {
        room = _room;
    }

    public float GetMonsterDamage()
    {
        return currentDamage;
    }

    public void SetMonsterStand(IMove stand)
    {
        move = stand;
    }

    public void SetSummonBoss(bool _active)
    {
        isSummonBoss = _active;
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

public class MonsterAttack : ISkill
{
    private Monster monster;
    private float coolTime;
    public bool isActive { get; set; }

    public MonsterAttack(Monster _monster, float _coolTime)
    {
        monster = _monster;
    }

    public void SkillKeyDown()
    {
        monster.SetAnimationBool("Walk", false);
        monster.SetAnimationBool("Run", false);

        monster.SetAnimationTrigger("Attack");
    }

    public void SkillKeyUp() { }
    public IEnumerator SkillCoolTime()
    {
        yield return null;
    }

    public float GetDamage()
    {
        return 0;
    }

    public float GetDamageMagnification()
    {
        return 0;
    }


    public Sprite GetImage() { return null; }

    public float GetCoolTime() { return 0; }

    public string GetExplain()
    {
        return null;
    }

    public bool GetToggleState()
    {
        return false;
    }
}

public class MonsterAttackStay : ISkill
{

    public bool isActive { get; set; }

    public MonsterAttackStay() { }

    public void SkillKeyDown() { }
    public void SkillKeyUp() { }
public IEnumerator SkillCoolTime()
    {
        yield return null;
    }

    public float GetDamage()
    {
        return 0;
    }

    public float GetDamageMagnification()
    {
        return 0;
    }


    public Sprite GetImage() { return null; }

    public float GetCoolTime() { return 0; }

    public string GetExplain()
    {
        return null;
    }

    public bool GetToggleState()
    {
        return false;
    }
}

public class MonsterMoveStay : IMove
{
    private NavMeshAgent nav;
    public MonsterMoveStay(NavMeshAgent _nav)
    {
        nav = _nav;
    }

    public void Move()
    {
        nav.isStopped = true;
    }
}

public class MonsterHit : IMove
{
    private Monster monster;
    private NavMeshAgent nav;

    private Vector3 knockbackDir;
    private float knockbackPower;
    public bool isKnockBack { get; set; }

    public MonsterHit(Monster _monster, NavMeshAgent _nav) { monster = _monster; nav = _nav; }

    public void Move()
    {
        nav.isStopped = true;

        if (isKnockBack)
        {
            monster.transform.Translate(knockbackDir * knockbackPower * Time.deltaTime, Space.World);
            knockbackPower -= knockbackPower * 0.9f * Time.deltaTime;
        }
    }

    public void SetKnockbackPower(float _power)
    {
        knockbackPower = _power;
    }

    public void SetKnockBackDir(Vector3 dir)
    {
        knockbackDir = dir;
    }
}

public class MonsterDead : IMove
{
    private Monster monster;
    private NavMeshAgent nav;

    public MonsterDead(Monster _monster, NavMeshAgent _nav) { monster = _monster; nav = _nav; }

    public void Move()
    {
        monster.GetComponent<Collider>().enabled = false;
        nav.enabled = false;
    }
}

public class MonsterStay : IMove
{
    private NavMeshAgent nav;
    private MonsterSpawn room;
    private bool isStay;
    public IEnumerator stay;

    public MonsterStay(NavMeshAgent _nav, MonsterSpawn _room)
    {
        nav = _nav;
        room = _room;
        isStay = false;
        stay = IEnumStay();
    }

    public void Move()
    {
        nav.isStopped = false;
    }

    private IEnumerator IEnumStay()
    {
        Debug.Log("A");
        if (isStay) yield return null;
        Debug.Log("B");
        isStay = true;
        float time = Random.Range(10.0f, 20.0f);
        nav.destination = room.GetMoveTile();
        yield return new WaitForSeconds(2);
        isStay = false;
    }
}