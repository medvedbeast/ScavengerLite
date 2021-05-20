using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject target;
    public float cameraSpeed = 5.0f;
    public float cameraOffsetX = 20.0f;
    public float cameraOffsetY = 20.0f;

    void Start()
    {

    }

    void FixedUpdate()
    {
        if (Game.gameState == Enumerations.GAME_STATE.GAME)
        {
            var newPosition = new Vector3(target.transform.position.x - cameraOffsetX, 20, target.transform.position.z - cameraOffsetY);
            transform.position = Vector3.Slerp(transform.position, newPosition, Time.fixedDeltaTime * cameraSpeed);
        }
    }
}
