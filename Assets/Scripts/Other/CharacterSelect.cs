using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Collections.LowLevel.Unsafe;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelect : MonoBehaviour
{
    [Header("Charcter"), SerializeField]
    private Character[] characters;
    private int characterNum = 9999;

    [Header("ModelCamera"), SerializeField]
    private Camera modelCam;
    [SerializeField ,Range(1, 10)]
    private float camSpeed;

    [Header("CamPos"), SerializeField]
    private Transform[] characterCamPos;

    [Header("UseSkills"), SerializeField]
    private Image[] useSkillSlots;
    private Dictionary<int, List<ISkill>> characterSkillList = new Dictionary<int, List<ISkill>>();
    private List<ISkill> archerSkillList = new List<ISkill>();
    private List<ISkill> paladinSkillList = new List<ISkill>();
    private ISkill[] useSkillList = new ISkill[4];
    private int slotNum = 9999;

    [Header("AllSkills"), SerializeField]
    private Image[] skillSlots;

    [Header("ArcherSkill"), SerializeField]
    private Sprite archerNormalAttack;
    private DummyClass.ArcherNormalAttack archerNormal;
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
    private DummyClass.PaladinNormalAttack paladinNormal;
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

    [Header("DragImage"), SerializeField]
    private Image dragImage;

    [Header("NoneSelectPanel"), SerializeField]
    private Image noneSelectPanel;
    private Transform target;
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
        DragImageMove();
    }

    private void DefineSkill()
    {
        archerNormal = new DummyClass.ArcherNormalAttack(archerNormalAttack);
        kick = new ArcherSubAttack(archerSubAttack);
        spreadArrow = new ArcherSpreadArrow(15.0f, 1.25f, archerSpreadArrow);
        multiArrow = new ArcherMultiArrow(10.0f, 1.5f, archerMultiArrow);

        archerSkillList.Add(archerNormal);
        archerSkillList.Add(kick);
        archerSkillList.Add(spreadArrow);
        archerSkillList.Add(multiArrow);

        paladinNormal = new DummyClass.PaladinNormalAttack(paladinNormalAttack);
        block = new PaladinBlock(1.5f, 10.0f, paladinBlock);
        slashAttack = new PaladinSlashAttack(10.0f, 1.5f, paladinSlashAttack);
        chainAttack = new PaladinChainAttack(3.0f, 1.7f, paladinChainAttack);

        paladinSkillList.Add(paladinNormal);
        paladinSkillList.Add(block);
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
        for (int i = 0; i < useSkillSlots.Length; i++)
        {
            useSkillSlots[i].sprite = null;
            useSkillSlots[i].enabled = false;

            useSkillList[i] = null;
        }

        for (int i = 0; i < skillSlots.Length; i++)
        {
            skillSlots[i].sprite = null;
            skillSlots[i].enabled = false;
        }
    }

    private void DragImageMove()
    {
        dragImage.transform.position = Input.mousePosition;
    }

    public void SelectUseSkillSlot(int num)
    {
        if (useSkillList[num] == null) return;

        skillExplain.text = useSkillList[num].GetExplain();
    }

    public void SelectSkillSlot(int num)
    {
        if (characterNum >= characterSkillList.Count ||
            characterSkillList[characterNum][num + 1] == null) return;

        skillExplain.text = characterSkillList[characterNum][num + 1].GetExplain();
    }

    public void SelectCharacter(int num)
    {
        if (characterNum == num) return;

        characterNum = num;
        target = characterCamPos[num];
        ResetSkillSlots();

        useSkillList[0] = characterSkillList[num][0];

        useSkillSlots[0].sprite = characterSkillList[num][0].GetImage();
        useSkillSlots[0].enabled = true;

        for (int i = 0; i < skillSlots.Length; i++)
        {
            if (characterSkillList[num].Count < i + 2) break;

            skillSlots[i].sprite = characterSkillList[num][i + 1].GetImage();
            skillSlots[i].enabled = true;
        }

        noneSelectPanel.gameObject.SetActive(false);
    }

    public void UseSkillSlotMouseEnter(int num)
    {
        slotNum = num;
    }

    public void UseSKillSlotMouseExit()
    {
        slotNum = 9999;
    }

    public void SkillSlotBeginDrag(int num)
    {
        if (characterSkillList[characterNum].Count <= num+1) return;
        dragImage.sprite = characterSkillList[characterNum][num + 1].GetImage();
        dragImage.enabled = true;
    }

    public void SkillSlotEndDrag(int num)
    {
        if (slotNum > useSkillList.Length || slotNum == 0 ||
            characterSkillList[characterNum][num] == null)
        {
            dragImage.enabled = false;
            slotNum = 9999; 
            return;
        }

        int useSlotNum = 0;
        bool isHave = false;

        while (useSlotNum < useSkillList.Length)
        {
            if (useSkillList[useSlotNum] == characterSkillList[characterNum][num + 1])
            {
                isHave = true;
                break;
            }

            ++useSlotNum;
        }

        if (isHave)
        {
            if (slotNum == useSlotNum)
            {
                dragImage.enabled = false;
                slotNum = 9999;
                return;
            }

            else if (useSkillList[slotNum] == null)
            {
                useSkillList[slotNum] = characterSkillList[characterNum][num + 1];
                useSkillList[useSlotNum] = null;
            }

            else
            {
                ISkill skill = useSkillList[slotNum];
                useSkillList[slotNum] = characterSkillList[characterNum][num + 1];
                useSkillList[useSlotNum] = skill;
            }
       
        }

        else
            useSkillList[slotNum] = characterSkillList[characterNum][num + 1];

        for (int i = 0; i < useSkillList.Length; i++)
        {
            if (useSkillList[i] == null)
            {
                useSkillSlots[i].enabled = false;
            }

            else
            {
                useSkillSlots[i].sprite = useSkillList[i].GetImage();
                useSkillSlots[i].enabled = true;
            }
        }

        dragImage.enabled = false;
        slotNum = 9999;
    }

    public void StartGame()
    {
        CharacterInfo.instance.characterNum = characterNum;
        CharacterInfo.instance.SetSkills(useSkillList);
    }
}

namespace DummyClass
{
    public class ArcherNormalAttack : ISkill
    {
        private Sprite image;
        public ArcherNormalAttack(Sprite _image) { image = _image; }

        public bool isActive { get; set; }

        public void SkillKeyDown() { }

        public void SkillKeyUp() { }
        public IEnumerator SkillCoolTime() { return null; }
        public float GetDamage() { return 0; }
        public float GetDamageMagnification() { return 0; }
        public float GetCoolTime() { return 0; }
        public string GetExplain()
        {
            return "[기본 공격]\n\n조준한 적에게 화살을 발사합니다. 아무도 조준되어있지 않다면 어디로 날라갈지 알 수 없습니다.";
        }
        public Sprite GetImage() { return image; }

        public bool GetToggleState()
        {
            return false;
        }
    }

    public class PaladinNormalAttack : ISkill
    {
        private Sprite image;

        public PaladinNormalAttack(Sprite _image) { image = _image; }

        public bool isActive { get; set; }

        public void SkillKeyDown() { }

        public void SkillKeyUp() { }
        public IEnumerator SkillCoolTime() { return null; }
        public float GetDamage() { return 0; }
        public float GetDamageMagnification() { return 0; }
        public float GetCoolTime() { return 0; }
        public string GetExplain()
        {
            return "[기본 공격]\n\n전방을 향해 무기를 휘두릅니다. 누르고 있으면 계속해서 공격합니다.";
        }
        public Sprite GetImage() { return image; }

        public bool GetToggleState()
        {
            return false;
        }
    }
}