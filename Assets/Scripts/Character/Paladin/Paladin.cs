using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Paladin : Character
{
    private PaladinNormalAttack attack;
    private PaladinSlashAttack slash;
    private PaladinChainAttack chain;
    private PaladinBlock block;

    [SerializeField]
    private Sprite SlashImage;
    [SerializeField]
    private Sprite ChainImage;
    [SerializeField]
    private Sprite BlockImage;

    [SerializeField]
    private PaladinWeapon weapon;

    private bool isChain;
    public bool isBlock;
    private bool isJustBlock;

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
        block = new PaladinBlock(this, 1.5f, 10.0f, BlockImage);

        normalAttack = attack;
        subSkill = slash;
        mainSkill = chain;
        subAttack = block;

        isChain = false;
    }

    public override void Hit(float damage, Vector3 direction)
    {
        float dot = Vector3.Dot(transform.forward, direction);

        if (isBlock && dot < 0)
        {
            if (isJustBlock)
            {
                StopCoroutine(block.buff);
                StartCoroutine(block.buff);
                damage = 0;
            }

            else
            {
                damage = damage * 0.5f;
            }
        }

        damage -= damage * currentDefense;
        damage = damage < 0 ? 0 : damage;
        currentCharacterHP -= damage;

        if (currentCharacterHP <= 0)
        {
            PlayerControll.instance.SetDeathState(true);
            ResetAnimation();
            SetAnimationBool("Dead", true);
            return;
        }

        SetAnimationTrigger("Hit");
        SetAnimationLayerWeight(1, 1);
        UIManager.instance.PlayerHit(currentCharacterHP);
    }

    public override void SubAttackPressDown()
    {
        bool active;

        if (subAttack == chain)
            active = (!subAttack.isActive && !isAction) || !isChain;

        else if (subAttack == block)
            active = (!subAttack.isActive && !isAction) || isBlock;

        else
            active = !subAttack.isActive && !isAction;

        if (active)
        {
            isAttack = true;
            isAction = true;
            ResetAnimation();
            subAttack.Skill();

            if (subSkill != chain)
                StartCoroutine(subAttack.SkillCoolTime());
        }
    }

    public override void SubAttackPressUp()
    {
        bool active;

        if (subAttack == chain)
            active = (!subAttack.isActive && !isAction) || !isChain;

        else if (subAttack == block)
            active = (!subAttack.isActive && !isAction) || isBlock;

        else
            active = !subAttack.isActive && !isAction;

        if (active)
        {
            isAttack = true;
            isAction = true;
            ResetAnimation();
            subAttack.Skill();

            if (subSkill != chain)
                StartCoroutine(subAttack.SkillCoolTime());
        }
    }

    public override void MainSkill()
    {
        bool active;
        if (mainSkill == chain)
            active = (!mainSkill.isActive && !isAction) || !isChain;

        else if (mainSkill == block)
            active = (!mainSkill.isActive && !isAction) || isBlock;

        else
            active = !mainSkill.isActive && !isAction;

        if (active)
        {
            isAttack = true;
            isAction = true;
            ResetAnimation();
            mainSkill.Skill();

            if(mainSkill != chain)
                StartCoroutine(mainSkill.SkillCoolTime());
        }   
    }

    public override void SubSkill()
    {
        bool active;
        if (subSkill == chain)
            active = (!subSkill.isActive && !isAction) || !isChain;

        else if (subSkill == block)
            active = (!subSkill.isActive && !isAction) || isBlock;

        else
            active = !subSkill.isActive && !isAction;

        if (active)
        {
            isAttack = true;
            isAction = true;
            ResetAnimation();
            subSkill.Skill();

            if (subSkill != chain)
                StartCoroutine(subSkill.SkillCoolTime());
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

    public void ChainCombo()
    {
        chain.isActive = false;
        isChain = false;
    }

    public void ChainComboEnd()
    {
        chain.isActive = true;
        isChain = true;
    }

    public void ChainCoolTime()
    {
        StartCoroutine(chain.SkillCoolTime());
    }

    public void JuskBlock()
    {
        isJustBlock = true;
    }

    public void JustBlockEnd()
    {
        isJustBlock = false;
    }

    public void BlockActive(bool _active)
    {
        isBlock = _active;
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
        UIManager.instance.SetMainSkillCoolTime();
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
    public bool isActive { get; set; }

    public PaladinChainAttack(Paladin _character, PaladinWeapon _weapon, float _coolTime, float _damageManification, Sprite _sprite)
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
            character.SetAnimationTrigger("Chain");
        }
    }

    public IEnumerator SkillCoolTime()
    {
        isActive = true;
        UIManager.instance.SetMainSkillCoolTime();
        yield return new WaitForSeconds(coolTime);
        character.ChainCombo();

        //character.OffChainAttack();
        //int num = chainNum;

        //if (num != 3)
        //{
        //    yield return new WaitForSeconds(1.5f);
        //    Debug.Log(num.ToString() + ", " + chainNum.ToString());
        //    if (chainNum != num)
        //        yield return null;
        //    else
        //    {
        //        character.OffChainAttack();
        //        isActive = true;
        //        yield return new WaitForSeconds(coolTime);
        //        Debug.Log("B");
        //        isActive = false;
        //        character.OnChainAttack();
        //        chainNum = 0;
        //    }
        //}
        //else
        //{
        //    character.OffChainAttack();
        //    isActive = true;
        //    yield return new WaitForSeconds(coolTime);
        //    isActive = false;
        //    character.OnChainAttack();
        //    chainNum = 0;
        //}
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

public class PaladinBlock : ISkill
{
    private Paladin character;
    private float damageMagnification;
    private float buffTime;
    private Sprite image;
    public IEnumerator buff { get; }
    public bool isActive { get; set; }

    public PaladinBlock(Paladin _character, float _damageMagnification, float _buffTime, Sprite _sprite)
    {
        character = _character;
        damageMagnification = _damageMagnification;
        buffTime = _buffTime;
        image = _sprite;
        buff = JustBlockBuff();
    }

    public void Skill()
    {
        character.ResetAnimation();
        character.SetisAction(true);
        character.SetAnimationBool("Block", true);
        character.BlockActive(true);

        if (Input.GetMouseButtonUp(1))
        {
            character.ResetAnimation();
            character.BlockActive(false);
        }
    }

    public IEnumerator JustBlockBuff()
    {
        float damage = character.GetCharacterCurrentDamage();
        character.SetCharacterCurrentDamage(damage * damageMagnification);
        yield return new WaitForSeconds(buffTime);
        character.SetCharacterCurrentDamage(damage);
    }

    public IEnumerator SkillCoolTime()
    {
        yield return null;
    }

    public float GetDamage()
    {
        return 0;
    }

    public Sprite GetImage()
    {
        return image;
    }

    public float GetCoolTime() { return 0; }
}