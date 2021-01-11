using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Net.Mime;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

public class Skeleton : Monster
{
    [SerializeField]
    private float runChaseDistance;
    private bool isStand;

    [SerializeField]
    private MonsterWeapon weapon;

    //protected MonsterCharacterCross cross;
    private SkeletonChase skeletonChase;
    //private SkeletonStand skeletonStand;
    private MonsterAttack skeletonAttack;
    private MonsterHit hit;
    private MonsterDead dead;

    protected override void Start()
    {
        base.Start();
        //cross = new MonsterCharacterCross(this as Monster, 3.0f);

        StartCoroutine(State());
        //StartCoroutine(cross.DirUpdate());
    }

    protected IEnumerator State()
    {
        while (true)
        {
            if (move == hit)
            {
                yield return new WaitForSeconds(0.3f);
                move = monsterMoveStay;
                attack = monsterAttackStay;

                ResetAnimation();
                hit.isKnockBack = false;
            }

            else if (move == dead)
            {
                break;
            }

            else
            {
                float dis = Vector3.Distance(transform.position, character.transform.position);
                if (dis < attackDistance)
                {
                    if (monsterDirection.GetinDirection(attackDirection))
                    {
                        nav.isStopped = true;
                        attack = skeletonAttack;
                        move = monsterMoveStay;

                        attack.Skill();
                        StartCoroutine(attack.SkillCoolTime());

                        float time = Random.Range(2.0f, 5.0f);
                        yield return new WaitForSeconds(time);

                        if (move != hit && move != dead)
                        {
                            attack = monsterAttackStay;
                            move = skeletonChase;
                        }
                    }
                }
                else
                {
                    if (move != hit && move != dead)
                    {
                        attack = monsterAttackStay;
                        move = skeletonChase;
                    }

                    if (dis > runChaseDistance && nav.speed < currentSpeed * 1.5f) nav.speed = currentSpeed * 3f;
                    else nav.speed = currentSpeed;
                }
            }

            weapon.SetDamage(currentDamage);
            yield return new WaitForSeconds(0.1f);
        }
    }

    protected override void InitData()
    {
        base.InitData();

        state.hp = 10;
        state.moveSpeed = 3;
        state.jumpPower = 5;
        state.attackDamage = 1;
        state.attackSpeed = 1;

        currentHP = state.hp;
        currentSpeed = state.moveSpeed;
        currentDefense = state.defense;
        currentDamage = state.attackDamage;

        skeletonChase = new SkeletonChase(this, nav);
        skeletonAttack = new MonsterAttack(this as Monster, 0);
        //chase = new SkeletonChase(this, nav, currentSpeed);
        //stand = new SkeletonStand(this, nav, currentSpeed);
        hit = new MonsterHit(this, nav);
        dead = new MonsterDead(this, nav);

        move = skeletonChase;
        attack = monsterAttackStay;
    }

    protected override void HitDamage(float Damage, int instanceID, bool knockBack = false, float knockBackPower = 0)
    {
        if (transform.GetInstanceID() != instanceID) return;

        base.HitDamage(Damage, instanceID, knockBack, knockBackPower);

        if (knockBack)
        {
            hit.isKnockBack = true;
            hit.SetKnockbackPower(knockBackPower);

            Vector3 dir = character.transform.forward;
            hit.SetKnockBackDir(dir);
        }

        attack = monsterAttackStay;

        if (currentHP <= 0)
        {
            SetAnimationTrigger("Dead");
            move = dead;

            EventManager.instance.SubHitEvent(base.HitDamage);
            EventManager.instance.SubMonsterBuffDamageEvent(BuffDamage);

            StartCoroutine(DeadTime());
        }

        else if(move != hit)
        {
            SetAnimationTrigger("Hit");
            move = hit;
        }
    }

    public float GetRunChaseDistance()
    {
        return runChaseDistance;
    }

    protected override IEnumerator DeadTime()
    {
        yield return new WaitForSeconds(5.0f);

        Color color = isSummonBoss ? new Color(0, 0.78125f, 0.625f, 1) : new Color(0.5859f, 0.5859f, 0.5859f, 1);

        ItemList.instance.RespawnSphere(transform.position, color);

        Renderer[] renderer = GetComponentsInChildren<Renderer>();
        float num = 0;
        while (num < 1.5)
        {
            for (int i = 0; i < renderer.Length; i++)
                renderer[i].material.SetFloat("_DissolveAmount", num);

            num += Time.deltaTime;
            yield return null;
        }
        gameObject.SetActive(false);
    }

    public void SetSkeletonMoveStand()
    {
        //move = skeletonStand;
    }

