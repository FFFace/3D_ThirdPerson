using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Warrok : Monster
{ 
    [SerializeField]
    private List<Monster> summonMonster = new List<Monster>();

    [SerializeField]
    private MonsterWeapon weapon;

    private WarrokSummonSkill summonSkill;
    private WarrokBuffSkill buffSkill;
    private WarrokJumpSKill jumpSkill;
    private WarrokAttack normalAttack;

    private WarrokChase warrokChase;
    private WarrokStand warrokStand;
    private MonsterHit warrokHit;
    private MonsterDead warrokDead;

    protected override void Start()
    {
        base.Start();

        summonSkill = new WarrokSummonSkill(this, 5, 25.0f, summonMonster, room);
        buffSkill = new WarrokBuffSkill(this, room, 30.0f, 10.0f, 1.25f);
        jumpSkill = new WarrokJumpSKill(this, 15.0f, 1.5f);
        normalAttack = new WarrokAttack(this);

        //attack = nomalAttack;
        //chase = new WarrokChase(this, nav, currentSpeed);
        //stand = new WarrokStand(this, nav, currentSpeed);
        //hit = new MonsterHit(this);
        //dead = new MonsterDead(this);

        warrokChase = new WarrokChase(this, nav, currentSpeed);
        warrokStand = new WarrokStand(this, nav, currentSpeed);
        warrokHit = new MonsterHit(this, nav);
        warrokDead = new MonsterDead(this, nav);

        move = warrokChase;

        StartCoroutine(State());
    }

    private IEnumerator State()
    {
        while (true)
        {
            if (move != warrokHit && move != warrokDead)
            {
                float dis = Vector3.Distance(transform.position, character.transform.position);

                if (dis < attackDistance && monsterDirection.GetinDirection(attackDirection)) attack = normalAttack;
                else if (summonSkill.isActive) attack = summonSkill;
                else if (buffSkill.isActive) attack = buffSkill;
                else if (jumpSkill.isActive && dis < 7.0f) attack = jumpSkill;
                else
                {
                    attack = monsterAttackStay;
                    move = warrokChase;
                }

                if (attack.isActive)
                {
                    nav.isStopped = true;
                    move = monsterMoveStay;

                    attack.Skill();
                    StartCoroutine(attack.SkillCoolTime());

                    yield return new WaitForSeconds(5);
                    nav.isStopped = false;
                    move = warrokChase;
                    attack = monsterAttackStay;
                }
            }

            else
            {
                move = monsterMoveStay;
                attack = monsterAttackStay;

                ResetAnimation();
            }

            weapon.SetDamage(currentDamage);
            yield return new WaitForSeconds(0.1f);
        }
    }

    protected override void InitData()
    {
        base.InitData();

        state.hp = 100;
        state.moveSpeed = 3;
        state.jumpPower = 5;
        state.attackDamage = 1;
        state.attackSpeed = 1;

        currentHP = state.hp;
        currentSpeed = state.moveSpeed;
        currentDefense = state.defense;
        currentDamage = state.attackDamage;
    }

    protected override void HitDamage(float Damage, int instanceID, bool knockBack = false, float knockBackPower = 0)
    {
        if (transform.GetInstanceID() != instanceID) return;

        base.HitDamage(Damage, instanceID, knockBack, knockBackPower);

        attack = monsterAttackStay;

        if (currentHP <= 0)
        {
            move = warrokDead;

            EventManager.instance.SubHitEvent(base.HitDamage);
            EventManager.instance.SubMonsterBuffDamageEvent(BuffDamage);

            StartCoroutine(DeadTime());
        }

        else
        {
            move = warrokHit;
        }
    }

    //protected override void Attack()
    //{
    //    if (!isAttack)
    //    {
    //        isAttack = true;
    //        nav.isStopped = true;
    //        attack.Skill();
    //        float time = 2.5f;
    //        StartCoroutine(AttackCoolTime(time));
    //        StartCoroutine(attack.SkillCoolTime());

    //        attack = nomalAttack;
    //    }

    //    else if (isAttackDecision)
    //    {
    //        character.Hit(currentDamage);
    //        isAttackDecision = false;
    //    }
    //}

    //protected override void Chase()
    //{
    //    nav.isStopped = false;
    //    nav.destination = character.transform.position;
    //    chase.Move();

    //    if (summonSkill.isActive)
    //    {
    //        attack = summonSkill;
    //        action = MonsterAction.ATTACK;
    //        return;
    //    }

    //    else if (buffSkill.isActive)
    //    {
    //        attack = buffSkill;
    //        action = MonsterAction.ATTACK;
    //        return;
    //    }

    //    float dis = Vector3.Distance(transform.position, character.transform.position);

    //    action = !isAttack ? dis < attackDistance ? monsterDirection.GetinDirection(attackDirection) ?
    //        MonsterAction.ATTACK : MonsterAction.CHASE : MonsterAction.CHASE : MonsterAction.STAND;
    //}

    protected override IEnumerator DeadTime()
    {
        base.DeadTime();
        MonsterPooling.instance.MonsterEnqueue<Warrok>(this);

        yield return null;
    }

    public void EnableWeapon()
    {
        weapon.SetActiveCollider(true);
    }

    public void DisableWeapon()
    {
        weapon.SetActiveCollider(false);
        SetAnimationBool("JumpSkill", false);
    }
}

public class WarrokChase : IMove
{
    private Monster monster;
    private Character character;
    private float currentSpeed;
    private NavMeshAgent nav;

