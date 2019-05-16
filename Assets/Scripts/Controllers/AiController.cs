using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AiController : MonoBehaviour
{
    public float turnSpeedModifier;
    [Space(15)]
    public bool isInitiated = false;
    public float initiationRadius;
    public float shootingDistance;
    public float followDistance;
    [Space(15)]
    public GameObject turret;
    public List<GameObject> weapons = new List<GameObject>();

    private NavMeshAgent agent;
    private NavMeshObstacle obstacle;
    private Unit unit;
    private bool shootingPathClear;
    private BoxCollider hitbox;

    void Start()
    {
        hitbox = GetComponent<BoxCollider>();
        agent = GetComponent<NavMeshAgent>();
        obstacle = GetComponent<NavMeshObstacle>();
        unit = GetComponent<Unit>();
        GetComponent<Destructible>().OnDestroy += OnUnitDestroyed;
    }

    void FixedUpdate()
    {
        if (Game.gameState == Enumerations.GAME_STATE.GAME)
        {
            if (!isInitiated && Vector3.Distance(transform.position, Game.player.transform.position) <= initiationRadius)
            {
                var playerPosition = Game.player.transform.position;
                playerPosition.y = 0.5f;
                var position = transform.position;
                position.y = 0.5f;
                var direction = playerPosition - position;
                RaycastHit hit = new RaycastHit();
                Vector3 origin = new Vector3(transform.position.x, 0.5f, transform.position.z);
                Debug.DrawRay(origin, direction, Color.green);
                var mask = 0b100100000000;
                if (Physics.Raycast(origin, direction, out hit, initiationRadius, mask))
                {
                    if (hit.collider.gameObject == Game.player.gameObject)
                    {
                        isInitiated = true;
                    }
                }
            }
            if (isInitiated)
            {
                shootingPathClear = false;
                var playerPosition = Game.player.transform.position;
                playerPosition.y = 0.5f;
                var position = transform.position;
                position.y = 0.5f;
                var direction = playerPosition - position;
                RaycastHit hit = new RaycastHit();
                Vector3 origin = new Vector3(transform.position.x, 0.5f, transform.position.z);
                Debug.DrawRay(origin, direction, Color.red);
                var mask = 0b100100000000;
                if (Physics.Raycast(origin, direction, out hit, shootingDistance, mask))
                {
                    if (hit.collider.gameObject == Game.player.gameObject)
                    {
                        shootingPathClear = true;
                    }
                }

                if (Vector3.Distance(transform.position, Game.player.transform.position) > followDistance || !shootingPathClear)
                {
                    obstacle.enabled = false;
                    agent.enabled = true;
                    agent.SetDestination(Game.player.transform.position);
                }
                else
                {
                    obstacle.enabled = true;
                    agent.enabled = false;
                }
                if (Vector3.Distance(transform.position, Game.player.transform.position) <= shootingDistance && shootingPathClear)
                { 
                    Quaternion targetRotation = Quaternion.LookRotation(Game.player.transform.position - turret.transform.position);
                    targetRotation.x = turret.transform.rotation.x;
                    targetRotation.z = turret.transform.rotation.z;
                    turret.transform.rotation = Quaternion.Lerp(turret.transform.rotation, targetRotation, Time.deltaTime * turnSpeedModifier);
                    float angle = Quaternion.Angle(turret.transform.rotation, targetRotation);
                    if (angle <= 10)
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
                }
            }
        }
    }

    public void OnUnitDestroyed()
    {
        agent.enabled = false;
        obstacle.enabled = true;
        enabled = false;
    }
}
