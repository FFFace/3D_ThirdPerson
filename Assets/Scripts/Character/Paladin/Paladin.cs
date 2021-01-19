using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Paladin : Character
{
    private PaladinNormalAttack attack;
    private PaladinSlashAttack slash;
    private PaladinChainAttack chain;

    [SerializeField]
    private Sprite SlashImage;
    [SerializeField]
    private Sprite ChainImage;
    [SerializeField]
    private PaladinWeapon weapon;

    private bool isChain;

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
        chain = new PaladinChainAttack(this, weapon, 3.0f, 1.7f, ChainImage);

        normalAttack = attack;
        subSkill = slash;
        mainSkill = chain;

        isChain = false;
    }

    public override void SubAttack()
    {
        bool active;
        if (subAttack != chain)
            active = !subAttack.isActive && !isAction;
        else
            active = (!subAttack.isActive && !isAction) || (!subAttack.isActive && !isChain);

        if (active)
        {
            isAttack = true;
            isAction = true;
            ResetAnimation();
            subAttack.Skill();

            StartCoroutine(subAttack.SkillCoolTime());
            UIManager.instance.SetSubAttackCoolTime();
        }
    }

    public override void MainSkill()
    {
        bool active;
        if (mainSkill != chain)
            active = !mainSkill.isActive && !isAction;
        else
            active = (!mainSkill.isActive && !isAction) || (!mainSkill.isActive && !isChain);

        if (active)
        {
            isAttack = true;
            isAction = true;
            ResetAnimation();
            mainSkill.Skill();

            StartCoroutine(mainSkill.SkillCoolTime());
            UIManager.instance.SetMainSkillCoolTime();
        }   
    }

    public override void SubSkill()
    {
        bool active;
        if (subAttack != chain)
            active = !subSkill.isActive && !isAction;
        else
            active = (!subSkill.isActive && !isAction) || (!subSkill.isActive && !isChain);

        if (active)
        {
            isAttack = true;
            isAction = true;
            ResetAnimation();
            subSkill.Skill();

            StartCoroutine(subSkill.SkillCoolTime());
            UIManager.instance.SetSubSkillCoolTime();
        }
    }

    public void AttackColliderOn()
    {
        weapon.Attack();
    }

    public void AttackColliderOff()
    {
        weapon.AttackEnd();
    }

    public override void AttackEnd()
    {
        base.AttackEnd();
        SetAnimationBool("Walk", false);
    }

    public void OnChainAttack()
    {
        isChain = false;
    }

    public void OffChainAttack()
    {
        isChain = true;
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
        character.SetAnimationBool("Walk", true);
        character.SetAnimationLayerWeight(1, 1);
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
        isActive = true;
        yield return new WaitForSeconds(coolTime);
        isActive = false;
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

public class PaladinChainAttack : ISkill
{
    private Paladin character;
    private PaladinWeapon weapon;
    private float coolTime;
    private float damageMagnification;
    private Sprite image;
    private int chainNum;
    public bool isActive { get; set; }

    public PaladinChainAttack(Paladin _character, PaladinWeapon _weapon, float _coolTime, float _damageManification, Sprite _sprite)
    {
        character = _character;
        weapon = _weapon;
        coolTime = _coolTime;
        damageMagnification = _damageManification;
        image = _sprite;
        chainNum = 0;
    }

    public void Skill()
    {
        if (!isActive)
        {
            character.ResetAnimation();
            character.SetisAction(true);
            weapon.SetDamage(character.GetCharacterCurrentDamage() * damageMagnification);
            character.SetAnimationTrigger("Chain");
            chainNum++;
        }
    }

    public IEnumerator SkillCoolTime()
    {
        int num = chainNum;
        if (num != 3)
        {
            yield return new WaitForSeconds(0.5f);
            if (chainNum != num)
                yield return null;
            else
            {
                character.OffChainAttack();
                yield return new WaitForSeconds(coolTime);
                isActive = false;
                character.OnChainAttack();
                chainNum = 0;
            }
        }
        else
        {
            character.OffChainAttack();
            yield return new WaitForSeconds(coolTime);
            isActive = false;
            character.OnChainAttack();
            chainNum = 0;
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

