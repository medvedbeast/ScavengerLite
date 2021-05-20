using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleSystemController : MonoBehaviour
{
    private ParticleSystem system;

    void Start()
    {
        system = GetComponent<ParticleSystem>();
    }

    public void Update()
    {
        if (!system.isEmitting)
        {
            Destroy(this.gameObject);
        }
    }
}
