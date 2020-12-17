using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEditorInternal;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    [SerializeField]
    private float speed;
    private float knockBackPower;
    private float damage;
    private bool isRecharge;
    private bool isKnockBack;
    private bool isSpread;
    private bool isSkillSpread;


    private IArrowMove arrowMove;
    private List<IItemEffect> itemEffects = new List<IItemEffect>();
    private bool isHit = false;

    private Character character;
    private Transform target;

    private void OnEnable()
    {
        isHit = false;
    }

    private void Start()
    {
        character = Character.instance;
        arrowMove = new ArcherArrow(this);
    }

    public void SetCharacter(Character _character)
    {
        character = _character;
    }

    public void SetTarget(Transform _target)
    {
        target = _target;
    }
    public Transform GetTarget()
    {
        return target;
    }

    public void SetRecharge(bool active)
    {
        isRecharge = active;
    }

    public void SetKnockBack(bool active, float power)
    {
        isKnockBack = active;
        knockBackPower = power;
    }

    public void SetArrowMove(IArrowMove _arrowMove)
    {
        arrowMove = _arrowMove;
    }

    public void SetArrowDamage(float _damage)
    {
        damage = _damage;
    }

    /// <summary>
    /// 캐릭터 스킬효과에 의한 투사체 Spread 효과
    /// </summary>
    /// <param name="active"></param>
    public void SetSkillSpread(bool active)
    {
        isSkillSpread = active;
    }
    /// <summary>
    /// 캐릭터 아이템효과에 의한 투사체 Spread 효과
    /// </summary>
    /// <param name="active"></param>
    public void SetItemSpread(bool active)
    {
        isSpread = active;
    }

    private void Update()
    {
        if(!isHit)
            arrowMove.Move();            
    }

    private void ArrowSpread()
    {
        if (isSkillSpread) 
        {
            Arrow[] arrows = new Arrow[10];

            for (int i = 0; i < arrows.Length; i++)
            {
                arrows[i] = character.ArrowDequeue();
                float num = (360 / arrows.Length) * i;
                arrows[i].transform.rotation = Quaternion.Euler(new Vector3(0, num, 0));
                arrows[i].transform.position = transform.position + arrows[i].transform.forward * 2.5f;
                arrows[i].SetArrowDamage(damage);
                arrows[i].SetSkillSpread(false);
                arrows[i].gameObject.SetActive(true);
            }

            isSkillSpread = false;
        }

        //TODO : 투사체 퍼짐 효과 아이템 만든 후 작성해야함
        //else if (isSpread) 
        //{


        //    isSpread = false;
        //}
    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("Monster"))
        {
            float currentDamage = isRecharge ? damage * 1.5f : damage;
            EventManager.instance.AttackEnemy(damage, other.transform.GetInstanceID(), isKnockBack, knockBackPower);

            GetComponent<Collider>().enabled = false;
            transform.SetParent(other.gameObject.transform.GetChild(0));
            StartCoroutine(DisableTime());
            isHit = true;

            foreach (var effect in itemEffects)
                effect.Effect();

            ArrowSpread();
        }

        // 오브젝트에 회전값이 있고, parent의 scale의 비율이 1:1:1이 아닐 때, 메시가 깨지는 버그가 있음.
        else if (other.CompareTag("Ground") || other.CompareTag("Wall"))
        {
            GetComponent<Collider>().enabled = false;
            StartCoroutine(DisableTime());
            isHit = true;
            Debug.Log(other.name);
        }
    }

    private IEnumerator DisableTime()
    {
        yield return new WaitForSeconds(5.0f);

        gameObject.SetActive(false);
        transform.parent = null;
        GetComponent<Collider>().enabled = true;
        character.ArrowEnqueue(this);
    }

    public float GetArrowSpeed()
    {
        return speed;
    }
}

public class ArcherArrow : IArrowMove
{
    private Arrow arrow;
    private float speed;
    private bool isSpread;
    public ArcherArrow(Arrow _arrow) { arrow = _arrow; speed = arrow.GetArrowSpeed(); }

    public void Move()
    {
        Transform target = arrow.GetTarget();
        if (target)
        {
            Vector3 pos = target.position;
            pos.y += 1.0f;
            Vector3 normal = (pos - arrow.transform.position).normalized;
            arrow.transform.rotation = Quaternion.Lerp(arrow.transform.rotation, Quaternion.LookRotation(normal), speed * 0.5f * Time.deltaTime);
        }
        else
        {
            Vector3 dir = new Vector3(90, 0, 0);
            arrow.transform.rotation = Quaternion.Lerp(arrow.transform.rotation, Quaternion.Euler(dir), 1 * Time.deltaTime);
        }
        arrow.transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }
}