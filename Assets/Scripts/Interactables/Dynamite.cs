using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.Events;

public class Dynamite : XRGrabInteractable
{
    public UnityEvent OnExplode;
    private bool _isActivated = false;

    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        base.OnSelectEntered(args);
        if(args.interactorObject.transform.GetComponent<XRSocketInteractor>() != null)
        {
            _isActivated = true;
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        Debug.Log("Collision with " + other.gameObject.name);
        Debug.Log("Is activated: " + _isActivated);
        Debug.Log("Is wand projectile: " + other.gameObject.GetComponent<WandProjectile>() != null);
        if (_isActivated && other.gameObject.GetComponent<WandProjectile>() != null)
        {
            Debug.Log("Explode");
            OnExplode?.Invoke();
        }
    }
}
