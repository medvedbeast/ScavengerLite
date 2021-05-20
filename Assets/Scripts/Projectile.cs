using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour
{

    public GameObject owner;
    public Weapon weapon;
    public Vector3 direction;
    public float lifetime;
    public bool penetrative;
    public float speed;

    public void Start()
    {

    }

    public void FixedUpdate()
    {
        if (lifetime > 0)
        {
            lifetime -= Time.deltaTime;
        }
        else
        {
            GameObject.Destroy(this.gameObject);
        }
    }
}
