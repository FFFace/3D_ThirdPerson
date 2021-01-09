using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("-Skill Image-")]

    [SerializeField]
    private Image mainSKill;
    private float mainSkillCoolTime = 1;
    private float mainSkillCurrentTime=0;
    [SerializeField]
    private Image subSkill;
    private float subSkillCoolTime = 1;
    private float subSkillCurrentTime=0;
    [SerializeField]
    private Image subAttack;
    private float subAttackCoolTime = 1;
    private float subAttackCurrentTime=0;

    [Space, Header("-Skill AmountImage-")]
    [SerializeField]
    private Image skill1Amount;
    [SerializeField]
    private Image skill2Amount;
    [SerializeField]
    private Image skill3Amount;

    [Space, Header("-Item-")]
    [SerializeField]
    private Image itemPanel;
    [SerializeField]
    private Image itemImageBG;
    [SerializeField]
    private Image itemImage;
    [SerializeField]
    private Text itemText;

    private Queue<Item> items = new Queue<Item>();

    private bool isGetItem;

    public static UIManager instance;


    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        StartCoroutine(SkillCoolTime());
    }

    private IEnumerator SkillCoolTime()
    {
        while(true)
        {
            if (mainSkillCurrentTime > 0) mainSkillCurrentTime -= 0.1f;
            if (subSkillCurrentTime > 0) subSkillCurrentTime -= 0.1f;
            if (subAttackCurrentTime > 0) subAttackCurrentTime -= 0.1f;

            skill1Amount.fillAmount = mainSkillCurrentTime / mainSkillCoolTime;
            skill2Amount.fillAmount = subSkillCurrentTime / subSkillCoolTime;
            skill3Amount.fillAmount = subAttackCurrentTime / subAttackCoolTime;

            yield return new WaitForSeconds(0.1f);
        }
    }

    public void SetSkillImage(Sprite _skill1, Sprite _skill2, Sprite _skill3)
    {
        mainSKill.sprite = _skill1;
        subSkill.sprite = _skill2;
        subAttack.sprite = _skill3;
    }

    public void SetSkillCoolTime(float _mainSkill, float _subSkill, float _subAttack)
    {
        mainSkillCoolTime = _mainSkill;
        subSkillCoolTime = _subSkill;
        subAttackCoolTime = _subAttack;
    }

    public void SetMainSkillCoolTime()
    {
        mainSkillCurrentTime = mainSkillCoolTime;
    }

    public void SetSubSkillCoolTime()
    {
        subSkillCurrentTime = subSkillCoolTime;
    }

    public void SetSubAttackCoolTime( )
    {
        subAttackCurrentTime = subAttackCoolTime;
    }

    //public void PlayerGetItem(Item _item)
    //{
    //    items.Enqueue(_item);
    //    if (!isGetItem) StartCoroutine(IEnumGetItem());
    //}

    //private IEnumerator IEnumGetItem()
    //{
    //    isGetItem = true;

    //    while (isGetItem)
    //    {
    //        if (items.Count > 0)
    //        {
    //            Item item = items.Dequeue();

    //            itemImage.sprite = item.GetItemSprite;
    //            itemText.text = item.GetItemEx;

    //            itemPanel.color = Color.white;
    //            itemImageBG.color = Color.white;
    //            itemImage.color = Color.white;
    //            itemText.color = Color.white;

    //            yield return new WaitForSeconds(3.0f);

    //            float time = 0;
    //            Color color = new Color(1, 1, 1, 0);

    //            while (time >= 2)
    //            {
    //                itemPanel.color = Color.Lerp(itemPanel.color, color, Time.deltaTime);
    //                itemImageBG.color = Color.Lerp(itemImageBG.color, color, Time.deltaTime);
    //                itemImage.color = Color.Lerp(itemImage.color, color, Time.deltaTime);
    //                itemText.color = Color.Lerp(itemText.color, color, Time.deltaTime);

    //                time += Time.deltaTime;
    //                yield return null;
    //            }
    //        }
    //        else
    //            isGetItem = false;
    //    }
    //}
}
