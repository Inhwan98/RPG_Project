using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonEffect : MonoBehaviour
{
    ParticleSystem ps;
    List<ParticleSystem.Particle> inside = new List<ParticleSystem.Particle>();

    private void Awake()
    {
        
    }

    private void OnParticleTrigger()
    {
        Debug.Log("Cube Trigger");
        ps.GetTriggerParticles(ParticleSystemTriggerEventType.Inside, inside);

        foreach (var v in inside)
        {
            Debug.Log("Cube Trigger2");
        }
    }


}
