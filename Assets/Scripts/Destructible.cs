using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Destructible : MonoBehaviour
{
    public Unit unit;
    public bool penetrable = false;
    public bool isDesturcted = false;

    public event System.Action OnDestroy;
    public event System.Action<Destructible> OnHpAmountChanged;

    public void Start()
    {
        unit = GetComponent<Unit>();
    }

    public void OnTriggerEnter(Collider c)
    {
        if (c.gameObject.layer == 9)
        {
            Projectile projectile = c.GetComponent<Projectile>();
            Unit shooter = projectile.owner.GetComponent<Unit>();

            if (!isDesturcted && shooter != null && unit != null)
            {
                if (shooter == unit)
                {
                    return;
                }

                Weapon w = projectile.weapon;
                unit.hp -= (w.damage * shooter.outputDamageMultiplier) * (Mathf.Clamp((1 - unit.armor * 0.01f) * unit.inputDamageMultiplier, 0, 1));

                OnHpAmountChanged?.Invoke(this);

                /* TODO: make unit die */
                if (unit.hp <= 0)
                {
                    OnDestroy?.Invoke();
                    shooter.OnUnitDestroyed(unit);
                    GameObject effect = Instantiate(Resources.Load("Prefabs/Particle Systems/explosion_1")) as GameObject;
                    effect.transform.position = c.transform.position;
                    effect.transform.parent = GameObject.Find("Effects").transform;
                    var agent = GetComponent<NavMeshAgent>();
                    if (agent != null)
                    {
                        agent.enabled = false;
                    }
                    isDesturcted = true;
                }
            }

            if ((projectile.penetrative && penetrable) == false)
            {
                Destroy(c.gameObject);
                GameObject effect = Instantiate(Resources.Load($"Prefabs/Particle Systems/{projectile.weapon.hitEffectName}")) as GameObject;
                effect.transform.position = c.transform.position;
                effect.transform.parent = GameObject.Find("Effects").transform;
            }
        }
    }
}
