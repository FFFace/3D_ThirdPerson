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
            headMove = new DragonHeadMove(this);
            headMove.SetTarget();
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


            yield return new WaitForSeconds(0.1f);
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
}

public class DragonHeadMove : IMove
{
    private Dragon dragon;
    private Character character;
    private float speed;
    private float maxHeight;
    private float time;
    private Vector3 startPos;
    private Vector3 target;

    public DragonHeadMove(Dragon _dragon)
    {
        dragon = _dragon;
        character = Character.instance;
    }

    public void Move()
    {
        Vector3 targetDir = (target - dragon.transform.position).normalized;
        Vector3 targetHalfDir = (target / 2 - dragon.transform.position).normalized;

        float dot = Vector3.Dot(targetDir, targetHalfDir);
        dot = Mathf.Sign(dot);

        float height = maxHeight - dragon.transform.position.y;
        float heightRate = height / maxHeight * dot;

        dragon.transform.Translate(Vector3.up * speed * heightRate * Time.deltaTime);

        dragon.transform.Translate(targetDir * speed * Time.deltaTime);
    }

    public void SetTarget()
    {
        startPos = dragon.transform.position;
        Vector3 dir = character.transform.position - dragon.transform.position;
        dir.y = dragon.transform.position.y;
        dir.Normalize();

        target = character.transform.position + dir * Random.Range(1f, 4f);
        speed = Vector3.Distance(dragon.transform.position, target)/3;
        maxHeight = Random.Range(3f, 6f);

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
        float dis = Vector3.Distance(dragon.transform.position, front.position) - (radius * 2);

        dragon.transform.Translate(dir * dis * 20 * Time.deltaTime);
    }
}
