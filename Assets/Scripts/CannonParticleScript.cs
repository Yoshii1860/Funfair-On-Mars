using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonParticleScript : MonoBehaviour
{
    [SerializeField] private CannonGame _cannonGame;

    private void OnParticleTrigger() 
    {
        _cannonGame.PlayerDamageReceived++;
    }
}