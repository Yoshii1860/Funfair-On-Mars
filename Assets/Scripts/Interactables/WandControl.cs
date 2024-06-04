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

    private bool _isFired = false;

    protected override void OnActivated(ActivateEventArgs args)
    {
        base.OnActivated(args);
        if (_projectilePrefab != null)
        {
            Instantiate(_projectilePrefab, _projectileSpawnPoint.position, _projectileSpawnPoint.rotation);
        }
    }

    protected override void OnDeactivated(DeactivateEventArgs args)
    {
        base.OnDeactivated(args);
    }
}
