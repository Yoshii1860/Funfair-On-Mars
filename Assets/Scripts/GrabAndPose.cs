using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class GrabAndPose : MonoBehaviour
{
    [SerializeField] private GameObject _rightHandPose;
    [SerializeField] private GameObject _leftHandPose;

    void Start()
    {
        XRGrabInteractable grabInteractable = GetComponent<XRGrabInteractable>();

        grabInteractable.selectEntered.AddListener(SetupPose);
        grabInteractable.selectExited.AddListener(ResetPose);

        _rightHandPose.SetActive(false);
        _leftHandPose.SetActive(false);
    }

    public void SetupPose(BaseInteractionEventArgs arg)
    {
        Debug.Log("SetupPose");
        if (arg.interactorObject is XRDirectInteractor)
        {
            PhysicsHandReference handReference = arg.interactorObject.transform.GetComponent<PhysicsHandReference>();
            handReference.DisableHandMeshes();

            if (handReference.HandType == PhysicsHandReference.HandModelType.Left)
            {
                _leftHandPose.SetActive(true);
            }
            else
            {
                _rightHandPose.SetActive(true);
            }
        }
    }

    public void ResetPose(BaseInteractionEventArgs arg)
    {
        Debug.Log("ResetPose");
        if (arg.interactorObject is XRDirectInteractor)
        {
            PhysicsHandReference handReference = arg.interactorObject.transform.GetComponent<PhysicsHandReference>();
            handReference.EnableHandMeshes();
            _rightHandPose.SetActive(false);
            _leftHandPose.SetActive(false);        
        }
    }
}
