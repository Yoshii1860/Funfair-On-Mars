using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class XRHoldInteractable : XRGrabInteractable
{
    private Coroutine _ignoreGripCoroutine;
    private Vector3 _initialAttachLocalPos;
    private Quaternion _initialAttachLocalRot;
    private Transform _handAttachTransform;

    protected override void OnSelectEntering(SelectEnterEventArgs args)
    {
        Transform handTransform = args.interactorObject.transform.GetComponent<PhysicsHandReference>().HandReference;

        // Create attach point where the hand is
        if (attachTransform == null)
        {
            GameObject attachPoint = new GameObject("AttachPoint");
            attachPoint.transform.SetParent(handTransform, false);
            attachTransform = attachPoint.transform;
        }

        _initialAttachLocalPos = attachTransform.localPosition;
        _initialAttachLocalRot = attachTransform.localRotation;

        attachTransform.position = handTransform.position;
        attachTransform.rotation = handTransform.rotation;

        // Save the original attach transform and nullify it to prevent XRGrabInteractable from setting it
        _handAttachTransform = args.interactor.attachTransform;
        args.interactor.attachTransform = null;

        base.OnSelectEntering(args);
    }

    protected override void OnSelectExiting(SelectExitEventArgs args)
    {
        base.OnSelectExiting(args);

        args.interactor.attachTransform = _handAttachTransform;
        attachTransform.localPosition = _initialAttachLocalPos;
        attachTransform.localRotation = _initialAttachLocalRot;
    }
}
