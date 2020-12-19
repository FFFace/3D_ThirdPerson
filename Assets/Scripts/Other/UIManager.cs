using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("-Skill Image-")]

    [SerializeField]
    private Image skill1;
    [SerializeField]
    private Image skill2;
    [SerializeField]
    private Image skill3;

    public static UIManager instance;


    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    public void SetSkillImage(Sprite _skill1, Sprite _skill2, Sprite _skill3)
    {
        skill1.sprite = _skill1;
        skill2.sprite = _skill2;
        skill3.sprite = _skill3;
    }
}
