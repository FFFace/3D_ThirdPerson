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

    protected IAttackAction nomalAttack;
    protected ISkill subAttack;
    protected ISkill mainSkill;
    protected ISkill subSkill;
    protected IRecharge recharge;
    protected ICharacterUpdate characterUpdate;
    protected IActiveObj activeArrow;
    protected IItemEffect effect;

    protected float currentMoveSpeed;
    protected float currentAttackSpeed;
    protected float currentAttackDamage;
    protected float currentJumpPower;
    protected float currentCharacterHP;
    protected float currentDefense;
    protected float currentKnockBackPower;

    [SerializeField]
    protected float rechargeTime;
    protected float subAttackCoolTime;
    protected float mainSkillCoolTime;
    protected float subSkillCoolTime;


    protected Dictionary<Item, int> items = new Dictionary<Item, int>();
    protected List<IItemEffect> itemEffects = new List<IItemEffect>();

    [SerializeField]
    protected Arrow arrow;
    private Queue<Arrow> arrows = new Queue<Arrow>();

    [SerializeField]
    private Transform arrowFireTR;

    protected bool isJump;
    protected bool isDodge;
    protected bool isAttack;
    protected bool isSubAttack;
    protected bool isMainSkill;
    protected bool isSubSkill;
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
        StartCoroutine(CharacterUpdate());
    }

    private void Update()
    {
        Vector3 pos = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 5));
        Ray ray = new Ray(pos, Camera.main.transform.forward);
    }

    protected virtual void InitData()
    {
        rigid = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
    }

    protected virtual Arrow CreateArrow()
    {
        Arrow obj = Instantiate(arrow, transform.position, transform.rotation) as Arrow;
        obj.gameObject.SetActive(false);
        return obj;
    }

    public void ArrowEnqueue(Arrow obj)
    {
        arrows.Enqueue(obj);
    }

    public Arrow ArrowDequeue()
    {
        Arrow obj = arrows.Count > 0 ? arrows.Dequeue() : CreateArrow();
        obj.SetRecharge(isRecharge);
        return obj.gameObject.activeSelf ? CreateArrow() : obj;
    }

    public Transform GetArrowFireTR()
    {
        arrowFireTR.localRotation = Quaternion.Euler(new Vector3(0, 0, 0));

        Vector3 rot = arrowFireTR.rotation.eulerAngles;
        rot += new Vector3(-Random.Range(10.0f, 20.0f), Random.Range(-15.0f, 15.0f), 0);
        arrowFireTR.rotation = Quaternion.Euler(rot);

        return arrowFireTR;
    }

    public void Move()
    {
        if (!isDodge)
        {
            float x = Input.GetAxis("Horizontal");
            float z = Input.GetAxis("Vertical");

            Vector3 dir = new Vector3(x, 0, z);
            dir = dir.magnitude > 1 ? dir.normalized : dir;

            transform.Translate(dir * currentMoveSpeed * Time.deltaTime);

            SetAnimationFloat("Horizontal", dir.x);
            SetAnimationFloat("Vertical", dir.z);
        }
    }

    public virtual void AttackPressDown()
    {
        recharge.Recharge();
    }

    public virtual void AttackPressUp()
    {
        if (!isAttack)
        {
            isAttack = true;
            nomalAttack.Attack();
        }
    }

    public virtual void SubAttack()
    {
        //if (!isSubAttack)
        //{
        //    isSubAttack = true;
        //    isAttack = true;
        //    ResetAnimation();
        //    subAttack.Attack();
        //    StartCoroutine(SubAttackCoolTime(subAttackCoolTime));
        //}

        if (!subAttack.isActive)
        {
            isAttack = true;
            ResetAnimation();
            subAttack.Skill();
            StartCoroutine(subAttack.SkillCoolTime());
        }
    }

    public virtual void MainSkill()
    {
        mainSkill.Skill();
    }

    public virtual void SubSkill()
    {
        subSkill.Skill();
    }

    public virtual void Dodge()
    {
        if (!isDodge)
        {
            isDodge = true;
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

    private IEnumerator CharacterUpdate()
    {
        while (true)
        {
            characterUpdate.CharacterUpdate();
            yield return new WaitForSeconds(0.1f);
        }
    }

    public virtual void AttackEnd()
    {
        isAttack = false;
        SetAnimationBool("Recharge", false);
    }
    /// <summary>
    /// 크로스 헤어에 잡히는 첫번째 몬스터 Transform 리턴
    /// </summary>
    /// <returns></returns>
    public Transform MonsterInCrossHair()
    {
        Vector3 pos = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 5));
        Ray ray = new Ray(pos, Camera.main.transform.forward);
        RaycastHit hit;
        LayerMask layer = 1 << LayerMask.NameToLayer("Monster");

        return Physics.BoxCast(pos, new Vector3(0.25f, 0.25f, 0.25f), transform.forward, out hit, transform.rotation, 20, layer) ? hit.transform : null;
        //return Physics.Raycast(ray, out hit, 20, layer) ? hit.transform : null;
    }

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

        currentAttackDamage += _item.GetItemDamage();
        currentAttackDamage += state.attackDamage * _item.GetItemDamageMag();
        currentAttackSpeed += state.attackSpeed * _item.GetItemAttackSpeedMag();
        currentCharacterHP += _item.GetItemHP();
        currentCharacterHP += state.hp * _item.GetItemHP();
        currentMoveSpeed += state.moveSpeed * _item.GetItemMoveSpeedMag();
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

            currentAttackDamage -= _item.GetItemDamage();
            currentAttackDamage -= state.attackDamage * _item.GetItemDamageMag();
            currentAttackSpeed -= state.attackSpeed * _item.GetItemAttackSpeedMag();
            currentCharacterHP -= _item.GetItemHP();
            currentCharacterHP -= state.hp * _item.GetItemHP();
            currentMoveSpeed -= state.moveSpeed * _item.GetItemMoveSpeedMag();
        }

        else
            Debug.Log("존재하지 않는 아이템");
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
