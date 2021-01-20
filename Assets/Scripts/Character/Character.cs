using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEditor.Animations;
using UnityEditor.Experimental.GraphView;
using UnityEditor.MemoryProfiler;
using UnityEngine;

public class Character : MonoBehaviour
{
    public static Character instance;
    protected CharacterState state = new CharacterState();

    protected IAttackAction normalAttack;
    protected ISkill subAttack;
    protected ISkill mainSkill;
    protected ISkill subSkill;
    //protected IRecharge recharge;
    //protected ICharacterUpdate characterUpdate;
    protected IActiveObj activeArrow;
    protected IItemEffect effect;

    protected float currentMoveSpeed;
    protected float currentAttackSpeed;
    protected float currentAttackDamage;
    protected float currentJumpPower;
    protected float currentCharacterHP;
    protected float currentDefense;
    protected float currentKnockBackPower;

    protected float subAttackCoolTime;
    protected float mainSkillCoolTime;
    protected float subSkillCoolTime;

    protected Dictionary<Item, int> items = new Dictionary<Item, int>();
    protected List<IItemEffect> itemEffects = new List<IItemEffect>();

    protected bool isJump;
    public bool isAttack;
    protected bool isAction;
    protected bool isRecharge;

    private Rigidbody rigid;
    protected Animator anim;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    protected virtual void Start()
    {
        InitData();
        //StartCoroutine(CharacterUpdate());
    }

    protected virtual void Update()
    {
        
    }

    protected virtual void InitData()
    {
        rigid = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
    }

    public void Move()
    {
        if (!isAction)
        {
            float x = Input.GetAxis("Horizontal");
            float z = Input.GetAxis("Vertical");

            Vector3 dir = new Vector3(x, 0, z);
            dir = dir.magnitude > 1 ? dir.normalized : dir;

            if (isAttack)
                transform.Translate(dir * currentMoveSpeed * 0.5f * Time.deltaTime);
            else
                transform.Translate(dir * currentMoveSpeed * Time.deltaTime);

            SetAnimationFloat("Horizontal", dir.x);
            SetAnimationFloat("Vertical", dir.z);
        }
    }

    public virtual void AttackPressDown()
    {
        if (!isAction && !isAttack)
        {
            isAttack = true;
            normalAttack.Attack();
        }
    }

    public virtual void AttackPressUp()
    {
        if (!isAttack)
        {
            isAttack = true;
            normalAttack.Attack();
        }
    }

    public virtual void SubAttackPressDown()
    {
        //if (!isSubAttack)
        //{
        //    isSubAttack = true;
        //    isAttack = true;
        //    ResetAnimation();
        //    subAttack.Attack();
        //    StartCoroutine(SubAttackCoolTime(subAttackCoolTime));
        //}

        if (!subAttack.isActive && !isAction)
        {
            isAttack = true;
            isAction = true;
            ResetAnimation();
            subAttack.Skill();

            StartCoroutine(subAttack.SkillCoolTime());
            UIManager.instance.SetSubAttackCoolTime();
        }
    }

    public virtual void SubAttackPressUp()
    {
        if (!subAttack.isActive && !isAction)
        {
            isAttack = true;
            isAction = true;
            ResetAnimation();
            subAttack.Skill();

            StartCoroutine(subAttack.SkillCoolTime());
            UIManager.instance.SetSubAttackCoolTime();
        }
    }

    public virtual void MainSkill()
    {
        if (!mainSkill.isActive && !isAction)
        {
            isAttack = true;
            isAction = true;
            ResetAnimation();
            mainSkill.Skill();

            StartCoroutine(mainSkill.SkillCoolTime());
            UIManager.instance.SetMainSkillCoolTime();
        }
    }

    public virtual void SubSkill()
    {
        if (!subSkill.isActive && !isAction)
        {
            isAttack = true;
            isAction = true;
            ResetAnimation();
            subSkill.Skill();

            StartCoroutine(subSkill.SkillCoolTime());
            UIManager.instance.SetSubSkillCoolTime();
        }
    }

    public virtual void Dodge()
    {
        if (!isAction)
        {
            isAction = true;
            SetAnimationBool("Dodge", true);
        }
    }

    public virtual void Hit(float damage)
    {
        damage -= damage * currentDefense;
        damage = damage < 1 ? 1 : damage;
        currentCharacterHP -= damage;

        if (currentCharacterHP <= 0)
        {
            PlayerControll.instance.SetDeathState(true);
            ResetAnimation();
            SetAnimationBool("Dead", true);
            return;
        }

        SetAnimationTrigger("Hit");
        UIManager.instance.PlayerHit(currentCharacterHP);
    }