    public void SetSkeletonMoveChase()
    {
        move = skeletonChase;
    }

    public void EnableWeapon()
    {
        weapon.SetActiveCollider(true);
    }

    public void DisableWeapon()
    {
        weapon.SetActiveCollider(false);
    }
}

public class SkeletonChase : IMove
{
    private Skeleton skeleton;
    private Character character;
    private NavMeshAgent nav;
    private MonsterAttackDirection attackDirection;

    //private float currentSpeed;
   // private float distance;

    private IStand move;
    //private SkeletonStand skeletonStand;

    public SkeletonChase(Skeleton _skeleton, NavMeshAgent _nav)
    {
        skeleton = _skeleton;
        nav = _nav;
        character = Character.instance;
        attackDirection = new MonsterAttackDirection(skeleton);
        //distance = _distance;
        //currentSpeed = _speed;
    }

    public void Move()
    {
        nav.isStopped = false;
        nav.destination = character.transform.position;

        // NavMeshAgent를 통해 목적지에 도착 시, 이동뿐만 아니라 회전도 멈추기 때문에 직접 회전
        skeleton.transform.rotation = Quaternion.Lerp(skeleton.transform.rotation, Quaternion.LookRotation(skeleton.GetLookCharacterRotation()), 15 * Time.deltaTime);

        //float dis = Vector3.Distance(skeleton.transform.position, character.transform.position);

        //float speed = currentSpeed;
        //speed = !(speed == currentSpeed * 1.5f) ? dis > skeleton.GetRunChaseDistance() ? currentSpeed * 1.5f : currentSpeed : currentSpeed * 1.5f;

        //string name = (speed == currentSpeed) ? "Walk" : "Run";

        skeleton.SetAnimationBool("Walk", true);
        skeleton.SetAnimationFloat("Speed", nav.speed);

        //nav.speed = speed;

        //if (!skeleton.GetMonsterAttackState())
        //{
        //    if (dis < distance && attackDirection.GetinDirection(60))
        //    {
        //        skeleton.SetMonsterState(Monster.MonsterAction.ATTACK);
        //    }
        //}

        //else
        //    skeleton.SetSkeletonMoveStand();
    }
}

//public class SkeletonChase : IMove
//{
//    private Skeleton skeleton;
//    private Character character;
//    private float currentSpeed;
//    private NavMeshAgent nav;

//    public SkeletonChase(Skeleton _skeleton, NavMeshAgent _nav, float _speed) 
//    {
//        skeleton = _skeleton;
//        nav = _nav; 
//        currentSpeed = _speed;
//        character = Character.instance;
//    }

//    public void Move()
//    {
//        // NavMeshAgent를 통해 목적지에 도착 시, 이동뿐만 아니라 회전도 멈추기 때문에 직접 회전
//        skeleton.transform.rotation = Quaternion.Lerp(skeleton.transform.rotation, Quaternion.LookRotation(skeleton.GetLookCharacterRotation()), 15 * Time.deltaTime);


//        float dis = Vector3.Distance(skeleton.transform.position, character.transform.position);

//        float speed = currentSpeed;
//        speed = !(speed == currentSpeed * 1.5f) ? dis > skeleton.GetRunChaseDistance() ? currentSpeed * 1.5f : currentSpeed : currentSpeed * 1.5f;

//        string name = (speed == currentSpeed) ? "Walk" : "Run";
//        skeleton.SetAnimationBool(name, true);
//        nav.speed = speed;
//    }
//}

//public class SkeletonStand : IStand
//{
//    private Skeleton skeleton;
//    private NavMeshAgent nav;
//    private float speed;

//    public SkeletonStand(Skeleton _skeleton, NavMeshAgent _nav, float _speed)
//    {
//        skeleton = _skeleton;
//        nav = _nav;
//        speed = _speed;
//    }

//    public void Stand(Vector3 dir)
//    {
//        nav.isStopped = false;

//        skeleton.SetAnimationBool("Run", false);
//        skeleton.SetAnimationBool("Walk", true);

//        // NavMeshAgent를 통해 목적지에 도착 시, 이동뿐만 아니라 회전도 멈추기 때문에 직접 회전
//        skeleton.transform.rotation = Quaternion.Lerp(skeleton.transform.rotation, Quaternion.LookRotation(dir), 15 * Time.deltaTime);

//        // 이동하고자 하는 방향에 거리를 조금 더 두어 NevMeshAgent의 Stopping Distance의 영향을 받지 않음.
//        nav.destination = skeleton.transform.position + dir * 2;

//        if (!skeleton.GetMonsterAttackState()) skeleton.SetSkeletonMoveChase();
//    }
//}
