using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Collections.LowLevel.Unsafe;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelect : MonoBehaviour
{
    [Header("ModelCamera"), SerializeField]
    private Camera modelCam;
    [SerializeField ,Range(1, 10)]
    private float camSpeed;

    [Header("CamPos"), SerializeField]
    private Transform[] characterCamPos;

    [Header("UseSkills"), SerializeField]
    private Image[] useSkillSlots;
    private Dictionary<int, List<ISkill>> characterSkillList;
    private List<ISkill> archerSkillList;
    private List<ISkill> paladinSkillList;

    [Header("AllSkills"), SerializeField]
    private Image[] skillSlots;

    [Header("ArcherSkill"), SerializeField]
    private Sprite archerNormalAttack;
    private CharacterNormalAttack archerNormal;
    [SerializeField]
    private Sprite archerSubAttack;
    private ArcherSubAttack kick;
    [SerializeField]
    private Sprite archerSpreadArrow;
    private ArcherSpreadArrow spreadArrow;
    [SerializeField]
    private Sprite archerMultiArrow;
    private ArcherMultiArrow multiArrow;

    [Header("PaladinSkill"), SerializeField]
    private Sprite paladinNormalAttack;
    private CharacterNormalAttack paladinNormal;
    [SerializeField]
    private Sprite paladinBlock;
    private PaladinBlock block;
    [SerializeField]
    private Sprite paladinChainAttack;
    private PaladinChainAttack chainAttack;
    [SerializeField]
    private Sprite paladinSlashAttack;
    private PaladinSlashAttack slashAttack;

    [Header("ExplainText"), SerializeField]
    private Text skillExplain;

    private Transform target;
    private Character selectCharacter;

    private void Awake()
    {
        DefineSkill();
    }

    private void Start()
    {
        target = modelCam.transform;
    }

    private void Update()
    {
        CameraMove();
    }

    private void DefineSkill()
    {
        archerNormal = new CharacterNormalAttack(archerNormalAttack);
        kick = new ArcherSubAttack(archerSubAttack);
        spreadArrow = new ArcherSpreadArrow(15.0f, 1.25f, archerSpreadArrow);
        multiArrow = new ArcherMultiArrow(10.0f, 1.5f, archerMultiArrow);

        archerSkillList.Add(spreadArrow);
        archerSkillList.Add(multiArrow);

        paladinNormal = new CharacterNormalAttack(paladinNormalAttack);
        block = new PaladinBlock(1.5f, 10.0f, paladinBlock);
        slashAttack = new PaladinSlashAttack(10.0f, 1.5f, paladinSlashAttack);
        chainAttack = new PaladinChainAttack(3.0f, 1.7f, paladinChainAttack);

        paladinSkillList.Add(slashAttack);
        paladinSkillList.Add(chainAttack);

        characterSkillList.Add(0, archerSkillList);
        characterSkillList.Add(1, paladinSkillList);
    }

    private void CameraMove()
    {
        modelCam.transform.position = Vector3.Lerp(modelCam.transform.position, target.position, camSpeed * Time.deltaTime);
    }

    private void ResetSkillSlots()
    {
        for (int i = 2; i < useSkillSlots.Length; i++)
        {
            useSkillSlots[i].sprite = null;
            useSkillSlots[i].enabled = false;
        }

        for (int i = 2; i < skillSlots.Length; i++)
        {
            skillSlots[i].sprite = null;
            skillSlots[i].enabled = false;
        }
    }

    public void SelectCharacter(int num)
    {
        target = characterCamPos[num];
        ResetSkillSlots();
    }

    public void SkillBeginDrag()
    {

    }

    public void SKillEndDrag()
    {

    }
}

public class SkillSlot
{
    public ISkill skill { get; private set; }
}

public class CharacterNormalAttack : ISkill
{
    private Sprite image;
    private string explain;

    public CharacterNormalAttack(Sprite _image) { image = _image; }

    public bool isActive { get; set; }

    public void Skill() { }
    public IEnumerator SkillCoolTime() { return null; }
    public float GetDamage() { return 0; }
    public float GetDamageMagnification() { return 0; }
    public float GetCoolTime() { return 0; }
    public string GetExplain() 
    {
        return explain;
    }
    public Sprite GetImage() { return image; }

    public void Setexplain(string _string)
    {
        explain = _string;
    }
}