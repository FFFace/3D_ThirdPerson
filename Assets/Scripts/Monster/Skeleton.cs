using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

public class Skeleton : Monster
{
    [SerializeField]
    private float runChaseDistance;

    protected MonsterCharacterCross cross;

    protected override void Start()
    {
        base.Start();
        cross = new MonsterCharacterCross(this as Monster, 3.0f);

        StartCoroutine(cross.DirUpdate());
    }

    protected override void Update()
    {
        base.Update();
        cross.CrossUpdate();
    }

    protected override void Stand()
    {
        nav.isStopped = false;
        stand.Stand(cross.GetCross());
        base.Stand();
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

        attack = new MonsterAttack(this as Monster, 0);
        chase = new SkeletonChase(this, nav, currentSpeed);
        stand = new SkeletonStand(this, nav, currentSpeed);
        hit = new MonsterHit(this as Monster);
        dead = new MonsterDead(this as Monster);
    }

    public float GetRunChaseDistance()
    {  
        return runChaseDistance;
    }

    protected override IEnumerator DeadTime()
    {
        base.DeadTime();
        MonsterPooling.instance.MonsterEnqueue<Skeleton>(this);

        yield return null;
    }
}

public class SkeletonChase : IMove
{
    private Skeleton skeleton;
    private Character character;
    private float currentSpeed;
    private NavMeshAgent nav;

    public SkeletonChase(Skeleton _skeleton, NavMeshAgent _nav, float _speed) 
    {
        skeleton = _skeleton;
        nav = _nav; 
        currentSpeed = _speed;
        character = Character.instance;
    }

    public void Move()
    {
        // NavMeshAgent를 통해 목적지에 도착 시, 이동뿐만 아니라 회전도 멈추기 때문에 직접 회전
        skeleton.transform.rotation = Quaternion.Lerp(skeleton.transform.rotation, Quaternion.LookRotation(skeleton.GetLookCharacterRotation()), 15 * Time.deltaTime);
  

        float dis = Vector3.Distance(skeleton.transform.position, character.transform.position);

        float speed = currentSpeed;
        speed = !(speed == currentSpeed * 1.5f) ? dis > skeleton.GetRunChaseDistance() ? currentSpeed * 1.5f : currentSpeed : currentSpeed * 1.5f;

        string name = (speed == currentSpeed) ? "Walk" : "Run";
        skeleton.SetAnimationBool(name, true);
        nav.speed = speed;
    }
}

public class SkeletonStand : IStand
{
    private Skeleton skeleton;
    private NavMeshAgent nav;
    private float speed;

    public SkeletonStand(Skeleton _skeleton, NavMeshAgent _nav, float _speed)
    {
        skeleton = _skeleton;
        nav = _nav;
        speed = _speed;
    }

    public void Stand(Vector3 dir)
    {
        skeleton.SetAnimationBool("Run", false);
        skeleton.SetAnimationBool("Walk", true);

        // NavMeshAgent를 통해 목적지에 도착 시, 이동뿐만 아니라 회전도 멈추기 때문에 직접 회전
        skeleton.transform.rotation = Quaternion.Lerp(skeleton.transform.rotation, Quaternion.LookRotation(dir), 15 * Time.deltaTime);

        // 이동하고자 하는 방향에 거리를 조금 더 두어 NevMeshAgent의 Stopping Distance의 영향을 받지 않음.
        nav.destination = skeleton.transform.position + dir * 2;
    }
}
