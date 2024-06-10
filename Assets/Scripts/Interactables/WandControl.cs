using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class WandControl : XRGrabInteractable
{
    [Space(10)]
    [Header("Wand Control Settings")]
    [SerializeField] GameObject _projectilePrefab;
    [SerializeField] Transform _projectileSpawnPoint;
    [SerializeField] private ParticleSystem _magicParticles;
    [SerializeField] private float _delayPS = 0.5f;

    private bool _isFired = false;

    protected override void OnActivated(ActivateEventArgs args)
    {
        base.OnActivated(args);
        if (_projectilePrefab != null)
        {
            Instantiate(_projectilePrefab, _projectileSpawnPoint.position, _projectileSpawnPoint.rotation);
            if (!_isFired && !_magicParticles.isPlaying)
            {
                Debug.Log("Firing Magic");
                _magicParticles.Play();
                _isFired = true;
                StartCoroutine(ResetVFX(_delayPS));
            }
        }
    }

    protected override void OnDeactivated(DeactivateEventArgs args)
    {
        base.OnDeactivated(args);
    }

    private IEnumerator ResetVFX(float delay)
    {
        yield return new WaitForSeconds(delay);
        Debug.Log("Stopping Magic");
        _magicParticles.Stop();
        _isFired = false;
    }
}
