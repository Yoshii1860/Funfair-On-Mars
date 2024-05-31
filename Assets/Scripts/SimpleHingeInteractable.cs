using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class SimpleHingeInteractable : XRSimpleInteractable
{
    [SerializeField] bool _isLocked = true;

    private Transform _grabHand;
    private const string DEFAULT_LAYER = "Default";
    private const string GRAB_LAYER = "Grab"; 

    protected virtual void Update()
    {
        if (_grabHand != null)
        {
            transform.LookAt(_grabHand, transform.forward);
        }
    }

    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        if (!_isLocked)
        {
            base.OnSelectEntered(args);
            _grabHand = args.interactor.transform;
        }
    }

    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        base.OnSelectExited(args);
        _grabHand = null;
        ChangeLayerMask(GRAB_LAYER);
    }

    public void Unlock()
    {
        _isLocked = false;
    }

    public void Release()
    {
        ChangeLayerMask(DEFAULT_LAYER);
    }

    private void ChangeLayerMask(string mask)
    {
        interactionLayers = InteractionLayerMask.GetMask(mask);
    }
}