    public WarrokChase(Monster _monster, NavMeshAgent _nav, float _speed)
    {
        monster = _monster;
        nav = _nav;
        currentSpeed = _speed;
        character = Character.instance;
    }

    public void Move()
    {
        nav.isStopped = false;
        nav.destination = character.transform.position;
        // NavMeshAgent를 통해 목적지에 도착 시, 이동뿐만 아니라 회전도 멈추기 때문에 직접 회전
        monster.transform.rotation = Quaternion.Lerp(monster.transform.rotation, Quaternion.LookRotation(monster.GetLookCharacterRotation()), 15 * Time.deltaTime);

        monster.SetAnimationBool("Walk", true);
        nav.speed = currentSpeed;
    }
}
public class WarrokStand : IMove
{
    private Monster monster;
    private NavMeshAgent nav;
    private float speed;

    public WarrokStand(Monster _monster, NavMeshAgent _nav, float _speed)
    {
        monster = _monster;
        nav = _nav;
        speed = _speed;
    }

    public void Move()
    {
        nav.isStopped = true;
        monster.SetAnimationBool("Walk", false);
    }
}

public class WarrokAttack : ISkill
{
    private Monster monster;
    public bool isActive { get; set; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="_monster">몬스터</param>
    /// <param name="_coolTime">스킬 쿨타임</param>
    /// <param name="_damageMagnification">스킬 데미지 배율</param>
    public WarrokAttack(Monster _monster)
    {
        monster = _monster;
        isActive = true;
    }

    public void Skill()
    {
        monster.SetAnimationBool("Walk", false);

        monster.SetAnimationTrigger("Attack");
    }

    public IEnumerator SkillCoolTime()
    {
        yield return null;
    }

    public float GetDamage()
    {
        return monster.GetMonsterDamage();
    }

    public Sprite GetImage() { return null; }

    public float GetCoolTime() { return 0; }
}

public class WarrokJumpSKill : ISkill
{
    private Monster monster;
    private float coolTime;
    private float magnification;
    public bool isActive { get; set; }

    public WarrokJumpSKill(Monster _monster, float _coolTime, float _magnification)
    {
        monster = _monster;
        coolTime = _coolTime;
        magnification = _magnification;

        isActive = true;
    }

    public void Skill()
    {
        monster.SetAnimationBool("Walk", false);
        monster.SetAnimationBool("JumpSkill", true);
    }

    public IEnumerator SkillCoolTime()
    {
        isActive = false;
        yield return new WaitForSeconds(coolTime);
        isActive = true;
    }

    public float GetDamage()
    {
        return monster.GetMonsterDamage() * magnification;
    }

    public Sprite GetImage() { return null; }

    public float GetCoolTime() { return 0; }
}

public class WarrokBuffSkill : ISkill
{
    private Monster monster;
    private MonsterSpawn spawn;
    private float coolTime;
    private float magnification;
    private float buffTime;
    public bool isActive { get; set; }

    public WarrokBuffSkill(Monster _monster, MonsterSpawn _monsterSpawn, float _coolTime, float _buffTime, float _magnification)
    {
        monster = _monster;
        coolTime = _coolTime;
        spawn = _monsterSpawn;
        magnification = _magnification;
        buffTime = _buffTime;

        isActive = true;
    }

    public void Skill()
    {
        monster.SetAnimationBool("Walk", false);
        monster.SetAnimationTrigger("Buff");

        EventManager.instance.MonsterBuffDamage(spawn, buffTime, magnification);
    }

    public IEnumerator SkillCoolTime()
    {
        isActive = false;

        yield return new WaitForSeconds(coolTime);
        isActive = true;
    }

    public float GetDamage()
    {
        return 0;
    }

    public Sprite GetImage() { return null; }

    public float GetCoolTime() { return 0; }
}

public class WarrokSummonSkill : ISkill
{
    private Monster monster;
    private float coolTime;
    private MonsterSpawn spawn;

    private List<Monster> monsters = new List<Monster>();
    private int num;

    public bool isActive { get; set; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="_monster"></param>
    /// <param name="_num"></param>
    /// <param name="_coolTime"></param>
    /// <param name="_monsters"></param>
    public WarrokSummonSkill(Monster _monster, int _num, float _coolTime, List<Monster> _monsters, MonsterSpawn _spawn)
    {
        monster = _monster;
        num = _num;
        coolTime = _coolTime;
        spawn = _spawn;
        isActive = true;

        foreach (var monster in _monsters)
        {
            monsters.Add(monster);
        }
    }

    public void Skill()
    {
        for (int i = 0; i < num; i++)
        {
            int kind = Random.Range(0, monsters.Count);
            Monster summon = null;
            switch (monsters[kind])
            {
                case Skeleton skeleton:
                    summon = MonsterPooling.instance.MonsterDequeue<Skeleton>();
                    break; ;
            }

            Vector3 pos = spawn.GetMoveTile();
            summon.transform.position = pos;
            summon.gameObject.SetActive(true);
            summon.SetMonsterRoom(spawn);
            summon.SetMonsterState(Monster.MonsterAction.CHASE);
        }

        monster.ResetAnimation();
        monster.SetAnimationBool("Summon", true);
    }

    public IEnumerator SkillCoolTime()
    {
        isActive = false;

        yield return new WaitForSeconds(coolTime);
        isActive = true;
    }

    public float GetDamage()
    {
        return 0;
    }

    public Sprite GetImage() { return null; }

    public float GetCoolTime() { return 0; }
}
