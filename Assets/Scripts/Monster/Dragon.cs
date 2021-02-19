using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.AI;

public class Dragon : Monster
{
    [SerializeField]
    private bool isHead;

    [SerializeField]
    private int bodyMaxNum;
    private int bodyNum;

    [SerializeField]
    private GameObject body;
    public Dragon front;
    public Dragon back;

    [SerializeField]
    private float maxHeight;
        [SerializeField]
    private float minHeight;

    private static DragonHeadMove headMove;
    private static DragonHeadMoveStay headMoveStay;
    private DragonBodyMove bodyMove;

    protected override void OnEnable()
    {
        InitData();
        StartCoroutine(State());
    }

    protected override void Start()
    {
        if (headMove == null)
        {
            headMove = new DragonHeadMove(this, maxHeight, minHeight, 5f);
            headMoveStay = new DragonHeadMoveStay();
        }

        else
            bodyMove = new DragonBodyMove(this, front);

        if (isHead)
        {
            move = headMove;
            bodyNum = 1;
        }
        else
        {
            move = bodyMove;
        }

        if (bodyNum < bodyMaxNum)
        {
            GameObject obj = Instantiate(body);
            Dragon dragon = obj.GetComponent<Dragon>();

            back = dragon;
            dragon.bodyMaxNum = bodyMaxNum;
            dragon.bodyNum = bodyNum + 1;
            dragon.front = this;
            dragon.body = body;
            dragon.isHead = false;
        }
        else
            back = null;
    }

    private IEnumerator State()
    {
        while (true)
        {

            yield return new WaitForSeconds(0.3f);

            headMove.SetTarget();
        }
    }

    protected override void InitData()
    {
        EventManager.instance.AddHitEvent(HitDamage);
        character = Character.instance;

        state.hp = 100;
        state.attackDamage = 1;

        currentHP = state.hp;
        currentDamage = state.attackDamage;

        //nav = GetComponent<NavMeshAgent>();

        //nav.destination = Character.instance.transform.position;
        //nav.enabled = true;
        //monsterDirection = new MonsterAttackDirection(this);
        anim = GetComponent<Animator>();
        rigid = GetComponent<Rigidbody>();
        GetComponent<Collider>().enabled = true;
        attack = monsterAttackStay;

        if (headMove != null) move = headMove;
        //rigid.isKinematic = false;

        //ResetAnimation();
        transform.parent = null;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Character"))
        {
            Vector3 dir = other.transform.position - transform.position;
            dir.y = 0;
            dir.Normalize();
            character.Hit(currentDamage, dir);
        }
    }

    protected override void HitDamage(float Damage, int instanceID, bool knockBack = false, float knockBackPower = 0)
    {
        if (transform.GetInstanceID() != instanceID) return;

        currentHP -= Damage;
        HitDamageFront(Damage);
        HitDamageBack(Damage);

        if (currentHP < 1)
        {
            move = headMoveStay;
            StartCoroutine("DeadTime");
            return;
        }
        HitColor();
    }

    private void HitColor()
    {
        Renderer renderer = GetComponent<Renderer>();
        renderer.material.SetColor("_Color", new Color(0.5f, 0, 0, 1));
    }

    private void HitDamageFront(float _damage)
    {
        if (front)
        {
            front.currentHP -= _damage;
            if (front.currentHP < 1) StartCoroutine(front.DeadTime());
            else front.HitColor();

            front.HitDamageFront(_damage);
        }
    }

    private void HitDamageBack(float _damage)
    {
        if (back)
        {
            back.currentHP -= _damage;
            if (back.currentHP < 1) StartCoroutine(back.DeadTime());
            else back.HitColor();

            back.HitDamageBack(_damage);
        }
    }
    
    protected override IEnumerator DeadTime()
    {
        float time = 0;
        Renderer renderer = GetComponent<Renderer>();
        GetComponent<Collider>().enabled = false;

        move = headMoveStay;

        while (time < 2)
        {
            time += Time.deltaTime;

            Color color = renderer.material.GetColor("_Color");

            color = Color.Lerp(color, Color.black, Time.deltaTime);

            renderer.material.SetColor("_Color", color);


            yield return null;
        }

        time = 0;
        yield return new WaitForSeconds(2.0f);

        while (time < 1.5f)
        {
            renderer.material.SetFloat("_DissolveAmount", time);

            time += 0.5f * Time.deltaTime;
            yield return null;
        }

        gameObject.SetActive(false);
        renderer.material.SetFloat("_DissolveAmount", 0);
        renderer.material.SetColor("_Color", new Color(0.5f, 0.5f, 0.5f, 1));
        GetComponent<Collider>().enabled = true;

        if (front) transform.parent = front.transform;
        MonsterPooling.instance.MonsterEnqueue(this);
    }

    public void DragonHeadStay()
    {
        move = headMoveStay;
    }

    public void DragonHeadMove()
    {
        move = headMove;
    }
}

public class DragonHeadMove : IMove
{
    private Dragon dragon;
    private Character character;
    private Renderer renderer;
    private float speed;
    private float maxHeight;
    private float minHeight;
    private Vector3 yDir;
    private Vector3 xzDir;

    public DragonHeadMove(Dragon _dragon, float _maxHeight, float _minHeight, float _speed)
    {
        dragon = _dragon;
        renderer = dragon.GetComponent<Renderer>();
        maxHeight = _maxHeight;
        minHeight = _minHeight;
        speed = _speed;
        character = Character.instance;
        yDir = Vector3.up;
    }

    public void Move()
    {
        float heightRate = (maxHeight - dragon.transform.position.y) / maxHeight;
        heightRate = Mathf.Clamp(heightRate, 0.1f, 1.0f);

        Color color = Color.Lerp(renderer.material.GetColor("_Color"), new Color(0.5f, 0.5f, 0.5f, 1), Time.deltaTime);
        renderer.material.SetColor("_Color", color);

        dragon.transform.Translate(xzDir.normalized * speed * Time.deltaTime);
        dragon.transform.Translate(yDir * heightRate * speed * 3 * Time.deltaTime);
    }

    public void SetTarget()
    {
        if (maxHeight - dragon.transform.position.y < 0.1f)
            yDir = Vector3.down;

        else if (dragon.transform.position.y < minHeight)
            yDir = Vector3.up;

        Vector3 charPos = character.transform.position;
        charPos.y = minHeight;

        Vector3 pos = dragon.transform.position;
        pos.y = minHeight;

        Vector3 dir = (charPos - pos).normalized;
        //Vector3 cross = Vector3.Cross(dir, character.transform.up) * 2;

        xzDir = dir;

    }
}

public class DragonHeadMoveStay : IMove
{
    public void Move() { }
}

public class DragonBodyMove : IMove
{
    private Dragon dragon;
    private Dragon front;
    private float radius;
    private Renderer renderer;

    public DragonBodyMove(Dragon _dragon, Dragon _front)
    {
        dragon = _dragon;
        front = _front;
        radius = dragon.GetComponent<SphereCollider>().radius;
        renderer = dragon.GetComponent<Renderer>();
    }

    public void Move()
    {
        Vector3 dir = (front.transform.position - dragon.transform.position).normalized;
        float dis = Vector3.Distance(dragon.transform.position, front.transform.position) / radius;
        
        Color color = Color.Lerp(renderer.material.GetColor("_Color"), new Color(0.5f, 0.5f, 0.5f, 1), Time.deltaTime);
        renderer.material.SetColor("_Color", color);

        dragon.transform.Translate(dir * dis * 4f * Time.deltaTime);
    }
}