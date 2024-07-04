using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PlayerParticleScript : MonoBehaviour
{
    [SerializeField] private CannonGame _cannonGame;
    [SerializeField] private Collider[] triggers;

    private Dictionary<string, Collider> triggerDict = new Dictionary<string, Collider>();
    private List<ParticleSystem.Particle> enterParticles = new List<ParticleSystem.Particle>();

    private ParticleSystem particleSystem;

    void Start()
    {
        particleSystem = GetComponent<ParticleSystem>();
        
        for (int i = 0; i < triggers.Length; i++)
        {
            triggerDict.Add(triggers[i].transform.parent.name, triggers[i]);
        }
    }

    private void OnParticleTrigger()
    {
        // Clear the lists
        enterParticles.Clear();

        // Get the particles that entered the trigger
        int numEnter = particleSystem.GetTriggerParticles(ParticleSystemTriggerEventType.Enter, enterParticles);

        for (int i = 0; i < numEnter; i++)
        {
            ParticleSystem.Particle p = enterParticles[i];

            // Access information about the particle
            Vector3 position = p.position;

            for (int j = 0; j < triggers.Length; j++)
            {
                if (triggers[j].bounds.Contains(position))
                {
                    // get key from collider in dictionary
                    string key = triggerDict.FirstOrDefault(x => x.Value == triggers[j]).Key;
                    _cannonGame.HitCannon(key);
                    Debug.Log("Hit " + key);
                    break;
                }
            }
        }
    }
}
