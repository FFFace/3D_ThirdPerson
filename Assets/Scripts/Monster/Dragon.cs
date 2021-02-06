using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Dragon : Monster
{
    private DragonMove dragonMove;
    private DragonChase dragonChase;

    protected override void OnEnable()
    {
        InitData();
        StartCoroutine(State());
        StartCoroutine(IEnumSummon());
    }

    protected override void Start()
    {
        base.Start();
    }

    private IEnumerator State()
    {
        yield return new WaitForSeconds(4.0f);
        nav.isStopped = false;

        move = dragonChase;
        float time = 0;

        while (true)
        {
            Debug.Log("State");
            float distance = Vector3.Distance(transform.position, nav.destination);

            if (move == dragonMove)
            {
                Debug.Log("Move");
                time += 0.1f;
                if (distance < 1 && time <= 5.0f)
                    nav.destination = room.GetMoveTile();

                if (time > 5.0f)
                {
                    move = dragonChase;
                    Debug.Log("Change Chase");
                }
            }

            else if (move == dragonChase)
            {
                Debug.Log("Chase");
                if (distance > 2)
                {
                    dragonChase.Chase();
                }

                else if (distance < 1.5f)
                {
                    Debug.Log("Change Move");
;                    move = dragonMove;
                    time = 0;
                }
            }

            yield return new WaitForSeconds(0.1f);
        }
    }

    private IEnumerator IEnumSummon()
    {
        float time = 1;
        Renderer[] renderer = GetComponentsInChildren<Renderer>();
        Collider[] collider = GetComponentsInChildren<Collider>();

        for (int i = 0; i < collider.Length; i++)
        {
            collider[i].enabled = false;
        }

        yield return new WaitForSeconds(2f);

        while (time > 0)
        {
            for (int i = 0; i < renderer.Length; i++)
                renderer[i].material.SetFloat("_DissolveAmount", time);

            time -= 0.5f * Time.deltaTime;
            yield return null;
        }

        for (int i = 0; i < collider.Length; i++)
        {
            collider[i].enabled = true;
        }
    }

    protected override void InitData()
    {
        EventManager.instance.AddHitEvent(HitDamage);
        EventManager.instance.AddMonsterBuffDamageEvent(BuffDamage);
        character = Character.instance;
        nav = GetComponent<NavMeshAgent>();

        nav.destination = Character.instance.transform.position;
        nav.enabled = true;
        monsterDirection = new MonsterAttackDirection(this);
        anim = GetComponentInChildren<Animator>();
        attack = monsterAttackStay;
        //rigid.isKinematic = false;

        monsterMoveStay = new MonsterMoveStay(nav);

        ResetAnimation();

        Renderer[] renderer = GetComponentsInChildren<Renderer>();
        for (int i = 0; i < renderer.Length; i++)
            renderer[i].material.SetFloat("_DissolveAmount", 1);

        state.hp = 50;
        state.moveSpeed = 5;
        state.jumpPower = 5;
        state.attackDamage = 5;
        state.attackSpeed = 1;

        currentHP = state.hp;
        currentSpeed = state.moveSpeed;
        currentDefense = state.defense;
        currentDamage = state.attackDamage;

        attack = monsterAttackStay;
        move = monsterMoveStay;

        dragonMove = new DragonMove(this, nav);
        dragonChase = new DragonChase(this, nav);
    }

    protected override void HitDamage(float Damage, int instanceID, bool knockBack = false, float knockBackPower = 0)
    {
        base.HitDamage(Damage, instanceID, knockBack, knockBackPower);
    }
}

public class DragonMove : IMove
{
    private Dragon dragon;
    private NavMeshAgent nav;

    public DragonMove(Dragon _dragon, NavMeshAgent _nav)
    {
        dragon = _dragon;
        nav = _nav;
    }


    public void Move()
    {
        nav.isStopped = false;
    }

    public void MoveUp()
    {
        Vector3 pos = nav.destination;
        pos.y = 6;
        Vector3 dir = (pos - dragon.transform.position).normalized;

        nav.baseOffset += dir.y * 3 * Time.deltaTime;
    }
}

public class DragonChase : IMove
{
    private Dragon dragon;
    private NavMeshAgent nav;

    public DragonChase(Dragon _dragon, NavMeshAgent _nav)
    {
        dragon = _dragon;
        nav = _nav;
    }

    public void Move()
    {
        nav.isStopped = false;
        Vector3 pos = nav.destination;
        pos.y = 1.2f;
        Vector3 dir = (pos - dragon.transform.position).normalized;

        nav.baseOffset += dir.y * 3 * Time.deltaTime;
    }

    public void Chase()
    {
        nav.destination = Character.instance.transform.position;
    }
}