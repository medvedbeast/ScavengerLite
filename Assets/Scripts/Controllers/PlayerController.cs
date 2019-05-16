using Enumerations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public float moveSpeedModifier = 0.175f;
    public float turnSpeedModifier = 5;
    [Space(15)]
    public GameObject turret;
    public List<GameObject> weapons = new List<GameObject>();

    private Unit unit;

    void Start()
    {
        unit = GetComponent<Unit>();
        
        var events = GameObject.Find("Core").GetComponent<Events>();

        var destructible = GetComponent<Destructible>();
        destructible.OnDestroy += OnPlayerDestroyed;
        destructible.OnHpAmountChanged += events.OnPlayerHpChanged;

        unit.storage.InventoryChanged += events.OnPlayerInventoryChanged;
        unit.storage.EquipmentChanged += events.OnPlayerEquipmentChanged;
        unit.UnitXpChanged += events.OnPlayerXpChanged;
    }

    void FixedUpdate()
    {
        if (Game.gameState == GAME_STATE.GAME)
        {
            Vector3 moveDirection = Vector3.zero;
            if (Input.GetKey(KeyCode.A))
            {
                moveDirection.x = -1;
            }
            if (Input.GetKey(KeyCode.D))
            {
                moveDirection.x = 1;
            }
            if (Input.GetKey(KeyCode.W))
            {
                moveDirection.z = 1;
            }
            if (Input.GetKey(KeyCode.S))
            {
                moveDirection.z = -1;
            }
            Move(moveDirection);

            Rotate();

            if (Input.GetMouseButton(0))
            {
                Shoot();
            }
        }
    }

    private void Move(Vector3 direction)
    {
        direction *= moveSpeedModifier * GetComponent<Unit>().moveSpeed;
        transform.position = new Vector3(transform.position.x + direction.x, transform.position.y + direction.y, transform.position.z + direction.z);
    }

    private void Rotate()
    {
        var mouse = GetMouseDirection();
        float angle = Vector3.Angle(Vector3.forward.normalized, mouse);
        if (Input.mousePosition.x < Screen.width / 2)
        {
            angle *= -1;
        }
        var targetRotation = Quaternion.Slerp(turret.transform.rotation, Quaternion.Euler(turret.transform.rotation.eulerAngles.x, angle, turret.transform.rotation.eulerAngles.z), turnSpeedModifier * unit.turnSpeed * Time.deltaTime);
        targetRotation.x = 0;
        targetRotation.z = 0;
        turret.transform.rotation = targetRotation;
    }

    private void Shoot()
    {
        int index = 0;
        for (int i = 0; i < unit.storage.equipment.Count; i++)
        {
            if (unit.storage.equipment[i] is Weapon)
            {
                Weapon w = unit.storage.equipment[i] as Weapon;
                w.Shoot(transform.gameObject, weapons[index]);
                index++;
            }
        }
    }

    public static Vector3 GetMouseDirection()
    {
        Vector3 mouse = new Vector3(Input.mousePosition.x - (Screen.width / 2), Input.mousePosition.y - (Screen.height / 2), Input.mousePosition.z);
        return new Vector3(mouse.normalized.x, mouse.normalized.z, mouse.normalized.y);
    }

    public void OnPlayerDestroyed()
    {
        SceneManager.LoadScene("level_1");
    }

    public void OnUnitInitialized()
    {
        UI.Initialize();
        var events = GameObject.Find("/Core").GetComponent<Events>();
        events.OnPlayerEquipmentChanged(unit);
        events.OnPlayerInventoryChanged(unit);
    }
}
