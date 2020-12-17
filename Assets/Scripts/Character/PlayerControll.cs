using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PlayerControll : MonoBehaviour
{
    [SerializeField]
    private Character character;

    private KeyCommand mouseLeftClick;
    private KeyCommand mouseRightClick;
    private KeyCommand mouseRightUp;
    private KeyCommand mouseLeftUp;
    private KeyCommand leftShift;
    private KeyCommand alpha1;
    private KeyCommand alpha2;

    private bool isDead;

    public static PlayerControll instance;

    [SerializeField]
    private float mouseAxisSensitive;

    public float mouseX = 0;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        InitData();
    }

    private void Update()
    {
        PlayerInput();
        PlayerAxis();
    }

    private void FixedUpdate()
    {
        PlayerJumpInput();
    }

    private void InitData()
    {
        mouseLeftClick = new NomalAttackPressDown(character);
        mouseLeftUp = new NomalAttackPressUp(character);
        mouseRightClick = new SubAttack(character);
        mouseRightUp = new SubAttack(character);
        leftShift = new Dodge(character);

        alpha1 = new MainSkill(character);
        alpha2 = new SubSkill(character);
    }

    private void PlayerInput()
    {
        if (!isDead)
        {
            character.Move();

            if (Input.GetMouseButton(0))
                mouseLeftClick.command();

            if (Input.GetMouseButtonUp(0))
                mouseLeftUp.command();

            if (Input.GetMouseButton(1))
                mouseRightClick.command();
            else if (Input.GetMouseButtonUp(1))
                mouseRightUp.command();

            if (Input.GetKeyDown(KeyCode.Alpha1))
                alpha1.command();

            if (Input.GetKeyDown(KeyCode.Alpha2))
                alpha2.command();

            if (Input.GetKey(KeyCode.LeftShift))
                leftShift.command();
        }
    }

    private void PlayerJumpInput()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            character.Jump();
    }

    private void PlayerAxis()
    {
        mouseX += Input.GetAxis("Mouse X") * mouseAxisSensitive;
        Vector3 camAngle = new Vector3(0, mouseX, 0);
        character.transform.rotation = Quaternion.Euler(camAngle);
    }

    public void SetDeathState(bool active)
    {
        isDead = active;
    }

    public bool GetDeathState()
    {
        return isDead;
    }
}

public class NomalAttackPressDown : KeyCommand
{
    private Character character;
    public NomalAttackPressDown(Character _character) { character = _character; }
    public void command()
    {
        character.AttackPressDown();
    }
}

public class NomalAttackPressUp : KeyCommand
{
    private Character character;
    public NomalAttackPressUp(Character _character) { character = _character; }

    public void command()
    {
        character.AttackPressUp();
    }
}

public class SubAttack : KeyCommand
{
    private Character character;
    public SubAttack(Character _character) { character = _character; }
    public void command()
    {
        character.SubAttack();
    }
}

public class Dodge : KeyCommand
{
    private Character character;
    public Dodge(Character _character) { character = _character; }
    public void command()
    {
        character.Dodge();
    }
}

public class MainSkill : KeyCommand
{
    private Character character;
    public MainSkill(Character _character) { character = _character; }
    public void command()
    {
        character.MainSkill();
    }
}

public class SubSkill : KeyCommand
{
    private Character character;
    public SubSkill(Character _character) { character = _character; }
    public void command()
    {
        character.SubSkill();
    }
}