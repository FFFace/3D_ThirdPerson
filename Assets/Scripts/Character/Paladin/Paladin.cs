using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Paladin : Character
{
    private PaladinNormalAttack attack;
    private PaladinSlashAttack slash;

    [SerializeField]
    private Sprite SlashImage;
    [SerializeField]
    private PaladinWeapon weapon;

    protected override void InitData()
    {
        base.InitData();

        state.hp = 30;
        state.attackDamage = 1;
        state.attackSpeed = 1;
        state.moveSpeed = 5;
        state.jumpPower = 5;

        currentMoveSpeed = state.moveSpeed;
        currentAttackDamage = state.attackDamage;
        currentAttackSpeed = state.attackSpeed;
        currentJumpPower = state.jumpPower;
        currentCharacterHP = state.hp;
        currentDefense = state.defense;
        currentKnockBackPower = 0;

        attack = new PaladinNormalAttack(this, weapon);
        slash = new PaladinSlashAttack(this, weapon, 10.0f, 1.5f, SlashImage);

        normalAttack = attack;
        subAttack = slash;
    }

    public void AttackColliderOn()
    {
        weapon.Attack();
    }

    public void AttackColliderOff()
    {
        weapon.AttackEnd();
    }
}

public class PaladinNormalAttack : IAttackAction
{
    private Paladin character;
    private PaladinWeapon weapon;

    public PaladinNormalAttack(Paladin _character, PaladinWeapon _weapon) { character = _character; weapon = _weapon; }

    public void Attack()
    {
        weapon.SetDamage(character.GetCharacterCurrentDamage());
        character.SetAnimationTrigger("Attack");
    }
}

public class PaladinSlashAttack : ISkill
{
    private Paladin character;
    private PaladinWeapon weapon;
    private float coolTime;
    private float damageMagnification;
    private Sprite image;
    public bool isActive { get; set; }

    public PaladinSlashAttack(Paladin _character, PaladinWeapon _weapon, float _coolTime, float _damageManification, Sprite _sprite)
    {
        character = _character;
        weapon = _weapon;
        coolTime = _coolTime;
        damageMagnification = _damageManification;
        image = _sprite;
    }

    public void Skill()
    {
        if (!isActive)
        {
            character.ResetAnimation();
            character.SetisAction(true);
            weapon.SetDamage(character.GetCharacterCurrentDamage() * damageMagnification);
            character.SetAnimationTrigger("Slash");
        }
    }

    public IEnumerator SkillCoolTime()
    {
        if (isActive) yield return null;
        else
        {
            isActive = true;
            yield return new WaitForSeconds(coolTime);
            isActive = false;
        }
    }

    public float GetDamage()
    {
        return character.GetCharacterCurrentDamage() * damageMagnification;
    }

    public Sprite GetImage()
    {
        return image;
    }

    public float GetCoolTime() { return coolTime; }
}
