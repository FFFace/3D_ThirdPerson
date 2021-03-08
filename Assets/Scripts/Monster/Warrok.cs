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

    [Space, Header("-Warrok Effect-"), SerializeField]
    private ParticleSystem groundImpace;

    protected override void OnEnable()
    {
        base.OnEnable();

        summonSkill = new WarrokSummonSkill(this, 5, 300f, summonMonster, room);
        buffSkill = new WarrokBuffSkill(this, room, 100f, 60.0f, 1.25f);
        jumpSkill = new WarrokJumpSKill(this, 15.0f, 1.5f);
        normalAttack = new WarrokAttack(this);

        warrokChase = new WarrokChase(this, nav, currentSpeed);
        warrokStand = new WarrokStand(this, nav, currentSpeed);
        warrokHit = new MonsterHit(this, nav);
        warrokDead = new MonsterDead(this, nav);

        move = monsterMoveStay;

        StartCoroutine(State());
    }

    private IEnumerator State()
    {
        yield return new WaitForSeconds(4f);

        while (true)
        {
            if (move == warrokHit)
            {
                yield return new WaitForSeconds(0.3f);
                move = monsterMoveStay;
                attack = monsterAttackStay;

                ResetAnimation();
                warrokHit.isKnockBack = false;
            }

            else if (move == warrokDead)
            {
                break;
            }

            else
            {
                float dis = Vector3.Distance(transform.position, character.transform.position);
                float time = 0;

                if (dis < attackDistance && monsterDirection.GetinDirection(attackDirection)) { attack = normalAttack; time = 2.0f; }
                else if (jumpSkill.isActive && dis < 7.0f) { attack = jumpSkill; time = 5.0f; }
                else if (summonSkill.isActive) { attack = summonSkill; time = 3.0f; }
                else if (buffSkill.isActive) { attack = buffSkill; time = 6.0f; }
                else
                {
                    if (move != warrokHit && move != warrokDead)
                    {
                        attack = monsterAttackStay;
                        move = warrokChase;
                    }
                }

                if (attack.isActive)
                {
                    nav.isStopped = true;
                    attack.SkillKeyDown();
                    move = monsterMoveStay;

                    StartCoroutine(attack.SkillCoolTime());

                    yield return new WaitForSeconds(time);
                    if (move != warrokHit && move != warrokDead)
                    {
                        move = warrokChase;
                        attack = monsterAttackStay;
                    }
                }
            }

            weapon.SetDamage(currentDamage);
            yield return new WaitForSeconds(0.1f);
        }
    }

    protected override void InitData()
    {
        base.InitData();

        state.hp = 30;
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

        Debug.Log("B");
        base.HitDamage(Damage, instanceID, knockBack, knockBackPower);

        attack = monsterAttackStay;

        if (currentHP <= 0)
        {
            move = warrokDead;
            SetAnimationTrigger("Dead");

            EventManager.instance.SubHitEvent(base.HitDamage);
            EventManager.instance.SubMonsterBuffDamageEvent(BuffDamage);

            StartCoroutine(DeadTime());
        }

        else
        {
            move = warrokHit;
            SetAnimationTrigger("Hit");
        }
    }

    protected override IEnumerator DeadTime()
    {
        GetComponent<Collider>().enabled = false;
        nav.enabled = false;

        yield return new WaitForSeconds(5.0f);

        Color color = new Color(0, 0.78125f, 0.625f, 1);

        ItemList.instance.RespawnSphere(transform.position, color);

        Renderer[] renderer = GetComponentsInChildren<Renderer>();
        float num = 0;
        while (num < 1.5)
        {
            for (int i = 0; i < renderer.Length; i++)
                renderer[i].material.SetFloat("_DissolveAmount", num);

            num += 0.5f * Time.deltaTime;
            yield return null;
        }
        gameObject.SetActive(false);

        for (int i = 0; i < renderer.Length; i++)
        {
            renderer[i].material.SetFloat("_DissolveAmount", 0);
            renderer[i].material.SetColor("_Color", new Color(0.5859f, 0.5859f, 0.5859f, 1));
        }
        GetComponent<Collider>().enabled = true;
        nav.enabled = true;
        buffParticle.gameObject.SetActive(false);

        MonsterPooling.instance.MonsterEnqueue(this);
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

    public void JumpSkillDamage()
    {
        float dis = Vector3.Distance(transform.position, character.transform.position);
        if (dis <= 10.0f)
        {
            Vector3 dir = character.transform.position - transform.position;
            dir.y = 0;
            character.Hit(jumpSkill.GetDamage(), dir.normalized);
        }

        groundImpace.Play();
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

    public void SkillKeyDown()
    {
        monster.SetAnimationBool("Walk", false);

        monster.SetAnimationTrigger("Attack");
    }

    public void SkillKeyUp() { }

    public IEnumerator SkillCoolTime()
    {
        isActive = false;
        yield return new WaitForSeconds(2.0f);
        isActive = true;
    }

    public float GetDamage()
    {
        return monster.GetMonsterDamage();
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

    public void SkillKeyDown()
    {
        monster.SetAnimationBool("Walk", false);
        monster.SetAnimationBool("JumpSkill", true);
    }

    public void SkillKeyUp() { }

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

    public float GetDamageMagnification()
    {
        return magnification;
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

    public void SkillKeyDown()
    {
        monster.SetAnimationBool("Walk", false);
        monster.SetAnimationTrigger("Buff");

        EventManager.instance.MonsterBuffDamage(spawn, buffTime, magnification);
    }

    public void SkillKeyUp() { }

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

    public float GetDamageMagnification()
    {
        return magnification;
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

public class WarrokSummonSkill : ISkill
{
    private Monster monster;
    private float coolTime;
    private MonsterSpawn spawn;

    private List<Monster> monsters = new List<Monster>();
    private int num;

    public bool isActive { get; set; }

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

    public void SkillKeyDown()
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
            summon.SetSummonBoss(true);
        }

        monster.ResetAnimation();
        monster.SetAnimationBool("Summon", true);
    }

    public void SkillKeyUp() { }

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
