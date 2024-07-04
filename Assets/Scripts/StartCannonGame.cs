using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartCannonGame : MonoBehaviour
{
    [SerializeField] private CannonGame _cannonGame;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (!_cannonGame.IsGameRunning)
            {
                _cannonGame.StartGame();
            }
        }
    }
}
