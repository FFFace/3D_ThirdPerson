using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PlayerControll : MonoBehaviour
{
    private Character character;

    private KeyCommand mouseLeftClick;
    private KeyCommand mouseRightClick;
    private KeyCommand mouseRightUp;
    private KeyCommand mouseLeftUp;
    private KeyCommand leftShift;
    private KeyCommand alpha1Down;
    private KeyCommand alpha1Up;
    private KeyCommand alpha2Down;
    private KeyCommand alpha2Up;

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
        StartCoroutine(IEnumUpdate());
    }

    private IEnumerator IEnumUpdate()
    {
        yield return new WaitForSeconds(1.0f);
        InitData();

        while (true)
        {
            PlayerInput();
            PlayerAxis();
            yield return null;
        }
    }

    public void Update()
    {
    }

    private void FixedUpdate()
    {
        PlayerJumpInput();
    }

    private void InitData()
    {
        mouseLeftClick = new NomalAttackPressDown(character);
        mouseLeftUp = new NomalAttackPressUp(character);
        mouseRightClick = new SubAttackPressDown(character);
        mouseRightUp = new SubAttackPressUp(character);
        leftShift = new Dodge(character);

        alpha1Down = new MainSkillPressDown(character);
        alpha1Up = new MainSkillPressUp(character);
        alpha2Down = new SubSkillPressDown(character);
        alpha2Up = new SubSKillPressUp(character);
    }

    private void PlayerInput()
    {
        if (!isDead)
        {
            character.Move();

            if (Input.GetMouseButton(0))
                mouseLeftClick.command();
            else if (Input.GetMouseButtonUp(0))
                mouseLeftUp.command();

            if (Input.GetMouseButton(1))
                mouseRightClick.command();
            else if (Input.GetMouseButtonUp(1))
                mouseRightUp.command();

            if (Input.GetKeyDown(KeyCode.Alpha1))
                alpha1Down.command();
            else if (Input.GetKeyUp(KeyCode.Alpha1))
                alpha1Up.command();

            if (Input.GetKeyDown(KeyCode.Alpha2))
                alpha2Down.command();
            else if (Input.GetKeyUp(KeyCode.Alpha2))
                alpha2Up.command();

            if (Input.GetKey(KeyCode.LeftShift))
                leftShift.command();
        }

        if (Input.GetKey(KeyCode.LeftAlt))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    private void PlayerJumpInput()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            character.Jump();
    }

    private void PlayerAxis()
    {
        if (!isDead)
        {
            mouseX += Input.GetAxis("Mouse X") * mouseAxisSensitive;
            Vector3 camAngle = new Vector3(0, mouseX, 0);
            character.transform.rotation = Quaternion.Euler(camAngle);
        }
    }

    public void SetDeathState(bool active)
    {
        isDead = active;
    }

    public bool GetDeathState()
    {
        return isDead;
    }

    public void SetTarget(GameObject obj)
    {
        character = obj.GetComponent<Character>();
    }

    public void SetPlayerCharacter(Character _character)
    {
        character = _character;
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

public class SubAttackPressDown : KeyCommand
{
    private Character character;
    public SubAttackPressDown(Character _character) { character = _character; }
    public void command()
    {
        character.SubAttackPressDown();
    }
}

public class SubAttackPressUp : KeyCommand
{
    private Character character;
    public SubAttackPressUp(Character _character) { character = _character; }
    public void command()
    {
        character.SubAttackPressUp();
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

public class MainSkillPressDown : KeyCommand
{
    private Character character;
    public MainSkillPressDown(Character _character) { character = _character; }
    public void command()
    {
        character.MainSkillPressDown();
    }
}

public class MainSkillPressUp : KeyCommand
{
    private Character character;

    public MainSkillPressUp(Character _character) { character = _character; }

    public void command()
    {
        character.MainSkillPressUp();
    }
}

public class SubSkillPressDown : KeyCommand
{
    private Character character;
    public SubSkillPressDown(Character _character) { character = _character; }
    public void command()
    {
        character.SubSkillPressDown();
    }
}

public class SubSKillPressUp : KeyCommand
{
    private Character character;
    public SubSKillPressUp(Character _character) { character = _character; }

    public void command()
    {
        character.SubSkillPressUp();
    }
}