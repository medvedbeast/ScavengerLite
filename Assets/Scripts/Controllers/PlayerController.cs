using Enumerations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public float moveSpeedModifier = 5;
    public float turnSpeedModifier = 5;
    [Space(15)]
    public GameObject turret;
    public List<GameObject> weapons = new List<GameObject>();

    private Unit unit;
    private Rigidbody rigidbody;
    private Camera camera;
    private Vector3 velocity;

    void Start()
    {
        unit = GetComponent<Unit>();
        rigidbody = GetComponent<Rigidbody>();
        camera = Camera.main;

        var events = GameObject.Find("Core").GetComponent<Events>();

        var destructible = GetComponent<Destructible>();
        destructible.OnDestroy += OnPlayerDestroyed;
        destructible.OnHpAmountChanged += events.OnPlayerHpChanged;

        unit.storage.InventoryChanged += events.OnPlayerInventoryChanged;
        unit.storage.EquipmentChanged += events.OnPlayerEquipmentChanged;
        unit.UnitXpChanged += events.OnPlayerXpChanged;
    }

    void Update()
    {
        if (Game.gameState == GAME_STATE.GAME)
        {
            velocity = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized * moveSpeedModifier;
        }
    }

    void FixedUpdate()
    {
        if (Game.gameState == GAME_STATE.GAME)
        {
            Move();
            Rotate();

            if (Input.GetMouseButton(0))
            {
                Shoot();
            }
        }
    }

    private void Move()
    {
        rigidbody.MovePosition(rigidbody.position + velocity * Time.fixedDeltaTime);
    }

    private void Rotate()
    {
        Vector3 mousePosition = camera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, camera.transform.position.y));
        turret.transform.LookAt(mousePosition + Vector3.up * transform.position.y);
        /*
        var mouse = GetMouseDirection();
        float angle = Vector3.Angle(Vector3.forward.normalized, mouse);
        if (Input.mousePosition.x < Screen.width / 2)
        {
            angle *= -1;
        }
        var targetRotation = Quaternion.Slerp(turret.transform.rotation, Quaternion.Euler(turret.transform.rotation.eulerAngles.x, angle, turret.transform.rotation.eulerAngles.z), turnSpeedModifier * unit.turnSpeed * Time.deltaTime);
        targetRotation.x = 0;
        targetRotation.z = 0;
        turret.transform.rotation = targetRotation;*/
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
