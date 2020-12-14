using System;
using UnityEngine;
using System.Dynamic;
using System.Collections;

public interface KeyCommand
{
    void command();
}

public class NomalAttackPressDown : KeyCommand
{
    private Character character;
    public NomalAttackPressDown(Character _character) { character = _character; }
    public void command()
    {
        character.AttackPressDown();
    }
}

public class NomalAttackPressUp : KeyCommand
{
    private Character character;
    public NomalAttackPressUp(Character _character) { character = _character; }

    public void command()
    {
        character.AttackPressUp();
    }
}

public class SubAttack : KeyCommand
{
    private Character character;
    public SubAttack(Character _character) { character = _character; }
    public void command()
    {
        character.SubAttack();
    }
}

public class Dodge : KeyCommand
{
    private Character character;
    public Dodge(Character _character) { character = _character; }
    public void command()
    {
        character.Dodge();
    }
}

public class MainSkill : KeyCommand
{
    private Character character;
    public MainSkill(Character _character) { character = _character; }
    public void command()
    {
        character.MainSkill();
    }
}

public class SubSkill : KeyCommand
{
    private Character character;
    public SubSkill(Character _character) { character = _character; }
    public void command()
    {
        character.SubSkill();
    }
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

public class CharacterState
{
    public float hp { get; set; }
    public float attackDamage { get; set; }
    public float attackSpeed { get; set; }
    public float moveSpeed { get; set; }
    public float jumpPower { get; set; }
    public float defense { get; set; }
}


