using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpreadItem : Item
{
    private Spread item;

    protected override void InitDate()
    {
        base.InitDate();

        effect = item;
    }
}


public class Spread : IItemEffect
{
    private Arrow[] arrows = new Arrow[10];

    public void Effect(Arrow _arrow)
    {
        for (int i = 0; i < arrows.Length; i++)
        {
            arrows[i] = Character.instance.ArrowDequeue();
            float num = (360 / arrows.Length) * i;
            arrows[i].transform.rotation = Quaternion.Euler(new Vector3(0, num, 0));
            arrows[i].transform.position = _arrow.transform.position + arrows[i].transform.forward * 2.5f;
            arrows[i].SetArrowDamage(_arrow.GetArrowDamage());
            arrows[i].SetSkillSpread(false);
            arrows[i].gameObject.SetActive(true);
        }
    }
}