﻿using System;
using UnityEngine;
using System.Dynamic;
using System.Collections;

public interface KeyCommand
{
    void command();
}

public interface IRecharge
{
    void Recharge();
}

public interface ICharacterUpdate
{
    void CharacterUpdate();
}


public interface IAttackAction
{
    void Attack();
}

public interface IActiveObj
{
    void Active();
}

public interface IArrowMove
{
    void Move();
}

public interface IMove
{
    void Move();
}

public interface IStand
{
    void Stand(Vector3 dir);
}


public interface ISkill
{ 
    bool isActive { get; set; }
    void Skill();

    IEnumerator SkillCoolTime();

    float GetDamage();
}
