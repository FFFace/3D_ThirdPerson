using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    public static Character instance;
    protected CharacterState state = new CharacterState();

    protected IAttackAction normalAttack;
    protected ISkill subAttack;
    protected ISkill mainSkill;
    protected ISkill subSkill;
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
        Debug.Log("Awake " + anim + name);

        if (instance == null) instance = this;
        else Destroy(gameObject);

        rigid = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
    }

    protected virtual void Start()
    {
        InitData();
    }

    protected virtual void Update()
    {

    }

    protected virtual void InitData()
    {
        Debug.Log("Inint " + anim + name);

        if (CharacterInfo.instance.GetSkills(0) != null)
        {
            mainSkill = CharacterInfo.instance.GetSkills(0);
            UIManager.instance.SetMainSkill(mainSkill);
        }
        if (CharacterInfo.instance.GetSkills(1) != null)
        {
            subSkill = CharacterInfo.instance.GetSkills(1);
            UIManager.instance.SetSubSkill(subSkill);
        }
        if (CharacterInfo.instance.GetSkills(2) != null)
        {
            subAttack = CharacterInfo.instance.GetSkills(2);
            UIManager.instance.SetSubAttack(subAttack);
        }

        UIManager.instance.SetPlayerMaxHPBar(state.hp);
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
        if (!subAttack.isActive && !isAction)
        {
            isAttack = true;
            isAction = true;
            ResetAnimation();
            subAttack.SkillKeyDown();

            StartCoroutine(subAttack.SkillCoolTime());
            UIManager.instance.SetSubAttackCoolTime(subAttack.isActive);
        }
    }

    public virtual void SubAttackPressUp()
    {
        if (!subAttack.isActive && !isAction)
        {
            isAttack = true;
            isAction = true;
            ResetAnimation();
            subAttack.SkillKeyUp();

            StartCoroutine(subAttack.SkillCoolTime());
            UIManager.instance.SetSubAttackCoolTime(subAttack.isActive);
        }
    }

    public virtual void MainSkillPressDown()
    {
        if (!mainSkill.isActive && !isAction)
        {
            isAttack = true;
            isAction = true;
            ResetAnimation();
            mainSkill.SkillKeyDown();

            StartCoroutine(mainSkill.SkillCoolTime());
            UIManager.instance.SetMainSkillCoolTime(mainSkill.isActive);
        }
    }

    public virtual void MainSkillPressUp()
    {

    }

    public virtual void SubSkillPressDown()
    {
        if (!subSkill.isActive && !isAction)
        {
            isAttack = true;
            isAction = true;
            ResetAnimation();
            subSkill.SkillKeyUp();

            StartCoroutine(subSkill.SkillCoolTime());
            UIManager.instance.SetSubSkillCoolTime(subSkill.isActive);
        }
    }

    public virtual void SubSkillPressUp()
    {

    }

    public virtual void Dodge()
    {
    }

    public virtual void Hit(float damage, Vector3 direction)
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

    public virtual void AttackEnd()
    {
        isAttack = false;
    }

    public virtual void EndAction(float _time)
    {
        StartCoroutine(IEnumEndAction(_time));
    }

    protected virtual IEnumerator IEnumEndAction(float _time)
    {
        yield return new WaitForSeconds(_time);
        isAction = false;
    }

    public void ActionActive()
    {
        isAction = true;
    }

    public void AttackActive()
    {
        isAttack = true;
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

    public void SetCharacterCurrentDamage(float _damage)
    {
        currentAttackDamage = _damage;
    }

    public void SetAnimationBool(string name, bool active)
    {
        anim.SetBool(name, active);
    }

    public void SetAnimationTrigger(string name)
    {
        anim.SetTrigger(name);
    }

    public void SetAnimationFloat(string name_, float num)
    {
        Debug.Log("SetAnimationFloat " + anim + name);
        anim.SetFloat(name_, num);
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

    public bool GetActionState()
    {
        return isAction;
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
