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

    private SkeletonChase skeletonChase;
    private MonsterAttack skeletonAttack;
    private MonsterHit hit;
    private MonsterDead dead;

    protected override void OnEnable()
    {
        base.OnEnable();
        StartCoroutine(State());
    }

    protected IEnumerator State()
    {
        yield return new WaitForSeconds(4f);
       // move = stay;
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
                nav.angularSpeed = 0;
                float dis = Vector3.Distance(transform.position, character.transform.position);
                if (dis < attackDistance)
                {
                    if (monsterDirection.GetinDirection(attackDirection))
                    {
                        nav.isStopped = true;
                        attack = skeletonAttack;
                        move = monsterMoveStay;

                        attack.SkillKeyDown();
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
        hit = new MonsterHit(this, nav);
        dead = new MonsterDead(this, nav);

        move = monsterMoveStay;
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
        GetComponent<Collider>().enabled = false;
        nav.enabled = false;

        yield return new WaitForSeconds(5.0f);

        Color color = isSummonBoss ? new Color(0.5859f, 0.5859f, 0.5859f, 1) : new Color(0, 0.78125f, 0.625f, 1);

        ItemList.instance.RespawnSphere(transform.position, color);

        Renderer[] renderer = GetComponentsInChildren<Renderer>();
        float num = 0;
        while (num < 1.5f)
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
            renderer[i].material.SetFloat("_RedColor", 0);
        }
        GetComponent<Collider>().enabled = true;
        nav.enabled = true;
        buffParticle.gameObject.SetActive(false);

        MonsterPooling.instance.MonsterEnqueue(this);
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

    public SkeletonChase(Skeleton _skeleton, NavMeshAgent _nav)
    {
        skeleton = _skeleton;
        nav = _nav;
        character = Character.instance;
        attackDirection = new MonsterAttackDirection(skeleton);
    }

    public void Move()
    {
        nav.isStopped = false;
        nav.destination = character.transform.position;

        skeleton.transform.rotation = Quaternion.Lerp(skeleton.transform.rotation, Quaternion.LookRotation(skeleton.GetLookCharacterRotation()), 15 * Time.deltaTime);

        skeleton.SetAnimationBool("Walk", true);
        skeleton.SetAnimationFloat("Speed", nav.speed);
    }
}