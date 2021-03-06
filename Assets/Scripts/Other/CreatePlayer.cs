using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatePlayer : MonoBehaviour
{
    [SerializeField]
    private Transform target;
    private GameObject character;

    [SerializeField]
    private PlayerControll playerController;
    [SerializeField]
    private CameraControll cameraController;
    [SerializeField]
    private Minimap minimap;

    private void Start()
    {
        Create();
    }

    private void Create()
    {
        character = Instantiate(CharacterInfo.instance.SelectPlayer(), target.position, Quaternion.identity);

        playerController.SetTarget(character);
        cameraController.SetTarget(character);
        minimap.SetTarget(character);
    }
}
