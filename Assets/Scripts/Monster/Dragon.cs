using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Dragon : Monster
{
    private DragonMove dragonMove;
    private DragonChase dragonChase;

    protected override void OnEnable()
    {
        base.OnEnable();
        StartCoroutine(State());
        Debug.Log("D");
    }

    private IEnumerator State()
    {
        Debug.Log("E");
        yield return new WaitForSeconds(4.0f);
        Debug.Log("F");
        nav.isStopped = false;

        move = dragonChase;
        float time = 0;

        while (true)
        {
            float distance = Vector3.Distance(transform.position, nav.destination);

            if (move == dragonMove)
            {
                time += 0.1f;
                if (distance < 1)
                    nav.destination = room.GetMoveTile();

                if (time > 5.0f)
                    move = dragonChase;
            }

            else if (move == dragonChase)
            {
                if (distance > 2)
                {
                    dragonChase.Chase();
                }

                else if (distance < 1)
                {
                    move = dragonMove;
                }
            }

            yield return new WaitForSeconds(0.1f);
        }
    }

    protected override void InitData()
    {
        base.InitData();

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

        anim = GetComponentInChildren<Animator>();

        dragonMove = new DragonMove(this, nav);
        dragonChase = new DragonChase(nav);

        Debug.Log("C");
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
        Debug.Log("B");
        nav.isStopped = false;
    }

    public void MoveUp()
    {
        Vector3 dir = nav.destination;
        dir.y = 6;
        dir.Normalize();

        nav.baseOffset += dir.y * 6 * Time.deltaTime;
    }
}

public class DragonChase : IMove
{
    private NavMeshAgent nav;

    public DragonChase(NavMeshAgent _nav)
    {
        nav = _nav;
    }

    public void Move()
    {
        Debug.Log("A");
        nav.isStopped = false;
        Vector3 dir = nav.destination;
        dir.y = 1;
        dir.Normalize();

        nav.baseOffset -= dir.y * Time.deltaTime;
    }

    public void Chase()
    {
        nav.destination = Character.instance.transform.position;
    }
}