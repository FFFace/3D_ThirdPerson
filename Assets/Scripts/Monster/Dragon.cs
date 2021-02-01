using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Dragon : Monster
{
    private DragonMove dragonMove;

    protected override void InitData()
    {
        base.InitData();

        state.hp = 30;
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
}