    public virtual void Jump()
    {
        if (!isJump)
        {
            rigid.AddForce(Vector3.up * currentJumpPower, ForceMode.Impulse);
            isJump = true;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Ground"))
        {
            Bounds bounds = collision.collider.bounds;
            BoxCollider col = GetComponent<BoxCollider>();
            isJump = col.center.y - col.bounds.size.y / 2 > bounds.center.y + bounds.size.y ? false : true;
        }
    }

    //private IEnumerator CharacterUpdate()
    //{
    //    while (true)
    //    {
    //        characterUpdate.CharacterUpdate();
    //        yield return new WaitForSeconds(0.1f);
    //    }
    //}

    public virtual void AttackEnd()
    {
        isAttack = false;
    }

    public virtual void EndAction(float _time)
    {
        //isAction = false;
        StartCoroutine(IEnumEndAction(_time));
    }

    protected virtual IEnumerator IEnumEndAction(float _time)
    {
        yield return new WaitForSeconds(_time);
        isAction = false;
    }

    public void SetisAction(bool active)
    {
        isAction = active;
    }

    /// <summary>
    /// 크로스 헤어에 잡히는 첫번째 몬스터 Transform 리턴
    /// </summary>
    /// <returns></returns>

    public CharacterState GetCharacterState()
    {
        return state;
    }

    public void SetRecharge(bool active)
    {
        isRecharge = active;
    }

    public float GetCharacterCurrentDamage()
    {
        return currentAttackDamage;
    }

    public void SetAnimationBool(string name, bool active)
    {
        anim.SetBool(name, active);
    }

    public void SetAnimationTrigger(string name)
    {
        anim.SetTrigger(name);
    }

    public void SetAnimationFloat(string name, float num)
    {
        anim.SetFloat(name, num);
    }

    public void ResetAnimation()
    {
        foreach (var some in anim.parameters)
        {
            if (some.type == AnimatorControllerParameterType.Bool)
            {
                anim.SetBool(some.name, false);
            }
        }

        anim.SetLayerWeight(1, 0);
    }

    public void SetAnimationLayerWeight(int layer, float weight)
    {
        anim.SetLayerWeight(layer, weight);
    }

    public void SetSubAttackCoolTime(float time)
    {
        subAttackCoolTime = time;
    }

    public void SetMainSkillCoolTime(float time)
    {
        mainSkillCoolTime = time;
    }

    public void SetSubSkillCoolTime(float time)
    {
        subSkillCoolTime = time;
    }

    public void AddItem(Item _item)
    {
        if (items.ContainsKey(_item))
        {
            items[_item]++;
        }
        else
        {
            items.Add(_item, 1);
            itemEffects.Add(_item.GetItemEffect());
        }

        currentAttackDamage += _item.GetItemDamage;
        currentAttackDamage += state.attackDamage * _item.GetItemDamageMag;
        currentAttackSpeed += state.attackSpeed * _item.GetItemAttackSpeedMag;
        currentCharacterHP += _item.GetItemHP;
        currentCharacterHP += state.hp * _item.GetItemHP;
        currentMoveSpeed += state.moveSpeed * _item.GetItemMoveSpeedMag;

        IItemEffect effect = _item.GetItemEffect();
        if (effect != null)
            itemEffects.Add(effect);
    }

    public void DeleteItem(Item _item)
    {
        if (items.ContainsKey(_item))
        {
            items[_item]--;
            if (items[_item] <= 0)
            {
                items.Remove(_item);
                itemEffects.Remove(_item.GetItemEffect());
            }

            currentAttackDamage -= _item.GetItemDamage;
            currentAttackDamage -= state.attackDamage * _item.GetItemDamageMag;
            currentAttackSpeed -= state.attackSpeed * _item.GetItemAttackSpeedMag;
            currentCharacterHP -= _item.GetItemHP;
            currentCharacterHP -= state.hp * _item.GetItemHP;
            currentMoveSpeed -= state.moveSpeed * _item.GetItemMoveSpeedMag;
        }

        else
            Debug.Log("존재하지 않는 아이템");
    }

    public bool GetItemInDictionary(Item _item)
    {
        if (items.ContainsKey(_item)) return true;
        else return false;
    }

    public List<IItemEffect> GetItemEffectList()
    {
        return itemEffects;
    }
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
