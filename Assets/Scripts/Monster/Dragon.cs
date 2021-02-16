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
    private Transform front;

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

            dragon.bodyMaxNum = bodyMaxNum;
            dragon.bodyNum = bodyNum + 1;
            dragon.front = transform;
            dragon.body = body;
            dragon.isHead = false;
        }
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
        EventManager.instance.AddMonsterBuffDamageEvent(BuffDamage);
        character = Character.instance;
        //nav = GetComponent<NavMeshAgent>();

        //nav.destination = Character.instance.transform.position;
        //nav.enabled = true;
        //monsterDirection = new MonsterAttackDirection(this);
        anim = GetComponent<Animator>();
        rigid = GetComponent<Rigidbody>();
        GetComponent<Collider>().enabled = true;
        attack = monsterAttackStay;
        //rigid.isKinematic = false;

        monsterMoveStay = new MonsterMoveStay(nav);

        //ResetAnimation();
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
    private float speed;
    private float maxHeight;
    private float minHeight;
    private Vector3 yDir;
    private Vector3 xzDir;

    public DragonHeadMove(Dragon _dragon, float _maxHeight, float _minHeight, float _speed)
    {
        dragon = _dragon;
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
    private Transform front;
    private float radius;

    public DragonBodyMove(Dragon _dragon, Transform _front)
    {
        dragon = _dragon;
        front = _front;
        radius = dragon.GetComponent<SphereCollider>().radius;
    }

    public void Move()
    {
        Vector3 dir = (front.position - dragon.transform.position).normalized;
        float dis = Vector3.Distance(dragon.transform.position, front.position) / radius;

        dragon.transform.Translate(dir * dis * 4f * Time.deltaTime);
    }
}
