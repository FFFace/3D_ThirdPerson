using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
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

    public void SetSkillImage(Image _skill1, Image _skill2, Image _skill3)
    {
        skill1 = _skill1;
        skill2 = _skill2;
        skill3 = _skill3;
    }
}
