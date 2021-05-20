using UnityEngine;
using System.Collections;
using System.Xml.Serialization;
using System.IO;
using Enumerations;

public class Weapon : Module
{
    public float damage;
    public float attackSpeed;
    public string projectileName;
    public string hitEffectName;
    public WEAPON_TYPE weaponType;

    private float rechargeTime = 0;

    public override void Update()
    {
        if (rechargeTime > 0)
        {
            rechargeTime -= Time.deltaTime;
        }
    }

    public override bool IsUpdateable()
    {
        return true;
    }

    public void Shoot(GameObject owner, GameObject weaponObject)
    {
        if (rechargeTime <= 0)
        {
            string name = projectileName.Length > 0 ? projectileName : "projectile_1";
            GameObject p = Object.Instantiate(Resources.Load("Prefabs/Projectiles/" + name)) as GameObject;
            p.name = name;

            Projectile projectile = p.GetComponent<Projectile>();
            projectile.owner = owner;
            projectile.weapon = this;

            p.transform.position = new Vector3(weaponObject.transform.position.x, weaponObject.transform.position.y, weaponObject.transform.position.z);

            p.transform.Rotate(0, weaponObject.transform.parent.transform.rotation.eulerAngles.y, 0);
            p.transform.SetParent(GameObject.Find("Projectiles").transform);

            p.GetComponent<Rigidbody>().AddForce(-weaponObject.transform.up * projectile.speed, ForceMode.VelocityChange);
            rechargeTime = 60 / attackSpeed;
        }
    }
}
